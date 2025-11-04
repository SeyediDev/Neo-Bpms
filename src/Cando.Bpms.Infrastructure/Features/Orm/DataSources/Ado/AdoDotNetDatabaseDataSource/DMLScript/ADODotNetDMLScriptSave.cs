namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement dml functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    private string GenerateSaveScript(IEnumerable<SqlValueField> sqlValueFields,
        string keyFilterValues, bool giveOutput,
        string keyValue, Entity entity)
    {
        var commandString = new CandoStringBuilder();
        var dbTableName = GetTableDbName(entity, DataSrcDefinition.connection.DatabaseName);
        commandString += $@"IF EXISTS(SELECT 1 FROM {dbTableName} WHERE {keyFilterValues})
BEGIN
 {GenerateUpdateScript(new ElasticObject(), sqlValueFields, keyFilterValues, keyValue,
            entity)}
END ELSE BEGIN
 {GenerateInsertScript(giveOutput, entity)}
END";
        return commandString.ToString();
    }
}
