using Neo.Bpms.Domain.Models.Cmmn.Data.Query;

namespace Neo.Bpms.Infrastructure.Features.Orm.Command.Sql;

public abstract class QueryGeneratorSql(string databaseName) 
    : CommandGeneratorSql(databaseName), IQueryGenerator
{
}
