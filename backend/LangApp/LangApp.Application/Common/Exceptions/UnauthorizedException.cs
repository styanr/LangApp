using LangApp.Core.Common;

namespace LangApp.Application.Common.Exceptions;

public class UnauthorizedException : ApplicationException
{
    public Guid UserId { get; set; }
    public IIdentifiable Entity { get; set; }

    public UnauthorizedException(Guid userId, IIdentifiable entity) : base(
        $"The user '{userId}' is not authorized to access {entity.GetType().Name} with ID {entity.Id}")
    {
        UserId = userId;
        Entity = entity;
    }
}