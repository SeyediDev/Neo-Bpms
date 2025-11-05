using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Partitions;

namespace Neo.Bpms.Domain.Entities.Cmmn;

public class ModelNamespace : BaseModelClass
{
    #region Model
    public ModelNamespace(string name, string enName, string schema)
        : base(null, enName, name)
    {
        _schema = schema;
    }
    public ModelNamespace()
    {
    }
    private string _schema;

    public string Schema
    {
        get => _schema ?? "dbo";
        set => _schema = value;
    }

    public string Provider { get; set; }
    public bool DontSync { get; set; }
    #endregion Model

    #region Partition
    //public string FileGroup { get { return !string.IsNullOrEmpty(_fileGroup) ? _fileGroup : model.Id; } set { _fileGroup = value; } }
    public Dictionary<string, PartitionFunction> PartitionFunctions { get; set; }

    public void AddPartitionFunction(string id, Type valueType, PartitionFunctionType functionType,
        PartitionFunctionBoundaryType boundaryType, string startOfRange, string endOfRange, params string[] values)
    {
        PartitionFunction partitionFunction = new(id, id)
        {
            ValueType = valueType,
            FunctionType = functionType,
            BoundaryType = boundaryType,
            StartOfRange = startOfRange,
            EndOfRange = endOfRange,
            Values = []
        };
        if (values != null)
        {
            partitionFunction.Values.AddRange(values);
        }
        PartitionFunctions ??= [];
        PartitionFunctions.TryAdd(id, partitionFunction);
    }
    public PartitionFunction GetPartitionFunction(string id)
    {
        return PartitionFunctions?.Values.FirstOrDefault(p => p.Id == id);
    }

    public Dictionary<string,PartitionScheme> PartitionSchemes { get; set; }

    public void AddPartitionScheme(string id, string partitionFunctionId, FileGroupSelectionType fileGroupSelectionType,
        string fileGroupPrefix, params string[] fileGroups)
    {
        PartitionFunction partitionFunction = GetPartitionFunction(partitionFunctionId);
        if (partitionFunction == null) return;
        PartitionScheme partitionScheme = new(id, id)
        {
            PartitionFunction = partitionFunction,
            FileGroupSelectionType = fileGroupSelectionType,
            FileGroupPrefix = fileGroupPrefix,
            FileGroups = []
        };
        if (fileGroups != null)
        {
            partitionScheme.FileGroups.AddRange(fileGroups);
        }
        PartitionSchemes ??= [];
        PartitionSchemes.TryAdd(id, partitionScheme);
    }

    public PartitionScheme GetPartitionScheme(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        PartitionSchemes.TryGetValue(id, out PartitionScheme e);
        return e;
    }

    public bool DeletePartitionScheme(string id)
    {
        return string.IsNullOrEmpty(id) ? false : PartitionSchemes.Remove(id, out PartitionScheme e);
    }

    public bool DeleteEnum(string enumId)
    {
        return _enumerationCollection.Remove(enumId);
    }

    #endregion Partition
    #region EnumerationCollection
    [XmlIgnore]
    private SortedDictionary<string, Enumeration> _enumerationCollection = [];
    public void AddEnumeration(Enumeration myEnumeration, bool replace = false)
    {
        _enumerationCollection ??= [];
        if (_enumerationCollection.ContainsKey(myEnumeration.Id))
        {
            if (!replace)
            {
                System.Diagnostics.Debug.Assert(true, "Duplicate Enumeration:" + myEnumeration.Id + " in namespace :" + Id);
                return;
            }
            _enumerationCollection.Remove(myEnumeration.Id);
        }
        _enumerationCollection.Add(myEnumeration.Id, myEnumeration);
    }
    public SortedDictionary<string, Enumeration> GetEnums()
    {
        return _enumerationCollection;
    }
    public Enumeration GetEnum(string enumId)
    {
        if (string.IsNullOrEmpty(enumId)) return null;
        _enumerationCollection.TryGetValue(enumId, out Enumeration e);
        return e;
    }
    #endregion
    #region EntityCollection
    [XmlIgnore]
    internal SortedDictionary<string, Entity> Entities = [];
    [XmlIgnore]
    public long DbId { get; set; }


    public void AddEntity(Entity entity, bool replace = false)

    {
        Entities ??= [];
        if (Entities.ContainsKey(entity.Id))
        {
            if (!replace)
            {
                System.Diagnostics.Debug.Assert(true, "Duplicate entity:" + entity.Id + " in namespace :" + Id);
                throw new Exception("Duplicate entity:" + entity.Id + " in namespace :" + Id);
            }
            Entities.Remove(entity.Id);
        }
        Entities.Add(entity.Id, entity);
    }
    public Entity GetEntity(string entityId)
    {
        if (string.IsNullOrEmpty(entityId)) return null;
        Entities.TryGetValue(entityId, out Entity e);
        return e;
    }
    public Entity GetEntityByName(string name)
    {
        return string.IsNullOrEmpty(name) ? null : Entities.Values.FirstOrDefault(e => e.Name == name);
    }

    public SortedDictionary<string, Entity> GetEntities()
    {
        return Entities;
    }

    public IEnumerable<Entity> GetEntitiesOfProvider(string providerName)
    {
        return Entities?.Values.Where(e => e.ProviderName == providerName) ?? [];
    }
    #endregion EntityCollection

    public void SetComponents(ModelNamespace model)
    {
        Entities = model.Entities;
        _enumerationCollection = model._enumerationCollection;
    }

    public bool DeleteEntity(string entityId)
    {
        return Entities?.Remove(entityId) ?? false;
    }
}
