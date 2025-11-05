using Neo.Bpms.Domain.Entities.Cmmn.Data.Query;

namespace Neo.Bpms.Infrastructure.Features.Orm.Command.Sql;

public abstract class QueryGeneratorSql(string databaseName) 
    : CommandGeneratorSql(databaseName), IQueryGenerator
{
}
