using LangApp.Application.Assignments.Dto.Question;
using LangApp.Core.ValueObjects.Assignments.Question;

namespace LangApp.Application.Assignments.Extensions;

public static class QuestionActivityDetailsExtensions
{
    public static QuestionActivityDetails ToValueObject(this QuestionActivityDetailsDto dto)
    {
        return new QuestionActivityDetails(
            dto.Question,
            dto.Answers,
            dto.MaxLength
        );
    }
}
