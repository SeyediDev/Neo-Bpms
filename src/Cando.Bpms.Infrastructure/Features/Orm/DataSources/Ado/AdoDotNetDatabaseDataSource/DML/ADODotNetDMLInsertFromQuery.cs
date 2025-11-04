namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement dml functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    public override bool InsertFromQuery(DataSource queryDataSource)
    {
        SqlCommand = GenerateInsertScriptFromQuery(queryDataSource, DataSrcDefinition.Entity);
        var ret = openForUpdate(SqlCommand);
        if (!ret)
            SetException("12.1.19");
        return ret;
    }
}
