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
    public static AssignmentDetailsDto ToDto(this AssignmentDetailsReadModel details, bool restricted = false)
    {
        return details switch
        {
            MultipleChoiceAssignmentDetailsReadModel multipleChoiceDetails =>
                CreateMultipleChoiceDto(multipleChoiceDetails, restricted),
            FillInTheBlankAssignmentDetailsReadModel fillInTheBlankDetails =>
                CreateFillInTheBlankDto(fillInTheBlankDetails, restricted),
            PronunciationAssignmentDetailsReadModel pronunciationDetails =>
                CreatePronunciationDto(pronunciationDetails, restricted),
            _ => throw new LangAppException("Wrong assignment details type")
        };
    }

    private static AssignmentDetailsDto CreateMultipleChoiceDto(
        MultipleChoiceAssignmentDetailsReadModel details,
        bool restricted)
    {
        return restricted
            ? new MultipleChoiceAssignmentRestrictedDetailsDto(
                details.Questions
                    .Select(q => new MultipleChoiceRestrictedQuestionDto(
                        q.Question,
                        q.Options.Select(o => o.OptionDescription).ToList()))
                    .ToList())
            : new MultipleChoiceAssignmentDetailsDto(
                details.Questions
                    .Select(q => new MultipleChoiceQuestionDto(
                        q.Question,
                        q.Options.Select(o => o.OptionDescription).ToList(),
                        q.CorrectOptionIndex))
                    .ToList());
    }

    private static AssignmentDetailsDto CreateFillInTheBlankDto(
        FillInTheBlankAssignmentDetailsReadModel details,
        bool restricted)
    {
        return restricted
            ? new FillInTheBlankAssignmentRestrictedDetailsDto(
                details.Questions
                    .Select(q => new FillInTheBlankRestrictedQuestionDto(q.TemplateText))
                    .ToList())
            : new FillInTheBlankAssignmentDetailsDto(
                details.Questions
                    .Select(q => new FillInTheBlankQuestionDto(
                        q.TemplateText,
                        q.Answers.Select(a => new FillInTheBlankAnswerDto(a.AcceptableAnswers)).ToList()))
                    .ToList());
    }

    private static AssignmentDetailsDto CreatePronunciationDto(
        PronunciationAssignmentDetailsReadModel details,
        bool restricted)
    {
        // For pronunciation, both DTOs have the same structure
        return new PronunciationAssignmentDetailsDto(
            details.Language.Code,
            details.ReferenceText);
    }
}
