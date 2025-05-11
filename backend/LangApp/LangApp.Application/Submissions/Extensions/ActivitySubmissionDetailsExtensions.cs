using LangApp.Application.Submissions.Dto;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;

namespace LangApp.Application.Submissions.Extensions;

public static class SubmissionDetailsExtensions
{
    public static SubmissionDetails ToValueObject(this ActivitySubmissionDetailsDto dto)
    {
        return dto switch
        {
            MultipleChoiceActivitySubmissionDetailsDto multipleChoice =>
                new MultipleChoiceSubmissionDetails(multipleChoice.Answers
                    .Select(a => new MultipleChoiceAnswer(a.QuestionIndex, a.ChosenOptionIndex))
                    .ToList()),

            PronunciationActivitySubmissionDetailsDto pronunciation =>
                new PronunciationSubmissionDetails(pronunciation.RecordingUrl),

            _ => throw new ArgumentException($"Conversion for type {dto.GetType().Name} is not supported")
        };
    }
}
