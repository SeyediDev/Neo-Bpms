using Neo.Bpms.Domain.Entities.Cmmn.Entities;

namespace Neo.Bpms.Domain.Entities.Cmmn.Relationship;

public abstract class ModelEntityAssociation : EntityRelationship
{
    protected ModelEntityAssociation(Entity sourceEntity, string id, string name,
        Entity destEntity, string constraint, string dbConstraintNameMap,
        RelationDeleteUpdateBehavior onDeleteBehaviour,
        RelationDeleteUpdateBehavior onUpdateBehaviour)
        : base(sourceEntity, id, name, destEntity)
    {
        Constraint = constraint;
        DbConstraintNameMap = dbConstraintNameMap;
        OnDeleteBehaviour = onDeleteBehaviour;
        OnUpdateBehaviour = onUpdateBehaviour;
    }

    protected ModelEntityAssociation()
    {
    }

    public string Constraint { get; set; }
    public string DbConstraintNameMap { get; set; }
    public RelationDeleteUpdateBehavior OnDeleteBehaviour { get; set; }
    public RelationDeleteUpdateBehavior OnUpdateBehaviour { get; set; }
    public List<EntityRelationMap> Maps { get; set; }
}
