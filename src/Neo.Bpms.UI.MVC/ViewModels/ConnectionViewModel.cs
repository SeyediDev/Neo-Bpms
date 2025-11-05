using System.Diagnostics;

namespace Neo.Bpms.UI.MVC.ViewModels;

public class ConnectionsInformationViewModel
{
    public PerformanceCounter[] PerformanceCounters;
    public float[] PerformanceCountersValues;
    public List<ConnectionsViewModel> List;
}
public class ConnectionsViewModel
{
    public string Name { get; set; }
    public string State { get; set; }
    public string ConnectionType { get; set; }
    public string DataSource { get; set; }
    public string UserName { get; set; }
    public int Max { get; set; }
    public List<ConnectionsTableViewModel> Tables { get; }
    public List<ConnectionViewModel> Connections { get; }
    public long ConnectionsCounter { get; set; }

    public ConnectionsViewModel()
    {
        Tables = [];
        Connections = [];
    }
}
public class ConnectionViewModel
{
    public long Index { get; set; }
    public string Id { get; set; }
    public string Host { get; set; }
    public string Program { get; set; }
    public string State { get; set; }
    public string Login { get; set; }
    public long MemoryUsage { get; set; }
    public long CpuUsage { get; set; }
    public long PhysicalUsage { get; set; }
    public DateTime LoginTime { get; set; }
    public DateTime LastBatch { get; set; }
    public string Command { get; set; }
}
public class ConnectionsTableViewModel
{
    public long Index { get; set; }
    public string Name { get; set; }
    public long Counter { get; set; }
    public string LastCommand { get; set; }
    public TimeSpan LastDuration { get; set; }
    public DateTime LastDateTime { get; set; }
    public string ClientConnectionId { get; set; }
}
