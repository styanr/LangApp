using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects.Assignments.Writing;

namespace LangApp.Core.Tests.Assignments;

public class WritingActivityDetailsTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateWithExpectedProperties()
    {
        // Arrange
        const string prompt = "Write an essay about your favorite book";
        const int maxWords = 200;

        // Act
        var details = new WritingActivityDetails(prompt, maxWords);

        // Assert
        Assert.Equal(prompt, details.Prompt);
        Assert.Equal(maxWords, details.MaxWords);
        Assert.False(details.CanBeGradedAutomatically);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidPrompt_ShouldThrowException(string invalidPrompt)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<LangAppException>(() =>
            new WritingActivityDetails(invalidPrompt, 200));

        Assert.Equal("Prompt cannot be empty", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(9)]
    [InlineData(501)]
    [InlineData(1000)]
    public void Create_WithInvalidMaxWords_ShouldThrowException(int invalidMaxWords)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<LangAppException>(() =>
            new WritingActivityDetails("Write an essay", invalidMaxWords));

        Assert.Equal("Max words must be between 10 and 500", exception.Message);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(250)]
    [InlineData(500)]
    public void Create_WithValidMaxWords_ShouldCreateSuccessfully(int validMaxWords)
    {
        // Arrange & Act
        var details = new WritingActivityDetails("Write an essay", validMaxWords);

        // Assert
        Assert.Equal(validMaxWords, details.MaxWords);
    }
}