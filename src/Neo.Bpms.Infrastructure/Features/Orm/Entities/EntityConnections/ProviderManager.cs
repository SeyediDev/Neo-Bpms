using Neo.Bpms.Domain.Models.Cmmn.Data.Provider;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;

public class DataSourceProviderManager
{
    public static IDataProviderContainer ProviderContainer { get; set; }

    public static IDataProvider GetProvider(string name)
    {
        return ProviderContainer.GetProvider(name);
    }
    public static IDataProvider GetProvider(Entity entity)
    {
        return GetProvider(entity.ProviderName);
    }
}
