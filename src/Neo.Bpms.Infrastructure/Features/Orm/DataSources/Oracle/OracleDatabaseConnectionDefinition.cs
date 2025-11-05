using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Oracle;

public class OracleDatabaseConnectionDefinition(IConfiguration configuration, string providerName) : AdoDotNetDatabaseConnectionDefinition(configuration, providerName)
{
    public override void Init()
    {
    }
}
