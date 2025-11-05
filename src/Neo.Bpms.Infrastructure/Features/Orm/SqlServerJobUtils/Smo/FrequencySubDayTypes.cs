namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Smo;

[Flags]
public enum FrequencySubDayTypes
{
    /// <summary>Schedule reflects an activity scheduled using an hour as the unit.</summary>
    Hour = 8,
    /// <summary>
    /// Schedule reflects an activity scheduled using a minute as the unit. </summary>
    Minute = 4,
    /// <summary>
    /// Schedule reflects an activity scheduled using a second as the unit. </summary>
    Second = 2,
    /// <summary>
    /// Schedule reflects an activity that occurs once on a scheduled unit. </summary>
    Once = 1,
    /// <summary>Subunits are invalid for the scheduled activity.</summary>
    Unknown = 0,
}
