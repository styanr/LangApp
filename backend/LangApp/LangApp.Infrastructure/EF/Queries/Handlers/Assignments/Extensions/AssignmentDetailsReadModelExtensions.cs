using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Pronunciation;
using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.Assignments.FillInTheBlank;
using LangApp.Infrastructure.EF.Models.Assignments.MultipleChoice;
using LangApp.Infrastructure.EF.Models.Assignments.Pronunciation;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Assignments.Extensions;

public static class AssignmentDetailsReadModelExtensions
{
    public static ActivityDetailsDto ToDto(this ActivityDetailsReadModel details, bool restricted = false)
    {
        return details switch
        {
            MultipleChoiceActivityDetailsReadModel multipleChoiceDetails =>
                CreateMultipleChoiceDto(multipleChoiceDetails, restricted),
            FillInTheBlankActivityDetailsReadModel fillInTheBlankDetails =>
                CreateFillInTheBlankDto(fillInTheBlankDetails, restricted),
            PronunciationActivityDetailsReadModel pronunciationDetails =>
                CreatePronunciationDto(pronunciationDetails, restricted),
            _ => throw new LangAppException("Wrong assignment details type")
        };
    }

    private static ActivityDetailsDto CreateMultipleChoiceDto(
        MultipleChoiceActivityDetailsReadModel details,
        bool restricted)
    {
        return restricted
            ? new MultipleChoiceActivityRestrictedDetailsDto(
                details.Questions
                    .Select(q => new MultipleChoiceRestrictedQuestionDto(
                        q.Question,
                        q.Options.Select(o => o.OptionDescription).ToList()))
                    .ToList())
            : new MultipleChoiceActivityDetailsDto(
                details.Questions
                    .Select(q => new MultipleChoiceQuestionDto(
                        q.Question,
                        q.Options.Select(o => o.OptionDescription).ToList(),
                        q.CorrectOptionIndex))
                    .ToList());
    }

    private static ActivityDetailsDto CreateFillInTheBlankDto(
        FillInTheBlankActivityDetailsReadModel details,
        bool restricted)
    {
        return restricted
            ? new FillInTheBlankActivityRestrictedDetailsDto(
                details.TemplateText)
            : new FillInTheBlankActivityDetailsDto(
                details.TemplateText,
                details.Answers.Select(a => new FillInTheBlankAnswerDto(a.AcceptableAnswers)).ToList());
    }

    private static ActivityDetailsDto CreatePronunciationDto(
        PronunciationActivityDetailsReadModel details,
        bool restricted)
    {
        return new PronunciationActivityDetailsDto(
            details.Language.Code,
            details.ReferenceText);
    }
}
