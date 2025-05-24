using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;

namespace LangApp.Application.Assignments.Extensions;

public static class FillInTheBlankActivityDetailsExtensions
{
    public static FillInTheBlankActivityDetails ToValueObject(this FillInTheBlankActivityDetailsDto dto)
    {
        return new FillInTheBlankActivityDetails(dto.TemplateText,
            dto.Answers.Select(a => new FillInTheBlankAnswer(a.AcceptableAnswers)).ToList());
    }
}
