using System;
using FluentAssertions;
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

public class ActivitySubmissionTests
{
    private readonly Mock<IKeyGenerator> _keyGenMock = new Mock<IKeyGenerator>();

    private IActivitySubmissionFactory CreateActivitySubmissionFactory()
    {
        _keyGenMock.Setup(x => x.NewKey()).Returns(() => Guid.NewGuid());
        return new ActivitySubmissionFactory(_keyGenMock.Object);
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateSubmissionWithExpectedProperties()
    {
        // Arrange
        var factory = CreateActivitySubmissionFactory();
        var id = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var details = new MultipleChoiceSubmissionDetails([
            new MultipleChoiceAnswer(0, 1),
            new MultipleChoiceAnswer(1, 0),
        ]);

        // Act
        var submission = factory.Create(activityId, details);

        // Assert
        submission.ActivityId.Should().Be(activityId);
        submission.Details.Should().Be(details);
        submission.Status.Should().Be(GradeStatus.Pending);
        submission.Grade.Should().BeNull();
    }

    [Fact]
    public void UpdateGrade_ShouldSetGradeAndStatusToCompleted()
    {
        // Arrange
        var factory = CreateActivitySubmissionFactory();
        var details = new MultipleChoiceSubmissionDetails([
            new MultipleChoiceAnswer(0, 1),
            new MultipleChoiceAnswer(1, 0),
        ]);

        var submission = factory.Create(Guid.NewGuid(), details);
        var grade = new SubmissionGrade(new Percentage(100));

        // Act
        submission.UpdateGrade(grade);

        // Assert
        submission.Grade.Should().Be(grade);
        submission.Status.Should().Be(GradeStatus.Completed);
    }

    [Fact]
    public void Fail_ShouldSetStatusToFailedAndSetReason()
    {
        // Arrange
        var factory = CreateActivitySubmissionFactory();
        var details = new MultipleChoiceSubmissionDetails([
            new MultipleChoiceAnswer(0, 1),
            new MultipleChoiceAnswer(1, 0),
        ]);

        var submission = factory.Create(Guid.NewGuid(), details);
        var reason = "Some reason";

        // Act
        submission.Fail(reason);

        // Assert
        submission.Status.Should().Be(GradeStatus.Failed);
        submission.FailureReason.Should().Be(reason);
    }
}