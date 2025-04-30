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
    public static AssignmentDetailsDto ToDto(this AssignmentDetailsReadModel details)
    {
        return details switch
        {
            MultipleChoiceAssignmentDetailsReadModel multipleChoiceDetails => new MultipleChoiceAssignmentDetailsDto(
                multipleChoiceDetails.Questions
                    .Select(q => new MultipleChoiceQuestionDto(q.Question,
                        q.Options.Select(o => o.OptionDescription).ToList(),
                        q.CorrectOptionIndex)).ToList()
            ),
            FillInTheBlankAssignmentDetailsReadModel fillInTheBlankDetails => new FillInTheBlankAssignmentDetailsDto(
                fillInTheBlankDetails.Questions
                    .Select(q => new FillInTheBlankQuestionDto(
                        q.TemplateText,
                        q.Answers.Select(a => new FillInTheBlankAnswerDto(a.AcceptableAnswers)).ToList()
                    )).ToList()
            ),
            PronunciationAssignmentDetailsReadModel pronunciationDetails => new PronunciationAssignmentDetailsDto(
                pronunciationDetails.Language.Code,
                pronunciationDetails.ReferenceText
            ),
            _ => throw new LangAppException("Wrong assignment details type")
        };
    }
}
