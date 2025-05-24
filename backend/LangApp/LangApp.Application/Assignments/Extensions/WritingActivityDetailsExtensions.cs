using LangApp.Application.Assignments.Dto.Writing;
using LangApp.Core.ValueObjects.Assignments.Writing;

namespace LangApp.Application.Assignments.Extensions;

public static class WritingActivityDetailsExtensions
{
    public static WritingActivityDetails ToValueObject(this WritingActivityDetailsDto dto)
    {
        return new WritingActivityDetails(
            dto.Prompt,
            dto.MaxWords
        );
    }
}
