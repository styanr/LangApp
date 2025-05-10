using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Submissions;

namespace LangApp.Core.Services.GradingStrategies;

// TODO: is this "async" code a good choice?
public abstract class SynchronousGradingStrategy<TAssignmentDetails> : IGradingStrategy<TAssignmentDetails>
    where TAssignmentDetails : ActivityDetails
{
    public Task<SubmissionGrade> Grade(TAssignmentDetails assignment, SubmissionDetails submission,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            return Task.FromResult(ExecuteGrade(assignment, submission, cancellationToken));
        }
        catch (Exception e)
        {
            return Task.FromException<SubmissionGrade>(e);
        }
    }

    protected abstract SubmissionGrade ExecuteGrade(TAssignmentDetails assignment, SubmissionDetails submission,
        CancellationToken cancellationToken);
}