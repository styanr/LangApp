using LangApp.Core.Common;
using LangApp.Core.Entities.Submissions;

namespace LangApp.Core.Events.Submissions;

public record SubmissionCreated(Submission Submission) : IDomainEvent;