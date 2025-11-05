namespace Neo.Bpms.Domain.Models.Cmmn.Relationship;

public class Association : ModelEntityAssociation
{
    protected EntityFieldFlags Flags { get; }
    public bool NotMapped => CheckFlag(EntityFieldFlags.NotMap);
    public bool IsBitMask => CheckFlag(EntityFieldFlags.IsBitMask);
    public bool Hidden { get; set; }

    public Association()
    {
    }

    public Association(Entity baseEntity, string id, string name, string enName,
        Entity destEntity, string constraint, string dbConstraintNameMap,
        RelationDeleteUpdateBehavior onDeleteBehaviour,
        RelationDeleteUpdateBehavior onUpdateBehaviour, EntityFieldFlags fieldFlags)
        : base(baseEntity, id, name, destEntity,
            constraint, dbConstraintNameMap, onDeleteBehaviour, onUpdateBehaviour)
    {
        EnName = enName;
        Flags = fieldFlags;
    }

    public bool CheckFlag(EntityFieldFlags flg)
    {
        return ((int)Flags & (int)flg) != 0;
    }

    public Association Clone()
    {
        Association association = new(Parent as Entity, Id, Name, EnName, DestEntity,
            Constraint, DbConstraintNameMap, OnDeleteBehaviour, OnUpdateBehaviour, Flags);
        if (Maps != null)
        {
            association.Maps = [];
            foreach (EntityRelationMap map in Maps)
            {
                association.Maps.Add(map.Clone());
            }
        }
        return association;
    }
}
