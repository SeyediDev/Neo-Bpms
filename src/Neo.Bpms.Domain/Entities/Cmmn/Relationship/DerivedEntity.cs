using Neo.Bpms.Domain.Entities.Cmmn.Entities;

namespace Neo.Bpms.Domain.Entities.Cmmn.Relationship;

public class DerivedEntity
{
    public DerivedEntity(EntityRelationship referToEntity)
    {
        ReferToEntity = referToEntity;
    }

    public DerivedEntity()
    {
    }

    public EntityRelationship ReferToEntity { get; set; }
    public Entity SourceEntity => ReferToEntity.SourceEntity;
    public Entity DestEntity => ReferToEntity.DestEntity;
}