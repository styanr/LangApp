using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments.Question;
using LangApp.Core.ValueObjects.Submissions;
using LangApp.Core.ValueObjects.Submissions.Question;

namespace LangApp.Core.Services.GradingStrategies;

public class QuestionGradingStrategy : IGradingStrategy<QuestionActivityDetails>
{
    public Task<SubmissionGrade> Grade(QuestionActivityDetails assignment, SubmissionDetails submission,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }
}
