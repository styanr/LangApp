using System.Linq.Expressions;
using Hangfire;
using LangApp.Application.Common.Jobs;

namespace LangApp.Infrastructure.Hangfire;

public class JobScheduler : IJobScheduler
{
    private readonly IBackgroundJobClient _backgroundJobs;

    public JobScheduler(IBackgroundJobClient backgroundJobs)
    {
        _backgroundJobs = backgroundJobs;
    }

    public void Enqueue<TJob, TJobData>(Expression<Func<TJob, Task>> job)
        where TJob : IJob<TJobData>
        where TJobData : IJobData
    {
        _backgroundJobs.Enqueue(job);
    }

    public void Enqueue(Expression<Func<Task>> job)
    {
        _backgroundJobs.Enqueue(job);
    }
}
