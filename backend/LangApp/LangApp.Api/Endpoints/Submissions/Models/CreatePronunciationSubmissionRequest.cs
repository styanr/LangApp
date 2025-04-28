namespace LangApp.Api.Endpoints.Submissions.Models;

public record CreatePronunciationSubmissionRequest(Guid AssignmentId, IFormFile File);
