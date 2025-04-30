using LangApp.Application.Submissions.Dto;
using LangApp.Core.Exceptions;
using LangApp.Infrastructure.EF.Models.Submissions;
using LangApp.Infrastructure.EF.Models.Submissions.MultipleChoice;
using LangApp.Infrastructure.EF.Models.Submissions.Pronunciation;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Submissions.Extensions;

public static class SubmissionDetailsReadModelExtensions
{
    public static SubmissionDetailsDto ToDto(this SubmissionDetailsReadModel details)
    {
        return details switch
        {
            MultipleChoiceSubmissionDetailsReadModel multipleChoiceDetails =>
                new MultipleChoiceSubmissionDetailsDto(multipleChoiceDetails.Answers
                    .Select(a =>
                        new MultipleChoiceSubmissionAnswerDto(a.ChosenOptionIndex))
                    .ToList()
                ),

            PronunciationSubmissionDetailsReadModel pronunciationDetails =>
                new PronunciationSubmissionDetailsDto(pronunciationDetails.FileUri),
            _ => throw new LangAppException("Wrong submission details type")
        };
    }
}
