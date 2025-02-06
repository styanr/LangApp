using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Users.Dto;

namespace LangApp.Application.Users.Queries;

public class GetUser : IQuery<UserDto>
{
    public Guid Id { get; set; }
}