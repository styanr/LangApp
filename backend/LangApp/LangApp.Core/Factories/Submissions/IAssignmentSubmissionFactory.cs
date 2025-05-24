using System.Runtime.InteropServices;
using LangApp.Core.Entities.Submissions;

namespace LangApp.Core.Factories.Submissions;

public interface IAssignmentSubmissionFactory
{
    AssignmentSubmission Create(Guid assignmentId, Guid userId, List<ActivitySubmission> activitySubmissions);
}