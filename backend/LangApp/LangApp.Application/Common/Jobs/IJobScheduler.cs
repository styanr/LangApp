using System.Linq.Expressions;

namespace LangApp.Application.Common.Jobs;

public interface IJobScheduler
{
    void Enqueue<TJob>(Expression<Action<TJob>> job);
    void Enqueue(Expression<Action> job);
}
