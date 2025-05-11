using LangApp.Core.Exceptions;
using LangApp.Core.Services.GradingStrategies;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.FillInTheBlank;

namespace LangApp.Core.Tests.GradingStrategyTests;

public class FillInTheBlankGradingStrategyTests
{
    private readonly FillInTheBlankGradingStrategy _gradingStrategy;

    public FillInTheBlankGradingStrategyTests()
    {
        _gradingStrategy = new FillInTheBlankGradingStrategy();
    }

    [Fact]
    public async Task ExecuteGrade_WithAllCorrectAnswers_ShouldReturn100Percent()
    {
        // Arrange
        var activity = CreateSampleActivity();
        var submission = new FillInTheBlankSubmissionDetails([
            new FillInTheBlankSubmissionAnswer(0, "Paris"),
            new FillInTheBlankSubmissionAnswer(1, "Berlin"),
            new FillInTheBlankSubmissionAnswer(2, "Rome")
        ]);

        // Act
        var result = await _gradingStrategy.Grade(activity, submission, CancellationToken.None);

        // Assert
        Assert.Equal(100.0, result.ScorePercentage.Value);
    }

    [Fact]
    public async Task ExecuteGrade_WithPartiallyCorrectAnswers_ShouldReturnCorrectPercentage()
    {
        // Arrange
        var activity = CreateSampleActivity();
        var submission = new FillInTheBlankSubmissionDetails([
            new FillInTheBlankSubmissionAnswer(0, "Paris"),
            new FillInTheBlankSubmissionAnswer(1, "Incorrect"),
            new FillInTheBlankSubmissionAnswer(2, "Rome")
        ]);

        // Act
        var result = await _gradingStrategy.Grade(activity, submission, CancellationToken.None);

        // Assert
        Assert.Equal(66.67, Math.Round(result.ScorePercentage.Value, 2));
    }

    [Fact]
    public async Task ExecuteGrade_WithMissingAnswers_ShouldGradeOnlyProvidedAnswers()
    {
        // Arrange
        var activity = CreateSampleActivity();
        var submission = new FillInTheBlankSubmissionDetails([
            new FillInTheBlankSubmissionAnswer(0, "Paris"),
            new FillInTheBlankSubmissionAnswer(2, "Rome")
        ]);

        // Act
        var result = await _gradingStrategy.Grade(activity, submission, CancellationToken.None);

        // Assert
        // Only 2 out of 3 possible answers were provided, and both were correct
        Assert.Equal(66.67, Math.Round(result.ScorePercentage.Value, 2));
    }

    [Fact]
    public async Task ExecuteGrade_WithCaseInsensitiveMatching_ShouldReturnCorrectPercentage()
    {
        // Arrange
        var activity = CreateSampleActivity();
        var submission = new FillInTheBlankSubmissionDetails([
            new FillInTheBlankSubmissionAnswer(0, "paris"), // Lowercase
            new FillInTheBlankSubmissionAnswer(1, "BERLIN"), // Uppercase
            new FillInTheBlankSubmissionAnswer(2, "Rome")
        ]);

        // Act
        var result = await _gradingStrategy.Grade(activity, submission, CancellationToken.None);

        // Assert
        Assert.Equal(100.0, result.ScorePercentage.Value);
    }

    [Fact]
    public async Task ExecuteGrade_WithWhitespaceInAnswers_ShouldTrimAndEvaluateCorrectly()
    {
        // Arrange
        var activity = CreateSampleActivity();
        var submission = new FillInTheBlankSubmissionDetails([
            new FillInTheBlankSubmissionAnswer(0, "  Paris  "), // Extra spaces
            new FillInTheBlankSubmissionAnswer(1, "Berlin"),
            new FillInTheBlankSubmissionAnswer(2, "Rome ")
        ]);

        // Act
        var result = await _gradingStrategy.Grade(activity, submission, CancellationToken.None);

        // Assert
        Assert.Equal(100.0, result.ScorePercentage.Value);
    }

    [Fact]
    public async Task ExecuteGrade_WithAlternativeAcceptableAnswers_ShouldMarkCorrectly()
    {
        // Arrange
        var activity = CreateSampleActivityWithAlternatives();
        var submission = new FillInTheBlankSubmissionDetails([
            new FillInTheBlankSubmissionAnswer(0, "Paris"),
            new FillInTheBlankSubmissionAnswer(1, "Germany"), // Alternative answer
            new FillInTheBlankSubmissionAnswer(2, "Roma")
        ]);

        // Act
        var result = await _gradingStrategy.Grade(activity, submission, CancellationToken.None);

        // Assert
        Assert.Equal(100.0, result.ScorePercentage.Value);
    }

    [Fact]
    public async Task ExecuteGrade_WithInvalidAnswerIndices_ShouldIgnoreInvalidIndices()
    {
        // Arrange
        var activity = CreateSampleActivity();
        var submission = new FillInTheBlankSubmissionDetails([
            new FillInTheBlankSubmissionAnswer(0, "Paris"),
            new FillInTheBlankSubmissionAnswer(1, "Berlin"),
            new FillInTheBlankSubmissionAnswer(2, "Rome"),
            new FillInTheBlankSubmissionAnswer(3, "Invalid")
        ]);

        // Act
        var result = await _gradingStrategy.Grade(activity, submission, CancellationToken.None);

        // Assert
        Assert.Equal(100.0, result.ScorePercentage.Value);
    }

    [Fact]
    public async Task ExecuteGrade_WithNegativeAnswerIndices_ShouldIgnoreInvalidIndices()
    {
        // Arrange
        var activity = CreateSampleActivity();
        var submission = new FillInTheBlankSubmissionDetails([
            new FillInTheBlankSubmissionAnswer(0, "Paris"),
            new FillInTheBlankSubmissionAnswer(-1, "Invalid"), // Negative index
            new FillInTheBlankSubmissionAnswer(2, "Rome")
        ]);

        // Act
        var result = await _gradingStrategy.Grade(activity, submission, CancellationToken.None);

        // Assert
        Assert.Equal(66.67, Math.Round(result.ScorePercentage.Value, 2));
    }

    [Fact]
    public async Task ExecuteGrade_WithEmptyAnswers_ShouldNotMarkAsCorrect()
    {
        // Arrange
        var activity = CreateSampleActivity();
        var submission = new FillInTheBlankSubmissionDetails([
            new FillInTheBlankSubmissionAnswer(0, "Paris"),
            new FillInTheBlankSubmissionAnswer(1, ""), // Empty answer
            new FillInTheBlankSubmissionAnswer(2, "Rome")
        ]);

        // Act
        var result = await _gradingStrategy.Grade(activity, submission, CancellationToken.None);

        // Assert
        Assert.Equal(66.67, Math.Round(result.ScorePercentage.Value, 2));
    }

    [Fact]
    public async Task ExecuteGrade_WithWhitespaceOnlyAnswers_ShouldNotMarkAsCorrect()
    {
        // Arrange
        var activity = CreateSampleActivity();
        var submission = new FillInTheBlankSubmissionDetails([
            new FillInTheBlankSubmissionAnswer(0, "Paris"),
            new FillInTheBlankSubmissionAnswer(1, "   "), // Whitespace only
            new FillInTheBlankSubmissionAnswer(2, "Rome")
        ]);

        // Act
        var result = await _gradingStrategy.Grade(activity, submission, CancellationToken.None);

        // Assert
        Assert.Equal(66.67, Math.Round(result.ScorePercentage.Value, 2));
    }

    [Fact]
    public async Task ExecuteGrade_WithNullSubmission_ShouldThrowException()
    {
        // Arrange
        var activity = CreateSampleActivity();
        var submission = new SubmissionDetails(); // Not the correct type

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangAppException>(async () =>
            await _gradingStrategy.Grade(activity, submission, CancellationToken.None));

        Assert.Contains("not compatible with the assignment", exception.Message);
    }

    private FillInTheBlankActivityDetails CreateSampleActivity()
    {
        return new FillInTheBlankActivityDetails(
            "The capital of France is _ , the capital of Germany is _ , and the capital of Italy is _ .",
            [
                new FillInTheBlankAnswer(["Paris"]),
                new FillInTheBlankAnswer(["Berlin"]),
                new FillInTheBlankAnswer(["Rome"])
            ]
        );
    }

    private FillInTheBlankActivityDetails CreateSampleActivityWithAlternatives()
    {
        return new FillInTheBlankActivityDetails(
            "The capital of France is _ , the capital of Germany is _ , and the capital of Italy is _ .",
            [
                new FillInTheBlankAnswer(["Paris"]),
                new FillInTheBlankAnswer(["Berlin", "Germany"]),
                new FillInTheBlankAnswer(["Rome", "Roma"])
            ]
        );
    }
}
