using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Submissions;

namespace LangApp.Application.Common.Strategies;

public interface IGradingStrategyDispatcher
{
    Task<SubmissionGrade> Grade<TAssignmentDetails>(TAssignmentDetails assignmentDetails,
        SubmissionDetails submissionDetails,
        CancellationToken cancellationToken = default(CancellationToken))
        where TAssignmentDetails : ActivityDetails;
}