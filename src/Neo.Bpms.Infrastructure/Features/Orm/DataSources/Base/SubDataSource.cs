using Neo.Bpms.Domain.Models.Cmmn.Data.Base;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Base;

public class SubDataSource : JoinDefinition
{
    public DataSource dataSource;
}
