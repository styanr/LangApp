using LangApp.Core.Entities.Assignments;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;

namespace LangApp.Core.Factories.Assignments;

public interface IAssignmentFactory
{
    Assignment CreateMultipleChoice(
        MultipleChoiceAssignmentDetails assignmentDetails,
        Guid authorId,
        Guid groupId,
        DateTime dueTime,
        int maxScore);


    Assignment CreateFillInTheBlank(
        FillInTheBlankAssignmentDetails assignmentDetails,
        Guid authorId,
        Guid groupId,
        DateTime dueTime,
        int maxScore);
}