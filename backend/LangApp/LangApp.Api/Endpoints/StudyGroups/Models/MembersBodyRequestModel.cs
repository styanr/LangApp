namespace LangApp.Api.Endpoints.StudyGroups.Models;

public record MembersBodyRequestModel(IEnumerable<Guid> Members);