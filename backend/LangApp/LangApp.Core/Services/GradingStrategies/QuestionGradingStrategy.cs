using LangApp.Core.Exceptions;
using LangApp.Core.Exceptions.Grading;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.Question;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.Question;

namespace LangApp.Core.Services.GradingStrategies;

public class QuestionGradingStrategy : SynchronousGradingStrategy<QuestionActivityDetails>
{
    protected override SubmissionGrade ExecuteGrade(QuestionActivityDetails activity, SubmissionDetails submission,
        CancellationToken cancellationToken)
    {
        if (submission is not QuestionSubmissionDetails questionSubmission)
        {
            throw new IncompatibleSubmissionTypeException(submission.GetType(), activity.GetType());
        }

        string submitted = questionSubmission.Answer.Trim().ToLowerInvariant();

        bool isCorrect = activity.Answers
            .Any(answer => submitted == answer.Trim().ToLowerInvariant());

        var percentage = isCorrect ? new Percentage(100) : new Percentage(0);

        return new SubmissionGrade(percentage);
    }
}