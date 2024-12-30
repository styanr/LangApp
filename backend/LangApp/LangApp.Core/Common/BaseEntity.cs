namespace LangApp.Core.Common;

public abstract class BaseEntity
{
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }

    protected BaseEntity(Guid id, DateTimeOffset dateCreated, DateTimeOffset dateUpdated, DateTimeOffset dateDeleted,
        bool isDeleted)
    {
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;
        IsDeleted = isDeleted;
        Id = Guid.NewGuid();
        DateCreated = DateTimeOffset.Now.ToUniversalTime();
    }

    public Guid Id { get; private set; }
    public DateTimeOffset DateCreated { get; private set; } = DateTimeOffset.Now;
    public DateTimeOffset? DateUpdated { get; private set; }
    public DateTimeOffset? DateDeleted { get; private set; }
    public bool IsDeleted { get; private set; }
}