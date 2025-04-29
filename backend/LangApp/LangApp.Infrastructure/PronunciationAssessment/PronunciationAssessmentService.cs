using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using LangApp.Core.Services.PronunciationAssessment;
using LangApp.Core.ValueObjects;
using LangApp.Infrastructure.BlobStorage;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LangApp.Infrastructure.PronunciationAssessment;

public class PronunciationAssessmentService : IPronunciationAssessmentService
{
    private readonly BlobStorageService _blobStorageService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PronunciationAssessmentService> _logger;

    public PronunciationAssessmentService(
        BlobStorageService blobStorageService,
        IConfiguration configuration, ILogger<PronunciationAssessmentService> logger)
    {
        _blobStorageService = blobStorageService;
        _configuration = configuration;
        _logger = logger;
    }

    // TODO cleanup, no business logic here
    public async Task<SubmissionGrade> Assess(string fileUri, string referenceText, Language language)
    {
        var subscriptionKey = _configuration["Azure:Speech:SubscriptionKey"]
                              ?? throw new InvalidOperationException("Azure Speech SubscriptionKey is not configured");
        var region = _configuration["Azure:Speech:Region"]
                     ?? throw new InvalidOperationException("Azure Speech Region is not configured");
        var blobName = new Uri(fileUri).Segments.Last();
        var containerName = new Uri(fileUri).Segments.ElementAt(1);

        await using var audioStream = await _blobStorageService.DownloadFileAsync(containerName, blobName);

        var speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);

        var pronunciationConfig = new PronunciationAssessmentConfig(referenceText, GradingSystem.HundredMark,
            Granularity.Word, enableMiscue: false);

        using var pushStream = AudioInputStream.CreatePushStream();

        using var audioInput = AudioConfig.FromStreamInput(pushStream);

        // todo language
        using var recognizer = new SpeechRecognizer(speechConfig, "fr-FR", audioInput);

        pronunciationConfig.ApplyTo(recognizer);

        List<PronunciationAssessmentResult> results = [];
        TaskCompletionSource<bool> assessmentComplete = new TaskCompletionSource<bool>();

        recognizer.Recognized += (s, e) =>
        {
            _logger.LogInformation("Speech recognized with text: {Text}", e.Result.Text);
            
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                var pronunciationResult = PronunciationAssessmentResult.FromResult(e.Result);

                if (pronunciationResult is null)
                {
                    _logger.LogWarning("Pronunciation result was null for recognized text: {Text}", e.Result.Text);
                    return;
                }
                
                _logger.LogInformation(
                    "Pronunciation scores - Accuracy: {AccuracyScore}, Fluency: {FluencyScore}, Completeness: {CompletenessScore}, Pronunciation: {PronunciationScore}",
                    pronunciationResult.AccuracyScore,
                    pronunciationResult.FluencyScore,
                    pronunciationResult.CompletenessScore,
                    pronunciationResult.PronunciationScore);

                results.Add(pronunciationResult);
            }
            else
            {
                _logger.LogWarning("Speech not recognized. Reason: {Reason}", e.Result.Reason);
            }
        };

        recognizer.SessionStopped += (s, e) =>
        {
            _logger.LogInformation("Speech recognition session stopped");
            if (assessmentComplete.Task.IsCompleted == false)
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

        // Push audio data in chunks
        byte[] buffer = new byte[1024];
        int bytesRead;
        Console.WriteLine($"File size: {audioStream.Length} bytes");
        while ((bytesRead = await audioStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            pushStream.Write(buffer, bytesRead);
        }

        pushStream.Close();


        await recognizer.StopContinuousRecognitionAsync();

        var scoreData = CalculateAggregateScores(results);

        var score = new Percentage(scoreData.OverallScore);
        
        var feedback = GenerateFeedback(results, referenceText);

        return new SubmissionGrade(score, feedback);
    }

    private ScoreData CalculateAggregateScores(List<PronunciationAssessmentResult> results)
    {
        // TODO ALGO
        if (results.Count == 0)
            return new ScoreData(0, 0, 0, 0);
        
        double accuracySum = 0;
        double fluencySum = 0;
        double completenessSum = 0;
        double pronScoreSum = 0;

        foreach (var result in results)
        {
            accuracySum += result.AccuracyScore;
            fluencySum += result.FluencyScore;
            completenessSum += result.CompletenessScore;
            pronScoreSum += result.PronunciationScore;
        }

        int count = results.Count;
        double accuracyScore = accuracySum / count;
        double fluencyScore = fluencySum / count;
        double completenessScore = completenessSum / count;
        double pronScore = pronScoreSum / count;

        double overallScore = (accuracyScore * 0.4) + (fluencyScore * 0.3) +
                              (completenessScore * 0.2) + (pronScore * 0.1);

        return new ScoreData(
            OverallScore: overallScore,
            AccuracyScore: accuracyScore,
            FluencyScore: fluencyScore,
            CompletenessScore: completenessScore
        );
    }

    private string GenerateFeedback(List<PronunciationAssessmentResult> results, string referenceText)
    {
        // TODO Generate feedback based on scores
        return "good job";
    }
}

record ScoreData(
    double OverallScore,
    double AccuracyScore,
    double FluencyScore,
    double CompletenessScore
);
