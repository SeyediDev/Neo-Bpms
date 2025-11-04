using Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.JobManager;
using Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Smo;

namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Partition;

public class PartitionManager(PartitionOptions options,
    string partitionFunction, string partitionSchema, params string[] tables) : SmoJobManager(options)
{
    private PartitionOptions _partitionOptions => (PartitionOptions)_options;

    protected override void SetJobUtility(JobUtility jobUtility)
    {
        var scriptGenerator = new PartitionScriptGenerator(_options.DatabaseName,
                partitionFunction, partitionSchema, tables);
        var jobIsSet = jobUtility.SetupJob(JobName(),
            scriptGenerator.GetTSqlCommand(_partitionOptions.ExpireDays, _partitionOptions.ExpandDays),
            _options.JobStartTimeOfDay);
        if (!jobIsSet)
            NotifyJobSetFailure();
    }

    protected override string JobName()
    {
        return $"{_options.DatabaseName}_DailyPartition_{partitionSchema}";
    }
}
