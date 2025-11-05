using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Cmmn.Relationship;

public abstract class EntityRelationship : BaseModelClass
{

    protected EntityRelationship(Entity sourceEntity, string id, string name, Entity destEntity)
        : base(sourceEntity, id, name)
    {
        DestNamespaceId = destEntity.NamespaceId;
        DestEntityId = destEntity.Id;
        DestEntity = destEntity;
    }

    protected EntityRelationship()
        : base(null, null, null)
    {
    }

    public string DestNamespaceId { get; set; }//todo
    public string DestEntityId { get; set; }//todo
    [XmlIgnore]
    public Entity DestEntity;
    public Entity Entity() => DestEntity;
    [XmlIgnore]
    public Entity SourceEntity => Parent as Entity;

    public EntityField GetField(string fieldId)
        => DestEntity?.GetField(fieldId);
    public string ReferEntityKey() => ReferEntityKey(DestNamespaceId, DestEntityId);
    public static string ReferEntityKey(string namespaceId, string entityId) => $"{namespaceId}.{entityId}";
}