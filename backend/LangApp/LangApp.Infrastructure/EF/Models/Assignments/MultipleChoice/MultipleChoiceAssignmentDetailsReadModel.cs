using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;

namespace LangApp.Infrastructure.EF.Models.Assignments.MultipleChoice;

public class MultipleChoiceAssignmentDetailsReadModel : AssignmentDetailsReadModel
{
    public List<MultipleChoiceQuestionReadModel> Questions { get; set; }
};