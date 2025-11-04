using Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.JobManager;

namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Manager;

public class BackUpOptions : Options
{
    /// <summary>
    /// The path of the folder which SqlServer will use to store backup.
    /// </summary>
    public string BackupsSourcePath { get; set; }

    /// <summary>
    /// The path of the folder which backups will be moved to.
    /// </summary>
    public string BackupsDestinationPath { get; set; }

    /// <summary>
    /// Username. Needed if destination address is a network drive which needs login
    /// </summary>
    public string BackupsDestinationUsername { get; set; }

    /// <summary>
    /// Password. Needed if destination address is a network drive which needs login
    /// </summary>
    public string BackupsDestinationPassword { get; set; }

    ///// <summary>
    ///// Determines the period which backups will be maintained and not removed.
    ///// </summary>
    //public int KeepBackupsOfEveryXDays { get; set; }

    ///// <summary>
    ///// <see cref="KeepBackupsOfEveryXDays"/> is calculated from this date.
    ///// </summary>
    //public DateTime ReferenceDate { get; set; }

    /// <summary>
    /// When backup movements and removes occur. Default is 4 A.M.
    /// </summary>
    public TimeSpan BackupMovementStartTimeOfDay { get; set; } = new TimeSpan(4, 0, 0);

    /// <summary>
    /// Backups will be deleted after X days passed.
    /// </summary>
    public int MaximumBackupKeepingDays { get; set; }

    public override void Validate()
    {
        base.Validate();
        if (string.IsNullOrEmpty(BackupsSourcePath))
            throw new ArgumentNullException(nameof(BackupsSourcePath));
        if (string.IsNullOrEmpty(BackupsDestinationPath))
            throw new ArgumentNullException(nameof(BackupsDestinationPath));
    }
}
