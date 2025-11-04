using Neo.Bpms.Domain.Entities.Cmmn.Data.DDL;
using Neo.Bpms.Domain.Entities.Cmmn.Data.DML;
using Neo.Bpms.Domain.Entities.Cmmn.Data.Provider;
using Neo.Bpms.Domain.Entities.Cmmn.Data.Query;
using Neo.Bpms.Infrastructure.Features.Orm.Command.TSQL;
using Neo.Bpms.Infrastructure.Features.Orm.DataSources.SqlServer;

namespace Neo.Bpms.Infrastructure.Features.Orm.Provider;

public sealed class SqlServerProvider(IConfiguration configuration, ILogger logger, string name, bool noSync = false)
    : AdoDotNetProvider(configuration, name, noSync)
{
    private SqlServerDatabaseConnectionDefinition SqlServerDatabaseConnectionDefinition => Definition as SqlServerDatabaseConnectionDefinition;

    protected override ConnectionDefinition CreateConnectionDefinition()
    {
        return new SqlServerDatabaseConnectionDefinition(_configuration, Name);
    }

    public override IDataSource GetDataSource(Entity entity, LocalParameters connectionParameters, IAuditTrail auditTrail)
    {
        var tableName = EntityDbNameManager.GetDbTableName(entity);
        var sqlServer = new SQLServerDatabaseDataSourceDefinition(SqlServerDatabaseConnectionDefinition, tableName, entity);
        var dataSource = new SqlServerDatabaseDataSource(logger, sqlServer,
            new SqlServerDatabaseConnection(this, logger, SqlServerDatabaseConnectionDefinition, tableName),
            auditTrail as AuditTrail);
        return dataSource;
    }

    public override IDDLGenerator GetDDLGenerator()
    {
        return new DDLGeneratorTSql(DatabaseName);
    }

    public override IDMLGenerator GetDMLGenerator()
    {
        return new DMLGeneratorTSql(DatabaseName);
    }

    public override IQueryGenerator GetQueryGenerator()
    {
        return new QueryGeneratorTSql(DatabaseName);
    }

    public override IDDLManager GetDdlManager(IDataProvider provider, IConfiguration configuration, LocalParameters connectionParameters)
    {
        return new SqlServerDDLManager(this, configuration, connectionParameters);
    }
}
