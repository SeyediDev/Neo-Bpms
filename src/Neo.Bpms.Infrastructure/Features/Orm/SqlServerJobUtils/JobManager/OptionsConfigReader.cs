using Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Manager;

namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.JobManager;

public class OptionsConfigReader
{
    protected static void GetOptions(Options options, IConfiguration config)
    {
        options.DatabaseName = config["Database"];
        options.ConnectionString = $"Server={config["Server"]};User Id={config["UserId"]};Password={config["PW"]};";
        if (!string.IsNullOrEmpty(config[nameof(BackUpOptions.JobStartTimeOfDay)]))
            options.JobStartTimeOfDay = GetTime(config[nameof(BackUpOptions.JobStartTimeOfDay)]);
    }

    protected static TimeSpan GetTime(string s)
    {
        return TimeSpan.Parse(s);
    }

    protected static DateTime GetDate(string s)
    {
        return DateTime.Parse(s);
    }
}
