using System.Linq.Expressions;

namespace LangApp.Application.Common.Jobs;

public interface IJobScheduler
{
    void Enqueue<TJob, TJobData>(Expression<Func<TJob, Task>> job)
        where TJob : IJob<TJobData>
        where TJobData : IJobData;

    void Enqueue(Expression<Func<Task>> job);
}
