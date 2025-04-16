using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;

namespace LangApp.Application.Assignments.Extensions;

public static class FillInTheBlankAssignmentDetailsExtensions
{
    public static FillInTheBlankAssignmentDetails ToValueObject(this FillInTheBlankAssignmentDetailsDto dto)
    {
        var questions = dto.Questions
            .Select(q => new FillInTheBlankQuestion(
                q.TemplateText,
                q.Answers
                    .Select(a => new FillInTheBlankAnswer(a.AcceptableAnswers))
                    .ToList())).ToList();

        return new FillInTheBlankAssignmentDetails(questions);
    }
}