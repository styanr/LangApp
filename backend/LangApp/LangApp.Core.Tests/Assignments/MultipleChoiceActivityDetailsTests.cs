using LangApp.Core.ValueObjects.Assignments.MultipleChoice;

namespace LangApp.Core.Tests.Assignments;

public class MultipleChoiceActivityDetailsTests
{
    [Fact]
    public void Create_WithValidQuestions_ShouldSetProperties()
    {
        // Arrange
        var options1 = new List<MultipleChoiceOption> { new("A"), new("B") };
        var options2 = new List<MultipleChoiceOption> { new("C"), new("D") };
        var questions = new List<MultipleChoiceQuestion>
        {
            new MultipleChoiceQuestion("Q1", options1, 0),
            new MultipleChoiceQuestion("Q2", options2, 1)
        };
        // Act
        var details = new MultipleChoiceActivityDetails(questions);
        // Assert
        Assert.Equal(questions, details.Questions);
    }
}