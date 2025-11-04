using Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Smo;

namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.JobManager;

public abstract class SmoJobManager
{
    protected readonly Options _options;

    protected SmoJobManager(Options options)
    {
        _options = options;
        _options.Validate();
    }

    public void SetJob()
    {
        using (var jobUtility = new JobUtility(_options.ConnectionString, Log))
        {
            SetJobUtility(jobUtility);
        }

        SetTimer();
    }

    protected abstract void SetJobUtility(JobUtility jobUtility);

    protected virtual void SetTimer()
    {
    }

    protected abstract string JobName();

    protected virtual void NotifyJobSetFailure()
    {
        // todo
    }

    public List<JobSummary> GetAllJobs()
    {
        using (var jobUtility = new JobUtility(_options.ConnectionString, Log))
        {
            return jobUtility.GetAllJobs();
        }
    }

    public JobState GetJobStates()
    {
        throw new NotImplementedException();
    }

    public void DropJob()
    {
        using (var jobUtility = new JobUtility(_options.ConnectionString, Log))
        {
#pragma warning disable CS0612 // Type or member is obsolete
            jobUtility.DropJob(JobName());
#pragma warning restore CS0612 // Type or member is obsolete
        }
    }

    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;

    private static void Log(LogType type, string log, Exception exception = null)
    {
        switch (type)
        {
            case LogType.Trace:
                Logger.LogTrace(log);
                break;
            case LogType.Debug:
                Logger.LogDebug(log);
                break;
            case LogType.Info:
                Logger.LogInformation(log);
                break;
            case LogType.Error:
                if (exception != null)
                    Logger.LogError(exception, log);
                else if (log != null)
                    Logger.LogError(log);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
