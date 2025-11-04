using Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.JobManager;

namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Partition;

public class PartitionOptions : Options
{
    /// <summary>
    /// Data will be deleted after X days passed.
    /// </summary>
    public int ExpireDays { get; set; }

    /// <summary>
    /// Expand partition and ranges to X days.
    /// </summary>
    public int ExpandDays { get; set; }

    public override void Validate()
    {
        base.Validate();
        if (ExpireDays <= 0)
            ExpireDays = 60;
        if (ExpandDays <= 0)
            ExpandDays = 3;
    }
}