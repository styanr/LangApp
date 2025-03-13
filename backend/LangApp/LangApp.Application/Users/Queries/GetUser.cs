using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Users.Dto;

namespace LangApp.Application.Users.Queries;

public record GetUser(Guid Id) : IQuery<UserDto>;