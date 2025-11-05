namespace Neo.Bpms.Infrastructure.Features.Orm.Provider;
public class NeoTableConnection(string tableName)
{
    public string TableName { get; set; } = tableName;
    public long Counter { get; set; }
    public string LastCommand { get; set; }
    public TimeSpan LastDuration { get; set; }
    public DateTime LastDateTime { get; set; }
    public string ClientConnectionId { get; set; }
}