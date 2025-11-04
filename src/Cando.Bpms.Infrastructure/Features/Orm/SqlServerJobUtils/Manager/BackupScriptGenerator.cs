namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Manager;

internal class BackupScriptGenerator
{
    private readonly string _databaseName;
    private readonly string _diskPath;

    internal BackupScriptGenerator(string databaseName, string diskPath)
    {
        if (diskPath.Contains("'"))
            throw new Exception($"Dangerous diskPath: {diskPath}");
        if (databaseName.Contains("'"))
            throw new Exception($"Dangerous databaseName: {databaseName}");
        _databaseName = databaseName;
        _diskPath = diskPath;
        if (!_diskPath.EndsWith("\\"))
            _diskPath += "\\";
    }

    internal string GetTSqlCommand()
    {
        return $@"
DECLARE @path nvarchar(100) = Concat('{_diskPath}',CONVERT(CHAR(10), CURRENT_TIMESTAMP, 120), '.bak');
BACKUP DATABASE [{_databaseName}] TO  DISK = @path
WITH NOFORMAT, INIT,
NAME = N'{_databaseName} Full Backup', SKIP, COMPRESSION, CHECKSUM
GO";
    }
}
