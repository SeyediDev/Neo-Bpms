using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;

namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Smo;

public class JobUtility : IDisposable
{
    private readonly Server _server;

    private readonly Action<LogType, string, Exception> _log;

    /// <exception cref="SqlException"></exception>
    public JobUtility(string connectionString, Action<LogType, string, Exception> log = null)
    {
        _log = log;
        Log(LogType.Trace, "Attempting to connect to server.");
        try
        {
            _server = new Server();
            _server.ConnectionContext.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect;
            _server.ConnectionContext.ConnectionString = connectionString;
            _server.ConnectionContext.Connect();

            //              alternative approach:
            //              var conn = new ServerConnection(connectionString);
            //              _server = new Server(conn);
            Log(LogType.Trace, "Successfully connected to serer");
        }
        catch (SqlException ex)
        {
            Log(LogType.Error, "Failed to connect to server", ex);
            throw;
        }
    }

    public List<JobSummary> GetAllJobs()
    {
        Log(LogType.Trace, "Getting all jobs");
        List<JobSummary> result = [];
        foreach (Job job in _server.JobServer.Jobs)
        {
            result.Add(new JobSummary(job));
        }

        return result;
    }

    public Job FindJob(string jobName)
    {
        return _server.JobServer.Jobs[jobName];
    }

    public JobState GetJobState(string jobName)
    {
        Job job = FindJob(jobName);
        if (job == null)
        {
            Log(LogType.Error, $"Unable to find job: {jobName}");
            return JobState.NotFound;
        }

        JobState jobState = (JobState)Enum.Parse(typeof(JobState), job.CurrentRunStatus.ToString());
        return jobState;
    }

    [Obsolete]
    public bool DropJob(string jobName)
    {
        Job job = FindJob(jobName);
        if (job == null)
        {
            Log(LogType.Error, $"Unable to find job: {jobName}");
            return false;
        }

        _server.JobServer.DropJobByID(job.JobID);
        Log(LogType.Info, $"Job {jobName} dropped.");
        return true;
    }

    public List<string> GetJobHistory(string jobName, DateTime startRunDate)
    {
        Job job = FindJob(jobName);
        if (job == null)
        {
            Log(LogType.Error, $"Unable to find job: {jobName}");
            return null;
        }

        JobHistoryFilter filter = new()
        {
            StartRunDate = startRunDate
        };
        DataTable history = job.EnumHistory(filter);
        return history.AsEnumerable().Select(row => row["JobName"].ToString()).ToList();
    }

    /// <returns>Returns weather or not the job was started</returns>
    public bool StartJob(string jobName)
    {
        Job job = FindJob(jobName);
        if (job == null)
        {
            Log(LogType.Info, $"Unable to find job: {jobName}");
            return false;
        }

        if (!job.IsEnabled)
        {
            job.IsEnabled = true;
            Log(LogType.Trace, $"Enabling job: {jobName}");
        }

        try
        {
            job.Start();
            Log(LogType.Info, $"Job started: {jobName}");
            return true;
        }
        catch (Exception ex)
        {
            Log(LogType.Error, $"Unable to start SQL Job: {jobName}", ex);
            return false;
        }
    }

    /// <returns>Determines wearer or not a job was stopped</returns>
    public bool StopJob(string jobName)
    {
        Job job = FindJob(jobName);
        if (job == null)
        {
            Log(LogType.Info, $"Unable to find job: {jobName}");
            return false;
        }

        try
        {
            job.Stop();
            Log(LogType.Info, $"Job Stopped: {jobName}");
            return true;
        }
        catch (Exception ex)
        {
            Log(LogType.Error, $"Unable to Stop SQL Job: {jobName}", ex);
            return false;
        }
    }

    /// <summary>
    /// Set job.
    /// </summary>
    /// <param name="jobName">Name of the job.</param>
    /// <param name="tSqlCommand">Command of step with TransactSql SubSystem</param>
    /// <param name="startTimeOfDay">Jobs are set daily at this time</param>
    /// <param name="frequencySubDayTypes"></param>
    /// <param name="frequencySubDayInterval"></param>
    /// <returns>Determines weather or not a job was Set</returns>
    public bool SetupJob(string jobName, string tSqlCommand,
        TimeSpan startTimeOfDay,
        FrequencySubDayTypes frequencySubDayTypes = FrequencySubDayTypes.Once,
        int frequencySubDayInterval = 1)
    {
        Job job = FindJob(jobName);
        if (job != null)
        {
            Log(LogType.Info, $"Job {jobName} was already set.");
            return true;
        }
        try
        {
            JobServer jobServer = _server.JobServer;
            JobSchedule schedule = new(jobServer, $"{jobName}Schedule")
            {
                FrequencyTypes = FrequencyTypes.Daily,
                FrequencyInterval = 1,
                FrequencySubDayTypes = (Microsoft.SqlServer.Management.Smo.Agent.FrequencySubDayTypes)frequencySubDayTypes,
                ActiveStartDate = DateTime.Today,
                ActiveStartTimeOfDay = startTimeOfDay
            };
            if (frequencySubDayTypes is not FrequencySubDayTypes.Once and
                not FrequencySubDayTypes.Unknown)
            {
                schedule.FrequencySubDayInterval = frequencySubDayInterval;
            }

            schedule.Create();
            job = new Job(jobServer, $"{jobName}");
            job.Create();
            job.AddSharedSchedule(schedule.ID);
            job.ApplyToTargetServer(_server.Name);
            JobStep step = new(job, $"{jobName}Step")
            {
                Command = tSqlCommand,
                SubSystem = AgentSubSystem.TransactSql
            };
            step.Create();
            Log(LogType.Info, $"Job {jobName} is set.");
            return true;
        }
        catch (Exception ex)
        {
            Log(LogType.Error, $"Unable to set up SQL Job: {jobName}", ex);
            return false;
        }
    }

    public void Dispose()
    {
        _server.ConnectionContext.Disconnect();
    }

    private void Log(LogType type, string text, Exception exception = null)
    {
        _log?.Invoke(type, text, exception);
    }
}
