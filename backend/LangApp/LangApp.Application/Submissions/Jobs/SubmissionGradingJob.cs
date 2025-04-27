using LangApp.Application.Common.Jobs;

namespace LangApp.Application.Submissions.Jobs;

public record SubmissionGradingJobData(Guid SubmissionId) : IJobData;

public class SubmissionGradingJob : IJob<SubmissionGradingJobData>
{
    public void Execute(SubmissionGradingJobData data)
    {
        Console.WriteLine($"Hello WORLD! {data.SubmissionId}");
    }
}
