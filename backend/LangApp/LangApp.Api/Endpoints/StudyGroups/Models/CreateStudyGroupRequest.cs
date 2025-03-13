namespace LangApp.Api.Endpoints.StudyGroups.Models;

public record CreateStudyGroupRequest(
    string Name,
    string Language
);