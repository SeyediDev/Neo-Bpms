namespace Neo.Bpms.Infrastructure.Features.Orm.Command.Sql;

public abstract class CommandGeneratorSql(string databaseName) : CommandGenerator
{
    protected string DatabaseName { get; } = databaseName;
}