namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement dml functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    public override bool DeleteGroup(ElasticObject elasticObject)
    {
        SqlCommand = GenerateGroupDeleteScript();
        openForUpdate(SqlCommand);
        return true;
    }

    public override bool Delete(ElasticObject elasticObject)
    {
        if (elasticObject == null) return false;
        var keyFilterValues = FetchKeyFilterValue(elasticObject, elasticObject, out var keyValue);
        SqlCommand = GenerateDeleteScript(keyValue, keyFilterValues, DataSrcDefinition.Entity);
        openForUpdate(SqlCommand);
        return true;
    }

    public override bool Delete<T>(T obj)
    {
        if (obj == null) return false;
        var keyFilterValues = FetchKeyFilterValue(obj, obj, out var keyValue);
        SqlCommand = GenerateDeleteScript(keyValue, keyFilterValues, DataSrcDefinition.Entity);
        openForUpdate(SqlCommand);
        return true;
    }
}
