using LangApp.Core.Exceptions;
using LangApp.Core.Services.GradingStrategies;
using LangApp.Core.ValueObjects.Assignments.Question;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.Question;

namespace LangApp.Core.Tests.GradingStrategyTests;

public class QuestionGradingStrategyTests
{
    private readonly QuestionGradingStrategy _gradingStrategy = new();

    [Fact]
    public async Task Grade_WithExactMatchingAnswer_ShouldReturn100Percent()
    {
        // Arrange
        var activity = CreateSampleActivity(["Correct Answer"]);
        var submission = new QuestionSubmissionDetails("Correct Answer");

        // Act
        var result = await _gradingStrategy.Grade(activity, submission);

        // Assert
        Assert.Equal(100, result.ScorePercentage.Value);
    }

    [Fact]
    public async Task Grade_WithCaseInsensitiveMatch_ShouldReturn100Percent()
    {
        // Arrange
        var activity = CreateSampleActivity(["Correct Answer"]);
        var submission = new QuestionSubmissionDetails("correct answer");

        // Act
        var result = await _gradingStrategy.Grade(activity, submission);

        // Assert
        Assert.Equal(100, result.ScorePercentage.Value);
    }

    [Fact]
    public async Task Grade_WithTrimmedMatch_ShouldReturn100Percent()
    {
        // Arrange
        var activity = CreateSampleActivity(["Correct Answer"]);
        var submission = new QuestionSubmissionDetails("  Correct Answer  ");

        // Act
        var result = await _gradingStrategy.Grade(activity, submission);

        // Assert
        Assert.Equal(100, result.ScorePercentage.Value);
    }

    [Fact]
    public async Task Grade_WithIncorrectAnswer_ShouldReturn0Percent()
    {
        // Arrange
        var activity = CreateSampleActivity(["Correct Answer"]);
        var submission = new QuestionSubmissionDetails("Wrong Answer");

        // Act
        var result = await _gradingStrategy.Grade(activity, submission);

        // Assert
        Assert.Equal(0, result.ScorePercentage.Value);
    }

    [Fact]
    public async Task Grade_WithMultipleAcceptableAnswers_ShouldAcceptAnyMatch()
    {
        // Arrange
        var activity = CreateSampleActivity(["Answer A", "Answer B", "Answer C"]);
        var submission = new QuestionSubmissionDetails("Answer B");

        // Act
        var result = await _gradingStrategy.Grade(activity, submission);

        // Assert
        Assert.Equal(100, result.ScorePercentage.Value);
    }

    [Fact]
    public async Task Grade_WithIncompatibleSubmission_ShouldThrow()
    {
        // Arrange
        var activity = CreateSampleActivity(["Answer"]);
        var incompatibleSubmission = new SubmissionDetails();

        // Act & Assert
        await Assert.ThrowsAsync<LangAppException>(() =>
            _gradingStrategy.Grade(activity, incompatibleSubmission));
    }

    private QuestionActivityDetails CreateSampleActivity(List<string> answers)
    {
        return new QuestionActivityDetails(
            "What is the correct answer?",
            answers,
            maxLength: 100
        );
    }
}
