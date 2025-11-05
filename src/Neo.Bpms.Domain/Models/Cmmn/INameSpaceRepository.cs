namespace Neo.Bpms.Domain.Models.Cmmn;

public interface INameSpaceRepository
{
    Entity GetEntityByName(string name);
    Entity GetEntity(Type type);
    string GetEntityKey(Type type);
    Entity GetEntity(Type type, out string entityKey);
    Entity GetEntity(string nameSpaceId, string entityId);
    Entity NewEntity(ModelNamespace modelNamespace, Type entityType, string name, string id, string enName);
    Entity GetEntity<TEntity>();
    Entity GetEntityFromType(Type type);
    ModelNamespace AddNamespace(string namespaceId, string name, string schema);
    bool AddNamespace(ModelNamespace ns, bool replace = false);
    ModelNamespace GetNamespaceFromType(Type type);
    ModelNamespace GetModel(string namespaceId);
    bool DeleteNamespace(string id);
    string GetEnumText(string nameSpaceId, string enumId, long itemId, string culture);
    ModelNamespace GetModelFromCSharpNamespace(string tNamespace);
    string GetFormName(Type type);
    string GetReportName(Type type);
    string GetProcessName(Type type);
    ConcurrentDictionary<string, Entity> Entities { get; set; }
}
