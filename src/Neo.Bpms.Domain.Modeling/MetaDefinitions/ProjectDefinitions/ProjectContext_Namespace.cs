using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Domain.Model.Project;

public partial class ProjectContext
{
    public Dictionary<string, ModelNamespace> Namespaces { get; } = [];
    public ConcurrentDictionary<string, Entity> Entities { get; set; } = new ConcurrentDictionary<string, Entity>();

    public string GetEnumText(string nameSpaceId, string enumId, long itemId, string culture)
    {
        var model = GetModel(nameSpaceId);
        var enums = model?.GetEnums();
        if (enums?.ContainsKey(enumId) != true) return string.Empty;
        var enumList = enums[enumId];
        var enumItem = enumList.getItem(enumId);
        if (enumItem == null) return string.Empty;
        return culture == "fa" ? enumItem.Name : enumItem.EnName;
    }

    public ModelNamespace GetModelFromCSharpNamespace(string tNamespace)
    {
        var parts = tNamespace.Split('.');
        var name = parts.LastOrDefault();
        return GetModel(name);
    }

    public string GetFormName(Type type)
    {
        return type.Name;
    }

    public string GetReportName(Type type)
    {
        return type.Name;
    }


    public string GetProcessName(Type type)
    {
        return type.Name;
    }

    public ModelNamespace AddNamespace(string namespaceId, string name, string schema)
    {
        name ??= namespaceId;
        var ns = new ModelNamespace(namespaceId, name, schema);
        AddNamespace(ns);
        return ns;
    }

    public bool AddNamespace(ModelNamespace Namespace, bool replace = false)
    {
        if (Namespaces.ContainsKey(Namespace.Id))
        {
            if (!replace)
            {
                System.Diagnostics.Debug.Assert(true, "Duplicate Namespace:" + Namespace.Id);
                return false;
            }
            Namespaces.Remove(Namespace.Id);
        }
        Namespaces.Add(Namespace.Id, Namespace);
        return true;
    }

    public bool DeleteNamespace(string id)
    {
        Namespaces.Remove(id);

        return true;
    }

    public Entity GetEntityByEntityId(string sEntityId, string namespaceId = null)
    {
        if (string.IsNullOrEmpty(sEntityId))
            return null;
        var entityIds = sEntityId.Split('.');
        string modelId = namespaceId, entityId = sEntityId;
        if (entityIds.Length > 1)
        {
            modelId = entityIds[0];
            entityId = entityIds[1];
        }
        return (!string.IsNullOrEmpty(modelId) ? Namespaces.TryGetValue(modelId, out ModelNamespace value) ? value.GetEntity(entityId) : null : null) ??
               Namespaces.Values.Select(model => model.GetEntity(entityId)).FirstOrDefault(e => e != null);
    }
    public Entity GetEntityByDbNameOrKey(string sEntityId, string namespaceId = null)
    {
        //todo
        if (string.IsNullOrEmpty(sEntityId)) return null;
        var dbName = sEntityId[..1] == "[" ? (sEntityId[1..^1]) : null;
        if (!string.IsNullOrEmpty(dbName))
        {
            foreach (var model in Namespaces.Values)
            {
                var ets = model.GetEntities();
                foreach (var entity in ets.Values)
                {
                    if (entity.DbTableNameMap == dbName)
                        return entity;
                }
            }
        }
        if (string.IsNullOrEmpty(sEntityId)) return null;
        var _sEntityId = sEntityId.Split('.');
        string modelId = namespaceId, entityId = sEntityId;
        if (_sEntityId.Length > 1)
        {
            modelId = _sEntityId[0];
            entityId = _sEntityId[1];
        }
        if (string.IsNullOrEmpty(modelId))
            return Namespaces.Values.Select(model => model.GetEntity(entityId)).FirstOrDefault(entity => entity != null);
        return Namespaces.TryGetValue(modelId, out ModelNamespace value) ? value.GetEntity(entityId) : null;
    }

    public Entity GetEntityByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var nameParts = name.Split('.');
        string modelName = null, entityName = name;
        if (nameParts.Length > 1)
        {
            modelName = nameParts[0];
            entityName = nameParts[1];
        }
        if (string.IsNullOrEmpty(modelName))
            return Namespaces.Values.Select(model => model.GetEntityByName(entityName)).FirstOrDefault(entity => entity != null);
        return Namespaces.Values.FirstOrDefault(model => model.Name == modelName)?.GetEntityByName(entityName);
    }

    public UiEntity GetUiEntity(string modelId, string entityId)
    {
        return GetEntity(modelId, entityId) as UiEntity;
    }

    public Entity GetEntity(string nameSpaceId, string entityId)
    {
        if (string.IsNullOrEmpty(nameSpaceId))
            return GetEntityByEntityId(entityId);
        return !Namespaces.TryGetValue(nameSpaceId, out ModelNamespace value) ? null : value.GetEntity(entityId);
    }

    public Entity NewEntity(ModelNamespace modelNamespace, Type entityType, string name, string id, string enName)
    {
        return new UiEntity(modelNamespace, entityType, name, id, enName);//todo
    }

    public UiEntity GetUiEntity<TEntity>()
    {
        return GetEntity<TEntity>() as UiEntity;
    }

    public EntityAddress FetchEntityAddress<TEntity>()
    {
        Entity.FetchEntityNameFromType(out var namespaceId, out var entityId, typeof(TEntity));
        return new EntityAddress
        {
            NamespaceId = namespaceId,
            EntityId = entityId
        };
    }
    public Entity GetEntity<TEntity>()
    {
        return GetEntity(typeof(TEntity)) ??
            Namespaces.Values.SelectMany(model => model.GetEntities().Values).FirstOrDefault(entity => entity.EntityType == typeof(TEntity));
    }
    public Entity GetEntityFromType(Type type)
    {
        Entity.FetchEntityNameFromType(out var namespaceId, out var entityId, type);
        return GetEntity(namespaceId, entityId);
    }

    public ModelNamespace GetNamespaceFromType(Type type)
    {
        Entity.FetchEntityNameFromType(out var namespaceId, out _, type);
        return GetModel(namespaceId);
    }
    public ModelNamespace GetModel(string namespaceId)
    {
        if (string.IsNullOrEmpty(namespaceId)) return null;
        return !Namespaces.TryGetValue(namespaceId, out var ns) ? null : ns;
    }

    public Entity GetEntity(Type type)
    {
        return GetEntity(type, out _);
    }

    public Entity GetEntity(Type type, out string entityKey)
    {
        entityKey = GetEntityKey(type);
        Entities.TryGetValue(entityKey, out var entity);
        return entity;
    }

    public string GetEntityKey(Type type)
    {
        Entity.FetchEntityNameFromType(out var namespaceId, out var entityId, type);
        return $"{namespaceId}.{entityId}";
    }
}
