using LangApp.Application.Submissions.Dto;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.FillInTheBlank;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;
using LangApp.Core.ValueObjects.Submissions.Question;
using LangApp.Core.ValueObjects.Submissions.Writing;

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

            FillInTheBlankActivitySubmissionDetailsDto fillInTheBlank =>
                new FillInTheBlankSubmissionDetails(fillInTheBlank.Answers
                    .Select(a =>
                        new FillInTheBlankSubmissionAnswer(a.Index, a.Answer)).ToList()),

            QuestionActivitySubmissionDetailsDto question =>
                new QuestionSubmissionDetails(question.Answer),

            WritingActivitySubmissionDetailsDto writing =>
                new WritingSubmissionDetails(writing.Text),

            _ => throw new ArgumentException($"Conversion for type {dto.GetType().Name} is not supported")
        };
    }
}
