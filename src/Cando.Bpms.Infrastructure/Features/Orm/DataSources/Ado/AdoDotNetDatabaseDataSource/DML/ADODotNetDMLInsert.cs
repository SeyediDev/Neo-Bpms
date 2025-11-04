namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement dml functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    public override bool Insert(ElasticObject record, bool dontGiveOutput)
    {
        if (record == null) return false;
        Connection.Exceptions = null;
        SqlCommand = GenerateInsertScript(!dontGiveOutput, DataSrcDefinition.Entity);
        var ret = dontGiveOutput ? openForUpdate(SqlCommand) : openForRead(SqlCommand);
        if (ret && !dontGiveOutput)
        {
            ret = SetKeyValue(record);
            if (!ret)
                SetException("12.1.11");
        }
        else if (!ret)
            SetException("12.1.12");

        return ret;
    }

    public override bool insert<T>(T obj, bool dontGiveOutput)
    {
        if (obj == null) return false;
        SqlCommand = GenerateInsertScript(!dontGiveOutput, DataSrcDefinition.Entity);
        var ret = dontGiveOutput ? openForUpdate(SqlCommand) : openForRead(SqlCommand);
        if (ret && !dontGiveOutput)
            ret = SetKeyValue(obj);
        else if (!ret)
            SetException("12.1.18");
        return ret;
    }
}
