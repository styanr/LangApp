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

    public void Enqueue<TJob>(Expression<Action<TJob>> job)
    {
        _backgroundJobs.Enqueue<TJob>(job);
    }

    public void Enqueue(Expression<Action> job)
    {
        throw new NotImplementedException();
    }
}
