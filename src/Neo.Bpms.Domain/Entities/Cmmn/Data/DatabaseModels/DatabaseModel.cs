namespace Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;

public class DatabaseModel(string databaseName)
{
    public string DatabaseName { get; } = databaseName;
    public Dictionary<string, DbFileGroup> FileGroups { get; set; } = [];
    public Dictionary<string, DbPartitionScheme> PartitionSchemes { get; set; } = [];
    public Dictionary<string, DbPartitionFunction> PartitionFunctions { get; set; } = [];
    public Dictionary<string, DbSchema> Schemas { get; set; } = [];
    public DatabaseTableModels<DbTable> Tables { get; set; } = new(databaseName);
    public DatabaseTableModels<DbView> Views { get; set; } = new(databaseName);
}
public class DatabaseTableModels<T>(string databaseName)
{
    private readonly string _databaseName = databaseName;
    private Dictionary<string, T> _tables = [];

    public void Set(Dictionary<string, T> tables)
    {
        _tables = tables;
    }
    public void Set(string name, T table)
    {
        CheckName(name);
        _tables[name] = table;
    }
    public bool TryGetValue(string name, out T table)
    {
        CheckName(name);
        return _tables.TryGetValue(name, out table);
    }
    public bool TryGetValue(string schema, string name, out T table)
    {
        return _tables.TryGetValue($"[{schema}].[{name}]", out table);
    }
    public T FetchByLowerCase(string name)
    {
        CheckName(name);
        return _tables.FirstOrDefault(t => t.Key.ToLower() == name.ToLower()).Value;
    }
    public IEnumerable<T> Enumerable()
    {
        return _tables.Values;
    }
    public int Count => _tables.Count;
    public bool Add(string name, T table)
    {
        CheckName(name);
        return _tables.TryAdd(name, table);
    }
    public bool Remove(string name)
    {
        CheckName(name);
        return _tables.Remove(name);
    }

    public bool ContainsKey(string name)
    {
        CheckName(name);
        return _tables.ContainsKey(name);
    }
    private void CheckName(string name)
    {
        if (!name.StartsWith("[") || !name.EndsWith("]"))
            throw new ArgumentException($"{_databaseName}.{name}");
    }
}
