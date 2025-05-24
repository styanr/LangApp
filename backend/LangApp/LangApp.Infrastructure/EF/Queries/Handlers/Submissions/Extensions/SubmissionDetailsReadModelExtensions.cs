using LangApp.Application.Submissions.Dto;
using LangApp.Core.Exceptions;
using LangApp.Infrastructure.EF.Models.Submissions;
using LangApp.Infrastructure.EF.Models.Submissions.FillInTheBlank;
using LangApp.Infrastructure.EF.Models.Submissions.MultipleChoice;
using LangApp.Infrastructure.EF.Models.Submissions.Pronunciation;
using LangApp.Infrastructure.EF.Models.Submissions.Question;
using LangApp.Infrastructure.EF.Models.Submissions.Writing;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Submissions.Extensions;

public static class SubmissionDetailsReadModelExtensions
{
    public static ActivitySubmissionDetailsDto ToDto(this SubmissionDetailsReadModel details)
    {
        return details switch
        {
            MultipleChoiceSubmissionDetailsReadModel multipleChoiceDetails =>
                new MultipleChoiceActivitySubmissionDetailsDto(multipleChoiceDetails.Answers
                    .Select(a =>
                        new MultipleChoiceSubmissionAnswerDto(a.QuestionIndex, a.ChosenOptionIndex))
                    .ToList()
                ),

            PronunciationSubmissionDetailsReadModel pronunciationDetails =>
                new PronunciationActivitySubmissionDetailsDto(pronunciationDetails.FileUri),

            FillInTheBlankSubmissionDetailsReadModel fillInTheBlankDetails =>
                new FillInTheBlankActivitySubmissionDetailsDto(fillInTheBlankDetails.Answers
                    .Select(a => new FillInTheBlankSubmissionAnswerDto(a.Index, a.Answer)).ToList()),

            QuestionSubmissionDetailsReadModel questionDetails =>
                new QuestionActivitySubmissionDetailsDto(questionDetails.Answer),

            WritingSubmissionDetailsReadModel writingDetails =>
                new WritingActivitySubmissionDetailsDto(writingDetails.Text),
            _ => throw new LangAppException("Wrong submission details type")
        };
    }
}
