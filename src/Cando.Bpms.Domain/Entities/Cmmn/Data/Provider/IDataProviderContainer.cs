namespace Neo.Bpms.Domain.Entities.Cmmn.Data.Provider;

public interface IDataProviderContainer : IEnumerable<IDataProvider>
{
    void Init();
    IDataProvider GetProvider(string providerName);
}