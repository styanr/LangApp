namespace LangApp.Core.Common;

public abstract class BaseEntity : IIdentifiable
{
    protected BaseEntity() : this(Guid.NewGuid())
    {
    }

    protected BaseEntity(Guid id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        return obj is BaseEntity otherEntity && Id == otherEntity.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(BaseEntity? left, BaseEntity? right)
    {
        return !(left == right);
    }
}