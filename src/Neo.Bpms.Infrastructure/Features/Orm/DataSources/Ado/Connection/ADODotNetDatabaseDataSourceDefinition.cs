namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

public abstract class ADODotNetDatabaseDataSourceDefinition(AdoDotNetDatabaseConnectionDefinition connection,
    string name, Entity entity) : DataSourceDefinition(connection, name, entity)
{
}
