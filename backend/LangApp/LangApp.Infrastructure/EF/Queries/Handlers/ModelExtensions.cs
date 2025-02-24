using LangApp.Application.Lexicons.Dto;
using LangApp.Application.Users.Dto;
using LangApp.Infrastructure.EF.Models.Lexicons;
using LangApp.Infrastructure.EF.Models.Users;

namespace LangApp.Infrastructure.EF.Queries.Handlers;

public static class ModelExtensions
{
    public static FullNameDto ToDto(this FullNameReadModel fullNameReadModel)
    {
        return new FullNameDto(fullNameReadModel.FirstName, fullNameReadModel.LastName);
    }

    public static UserDto ToDto(this UserReadModel userReadModel)
    {
        return new UserDto(
            userReadModel.Id,
            userReadModel.Username,
            userReadModel.FullName.ToDto(),
            userReadModel.PictureUrl,
            userReadModel.Role
        );
    }

    public static LexiconEntryDto ToDto(this LexiconEntryReadModel entryReadModel)
    {
        return new LexiconEntryDto(
            entryReadModel.Id,
            entryReadModel.LexiconId,
            entryReadModel.Term,
            entryReadModel.Definitions?.Select(d => d.Value) ?? []
        );
    }
}