using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Submissions;

namespace LangApp.Core.Services.EvaluationStrategies;

public interface IGradingStrategy<TAssignmentDetails, TSubmissionDetails> where TAssignmentDetails : AssignmentDetails
    where TSubmissionDetails : SubmissionDetails
{
    SubmissionGrade Evaluate(TAssignmentDetails assignment, TSubmissionDetails submission);
}