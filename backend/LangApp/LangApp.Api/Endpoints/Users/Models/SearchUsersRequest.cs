using Microsoft.AspNetCore.Mvc;

namespace LangApp.Api.Endpoints.Users.Models;

public record SearchUsersRequest
{
    public SearchUsersRequest(string SearchTerm)
    {
        this.SearchTerm = SearchTerm;
    }

    public string SearchTerm { get; init; }

    public void Deconstruct(out string SearchTerm)
    {
        SearchTerm = this.SearchTerm;
    }
}
