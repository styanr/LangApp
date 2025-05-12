using System.Text;
using LangApp.Core.Exceptions;
using LangApp.Core.Services.PronunciationAssessment;
using LangApp.Core.ValueObjects;
using LangApp.Infrastructure.BlobStorage;
using LangApp.Infrastructure.PronunciationAssessment.Options;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LangApp.Infrastructure.PronunciationAssessment;

public class PronunciationAssessmentService : IPronunciationAssessmentService
{
    private readonly BlobStorageService _blobStorageService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PronunciationAssessmentService> _logger;
    private readonly IOptions<SpeechConfigOptions> _config;

    public PronunciationAssessmentService(
        BlobStorageService blobStorageService,
        IConfiguration configuration,
        ILogger<PronunciationAssessmentService> logger,
        IOptions<SpeechConfigOptions> config)
    {
        _blobStorageService = blobStorageService;
        _configuration = configuration;
        _logger = logger;
        _config = config;
    }

    public async Task<SubmissionGrade> Assess(string fileUri, string referenceText, Language language)
    {
        Uri? uri = null;
        try
        {
            uri = new Uri(fileUri);
        }
        catch (UriFormatException e)
        {
            throw new LangAppException("Invalid fileUri format.");
        }

        var blobName = uri.Segments.Last();
        var containerName = uri.Segments
                                .Skip(1)
                                .FirstOrDefault()?.TrimEnd('/')
                            ?? throw new LangAppException("Invalid fileUri format: container segment missing");

        if (!await _blobStorageService.Exists(containerName, blobName))
        {
            throw new LangAppException("File not found in blob storage.");
        }

        await using var audioStream = await _blobStorageService.DownloadFileAsync(containerName, blobName);

        var speechConfig = SpeechConfig.FromSubscription(_config.Value.SubscriptionKey, _config.Value.Region);
        var pronunciationConfig = new PronunciationAssessmentConfig(
            referenceText,
            GradingSystem.HundredMark,
            Granularity.Word,
            enableMiscue: false
        );

        using var pushStream = AudioInputStream.CreatePushStream();
        using var audioInput = AudioConfig.FromStreamInput(pushStream);
        using var recognizer = new SpeechRecognizer(speechConfig, language.Code, audioInput);

        pronunciationConfig.ApplyTo(recognizer);

        PronunciationAssessmentResult? result = null;
        var assessmentComplete = new TaskCompletionSource<bool>();

        recognizer.Recognized += (_, e) =>
        {
            _logger.LogInformation("Speech recognized: {Text}", e.Result.Text);

            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                var sdkResult = PronunciationAssessmentResult.FromResult(e.Result);
                if (sdkResult == null)
                {
                    _logger.LogWarning("Pronunciation result was null.");
                    return;
                }

                _logger.LogInformation(
                    "Scores → Accuracy: {Accuracy}, Fluency: {Fluency}, Completeness: {Completeness}, Overall: {Overall}",
                    sdkResult.AccuracyScore, sdkResult.FluencyScore, sdkResult.CompletenessScore,
                    sdkResult.PronunciationScore);
            }
            else
            {
                _logger.LogWarning("Recognition result reason: {Reason}", e.Result.Reason);
            }
        };

        recognizer.Canceled += (_, e) =>
        {
            _logger.LogWarning("Recognition canceled. Reason: {Reason}, Error: {Error}", e.Reason, e.ErrorDetails);
            if (!assessmentComplete.Task.IsCompleted)
            {
                if (e.Reason == CancellationReason.Error)
                    assessmentComplete.SetException(new Exception(e.ErrorDetails));
                else
                    assessmentComplete.SetResult(true);
            }
        };

        recognizer.SessionStopped += (_, _) =>
        {
            _logger.LogInformation("Session stopped.");
            if (!assessmentComplete.Task.IsCompleted)
            {
                assessmentComplete.SetResult(true);
            }
        };

        await recognizer.StartContinuousRecognitionAsync();

        try
        {
            var buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = await audioStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
            {
                pushStream.Write(buffer, bytesRead);
            }
        }
        finally
        {
            pushStream.Close();
        }

        await recognizer.StopContinuousRecognitionAsync();
        await assessmentComplete.Task;

        if (result is null)
        {
            throw new LangAppException("Speech was not recognized or assessment result is missing.");
        }

        var feedback = GenerateDetailedFeedback(result);

        var score = new Percentage(result.PronunciationScore);

        return new SubmissionGrade(score, feedback);
    }

    private static string GenerateDetailedFeedback(PronunciationAssessmentResult result)
    {
        var sb = new StringBuilder("Pronunciation feedback:\n");

        sb.Append("• Accuracy: ");
        sb.Append(result.AccuracyScore switch
        {
            >= 90 => "Excellent accuracy.",
            >= 75 => "Good, but minor mispronunciations.",
            >= 50 => "Some phonetic inaccuracies.",
            _ => "Many words were mispronounced."
        });
        sb.AppendLine();

        sb.Append("• Fluency: ");
        sb.Append(result.FluencyScore switch
        {
            >= 90 => "Speech was smooth and natural.",
            >= 70 => "Fairly fluent, minor hesitations.",
            >= 50 => "Noticeable pauses or pacing issues.",
            _ => "Speech was choppy or slow."
        });
        sb.AppendLine();

        sb.Append("• Completeness: ");
        sb.Append(result.CompletenessScore switch
        {
            >= 95 => "You spoke all expected words.",
            >= 80 => "Some words were omitted or unclear.",
            _ => "Many words were skipped or not detected."
        });

        return sb.ToString();
    }
}
