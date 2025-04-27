using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.MultipleChoice;

namespace LangApp.Core.Services.GradingStrategies;

// TODO: is this "async" code a good choice?
public class MultipleChoiceGradingStrategy
    : IGradingStrategy<MultipleChoiceAssignmentDetails>
{
    public Task<SubmissionGrade> Grade(MultipleChoiceAssignmentDetails assignment,
        SubmissionDetails submission, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            return Task.FromResult(ExecuteGrade(assignment, submission, cancellationToken));
        }
        catch (Exception e)
        {
            return Task.FromException<SubmissionGrade>(e);
        }
    }

    private SubmissionGrade ExecuteGrade(MultipleChoiceAssignmentDetails assignment, SubmissionDetails submission,
        CancellationToken cancellationToken)
    {
        if (submission is not MultipleChoiceSubmissionDetails multipleChoiceSubmissionDetails)
        {
            throw new LangAppException(
                $"Provided submission {submission.GetType()} is not compatible with the assignment {assignment.GetType()}");
        }

        if (multipleChoiceSubmissionDetails.Answers.Count != assignment.Questions.Count)
        {
            throw new LangAppException("Invalid multiple choice answer count");
        }

        double correctOptions = assignment.Questions.Zip(multipleChoiceSubmissionDetails.Answers)
            .Count(t => t.First.CorrectOptionIndex == t.Second.ChosenOptionIndex);

        Percentage percentage = new(correctOptions / assignment.Questions.Count * 100);

        return new SubmissionGrade(percentage);
    }
}
