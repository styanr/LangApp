using System.Text.Json;
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
    private readonly ILogger<PronunciationAssessmentService> _logger;
    private readonly IOptions<SpeechConfigOptions> _config;
    private readonly IAudioFetcher _audioFetcher;

    public PronunciationAssessmentService(
        BlobStorageService blobStorageService,
        IConfiguration configuration,
        ILogger<PronunciationAssessmentService> logger,
        IOptions<SpeechConfigOptions> config, IAudioFetcher audioFetcher)
    {
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

        bool enableProsody = language.Code == Language.EnglishUS.Code;

        return await ProcessAudioWithPronunciationAssessment(
            speechConfig,
            audioInput,
            language,
            stream.Stream,
            pushStream,
            referenceText,
            enableProsody);
    }

    private async Task<SubmissionGrade> ProcessAudioWithPronunciationAssessment(
        SpeechConfig speechConfig,
        AudioConfig audioInput,
        Language language,
        Stream audioStream,
        PushAudioInputStream pushStream,
        string referenceText,
        bool enableProsody)
    {
        var finalResult = new PronAssessmentResult();
        var recognizedWords = new List<string>();
        var pronWords = new List<Word>();
        var fluencyScores = new List<double>();
        var prosodyScores = new List<double>();
        var durations = new List<int>();
        var processingComplete = new TaskCompletionSource<bool>();

        using var recognizer = new SpeechRecognizer(speechConfig, language.Code, audioInput);

        var pronConfig = new PronunciationAssessmentConfig(
            referenceText,
            GradingSystem.HundredMark,
            Granularity.Phoneme,
            enableMiscue: true);

        if (enableProsody)
        {
            pronConfig.EnableProsodyAssessment();
        }

        pronConfig.ApplyTo(recognizer);


        recognizer.SessionStopped += (s, e) => { processingComplete.TrySetResult(true); };

        recognizer.Canceled += (s, e) => { processingComplete.TrySetResult(true); };

        recognizer.Recognized += (s, e) =>
        {
            var pronResult = PronunciationAssessmentResult.FromResult(e.Result);

            if (pronResult == null)
            {
                _logger.LogWarning("Pronunciation assessment result is null");
                return;
            }

            fluencyScores.Add(pronResult.FluencyScore);
            prosodyScores.Add(pronResult.ProsodyScore);

            pronWords.AddRange(pronResult.Words.Select(word =>
                new Word(word.Word, word.ErrorType, word.AccuracyScore)));

            foreach (var words in e.Result.Best().Select(w => w.Words))
            {
                var wordList = words.ToList();
                durations.Add(wordList.Sum(item => item.Duration));
                recognizedWords.AddRange(wordList.Select(item => item.Word).ToList());
            }
        };

        _logger.LogInformation("Starting pronunciation assessment");
        await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

        var buffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = await audioStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
        {
            pushStream.Write(buffer, bytesRead);
        }

        pushStream.Close();

        await processingComplete.Task.ConfigureAwait(false);

        await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
        _logger.LogInformation("Pronunciation assessment complete");
        var finalWords = ProcessFinalWordList(pronWords, recognizedWords, referenceText, true);

        var scores = CalculateFinalScores(finalWords, fluencyScores, prosodyScores, durations, referenceText,
            enableProsody);

        _logger.LogInformation("Final scores calculated: {Scores}", scores);
        return new SubmissionGrade(
            new Percentage(scores.pronunciationScore),
            JsonSerializer.Serialize(finalWords)
        );
    }

    private List<Word> ProcessFinalWordList(
        List<Word> pronWords,
        List<string> recognizedWords,
        string referenceText,
        bool enableMiscue)
    {
        _logger.LogInformation("Processing final word list");

        var referenceWords = CleanReferenceText(referenceText);

        var finalWords = enableMiscue
            ? ProcessWithMiscue(pronWords, recognizedWords, referenceWords)
            : [.. pronWords];

        _logger.LogInformation("Final word list processed");
        return finalWords;
    }

    private static string[] CleanReferenceText(string text)
    {
        return text.ToLower()
            .Split(' ')
            .Select(w => Regex.Replace(w, @"^[\p{P}\s]+|[\p{P}\s]+$", "", RegexOptions.None,
                TimeSpan.FromSeconds(1.5)))
            .ToArray();
    }

    private List<Word> ProcessWithMiscue(
        List<Word> pronWords,
        List<string> recognizedWords,
        string[] referenceWords)
    {
        _logger.LogInformation("Processing miscue");

        var differ = new Differ();
        var inlineBuilder = new InlineDiffBuilder(differ);
        var diffModel = inlineBuilder.BuildDiffModel(
            string.Join("\n", referenceWords),
            string.Join("\n", recognizedWords),
            ignoreWhitespace: true,
            ignoreCase: true,
            new LineChunker());

        var finalWords = new List<Word>();
        int currentIdx = 0;

        foreach (var delta in diffModel.Lines)
        {
            switch (delta.Type)
            {
                case ChangeType.Unchanged:
                    finalWords.Add(pronWords[currentIdx]);
                    currentIdx++;
                    break;

                case ChangeType.Deleted:
                    finalWords.Add(new Word(delta.Text, "Omission"));
                    break;

                case ChangeType.Inserted:
                    if (currentIdx < pronWords.Count)
                    {
                        var insertedWord = pronWords[currentIdx];
                        if (insertedWord.ErrorType == "None")
                        {
                            insertedWord.ErrorType = "Insertion";
                            finalWords.Add(insertedWord);
                        }

                        currentIdx++;
                    }

                    break;

                case ChangeType.Modified:
                    finalWords.Add(new Word(delta.Text, "Omission"));
                    if (currentIdx < pronWords.Count)
                    {
                        var modifiedWord = pronWords[currentIdx];
                        if (modifiedWord.ErrorType == "None")
                        {
                            modifiedWord.ErrorType = "Insertion";
                            finalWords.Add(modifiedWord);
                        }

                        currentIdx++;
                    }

                    break;
            }
        }

        _logger.LogInformation("Miscue processed");
        return finalWords;
    }


    private static (double accuracyScore, double completenessScore, double fluencyScore,
        double prosodyScore, double pronunciationScore)
        CalculateFinalScores(
            List<Word> finalWords,
            List<double> fluencyScores,
            List<double> prosodyScores,
            List<int> durations,
            string referenceText,
            bool enableProsody)
    {
        string[] referenceWords = referenceText.ToLower().Split(' ');

        var filteredWords = finalWords.Where(item => item.ErrorType != "Insertion");
        var wordsList = filteredWords.ToList();
        var accuracyScore = wordsList.Count != 0
            ? wordsList.Sum(item => item.AccuracyScore) / wordsList.Count
            : 0;

        var prosodyScore = prosodyScores.Count != 0
            ? prosodyScores.Sum() / prosodyScores.Count
            : 0;

        var fluencyScore = 0.0;
        if (fluencyScores.Any() && durations.Any())
        {
            fluencyScore = fluencyScores.Zip(durations, (x, y) => x * y).Sum() / durations.Sum();
        }

        var completenessScore = referenceWords.Length > 0
            ? (double)finalWords.Count(item => item.ErrorType == "None") / referenceWords.Length * 100
            : 0;

        completenessScore = completenessScore <= 100 ? completenessScore : 100;

        // Calculate overall pronunciation score
        // the weights are taken from microsoft documentation, modified a bit to make completeness more significant
        // https://github.com/Azure-Samples/cognitive-services-speech-sdk/blob/master/samples/csharp/sharedcontent/console/speech_recognition_samples.cs
        var scoreComponents = new Dictionary<string, (double score, double weight)>
        {
            ["Accuracy"] = (accuracyScore, enableProsody ? 0.3 : 0.5),
            ["Completeness"] = (completenessScore, 0.3),
            ["Fluency"] = (fluencyScore, 0.2)
        };

        // Add prosody component only when enabled
        if (enableProsody)
        {
            scoreComponents["Prosody"] = (prosodyScore, 0.2);
        }

        var pronunciationScore = scoreComponents.Sum(component =>
            component.Value.score * component.Value.weight);

        return (accuracyScore, completenessScore, fluencyScore, prosodyScore, pronunciationScore);
    }
}