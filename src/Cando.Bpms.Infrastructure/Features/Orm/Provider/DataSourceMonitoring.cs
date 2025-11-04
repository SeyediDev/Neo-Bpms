namespace Neo.Bpms.Infrastructure.Features.Orm.Provider;

public class DataSourceMonitoring
{
    #region LastCommand

    public ConcurrentDictionary<string, CandoTableConnection> CandoTableConnections =
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
            var tableConnection = FetchCandoTableConnection(tableName);
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
            var tableConnection = FetchCandoTableConnection(tableName);
            if (tableConnection != null)
                tableConnection.ClientConnectionId = clientConnectionId;
        }
    }

    private CandoTableConnection FetchCandoTableConnection(string tableName)
    {
        CandoTableConnections.TryGetValue(tableName, out var tableConnection);
        if (tableConnection != null)
            return tableConnection;
        if (tableName.IndexOf(".", StringComparison.Ordinal) >= 0)
            return null;
        tableConnection = new CandoTableConnection(tableName);
        CandoTableConnections.TryAdd(tableName, tableConnection);
        return tableConnection;
    }

    #endregion
}
