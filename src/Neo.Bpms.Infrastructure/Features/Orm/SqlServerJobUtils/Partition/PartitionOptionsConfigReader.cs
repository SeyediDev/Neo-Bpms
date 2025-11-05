using Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.JobManager;

namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Partition;

public class PartitionOptionsConfigReader : OptionsConfigReader
{
    public static PartitionOptions GetOptions()
    {
        var config = DependencyInjectionHolder.Instance.Configuration;
        var options = new PartitionOptions();
        GetOptions(options, config);
        if (!string.IsNullOrEmpty(config[nameof(PartitionOptions.ExpireDays)]))
            options.ExpireDays = int.Parse(config[nameof(PartitionOptions.ExpireDays)]);
        if (!string.IsNullOrEmpty(config[nameof(PartitionOptions.ExpandDays)]))
            options.ExpandDays = int.Parse(config[nameof(PartitionOptions.ExpandDays)]);
        return options;
    }
}
