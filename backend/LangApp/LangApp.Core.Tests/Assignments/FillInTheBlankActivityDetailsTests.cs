using LangApp.Core.Exceptions.Assignments;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;

namespace LangApp.Core.Tests.Assignments;

public class FillInTheBlankActivityDetailsTests
{
    [Fact]
    public void Create_WithValidTemplateAndAnswers_ShouldSetProperties()
    {
        // Arrange
        var template = "The _ is blue.";
        var answers = new List<FillInTheBlankAnswer> { new(new List<string> { "sky" }) };
        // Act
        var details = new FillInTheBlankActivityDetails(template, answers);
        // Assert
        Assert.Equal(template, details.TemplateText);
        Assert.Equal(answers, details.Answers);
    }

    [Fact]
    public void Create_WithNullTemplate_ShouldThrow()
    {
        // Arrange
        var answers = new List<FillInTheBlankAnswer> { new(new List<string> { "sky" }) };
        // Act & Assert
        Assert.Throws<InvalidFillInTheBlankQuestionText>(() => new FillInTheBlankActivityDetails(null, answers));
    }

    [Fact]
    public void Create_WithNoUnderscoreInTemplate_ShouldThrow()
    {
        // Arrange
        var answers = new List<FillInTheBlankAnswer> { new(new List<string> { "sky" }) };
        // Act & Assert
        Assert.Throws<InvalidFillInTheBlankQuestionText>(() => new FillInTheBlankActivityDetails("No blank here.", answers));
    }

    [Fact]
    public void Create_WithMismatchedAnswersCount_ShouldThrow()
    {
        // Arrange
        var template = "The _ is _.";
        var answers = new List<FillInTheBlankAnswer> { new(new List<string> { "sky" }) };
        // Act & Assert
        Assert.Throws<InvalidFillInTheBlankQuestionAnswers>(() => new FillInTheBlankActivityDetails(template, answers));
    }
}