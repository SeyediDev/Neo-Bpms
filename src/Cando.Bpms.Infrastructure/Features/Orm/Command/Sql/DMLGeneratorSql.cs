using Neo.Bpms.Domain.Entities.Cmmn.Data.DML;

namespace Neo.Bpms.Infrastructure.Features.Orm.Command.Sql;

public abstract class DMLGeneratorSql(string databaseName) 
    : CommandGeneratorSql(databaseName), IDMLGenerator
{
}
