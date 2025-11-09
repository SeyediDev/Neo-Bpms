using Neo.Bpms.Domain.Models.Cmmn.Data.DDL;
using Neo.Bpms.Domain.Models.Cmmn.Data.DML;
using Neo.Bpms.Domain.Models.Cmmn.Data.Provider;
using Neo.Bpms.Domain.Models.Cmmn.Data.Query;

namespace Neo.Bpms.Infrastructure.Features.Orm.Provider;

public abstract class DataProvider(IConfiguration configuration, string name)
    : IDataProvider
{
    protected readonly IConfiguration _configuration = configuration;

    public CancellationToken CancellationToken { get; set; }
    public string Name { get; set; } = name;

    public abstract bool DontHaveDataDictionary { get; }
    public abstract bool DontSync { get; }

    public abstract IDataSource GetDataSource(Entity entity, LocalParameters connectionParameters, IAuditTrail auditTrail);
    public abstract IDDLGenerator GetDDLGenerator();
    public abstract IDMLGenerator GetDMLGenerator();
    public abstract IQueryGenerator GetQueryGenerator();

    public abstract IDDLManager GetDdlManager(IDataProvider provider, IConfiguration configuration, LocalParameters connectionParameters);

    public abstract void SetConnectionParams(LocalParameters connectionParameters, LocalParameters localParameters);
    public abstract void SetRecordConnectionParams(LocalParameters connectionParameters, string recordId);
    
    protected ConnectionDefinition Definition { get; set; }
    public virtual string DatabaseName { get; } = null;

    public DataSourceMonitoring DataSourceMonitoring { get; set; } = new DataSourceMonitoring();

    public virtual void Init()
    {
        Definition = CreateConnectionDefinition();
        Definition?.Init();
    }

    protected abstract ConnectionDefinition CreateConnectionDefinition();

    public void SetLastCommand(string tableName, string command,
        TimeSpan duration, DateTime dateTime, bool addCounter)
    {
        DataSourceMonitoring.SetLastCommand(tableName, command, duration, dateTime, addCounter);
    }

    public void SetLastConnectivityId(string clientConnectionId, string tableName)
    {
        DataSourceMonitoring.SetLastConnectivityId(clientConnectionId, tableName);
    }

    public void Dispose()
    {
    }
}
