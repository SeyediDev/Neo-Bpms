namespace Neo.Bpms.Domain.Models.Cmmn.Relationship;

public class ParentEntity : Association
{
    public ParentEntity()
    {
    }
    public ParentEntity(Entity sourceEntity, string id, string name, string enName,
        Entity destEntity, string booleanFieldIdInParentThatPresentMe)
        : base(sourceEntity, id, name, enName, destEntity, null, null,
            RelationDeleteUpdateBehavior.Cascade,
            RelationDeleteUpdateBehavior.Cascade,
            EntityFieldFlags.None)
    {
        BooleanFieldIdInParentThatPresentMe = booleanFieldIdInParentThatPresentMe;
        if (string.IsNullOrEmpty(BooleanFieldIdInParentThatPresentMe))
            BooleanFieldIdInParentThatPresentMe = $"Is{sourceEntity.Id}";
    }

    public ParentEntity(Entity sourceEntity, Entity entity, string booleanFieldIdInParent)
        : this(sourceEntity, entity.Id, entity.Name, entity.EnName, entity, booleanFieldIdInParent)
    {
    }

    public string BooleanFieldIdInParentThatPresentMe { get; set; }

    public new ParentEntity Clone()
    {
        return new ParentEntity(SourceEntity, DestEntity, BooleanFieldIdInParentThatPresentMe);
    }
}