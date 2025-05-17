using LangApp.Core.Entities.Assignments;
using LangApp.Core.Enums;
using LangApp.Core.Exceptions.Assignments;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Assignments.Writing;
using LangApp.Core.ValueObjects.Assignments.Question;

namespace LangApp.Core.Tests.Assignments;

public class ActivityTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateActivityWithExpectedProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var details = new WritingActivityDetails("Write an essay", 200);
        const int maxScore = 10;
        const ActivityType type = ActivityType.Writing;

        // Act
        var activity = new Activity(id, details, maxScore, type);

        // Assert
        Assert.Equal(id, activity.Id);
        Assert.Equal(details, activity.Details);
        Assert.Equal(maxScore, activity.MaxScore);
        Assert.Equal(type, activity.Type);
        Assert.Equal(0, activity.Order); // Default order value
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WithInvalidMaxScore_ShouldThrowAssignmentMaxScoreInvalidException(int invalidMaxScore)
    {
        // Arrange
        var id = Guid.NewGuid();
        var details = new QuestionActivityDetails("What is the capital of France?", new List<string> { "Paris" }, 50);
        const ActivityType type = ActivityType.Question;

        // Act & Assert
        var exception = Assert.Throws<AssignmentMaxScoreInvalidException>(() =>
            new Activity(id, details, invalidMaxScore, type));

        Assert.Equal(invalidMaxScore, exception.MaxScore);
    }

    [Fact]
    public void UpdateOrder_ShouldChangeOrderValue()
    {
        // Arrange
        var activity = new Activity(
            Guid.NewGuid(),
            new WritingActivityDetails("Write about your day", 150),
            10,
            ActivityType.Writing
        );
        const int newOrder = 5;

        // Act
        activity.UpdateOrder(newOrder);

        // Assert
        Assert.Equal(newOrder, activity.Order);
    }
}