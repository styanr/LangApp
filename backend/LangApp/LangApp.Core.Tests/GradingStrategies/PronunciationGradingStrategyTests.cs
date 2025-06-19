using LangApp.Core.Exceptions;
using LangApp.Core.Exceptions.Assignments;
using LangApp.Core.Exceptions.Grading;
using LangApp.Core.Services.GradingStrategies;
using LangApp.Core.Services.PronunciationAssessment;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;
using Moq;

namespace LangApp.Core.Tests.GradingStrategyTests;

public class PronunciationGradingStrategyTests
{
    private readonly Mock<IPronunciationAssessmentService> _mockAssessmentService;
    private readonly PronunciationGradingStrategy _gradingStrategy;

    public PronunciationGradingStrategyTests()
    {
        _mockAssessmentService = new Mock<IPronunciationAssessmentService>();
        _gradingStrategy = new PronunciationGradingStrategy(_mockAssessmentService.Object);
    }

    [Fact]
    public async Task Grade_ValidSubmission_ReturnsExpectedGrade()
    {
        // Arrange
        var activity = new PronunciationActivityDetails("The quick brown fox", Language.EnglishUS);
        var submission = new PronunciationSubmissionDetails("https://storage.app/audio/fox.wav");

        var expectedGrade = new SubmissionGrade(new Percentage(92.5));
        _mockAssessmentService
            .Setup(s => s.Assess(submission.FileUri, activity.ReferenceText, activity.Language))
            .ReturnsAsync(expectedGrade);

        // Act
        var result = await _gradingStrategy.GradeAsync(activity, submission);

        // Assert
        Assert.Equal(expectedGrade.ScorePercentage.Value, result.ScorePercentage.Value);
        _mockAssessmentService.Verify(s =>
                s.Assess(submission.FileUri, activity.ReferenceText, activity.Language),
            Times.Once);
    }

    [Fact]
    public async Task Grade_InvalidSubmissionType_ThrowsException()
    {
        // Arrange
        var activity = new PronunciationActivityDetails("Some reference", Language.EnglishUS);
        var invalidSubmission = new SubmissionDetails();

        // Act & Assert
        await Assert.ThrowsAsync<IncompatibleSubmissionTypeException>(() =>
            _gradingStrategy.GradeAsync(activity, invalidSubmission));
    }

    [Fact]
    public void Constructor_EmptyReferenceText_ThrowsException()
    {
        Assert.Throws<InvalidPronunciationActivityReferenceTextException>(() =>
            new PronunciationActivityDetails("", Language.EnglishUS));
    }
}