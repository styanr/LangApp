using LangApp.Application.Submissions.Dto;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;

namespace LangApp.Application.Submissions.Extensions;

public static class MultipleChoiceSubmissionDetailsExtensions
{
    public static MultipleChoiceSubmissionDetails ToValueObject(this MultipleChoiceSubmissionDetailsDto dto)
    {
        return new MultipleChoiceSubmissionDetails(dto.Answers
            .Select(a => new MultipleChoiceAnswer(a.ChosenOptionIndex))
            .ToList());
    }
}
