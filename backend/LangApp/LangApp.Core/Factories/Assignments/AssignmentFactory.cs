using LangApp.Core.Entities.Assignments;
using LangApp.Core.Enums;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;

namespace LangApp.Core.Factories.Assignments;

public class AssignmentFactory : IAssignmentFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public AssignmentFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    public Assignment CreateMultipleChoice(MultipleChoiceAssignmentDetails assignmentDetails, Guid authorId,
        Guid groupId,
        DateTime dueTime, int maxScore)
    {
        var id = _keyGenerator.NewKey();
        return new Assignment(id, assignmentDetails, authorId, groupId, dueTime, maxScore,
            AssignmentType.MultipleChoice);
    }

    public Assignment CreateFillInTheBlank(FillInTheBlankAssignmentDetails assignmentDetails, Guid authorId,
        Guid groupId,
        DateTime dueTime, int maxScore)
    {
        var id = _keyGenerator.NewKey();
        return new Assignment(id, assignmentDetails, authorId, groupId, dueTime, maxScore,
            AssignmentType.FillInTheBlank);
    }
}