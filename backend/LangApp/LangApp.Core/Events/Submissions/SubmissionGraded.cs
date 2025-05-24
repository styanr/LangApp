using LangApp.Core.Common;
using LangApp.Core.Entities.Submissions;

namespace LangApp.Core.Events.Submissions;

public record SubmissionGraded(AssignmentSubmission Submission) : IDomainEvent;