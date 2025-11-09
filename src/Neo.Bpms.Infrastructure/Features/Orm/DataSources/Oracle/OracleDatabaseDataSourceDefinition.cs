using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Oracle;
public class OracleDatabaseDataSourceDefinition(OracleDatabaseConnectionDefinition connection, string name,
    Entity entity) : ADODotNetDatabaseDataSourceDefinition(connection, name, entity)
{
}
