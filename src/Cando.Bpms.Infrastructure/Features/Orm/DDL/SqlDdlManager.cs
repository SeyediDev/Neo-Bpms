using Neo.Bpms.Domain.Entities.Cmmn.Data.DDL;
using Neo.Bpms.Domain.Entities.Cmmn.Data.Provider;
using Neo.Bpms.Engine.DDL;

namespace Neo.Bpms.Infrastructure.Features.Orm.DDL;

public abstract class SqlDDLManager(IDataProvider provider, IConfiguration configuration, LocalParameters connectionParameters) 
    : DDLManager(provider, configuration, connectionParameters)
{
    protected override IEnumerable<ForeignKeyItem> GetForeignKeys()
    {
        var sql = DDLGenerator.GetForeignKeys(Options.SpecificEntity);

        return Select<ForeignKeyItem>(sql, "10.1.1.20");
    }
}
