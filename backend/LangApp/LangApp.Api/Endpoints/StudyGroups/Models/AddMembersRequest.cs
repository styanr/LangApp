namespace LangApp.Api.Endpoints.StudyGroups.Models;

public record AddMembersRequest(
    Guid StudyGroupId,
    IEnumerable<Guid> Members
);