using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;

namespace LangApp.Application.Assignments.Extensions;

public static class MultipleChoiceActivityDetailsExtensions
{
    public static MultipleChoiceActivityDetails ToValueObject(this MultipleChoiceActivityDetailsDto dto)
    {
        var questions = dto.Questions
            .Select(q => new MultipleChoiceQuestion(
                q.Question,
                q.Options.Select(o => new MultipleChoiceOption(o)).ToList(),
                q.CorrectOptionIndex))
            .ToList();

        return new MultipleChoiceActivityDetails(questions);
    }

    public static MultipleChoiceActivityDetailsDto ToDto(this MultipleChoiceActivityDetails details)
    {
        var questions = details.Questions
            .Select(q => new MultipleChoiceQuestionDto(
                q.Question,
                q.Options.Select(o => o.OptionDescription).ToList(),
                q.CorrectOptionIndex))
            .ToList();

        return new MultipleChoiceActivityDetailsDto(questions);
    }
}