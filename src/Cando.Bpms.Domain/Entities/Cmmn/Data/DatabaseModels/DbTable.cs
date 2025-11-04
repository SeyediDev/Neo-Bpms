namespace Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;

public class DbTable
{
    public DbTable(string schema, string name)
    {
        Name = name;
        Schema = schema;
        if (name.Contains('['))
        {

        }
        if (schema == null)
        {

        }
    }
    public string FullName => $"[{Schema}].[{Name}]";
    public string Name { get; set; }
    public string Schema { get; set; }
    public string FileGroup { get; set; }
    public string PartitionScheme { get; set; }
    public Dictionary<string, DbField> Fields { get; set; } = [];
    public Dictionary<string, DbIndex> Indexes { get; set; }
    public Dictionary<string, DbForeignKey> ForeignKeys { get; set; }
    public Dictionary<string, DbForeignKey> InForeignKeys { get; set; }
    public List<DataTrigger> Triggers { get; set; }

    public bool AddField(DbField dbField)
    {
        if (Fields.ContainsKey(dbField.Name))
            return false;
        Fields.Add(dbField.Name, dbField);
        return true;
    }

    public bool CheckField(string fieldName)
    {
        DbField dbField = GetField(fieldName);
        return dbField == null ? false : !dbField.Deleted;
    }

    public DbField GetField(string fieldName)
    {
        DbField f = null;
        if (!string.IsNullOrEmpty(fieldName))
            Fields.TryGetValue(fieldName, out f);
        return f;
    }

    public bool DeleteField(string fieldName)
    {
        return Fields.Remove(fieldName);
    }

    public DbIndex AddIndex(string name, bool isUnique, DbIndex.IndexType indexType, DbFileGroup dataSpace, bool isPk)
    {
        Indexes ??= [];
        if (Indexes.TryGetValue(name, out DbIndex value)) return value;
        DbIndex dbIndex = new()
        {
            IsUnique = isUnique,
            //DefragmentationPercent = 0;
            Name = name,
            DataSpace = dataSpace,
            Type = indexType,
            IsPrimaryKey = isPk
        };
        Indexes.Add(dbIndex.Name, dbIndex);
        return dbIndex;
    }

    public DbIndex GetIndex(string name)
    {
        DbIndex idx = null;
        Indexes?.TryGetValue(name, out idx);
        return idx;
    }

    public DbIndex PkIndex => Indexes?.Values.FirstOrDefault(i => i.IsPrimaryKey);

    public DbForeignKey AddForeignKey(string name, string deleteRule, string updateRule, DbTable pPkTable)
    {
        ForeignKeys ??= [];
        if (ForeignKeys.TryGetValue(name, out DbForeignKey value)) return value;
        DbForeignKey pForeignKey = new()
        {
            Name = name,
            BaceTable = pPkTable,
            ForeignTable = this,
            DeleteRule = DbForeignKey.GetForeignKeyRule(deleteRule),
            UpdateRule = DbForeignKey.GetForeignKeyRule(updateRule)
        };
        ForeignKeys.Add(name, pForeignKey);
        if (pPkTable != null)
        {
            pPkTable.InForeignKeys ??= [];
            if (!pPkTable.InForeignKeys.ContainsKey(name))
                pPkTable.InForeignKeys.Add(name, pForeignKey);
        }
        return pForeignKey;
    }

    public DbForeignKey GetForeignKey(string name)
    {
        DbForeignKey fk = null;
        ForeignKeys?.TryGetValue(name, out fk);
        return fk;
    }

}