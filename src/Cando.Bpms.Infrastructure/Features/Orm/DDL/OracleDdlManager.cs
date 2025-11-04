using Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Domain.Entities.Cmmn.Data.Provider;

namespace Neo.Bpms.Infrastructure.Features.Orm.DDL;

public sealed class OracleDDLManager(IDataProvider provider, IConfiguration configuration, LocalParameters connectionParameters) 
    : SqlDDLManager(provider, configuration, connectionParameters)
{
    protected override bool IsProviderSupportIdentity()
    {
        return false;
    }
    protected override void SetFieldLen(DbField dbField, ElasticObject item)
    {
        if (dbField.Type.ToUpper() == "NUMBER")
        {
            item.GetField("DATA_PRECISION", out var cv);
            dbField.Len = Convert.ToInt32(cv);
        }
    }
}
