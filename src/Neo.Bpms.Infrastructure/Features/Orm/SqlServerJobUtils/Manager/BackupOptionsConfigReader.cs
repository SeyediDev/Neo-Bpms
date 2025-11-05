using Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.JobManager;

namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Manager;

public class BackupOptionsConfigReader : OptionsConfigReader
{
    public static BackUpOptions GetOptions()
    {
        var config = DependencyInjectionHolder.Instance.Configuration;
        var options = new BackUpOptions
        {
            BackupsSourcePath = config[nameof(BackUpOptions.BackupsSourcePath)],
            BackupsDestinationPath = config[nameof(BackUpOptions.BackupsDestinationPath)],
            BackupsDestinationUsername = config[nameof(BackUpOptions.BackupsDestinationUsername)],
            BackupsDestinationPassword = config[nameof(BackUpOptions.BackupsDestinationPassword)],
            //KeepBackupsOfEveryXDays = int.Parse(config[nameof(BackUpOptions.KeepBackupsOfEveryXDays)]),
            //ReferenceDate = GetDate(config[nameof(BackUpOptions.ReferenceDate)]),
            MaximumBackupKeepingDays = int.Parse(config[nameof(BackUpOptions.MaximumBackupKeepingDays)])
        };
        GetOptions(options, config);
        if (!string.IsNullOrEmpty(config[nameof(BackUpOptions.BackupMovementStartTimeOfDay)]))
            options.BackupMovementStartTimeOfDay = GetTime(config[nameof(BackUpOptions.BackupMovementStartTimeOfDay)]);
        return options;
    }
}
