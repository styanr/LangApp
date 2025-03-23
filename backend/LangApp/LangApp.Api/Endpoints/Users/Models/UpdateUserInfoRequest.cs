using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Users.Models;

namespace LangApp.Api.Endpoints.Users.Models;

public record UpdateUserInfoRequest(
    string Username,
    FullNameModel FullName,
    string? PictureUrl
) : ICommand;