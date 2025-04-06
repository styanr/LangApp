using LangApp.Application.Assignments.Dto;
using LangApp.Core.Exceptions;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.Assignments.MultipleChoice;

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
            _ => throw new LangAppException("Wrong assignment details type")
        };
    }
}