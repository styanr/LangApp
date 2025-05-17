using System.Text.RegularExpressions;
using DiffPlex;
using DiffPlex.Chunkers;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using LangApp.Core.Services.PronunciationAssessment;
using LangApp.Core.ValueObjects;
using LangApp.Infrastructure.BlobStorage;
using LangApp.Infrastructure.PronunciationAssessment.Audio;
using LangApp.Infrastructure.PronunciationAssessment.Models;
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
    private readonly IAudioFetcher _audioFetcher;

    public PronunciationAssessmentService(
        BlobStorageService blobStorageService,
        IConfiguration configuration,
        ILogger<PronunciationAssessmentService> logger,
        IOptions<SpeechConfigOptions> config, IAudioFetcher audioFetcher)
    {
        _blobStorageService = blobStorageService;
        _configuration = configuration;
        _logger = logger;
        _config = config;
        _audioFetcher = audioFetcher;
    }

    public async Task<SubmissionGrade> Assess(string fileUri, string referenceText, Language language)
    {
        _logger.LogInformation("Assessing audio file {FileUri}", fileUri);
        var speechConfig = SpeechConfig.FromSubscription(_config.Value.SubscriptionKey, _config.Value.Region);

        var stream = await _audioFetcher.FetchAudioStream(fileUri);

        using var pushStream = AudioInputStream.CreatePushStream();
        using var audioInput = AudioConfig.FromStreamInput(pushStream);

        return await ProcessAudioWithPronunciationAssessment(
            speechConfig,
            audioInput,
            language,
            stream.Stream,
            pushStream,
            referenceText);
    }

    private async Task<SubmissionGrade> ProcessAudioWithPronunciationAssessment(
        SpeechConfig speechConfig,
        AudioConfig audioInput,
        Language language,
        Stream audioStream,
        PushAudioInputStream pushStream,
        string referenceText)
    {
        var finalResult = new PronAssessmentResult();
        var recognizedWords = new List<string>();
        var pronWords = new List<Word>();
        var finalWords = new List<Word>();
        var fluencyScores = new List<double>();
        var prosodyScores = new List<double>();
        var durations = new List<int>();
        var processingComplete = new TaskCompletionSource<bool>();

        using var recognizer = new SpeechRecognizer(speechConfig, language.Code, audioInput);

        // Configure pronunciation assessment
        var pronConfig = new PronunciationAssessmentConfig(
            referenceText,
            GradingSystem.HundredMark,
            Granularity.Word,
            enableMiscue: true);

        pronConfig.EnableProsodyAssessment();

        pronConfig.ApplyTo(recognizer);


        recognizer.SessionStopped += (s, e) => { processingComplete.TrySetResult(true); };

        recognizer.Canceled += (s, e) => { processingComplete.TrySetResult(true); };

        recognizer.Recognized += (s, e) =>
        {
            var pronResult = PronunciationAssessmentResult.FromResult(e.Result);

            fluencyScores.Add(pronResult.FluencyScore);
            prosodyScores.Add(pronResult.ProsodyScore);

            pronWords.AddRange(pronResult.Words.Select(word => new Word(word.Word, word.ErrorType, word.AccuracyScore)));

            foreach (var result in e.Result.Best())
            {
                durations.Add(result.Words.Sum(item => item.Duration));
                recognizedWords.AddRange(result.Words.Select(item => item.Word).ToList());
            }
        };

        _logger.LogInformation("Starting pronunciation assessment");
        // Start continuous recognition
        await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

        var buffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = await audioStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
        {
            pushStream.Write(buffer, bytesRead);
        }

        pushStream.Close();

        // Wait for completion
        await processingComplete.Task.ConfigureAwait(false);

        // Stop recognition
        await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
        _logger.LogInformation("Pronunciation assessment complete");
        // Process results and calculate final scores
        finalWords = ProcessFinalWordList(pronWords, recognizedWords, referenceText, true);

        // Calculate scores
        var scores = CalculateFinalScores(finalWords, fluencyScores, prosodyScores, durations, referenceText);

        // return new PronAssessmentResult
        // {
        //     AccuracyScore = scores.accuracyScore,
        //     CompletenessScore = scores.completenessScore,
        //     FluencyScore = scores.fluencyScore,
        //     ProsodyScore = scores.prosodyScore,
        //     PronunciationScore = scores.pronunciationScore,
        //     Words = finalWords
        // };

        return new SubmissionGrade(
            new Percentage(scores.accuracyScore),
            "Accuracy Score: " + scores.accuracyScore + "%"
        );
    }

    private List<Word> ProcessFinalWordList(
        List<Word> pronWords,
        List<string> recognizedWords,
        string referenceText,
        bool enableMiscue)
    {
        _logger.LogInformation("Processing final word list");
        var finalWords = new List<Word>();

        // Process reference text
        string[] referenceWords = referenceText.ToLower().Split(' ');
        for (int j = 0; j < referenceWords.Length; j++)
        {
            referenceWords[j] = Regex.Replace(referenceWords[j], @"^[\p{P}\s]+|[\p{P}\s]+$", "");
        }

        if (enableMiscue)
        {
            _logger.LogInformation("Processing miscue");
            var differ = new Differ();
            var inlineBuilder = new InlineDiffBuilder(differ);
            var diffModel =
                inlineBuilder.BuildDiffModel(string.Join("\n", referenceWords), string.Join("\n", recognizedWords),
                    ignoreWhitespace: true, ignoreCase: true, new LineChunker());

            int currentIdx = 0;

            foreach (var delta in diffModel.Lines)
            {
                if (delta.Type == ChangeType.Unchanged)
                {
                    finalWords.Add(pronWords[currentIdx]);
                    currentIdx += 1;
                }

                if (delta.Type == ChangeType.Deleted || delta.Type == ChangeType.Modified)
                {
                    var word = new Word(delta.Text, "Omission");
                    finalWords.Add(word);
                }

                if (delta.Type == ChangeType.Inserted || delta.Type == ChangeType.Modified)
                {
                    if (currentIdx < pronWords.Count)
                    {
                        Word w = pronWords[currentIdx];
                        if (w.ErrorType == "None")
                        {
                            w.ErrorType = "Insertion";
                            finalWords.Add(w);
                        }

                        currentIdx += 1;
                    }
                }
            }

            _logger.LogInformation("Miscue processed");
        }
        else
        {
            finalWords = pronWords;
        }

        _logger.LogInformation("Final word list processed");

        return finalWords;
    }

    private (double accuracyScore, double completenessScore, double fluencyScore,
        double prosodyScore, double pronunciationScore)
        CalculateFinalScores(
            List<Word> finalWords,
            List<double> fluencyScores,
            List<double> prosodyScores,
            List<int> durations,
            string referenceText)
    {
        string[] referenceWords = referenceText.ToLower().Split(' ');

        // Calculate accuracy score
        var filteredWords = finalWords.Where(item => item.ErrorType != "Insertion");
        var wordsList = filteredWords.ToList();
        var accuracyScore = wordsList.Count != 0
            ? wordsList.Sum(item => item.AccuracyScore) / wordsList.Count
            : 0;

        // Calculate prosody score
        var prosodyScore = prosodyScores.Count != 0
            ? prosodyScores.Sum() / prosodyScores.Count
            : 0;

        // Calculate fluency score
        var fluencyScore = 0.0;
        if (fluencyScores.Any() && durations.Any())
        {
            fluencyScore = fluencyScores.Zip(durations, (x, y) => x * y).Sum() / durations.Sum();
        }

        // Calculate completeness score
        var completenessScore = referenceWords.Length > 0
            ? (double)finalWords.Count(item => item.ErrorType == "None") / referenceWords.Length * 100
            : 0;

        completenessScore = completenessScore <= 100 ? completenessScore : 100;

        // Calculate overall pronunciation score
        var pronunciationScore =
            accuracyScore * 0.4 + prosodyScore * 0.2 + fluencyScore * 0.2 + completenessScore * 0.2;

        return (accuracyScore, completenessScore, fluencyScore, prosodyScore, pronunciationScore);
    }
}
