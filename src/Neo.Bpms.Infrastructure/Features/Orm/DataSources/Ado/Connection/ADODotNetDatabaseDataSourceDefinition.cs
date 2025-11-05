using Neo.Bpms.Domain.Models.Cmmn.Data.Base;
using Neo.Bpms.Domain.Models.Cmmn.Entities;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

public abstract class ADODotNetDatabaseDataSourceDefinition(AdoDotNetDatabaseConnectionDefinition connection,
    string name, Entity entity) : DataSourceDefinition(connection, name, entity)
{
}
