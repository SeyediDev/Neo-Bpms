using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.SqlServer;

public class SQLServerDatabaseDataSourceDefinition(SqlServerDatabaseConnectionDefinition connection, string name,
    Entity entity) : ADODotNetDatabaseDataSourceDefinition(connection, name, entity)
{
}