using LangApp.Core.Common;
using LangApp.Core.Exceptions;

namespace LangApp.Application.Common.Exceptions;

public class UnauthorizedException : LangAppException
{
    public Guid UserId { get; }
    public Guid? EntityId { get; }
    public string? EntityType { get; }

    public UnauthorizedException(Guid userId)
        : base($"The user '{userId}' is not authorized to perform this action.")
    {
        UserId = userId;
    }

    public UnauthorizedException(Guid userId, Guid entityId, string entityType)
        : base($"The user '{userId}' is not authorized to access {entityType} with ID {entityId}.")
    {
        UserId = userId;
        EntityId = entityId;
        EntityType = entityType;
    }

    public UnauthorizedException(Guid userId, IIdentifiable entity)
        : this(userId, entity.Id, entity.GetType().Name)
    {
    }
}
