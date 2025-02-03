using LangApp.Core.Exceptions;

namespace LangApp.Application.StudyGroups.Exceptions;

public class StudyGroupNotFoundException : LangAppException
{
    public Guid Id { get; }

    public StudyGroupNotFoundException(Guid id) : base($"Study group with ID {id} was not found.")
    {
        Id = id;
    }
}