namespace LangApp.Api.Endpoints.StudyGroups.Models;

public class MembersBodyRequestModel
{
    public IEnumerable<Guid> Members { get; set; }
}