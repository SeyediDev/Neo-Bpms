namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.JobManager;

public class Options
{
    public string DatabaseName { get; set; }

    /// <summary>
    /// The connection string of Sql Server database
    /// </summary>
    public string ConnectionString { get; set; }
    /// <summary>
    /// When backup job is set to. Default is 2 A.M.
    /// </summary>
    public TimeSpan JobStartTimeOfDay { get; set; } = new TimeSpan(2, 0, 0);

    public virtual void Validate()
    {
        if (string.IsNullOrEmpty(DatabaseName))
            throw new ArgumentNullException(nameof(DatabaseName));
        if (string.IsNullOrEmpty(ConnectionString))
            throw new ArgumentNullException(nameof(ConnectionString));
    }
}