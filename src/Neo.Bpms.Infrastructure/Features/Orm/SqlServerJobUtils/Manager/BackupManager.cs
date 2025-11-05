using Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.JobManager;
using Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Smo;

namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Manager;

public class BackupManager(BackUpOptions backUpOptions) : SmoJobManager(backUpOptions)
{
    private BackUpOptions _backUpOptions => (BackUpOptions)_options;
    private BackupMover _backupMover;

    protected override void SetJobUtility(JobUtility jobUtility)
    {
        var backupScriptGenerator =
            new BackupScriptGenerator(_backUpOptions.DatabaseName, _backUpOptions.BackupsSourcePath);
        var jobIsSet = jobUtility.SetupJob(JobName(),
            backupScriptGenerator.GetTSqlCommand(), _backUpOptions.JobStartTimeOfDay);
        if (!jobIsSet)
            NotifyJobSetFailure();
    }

    protected override void SetTimer()
    {
        _backupMover = new BackupMover(_backUpOptions);
        _backupMover.SetTimer();
    }

    protected override string JobName()
    {
        return $"{_backUpOptions.DatabaseName}DailyFullBackup";
    }
}
