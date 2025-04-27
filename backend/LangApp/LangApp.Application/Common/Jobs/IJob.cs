namespace LangApp.Application.Common.Jobs;

public interface IJobData;

public interface IJob<in TJobData> where TJobData : IJobData
{
    Task ExecuteAsync(TJobData jobData);
}
