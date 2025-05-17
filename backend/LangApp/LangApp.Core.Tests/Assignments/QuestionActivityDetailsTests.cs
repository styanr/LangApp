using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects.Assignments.Question;

namespace LangApp.Core.Tests.Assignments;

public class QuestionActivityDetailsTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateWithExpectedProperties()
    {
        // Arrange
        const string question = "What is the capital of France?";
        var answers = new List<string> { "Paris" };
        const int maxLength = 50;

        // Act
        var details = new QuestionActivityDetails(question, answers, maxLength);

        // Assert
        Assert.Equal(question, details.Question);
        Assert.Equal(answers, details.Answers);
        Assert.Equal(maxLength, details.MaxLength);
        Assert.True(details.CanBeGradedAutomatically); // Default for QuestionActivityDetails
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidQuestion_ShouldThrowException(string invalidQuestion)
    {
        // Arrange
        var answers = new List<string> { "Paris" };

        // Act & Assert
        var exception = Assert.Throws<LangAppException>(() =>
            new QuestionActivityDetails(invalidQuestion, answers, 50));

        Assert.Equal("Question cannot be empty", exception.Message);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(200)]
    public void Create_WithInvalidMaxLength_ShouldThrowException(int invalidMaxLength)
    {
        // Arrange
        const string question = "What is the capital of France?";
        var answers = new List<string> { "Paris" };

        // Act & Assert
        var exception = Assert.Throws<LangAppException>(() =>
            new QuestionActivityDetails(question, answers, invalidMaxLength));

        Assert.Equal("Max length must be between 0 and 100", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Create_WithValidMaxLength_ShouldCreateSuccessfully(int validMaxLength)
    {
        // Arrange
        const string question = "What is the capital of France?";
        var answers = new List<string> { "Paris" };

        // Act
        var details = new QuestionActivityDetails(question, answers, validMaxLength);

        // Assert
        Assert.Equal(validMaxLength, details.MaxLength);
    }
}