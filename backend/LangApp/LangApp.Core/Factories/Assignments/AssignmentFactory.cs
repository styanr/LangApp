using LangApp.Core.Entities.Assignments;
using LangApp.Core.Enums;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;

namespace LangApp.Core.Factories.Assignments;

public class AssignmentFactory : IAssignmentFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public AssignmentFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    private Assignment CreateAssignment(
        AssignmentDetails details,
        Guid authorId,
        Guid groupId,
        DateTime dueTime,
        int maxScore,
        AssignmentType type)
    {
        var id = _keyGenerator.NewKey();
        return new Assignment(id, details, authorId, groupId, dueTime, maxScore, type);
    }

    public Assignment CreateMultipleChoice(
        MultipleChoiceAssignmentDetails assignmentDetails,
        Guid authorId,
        Guid groupId,
        DateTime dueTime,
        int maxScore)
        => CreateAssignment(
            assignmentDetails,
            authorId,
            groupId,
            dueTime,
            maxScore,
            AssignmentType.MultipleChoice);

    public Assignment CreateFillInTheBlank(
        FillInTheBlankAssignmentDetails assignmentDetails,
        Guid authorId,
        Guid groupId,
        DateTime dueTime,
        int maxScore)
        => CreateAssignment(
            assignmentDetails,
            authorId,
            groupId,
            dueTime,
            maxScore,
            AssignmentType.FillInTheBlank);

    public Assignment CreatePronunciation(
        PronunciationAssignmentDetails assignmentDetails,
        Guid authorId,
        Guid groupId,
        DateTime dueTime,
        int maxScore)
        => CreateAssignment(
            assignmentDetails,
            authorId,
            groupId,
            dueTime,
            maxScore,
            AssignmentType.Pronunciation);
}