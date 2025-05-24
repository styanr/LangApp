using System;
using System.Collections.Generic;
using FluentAssertions;
using LangApp.Core.Entities.Assignments;
using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.Factories.Submissions;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;
using Moq;
using Xunit;

namespace LangApp.Core.Tests.Submissions;

public class AssignmentSubmissionTests
{
    private readonly Mock<IKeyGenerator> _keyGenMock = new Mock<IKeyGenerator>();
    private int _keyGenCallCount = 0;

    private IActivitySubmissionFactory CreateActivitySubmissionFactory()
    {
        _keyGenMock.Setup(x => x.NewKey()).Returns(() => Guid.NewGuid());
        return new ActivitySubmissionFactory(_keyGenMock.Object);
    }

    private IAssignmentSubmissionFactory CreateAssignmentSubmissionFactory()
    {
        _keyGenMock.Setup(x => x.NewKey()).Returns(() => Guid.NewGuid());
        return new AssignmentSubmissionFactory(_keyGenMock.Object);
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateSubmissionWithExpectedProperties()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var activitySubmissions = new List<ActivitySubmission>();
        var factory = CreateAssignmentSubmissionFactory();

        // Act
        var submission = factory.Create(assignmentId, studentId, activitySubmissions);

        // Assert
        submission.AssignmentId.Should().Be(assignmentId);
        submission.StudentId.Should().Be(studentId);
        submission.Score.Should().Be(0);
        submission.ActivitySubmissions.Should().BeEmpty();
        submission.Status.Should().Be(GradeStatus.Pending);
    }

    [Fact]
    public void AddActivitySubmission_ShouldAddSubmission()
    {
        // Arrange
        var assignmentFactory = CreateAssignmentSubmissionFactory();
        var activityFactory = CreateActivitySubmissionFactory();
        var submission = assignmentFactory.Create(Guid.NewGuid(), Guid.NewGuid(), new List<ActivitySubmission>());
        var details = new MultipleChoiceSubmissionDetails([
            new MultipleChoiceAnswer(0, 1),
            new MultipleChoiceAnswer(1, 0),
        ]);

        var activitySubmission = activityFactory.Create(Guid.NewGuid(), details);

        // Act
        submission.AddActivitySubmission(activitySubmission);

        // Assert
        submission.ActivitySubmissions.Should().ContainSingle().Which.Should().Be(activitySubmission);
    }

    [Fact]
    public void RemoveActivitySubmission_ShouldRemoveSubmission()
    {
        // Arrange
        var assignmentFactory = CreateAssignmentSubmissionFactory();
        var activityFactory = CreateActivitySubmissionFactory();
        var details = new MultipleChoiceSubmissionDetails([
            new MultipleChoiceAnswer(0, 1),
            new MultipleChoiceAnswer(1, 0),
        ]);

        var activitySubmission = activityFactory.Create(Guid.NewGuid(), details);
        var submission = assignmentFactory.Create(Guid.NewGuid(), Guid.NewGuid(),
            new List<ActivitySubmission> { activitySubmission });

        // Act
        submission.RemoveActivitySubmission(activitySubmission);

        // Assert
        submission.ActivitySubmissions.Should().BeEmpty();
    }

    [Fact]
    public void GradeActivitySubmission_ShouldUpdateStatusToCompletedIfAllCompleted()
    {
        // Arrange
        var assignmentFactory = CreateAssignmentSubmissionFactory();
        var activityFactory = CreateActivitySubmissionFactory();
        var details = new MultipleChoiceSubmissionDetails([
            new MultipleChoiceAnswer(0, 1),
            new MultipleChoiceAnswer(1, 0),
        ]);

        var activitySubmission = activityFactory.Create(Guid.NewGuid(), details);
        var submission = assignmentFactory.Create(Guid.NewGuid(), Guid.NewGuid(),
            new List<ActivitySubmission> { activitySubmission });
        var grade = new SubmissionGrade(new Percentage(100));

        // Act
        submission.GradeActivitySubmission(activitySubmission, grade);

        // Assert
        activitySubmission.Status.Should().Be(GradeStatus.Completed);
        submission.Status.Should().Be(GradeStatus.Completed);
    }

    [Fact]
    public void GradeActivitySubmission_ShouldUpdateStatusToFailedIfAnyFailed()
    {
        // Arrange
        var assignmentFactory = CreateAssignmentSubmissionFactory();
        var activityFactory = CreateActivitySubmissionFactory();
        var details = new MultipleChoiceSubmissionDetails([
            new MultipleChoiceAnswer(0, 1),
            new MultipleChoiceAnswer(1, 0),
        ]);

        var completed = activityFactory.Create(Guid.NewGuid(), details);
        var failed = activityFactory.Create(Guid.NewGuid(), details);
        var submission = assignmentFactory.Create(Guid.NewGuid(), Guid.NewGuid(), [completed, failed]);
        var grade = new SubmissionGrade(new Percentage(100));
        submission.GradeActivitySubmission(completed, grade);

        // Act
        submission.FailActivitySubmission(failed, "reason");

        // Assert
        submission.Status.Should().Be(GradeStatus.Failed);
    }

    [Fact]
    public void GradeActivitySubmission_ShouldUpdateStatusToNeedsReviewIfAnyNeedsReview()
    {
        // Arrange
        var assignmentFactory = CreateAssignmentSubmissionFactory();
        var activityFactory = CreateActivitySubmissionFactory();
        var details = new MultipleChoiceSubmissionDetails([
            new MultipleChoiceAnswer(0, 1),
            new MultipleChoiceAnswer(1, 0),
        ]);
        var needsReview = activityFactory.Create(Guid.NewGuid(), details);
        needsReview.GetType().GetProperty("Status")!.SetValue(needsReview, GradeStatus.NeedsReview);
        var completed = activityFactory.Create(Guid.NewGuid(), details);

        var submission = assignmentFactory.Create(Guid.NewGuid(), Guid.NewGuid(), [needsReview, completed]);
        var grade = new SubmissionGrade(new Percentage(100));

        // Act
        submission.GradeActivitySubmission(completed, grade);

        // Assert
        submission.Status.Should().Be(GradeStatus.NeedsReview);
    }

    [Fact]
    public void RecalculateTotalScore_ShouldSetScoreBasedOnCompletedActivities()
    {
        // Arrange
        var assignmentFactory = CreateAssignmentSubmissionFactory();
        var activityFactory = CreateActivitySubmissionFactory();

        var (activity1Id, activity2Id) = (Guid.NewGuid(), Guid.NewGuid());

        var activity1 = new Activity(activity1Id, null!, 10, ActivityType.Writing);
        var activity2 = new Activity(activity2Id, null!, 20, ActivityType.Writing);
        var activities = new List<Activity> { activity1, activity2 };

        var details = new MultipleChoiceSubmissionDetails([
            new MultipleChoiceAnswer(0, 1),
            new MultipleChoiceAnswer(1, 0),
        ]);

        var submission1 = activityFactory.Create(activity1Id, details);
        submission1.UpdateGrade(new SubmissionGrade(new Percentage(50)));
        submission1.GetType().GetProperty("Status")!.SetValue(submission1, GradeStatus.Completed);

        var submission2 = activityFactory.Create(activity2Id, details);
        submission2.UpdateGrade(new SubmissionGrade(new Percentage(100)));
        submission2.GetType().GetProperty("Status")!.SetValue(submission2, GradeStatus.Completed);
        var submission = assignmentFactory.Create(Guid.NewGuid(), Guid.NewGuid(), [submission1, submission2]);

        // Act
        submission.RecalculateTotalScore(activities);

        // Assert
        // 50% of 10 = 5, 100% of 20 = 20, total = 25
        submission.Score.Should().Be(25);
    }

    [Fact]
    public void Fail_ShouldSetStatusToFailed()
    {
        // Arrange
        var assignmentFactory = CreateAssignmentSubmissionFactory();
        var submission = assignmentFactory.Create(Guid.NewGuid(), Guid.NewGuid(), []);

        // Act
        submission.Fail();

        // Assert
        submission.Status.Should().Be(GradeStatus.Failed);
    }

    [Fact]
    public void FailActivitySubmission_ShouldFailActivityAndSubmission()
    {
        // Arrange
        var assignmentFactory = CreateAssignmentSubmissionFactory();
        var activityFactory = CreateActivitySubmissionFactory();
        var details = new MultipleChoiceSubmissionDetails([
            new MultipleChoiceAnswer(0, 1),
            new MultipleChoiceAnswer(1, 0),
        ]);

        var activitySubmission = activityFactory.Create(Guid.NewGuid(), details);
        var submission = assignmentFactory.Create(Guid.NewGuid(), Guid.NewGuid(),
            new List<ActivitySubmission> { activitySubmission });

        // Act
        submission.FailActivitySubmission(activitySubmission, "bad");

        // Assert
        activitySubmission.Status.Should().Be(GradeStatus.Failed);
        submission.Status.Should().Be(GradeStatus.Failed);
    }
}