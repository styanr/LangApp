namespace LangApp.Core.Exceptions.Assignments;

public class InvalidAssignmentAuthorIdException(Guid authorId)
    : LangAppException($"Author ID \"{authorId}\" is invalid. It cannot be an empty GUID.")
{
    public Guid AuthorId { get; } = authorId;
}
