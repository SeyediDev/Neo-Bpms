using Neo.Bpms.Domain.Entities.Cmmn.Entities;

namespace Neo.Bpms.Domain.Entities.Cmmn.Relationship;

public class CompositionEntity : EntityRelationship
{
    public CompositionEntity(Entity sourceEntity, string id, string name, Entity destEntity)
        : base(sourceEntity, id, name, destEntity)
    {
    }

    public CompositionEntity()
    {
    }

    public CompositionEntity Clone()
    {
        //todo
        return new CompositionEntity(SourceEntity, Id, Name, DestEntity);
    }
}