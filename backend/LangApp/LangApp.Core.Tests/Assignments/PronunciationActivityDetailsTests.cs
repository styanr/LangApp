using LangApp.Core.Exceptions.Assignments;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;

namespace LangApp.Core.Tests.Assignments;

public class PronunciationActivityDetailsTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var referenceText = "Bonjour";
        var language = Language.French;
        // Act
        var details = new PronunciationActivityDetails(referenceText, language);
        // Assert
        Assert.Equal(referenceText, details.ReferenceText);
        Assert.Equal(language, details.Language);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidReferenceText_ShouldThrow(string invalidText)
    {
        // Arrange
        var language = Language.EnglishUS;
        // Act & Assert
        Assert.Throws<InvalidPronunciationAssignmentDetailsException>(() => new PronunciationActivityDetails(invalidText, language));
    }
}