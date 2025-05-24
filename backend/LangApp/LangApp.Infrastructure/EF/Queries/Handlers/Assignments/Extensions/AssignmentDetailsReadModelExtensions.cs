using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Pronunciation;
using LangApp.Application.Assignments.Dto.Question;
using LangApp.Application.Assignments.Dto.Writing;
using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Core.ValueObjects.Assignments.Writing;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.Assignments.FillInTheBlank;
using LangApp.Infrastructure.EF.Models.Assignments.MultipleChoice;
using LangApp.Infrastructure.EF.Models.Assignments.Pronunciation;
using LangApp.Infrastructure.EF.Models.Assignments.Question;
using LangApp.Infrastructure.EF.Models.Assignments.Writing;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Assignments.Extensions;

public static class AssignmentDetailsReadModelExtensions
{
    public static AssignmentDto ToDto(this AssignmentReadModel assignment, bool submitted, bool restricted = false)
    {
        return new AssignmentDto(
            assignment.Id,
            assignment.Name,
            assignment.Description,
            assignment.AuthorId,
            assignment.StudyGroupId,
            assignment.DueDate,
            assignment.Activities.Aggregate(0, (acc, ac) => acc + ac.MaxScore),
            submitted,
            assignment.Activities.Select(ac =>
                new ActivityDto(
                    ac.Id,
                    ac.MaxScore,
                    ac.Details.ToDto(restricted)
                )
            ).ToList()
        );
    }

    public static AssignmentSlimDto ToSlimDto(this AssignmentReadModel assignment, bool submitted)
    {
        return new AssignmentSlimDto(
            assignment.Id,
            assignment.Name,
            assignment.Description,
            assignment.AuthorId,
            assignment.StudyGroupId,
            assignment.DueDate,
            assignment.Activities.Aggregate(0, (acc, ac) => acc + ac.MaxScore),
            submitted,
            assignment.Activities.Count
        );
    }

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
            QuestionActivityDetailsReadModel questionDetails =>
                CreateQuestionDto(questionDetails, restricted),
            WritingActivityDetailsReadModel writingDetails =>
                CreateWritingDto(writingDetails, restricted),
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
            details.ReferenceText,
            details.AllowAssessment,
            details.AllowListening);
    }

    private static ActivityDetailsDto CreateQuestionDto(
        QuestionActivityDetailsReadModel details,
        bool restricted)
    {
        return restricted
            ? new QuestionActivityRestrictedDetailsDto(
                details.Question,
                details.MaxLength)
            : new QuestionActivityDetailsDto(
                details.Question,
                details.Answers,
                details.MaxLength);
    }

    private static ActivityDetailsDto CreateWritingDto(
        WritingActivityDetailsReadModel details,
        bool restricted)
    {
        return new WritingActivityDetailsDto(details.Prompt, details.MaxWords);
    }
}