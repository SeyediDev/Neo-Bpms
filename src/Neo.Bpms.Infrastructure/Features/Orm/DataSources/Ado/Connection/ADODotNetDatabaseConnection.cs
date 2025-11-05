using System.Data;
using System.Data.Common;
using Neo.Bpms.Domain.Models.Cmmn.Data.Provider;
using Neo.Bpms.Domain.Models.Cmmn.Data.Transaction;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

public abstract class AdoDotNetDatabaseConnection(IDataProvider provider, AdoDotNetDatabaseConnectionDefinition definition)
    : Base.Connection(provider, definition)
{
    public DbConnection dbConnection;

    public abstract DbConnection GetNewConnection();

    public override bool Open(string tableName, DataTransaction transaction)
    {
        dbConnection ??= GetNewConnection();
        if (dbConnection == null || dbConnection.State == ConnectionState.Open)
        {
            return dbConnection != null;
        }

        int counter = 0;

        do
        {
            try
            {
                dbConnection.Open();
                break;
            }
            catch
            {
                counter++;
            }
        } while (counter < 3);
        EnlistTransaction(transaction);
        return dbConnection != null;
    }

    public override void EnlistTransaction(DataTransaction dataTransaction)
    {
        //todo Transaction: dbConnection.EnlistTransaction((System.Transactions.Transaction)dataTransaction.Transaction);
    }

    public override bool Close()
    {
        if (dbConnection == null)
        {
            return true;
        }

        try
        {
            lock (dbConnection)
            {
                if (dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection?.Close();
                }
            }
        }
        catch//( Exception e)
        {
            // ignored
        }
        try
        {
            dbConnection?.Dispose();
        }
        catch //(Exception e)
        {
            // ignored
        }

        dbConnection = null;
        return true;
    }

    public override string State()
    {
        ConnectionState state = dbConnection?.State ?? ConnectionState.Closed;
        return state.ToString("G");
    }

    public virtual string Id => GetType().Name;
}
