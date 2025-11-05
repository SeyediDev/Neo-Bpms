namespace Neo.Bpms.Domain.Models.Cmmn.Relationship;

public class BaseEntity : EntityRelationship
{
    public BaseEntity()
    {
    }

    public BaseEntity(Entity childEntity, Entity baseEntity, EntityField relationshipFieldId)
        : this(childEntity,
            $"{childEntity.Id}.ChildOf.{baseEntity.Id}",
            $"{childEntity.Id} ChildOf {baseEntity.Id}", baseEntity, relationshipFieldId)
    {
    }

    public BaseEntity(Entity childEntity, string id, string name, Entity baseEntity, EntityField relationshipField)
        : base(childEntity, id, name, baseEntity)
    {
        RelationshipField = relationshipField;
    }
    public EntityField RelationshipField { get; set; }
}