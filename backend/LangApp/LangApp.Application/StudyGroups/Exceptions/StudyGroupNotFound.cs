using LangApp.Core.Common.Exceptions;
using LangApp.Core.Exceptions;

namespace LangApp.Application.StudyGroups.Exceptions;

public class StudyGroupNotFound : NotFoundException
{
    public Guid Id { get; }

    public StudyGroupNotFound(Guid id) : base($"Study group with ID {id} was not found.")
    {
        Id = id;
    }
}