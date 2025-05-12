using LangApp.Core.Exceptions;
using LangApp.Core.Services.GradingStrategies;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;

namespace LangApp.Core.Tests.GradingStrategyTests;

using Xunit;

public class MultipleChoiceGradingStrategyTests
{
    private readonly MultipleChoiceGradingStrategy _gradingStrategy = new();

    [Fact]
    public async Task Grade_AllAnswersCorrect_ShouldReturnFullScore()
    {
        var activity = CreateSampleActivity();
        var submission = new MultipleChoiceSubmissionDetails([
            new(0, 2),
            new(1, 0),
            new(2, 1)
        ]);

        var result = await _gradingStrategy.Grade(activity, submission);

        Assert.Equal(100.0, result.ScorePercentage.Value, 2);
    }

    [Fact]
    public async Task Grade_SomeAnswersIncorrect_ShouldReturnPartialScore()
    {
        var activity = CreateSampleActivity();
        var submission = new MultipleChoiceSubmissionDetails([
            new(0, 1), // Incorrect
            new(1, 0), // Correct
            new(2, 3)
        ]);

        var result = await _gradingStrategy.Grade(activity, submission);

        Assert.Equal(33.33, Math.Round(result.ScorePercentage.Value, 2));
    }

    [Fact]
    public async Task Grade_EmptyActivityQuestions_ShouldThrow()
    {
        var activity = new MultipleChoiceActivityDetails([]);
        var submission = new MultipleChoiceSubmissionDetails([]);

        await Assert.ThrowsAsync<LangAppException>(() =>
            _gradingStrategy.Grade(activity, submission));
    }

    [Fact]
    public async Task Grade_IncorrectSubmissionType_ShouldThrow()
    {
        var activity = CreateSampleActivity();
        var invalidSubmission = new SubmissionDetails();

        await Assert.ThrowsAsync<LangAppException>(() =>
            _gradingStrategy.Grade(activity, invalidSubmission));
    }

    [Fact]
    public async Task Grade_AnswerWithInvalidQuestionIndex_ShouldIgnoreIt()
    {
        var activity = CreateSampleActivity();
        var submission = new MultipleChoiceSubmissionDetails([
            new(0, 2), // Correct
            new(99, 1), // Invalid index
            new(-1, 0)
        ]);

        var result = await _gradingStrategy.Grade(activity, submission);

        Assert.Equal(33.33, Math.Round(result.ScorePercentage.Value, 2));
    }

    [Fact]
    public async Task Grade_EmptySubmission_ShouldReturnZeroScore()
    {
        var activity = CreateSampleActivity();
        var submission = new MultipleChoiceSubmissionDetails([]);

        var result = await _gradingStrategy.Grade(activity, submission);

        Assert.Equal(0, result.ScorePercentage.Value);
    }

    [Fact]
    public void Submission_WithDuplicateIndexes_ShouldThrow()
    {
        Assert.Throws<LangAppException>(() =>
            new MultipleChoiceSubmissionDetails([
                new(0, 1),
                new(0, 2)
            ]));
    }

    private MultipleChoiceActivityDetails CreateSampleActivity()
    {
        return new MultipleChoiceActivityDetails([
            new("What is the capital of France?", [
                new("Lyon"),
                new("Marseille"),
                new("Paris"), // Correct
                new("Nice")
            ], correctOptionIndex: 2),


            new("What is the capital of Germany?", [
                new("Berlin"), // Correct
                new("Munich"),
                new("Hamburg")
            ], correctOptionIndex: 0),


            new("What is the capital of Italy?", [
                new("Milan"),
                new("Rome"), // Correct
                new("Venice"),
                new("Florence")
            ], correctOptionIndex: 1)
        ]);
    }
}
