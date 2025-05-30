using LangApp.Core.Exceptions;
using LangApp.Core.Exceptions.Grading;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.FillInTheBlank;

namespace LangApp.Core.Services.GradingStrategies;

public class FillInTheBlankGradingStrategy : SynchronousGradingStrategy<FillInTheBlankActivityDetails>
{
    protected override SubmissionGrade ExecuteGrade(
        FillInTheBlankActivityDetails activity,
        SubmissionDetails submission,
        CancellationToken cancellationToken)
    {
        if (submission is not FillInTheBlankSubmissionDetails fillInTheBlankSubmission)
        {
            throw new IncompatibleSubmissionTypeException(submission.GetType(), activity.GetType());
        }

        int totalBlanks = activity.Answers.Count;

        if (totalBlanks == 0)
        {
            return new SubmissionGrade(new Percentage(0));
        }

        int correctAnswers = 0;

        foreach (var submittedAnswer in fillInTheBlankSubmission.Answers)
        {
            if (submittedAnswer.Index < 0 || submittedAnswer.Index >= activity.Answers.Count)
            {
                continue;
            }

            var correctAnswer = activity.Answers[submittedAnswer.Index];
            if (IsCorrectAnswer(submittedAnswer.Answer, correctAnswer))
            {
                correctAnswers++;
            }
        }

        double percentageValue = totalBlanks > 0
            ? (double)correctAnswers / totalBlanks * 100
            : 0;

        var percentage = new Percentage(percentageValue);
        return new SubmissionGrade(percentage);
    }

    private bool IsCorrectAnswer(string submittedAnswer, FillInTheBlankAnswer correctAnswer)
    {
        if (string.IsNullOrWhiteSpace(submittedAnswer))
        {
            return false;
        }

        var normalizedSubmission = submittedAnswer.Trim().ToLowerInvariant();

        foreach (var acceptableAnswer in correctAnswer.AcceptableAnswers)
        {
            var normalizedAcceptable = acceptableAnswer.Trim().ToLowerInvariant();

            if (normalizedSubmission == normalizedAcceptable)
            {
                return true;
            }
        }

        return false;
    }
}