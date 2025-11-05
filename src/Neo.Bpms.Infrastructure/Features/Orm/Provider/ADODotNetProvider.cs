using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

namespace Neo.Bpms.Infrastructure.Features.Orm.Provider;

public abstract class AdoDotNetProvider(IConfiguration configuration, string name, bool dontSync = false) : DataProvider(configuration, name)
{
    public override string DatabaseName => AdoDotNetDefinition.DatabaseName;
    public string ConnectionString => AdoDotNetDefinition.ConnectionString;
    public override bool DontHaveDataDictionary => false;

    public override bool DontSync => dontSync;

    protected AdoDotNetDatabaseConnectionDefinition AdoDotNetDefinition =>
        Definition as AdoDotNetDatabaseConnectionDefinition;

    public override void SetConnectionParams(LocalParameters connectionParameters, LocalParameters localParameters)
    {
    }

    public override void SetRecordConnectionParams(LocalParameters connectionParameters, string recordId)
    {
    }
}
