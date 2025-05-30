using LangApp.Core.Entities.Assignments;
using LangApp.Core.Exceptions;
using LangApp.Core.Enums;
using LangApp.Core.Exceptions.Assignments;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;
using LangApp.Core.ValueObjects.Assignments.Writing;
using LangApp.Core.ValueObjects.Assignments.Question;

namespace LangApp.Core.Tests.Assignments;

public class AssignmentTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateAssignmentWithExpectedProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test Assignment";
        var description = "Test Description";
        var authorId = Guid.NewGuid();
        var studyGroupId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(7);

        // Act
        var assignment = Assignment.Create(id, name, description, authorId, studyGroupId, dueDate);

        // Assert
        Assert.Equal(id, assignment.Id);
        Assert.Equal(name, assignment.Name);
        Assert.Equal(description, assignment.Description);
        Assert.Equal(authorId, assignment.AuthorId);
        Assert.Equal(studyGroupId, assignment.StudyGroupId);
        Assert.Equal(dueDate, assignment.DueDate);
        Assert.Empty(assignment.Activities);
    }

    [Fact]
    public void Create_WithActivities_ShouldCreateAssignmentWithActivities()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test Assignment";
        var description = "Test Description";
        var authorId = Guid.NewGuid();
        var studyGroupId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(7);

        var activities = new List<Activity>
        {
            new Activity(
                Guid.NewGuid(),
                new WritingActivityDetails("Write an essay about your favorite book", 300),
                10,
                ActivityType.Writing
            ),
            new Activity(
                Guid.NewGuid(),
                new QuestionActivityDetails("What is the capital of France?", new List<string> { "Paris" }, 50),
                20,
                ActivityType.Question
            )
        };

        // Act
        var assignment = Assignment.Create(id, name, description, authorId, studyGroupId, dueDate, activities);

        // Assert
        Assert.Equal(2, assignment.Activities.Count);
        Assert.Equal(0, assignment.Activities[0].Order);
        Assert.Equal(1, assignment.Activities[1].Order);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrowException(string invalidName)
    {
        // Arrange
        var id = Guid.NewGuid();
        var description = "Test Description";
        var authorId = Guid.NewGuid();
        var studyGroupId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(7);

        // Act & Assert
        Assert.Throws<InvalidAssignmentNameException>(() =>
            Assignment.Create(id, invalidName, description, authorId, studyGroupId, dueDate));
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tooLongName = new string('A', 101); // 101 characters
        var description = "Test Description";
        var authorId = Guid.NewGuid();
        var studyGroupId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(7);

        // Act & Assert
        Assert.Throws<InvalidAssignmentNameException>(() =>
            Assignment.Create(id, tooLongName, description, authorId, studyGroupId, dueDate));
    }

    [Fact]
    public void Create_WithPastDueDate_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test Assignment";
        var description = "Test Description";
        var authorId = Guid.NewGuid();
        var studyGroupId = Guid.NewGuid();
        var pastDueDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        Assert.Throws<InvalidAssignmentDueDateException>(() =>
            Assignment.Create(id, name, description, authorId, studyGroupId, pastDueDate));
    }

    [Fact]
    public void AddActivity_ShouldAddAndReorderActivities()
    {
        // Arrange
        var assignment = Assignment.Create(
            Guid.NewGuid(),
            "Test Assignment",
            "Test Description",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(7)
        );

        var activity1 = new Activity(
            Guid.NewGuid(),
            new WritingActivityDetails("Write about your day", 200),
            10,
            ActivityType.Writing
        );

        var activity2 = new Activity(
            Guid.NewGuid(),
            new QuestionActivityDetails("What is your favorite color?", ["green"], 30),
            20,
            ActivityType.Question
        );

        // Act
        assignment.AddActivity(activity1);
        assignment.AddActivity(activity2);

        // Assert
        Assert.Equal(2, assignment.Activities.Count);
        Assert.Equal(0, assignment.Activities[0].Order);
        Assert.Equal(1, assignment.Activities[1].Order);
        Assert.Equal(activity1.Id, assignment.Activities[0].Id);
        Assert.Equal(activity2.Id, assignment.Activities[1].Id);
    }

    [Fact]
    public void RemoveActivity_ShouldRemoveAndReorderActivities()
    {
        // Arrange
        var activity1 = new Activity(
            Guid.NewGuid(),
            new WritingActivityDetails("Write an essay", 250),
            10,
            ActivityType.Writing
        );

        var activity2 = new Activity(
            Guid.NewGuid(),
            new QuestionActivityDetails("Answer this question", ["answer"], 50),
            20,
            ActivityType.Question
        );

        var activity3 = new Activity(
            Guid.NewGuid(),
            new PronunciationActivityDetails("Speaking exercise", Language.EnglishUS),
            30,
            ActivityType.Pronunciation
        );

        var activities = new List<Activity> { activity1, activity2, activity3 };

        var assignment = Assignment.Create(
            Guid.NewGuid(),
            "Test Assignment",
            "Test Description",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(7),
            activities
        );

        // Act
        assignment.RemoveActivity(activity2);

        // Assert
        Assert.Equal(2, assignment.Activities.Count);
        Assert.Equal(0, assignment.Activities[0].Order);
        Assert.Equal(1, assignment.Activities[1].Order);
        Assert.Equal(activity1.Id, assignment.Activities[0].Id);
        Assert.Equal(activity3.Id, assignment.Activities[1].Id);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var assignment = Assignment.Create(
            Guid.NewGuid(),
            "Test Assignment",
            "Test Description",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(7)
        );

        var newName = "Updated Assignment";

        // Act
        assignment.UpdateName(newName);

        // Assert
        Assert.Equal(newName, assignment.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_WithInvalidName_ShouldThrowException(string invalidName)
    {
        // Arrange
        var assignment = Assignment.Create(
            Guid.NewGuid(),
            "Test Assignment",
            "Test Description",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(7)
        );

        // Act & Assert
        Assert.Throws<InvalidAssignmentNameException>(() => assignment.UpdateName(invalidName));
    }

    [Fact]
    public void UpdateDueDate_WithValidDate_ShouldUpdateDueDate()
    {
        // Arrange
        var assignment = Assignment.Create(
            Guid.NewGuid(),
            "Test Assignment",
            "Test Description",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(7)
        );

        var newDueDate = DateTime.UtcNow.AddDays(14);

        // Act
        assignment.UpdateDueDate(newDueDate);

        // Assert
        Assert.Equal(newDueDate, assignment.DueDate);
    }

    [Fact]
    public void UpdateDueDate_WithPastDate_ShouldThrowException()
    {
        // Arrange
        var assignment = Assignment.Create(
            Guid.NewGuid(),
            "Test Assignment",
            "Test Description",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(7)
        );

        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        Assert.Throws<InvalidAssignmentDueDateException>(() => assignment.UpdateDueDate(pastDate));
    }
}