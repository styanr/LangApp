using LangApp.Application.Assignments.Dto;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;

namespace LangApp.Application.Assignments.Extensions;

public static class MultipleChoiceAssignmentDetailsExtensions
{
    public static MultipleChoiceAssignmentDetails ToValueObject(this MultipleChoiceAssignmentDetailsDto dto)
    {
        var questions = dto.Questions
            .Select(q => new MultipleChoiceQuestion(
                q.Question,
                q.Options.Select(o => new MultipleChoiceOption(o)).ToList(),
                q.CorrectOptionIndex))
            .ToList();

        return new MultipleChoiceAssignmentDetails(questions);
    }

    public static MultipleChoiceAssignmentDetailsDto ToDto(this MultipleChoiceAssignmentDetails details)
    {
        var questions = details.Questions
            .Select(q => new MultipleChoiceQuestionDto(
                q.Question,
                q.Options.Select(o => o.OptionDescription).ToList(),
                q.CorrectOptionIndex))
            .ToList();

        return new MultipleChoiceAssignmentDetailsDto(questions);
    }
}