using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Cmmn.Relationship;

public class WeakEntityAssociation : Association
{
    public WeakEntityAssociation()
    {
    }

    public WeakEntityAssociation(Entity baseEntity, string id, string name, string enName,
        Entity destEntity, string constraint, string dbConstraintNameMap, EntityFieldFlags fieldFlags)
        : base(baseEntity, id, name, enName, destEntity,
            constraint, dbConstraintNameMap,
            RelationDeleteUpdateBehavior.Cascade,
            RelationDeleteUpdateBehavior.Cascade, fieldFlags)
    {
    }

    public new WeakEntityAssociation Clone()
    {
        return new WeakEntityAssociation(Parent as Entity, Id, Name, EnName, DestEntity,
            Constraint, DbConstraintNameMap, Flags);
    }
}