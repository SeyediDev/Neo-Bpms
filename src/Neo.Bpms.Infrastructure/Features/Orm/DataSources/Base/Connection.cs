using Neo.Bpms.Domain.Models.Cmmn.Data.Provider;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Base;

public abstract class Connection(IDataProvider provider, ConnectionDefinition definition) : IConnection
{
    protected ConnectionDefinition definition = definition;

    protected IDataProvider Provider { get; set; } = provider;

    public ConnectionDefinition Definition => definition;
    public List<DataLayerException> Exceptions { get; set; }

    public virtual bool Open(string tableName, DataTransaction transaction)
    {
        EnlistTransaction(transaction);
        return true;
    }

    public abstract void EnlistTransaction(DataTransaction dataTransaction);
    public virtual bool Close() { return true; }
    public virtual string State()
    {
        return "Unasigned";
    }
}
