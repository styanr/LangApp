using LangApp.Application.Assignments.Exceptions;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Application.Submissions.Dto;
using LangApp.Application.Submissions.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Submissions.Commands;

public record EditActivitySubmissionGrade(
    Guid AssignmentSubmissionId,
    Guid ActivityId,
    SubmissionGradeDto Grade,
    Guid UserId
) : ICommand;

public class EditActivitySubmissionGradeHandler : ICommandHandler<EditActivitySubmissionGrade>
{
    private readonly IAssignmentSubmissionRepository _repository;
    private readonly IStudyGroupRepository _studyGroupRepository;
    private readonly IAssignmentRepository _assignmentRepository;

    public EditActivitySubmissionGradeHandler(IAssignmentSubmissionRepository repository,
        IStudyGroupRepository studyGroupRepository, IAssignmentRepository assignmentRepository)
    {
        _repository = repository;
        _studyGroupRepository = studyGroupRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task HandleAsync(EditActivitySubmissionGrade command, CancellationToken cancellationToken)
    {
        var assignmentSubmission = await _repository.GetAsync(command.AssignmentSubmissionId) ??
                                   throw new SubmissionNotFound(command.AssignmentSubmissionId);
        var activitySubmission =
            assignmentSubmission.ActivitySubmissions.FirstOrDefault(ac => ac.Id == command.ActivityId) ??
            throw new ActivityNotFound(command.ActivityId);
        var assignment = await _assignmentRepository.GetAsync(assignmentSubmission.AssignmentId) ??
                         throw new AssignmentNotFound(assignmentSubmission.AssignmentId);
        var studyGroup = await _studyGroupRepository.GetAsync(assignment.StudyGroupId) ??
                         throw new StudyGroupNotFound(assignment.StudyGroupId);


        if (!assignment.CanBeModifiedBy(command.UserId) || !studyGroup.CanBeModifiedBy(command.UserId))
        {
            throw new UnauthorizedException(command.UserId, assignmentSubmission);
        }

        var activities = assignment.Activities;
        assignmentSubmission.GradeActivitySubmission(activitySubmission,
            new SubmissionGrade(new Percentage(command.Grade.ScorePercentage), command.Grade.Feedback));

        assignmentSubmission.RecalculateTotalScore(activities);
        await _repository.UpdateAsync(assignmentSubmission);
    }
}
