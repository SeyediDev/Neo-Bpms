namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Manager;

public class BackupMover(BackUpOptions backUpOptions)
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;

    private Timer _timer;

    public void Do()
    {
        Logger.LogInformation("Doing backup movement.");
        CreateDirectoryIfNotExists();
        MoveLocalBackup();
        DeleteWhatShouldBeDeleted();
    }

    public void SetTimer()
    {
        _timer = new Timer(DoTimer, new AutoResetEvent(false),
            TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5));
    }

    private void DoTimer(object stateInfo)
    {
        try
        {
            if (CheckTimeOfBackup() &&
                Directory.GetFiles(backUpOptions.BackupsSourcePath).Length != 0)
            {
                Do();
            }
        }
        catch (Exception e)
        {
            Logger.LogError("File movement failed.");
            Logger.LogError(e, e.Message);
            // todo notify?
        }
    }

    private bool CheckTimeOfBackup()
    {
        return DateTime.Now.TimeOfDay > backUpOptions.BackupMovementStartTimeOfDay;
    }

    private void MoveLocalBackup()
    {
        foreach (var file in Directory.GetFiles(backUpOptions.BackupsSourcePath))
        {
            Logger.LogTrace("Moving backup {0}", file);
            var destPath = Path.Combine(backUpOptions.BackupsDestinationPath, Path.GetFileName(file));
            File.Move(file, destPath);
        }
    }

    private void DeleteWhatShouldBeDeleted()
    {
        foreach (var file in Directory.GetFiles(backUpOptions.BackupsDestinationPath))
        {
            if (DateTime.TryParseExact(Path.GetFileNameWithoutExtension(file), "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date))
            {
                if (!ShouldKeepBackup(date))
                {
                    Logger.LogTrace("Deleting file {0}", file);
                    File.Delete(file);
                }
                else
                {
                    Logger.LogTrace("Keeping file {0}", file);
                }
            }
            else
            {
                Logger.LogError("Invalid filename {0}. The filename must follow yyyy-MM-dd.bak format.", file);
            }
        }
    }

    private bool ShouldKeepBackup(DateTime date)
    {
        bool IsFirstOfMonth()
        {
            return date.Date.Day == 1;
        }

        //bool PeriodReached()
        //{
        //	return (date.Date - _backUpOptions.ReferenceDate.Date).Days % _backUpOptions.KeepBackupsOfEveryXDays == 0;
        //}

        bool MaximumDaysPassed()
        {
            return (date.Date - DateTime.Now.Date).Days > backUpOptions.MaximumBackupKeepingDays;
        }

        bool IsTodayBackup()
        {
            return date.Date == DateTime.Now.Date;
        }

        bool IsYesterdayBackup()
        {
            return date.Date == DateTime.Now.Date.AddDays(-1);
        }

        return IsTodayBackup()
               || IsYesterdayBackup()
               || IsFirstOfMonth() && !MaximumDaysPassed();
    }

    private void CreateDirectoryIfNotExists()
    {
        var exists = Directory.Exists(backUpOptions.BackupsDestinationPath);
        if (!exists && LoginIfNeeded())
        {
            exists = Directory.Exists(backUpOptions.BackupsDestinationPath);
        }

        if (!exists)
        {
            Logger.LogTrace("Creating directory {0}", backUpOptions.BackupsDestinationPath);
            Directory.CreateDirectory(backUpOptions.BackupsDestinationPath);
        }
        else
        {
            Logger.LogTrace("Destination directory exists. {path}", backUpOptions.BackupsDestinationPath);
        }
    }

    private bool LoginIfNeeded()
    {
        return !string.IsNullOrEmpty(backUpOptions.BackupsDestinationUsername);
    }
}
