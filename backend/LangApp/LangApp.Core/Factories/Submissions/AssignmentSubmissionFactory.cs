using LangApp.Core.Entities.Submissions;
using LangApp.Core.Services.KeyGeneration;

namespace LangApp.Core.Factories.Submissions;

public class AssignmentSubmissionFactory : IAssignmentSubmissionFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public AssignmentSubmission Create(Guid assignmentId, Guid userId, List<ActivitySubmission> activitySubmissions)
    {
        var id = _keyGenerator.NewKey();

        return AssignmentSubmission.Create(assignmentId, userId, 0, id, activitySubmissions);
    }
}