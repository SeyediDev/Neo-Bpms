namespace Neo.Bpms.Domain.Models.Cmmn.Data.Provider;

public interface IDataProviderContainer : IEnumerable<IDataProvider>
{
    void Init();
    IDataProvider GetProvider(string providerName);
}