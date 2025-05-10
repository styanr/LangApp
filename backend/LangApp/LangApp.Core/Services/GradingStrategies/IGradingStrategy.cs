using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Submissions;

namespace LangApp.Core.Services.GradingStrategies;

public interface IGradingStrategy<TAssignmentDetails> where TAssignmentDetails : ActivityDetails
{
    Task<SubmissionGrade> Grade(TAssignmentDetails assignment, SubmissionDetails submission,
        CancellationToken cancellationToken = default(CancellationToken));
}