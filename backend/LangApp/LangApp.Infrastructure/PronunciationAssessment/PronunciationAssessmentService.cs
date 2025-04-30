using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using LangApp.Core.Services.PronunciationAssessment;
using LangApp.Core.ValueObjects;
using LangApp.Infrastructure.BlobStorage;
using LangApp.Infrastructure.PronunciationAssessment.Options;
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
        IConfiguration configuration, ILogger<PronunciationAssessmentService> logger,
        IOptions<SpeechConfigOptions> config)
    {
        _blobStorageService = blobStorageService;
        _configuration = configuration;
        _logger = logger;
        _config = config;
    }

    public async Task<Percentage> Assess(string fileUri, string referenceText, Language language)
    {
        var blobName = new Uri(fileUri).Segments.Last();
        var containerName = new Uri(fileUri).Segments.ElementAt(1);

        await using var audioStream = await _blobStorageService.DownloadFileAsync(containerName, blobName);

        var speechConfig = SpeechConfig.FromSubscription(_config.Value.SubscriptionKey, _config.Value.Region);

        var pronunciationConfig = new PronunciationAssessmentConfig(referenceText, GradingSystem.HundredMark,
            Granularity.Word, enableMiscue: false);

        using var pushStream = AudioInputStream.CreatePushStream();
        using var audioInput = AudioConfig.FromStreamInput(pushStream);
        using var recognizer = new SpeechRecognizer(speechConfig, language.Value, audioInput);

        pronunciationConfig.ApplyTo(recognizer);

        PronunciationAssessmentResult? pronunciationResult = null;
        TaskCompletionSource<bool> assessmentComplete = new();

        recognizer.Recognized += (s, e) =>
        {
            _logger.LogInformation("Speech recognized with text: {Text}", e.Result.Text);

            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                var result = PronunciationAssessmentResult.FromResult(e.Result);

                if (result is null)
                {
                    _logger.LogWarning("Pronunciation result was null for recognized text: {Text}", e.Result.Text);
                    return;
                }

                _logger.LogInformation("Pronunciation score: {PronunciationScore}", result.PronunciationScore);
                pronunciationResult = result;
            }
            else
            {
                _logger.LogWarning("Speech not recognized. Reason: {Reason}", e.Result.Reason);
            }
        };

        recognizer.SessionStopped += (s, e) =>
        {
            _logger.LogInformation("Speech recognition session stopped");
            if (!assessmentComplete.Task.IsCompleted)
            {
                assessmentComplete.SetResult(true);
            }
        };

        recognizer.Canceled += (s, e) =>
        {
            _logger.LogWarning("Speech recognition canceled. Reason: {Reason}, Error Details: {ErrorDetails}",
                e.Reason, e.ErrorDetails);

            if (e.Reason != CancellationReason.EndOfStream)
            {
                assessmentComplete.SetException(new Exception($"Recognition canceled: {e.ErrorDetails}"));
            }
            else
            {
                assessmentComplete.SetResult(true);
            }
        };

        await recognizer.StartContinuousRecognitionAsync();

        byte[] buffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = await audioStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            pushStream.Write(buffer, bytesRead);
        }

        pushStream.Close();
        await recognizer.StopContinuousRecognitionAsync();

        if (pronunciationResult is null)
        {
            throw new Exception("No pronunciation result was obtained");
        }

        return new Percentage(pronunciationResult.PronunciationScore);
    }
}
