namespace Neo.Bpms.Infrastructure.Features.Orm.Provider;

public class DataSourceMonitoring
{
    #region LastCommand

    public ConcurrentDictionary<string, NeoTableConnection> NeoTableConnections =
        new();

    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    private readonly object _lock = new();
    public long ConnectionsCounter;

    public void SetLastCommand(string tableName, string command,
        TimeSpan duration, DateTime dateTime, bool addCounter)
    {
        if (string.IsNullOrEmpty(command?.Trim()))
        {
            Logger.LogError("run empty command for table {0}", tableName);
            return;
        }

        lock (_lock)
        {
            var tableConnection = FetchNeoTableConnection(tableName);
            if (tableConnection == null) return;
            tableConnection.LastCommand = command;
            tableConnection.LastDuration = duration;
            tableConnection.LastDateTime = dateTime;
            if (addCounter)
                tableConnection.Counter++;
        }
    }

    public void SetLastConnectivityId(string clientConnectionId, string tableName)
    {
        lock (_lock)
        {
            ConnectionsCounter++;
            var tableConnection = FetchNeoTableConnection(tableName);
            if (tableConnection != null)
                tableConnection.ClientConnectionId = clientConnectionId;
        }
    }

    private NeoTableConnection FetchNeoTableConnection(string tableName)
    {
        NeoTableConnections.TryGetValue(tableName, out var tableConnection);
        if (tableConnection != null)
            return tableConnection;
        if (tableName.IndexOf(".", StringComparison.Ordinal) >= 0)
            return null;
        tableConnection = new NeoTableConnection(tableName);
        NeoTableConnections.TryAdd(tableName, tableConnection);
        return tableConnection;
    }

    #endregion
}
