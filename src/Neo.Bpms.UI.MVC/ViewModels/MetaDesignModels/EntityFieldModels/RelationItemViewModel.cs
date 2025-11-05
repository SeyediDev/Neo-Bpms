using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;

public class RelationItemViewModel : DestEntityViewModel
{
    public RelationItemViewModel()
    {
    }
    public RelationItemViewModel(EntityField f)
    {
        Association associationEntity = f.AssociationEntity;
        EntityRelationship entityRelationship = associationEntity ?? (EntityRelationship)f.CompositionEntity ?? f.BaseEntity;
        prevId = f.Id;
        id = f.Id;
        relationType = GetRelationType(f);
        destNamespaceId = entityRelationship.DestNamespaceId;
        destEntityId = entityRelationship.DestEntityId;
        if (relationType == TVariableTypes.ParentEntity && entityRelationship is ParentEntity parentEntity)
            booleanFieldIdInParent = parentEntity.BooleanFieldIdInParentThatPresentMe;
        englishName = f.EnName ?? f.Id;
        persianName = f.Name;
        notMapped = associationEntity?.NotMapped ?? false;
        constraint = associationEntity?.Constraint ?? null;
        dbConstraintNameMap = associationEntity?.DbConstraintNameMap ?? null;
        onUpdateBehaviour = associationEntity?.OnUpdateBehaviour ?? null;
        onDeleteBehaviour = associationEntity?.OnDeleteBehaviour ?? null;
        if (relationType == TVariableTypes.WeakEntityAssociation || relationType == TVariableTypes.ParentEntity)
            onUpdateBehaviour = onDeleteBehaviour = RelationDeleteUpdateBehavior.Cascade;

        sourceFieldId = associationEntity?.Maps?.FirstOrDefault()?.SourceField;
        destFieldId = associationEntity?.Maps?.FirstOrDefault()?.DestField;
        maps = associationEntity?.Maps?.Select(m => new AssociationMapViewModel
        {
            sourceField = m.SourceField,
            destField = m.DestField
        }).ToList();
    }

    public string id { get; set; }
    public string prevId { get; set; }
    public TVariableTypes relationType { get; set; }
    public string persianName { get; set; }
    public string englishName { get; set; }
    public string constraint { get; set; }
    public string dbConstraintNameMap { get; set; }
    public bool notMapped { get; set; }
    public RelationDeleteUpdateBehavior? onUpdateBehaviour { get; set; }
    public RelationDeleteUpdateBehavior? onDeleteBehaviour { get; set; }
    public string destFieldId { get; set; }
    public string sourceFieldId { get; set; }
    public string booleanFieldIdInParent { get; set; }
    public List<AssociationMapViewModel> maps { get; set; }

    private static TVariableTypes GetRelationType(EntityField entityField)
    {
        if (entityField.CompositionEntity != null)
            return TVariableTypes.Composition;
        return entityField.BaseEntity != null
            ? TVariableTypes.BaseEntity
            : entityField.AssociationEntity switch
            {
                ParentEntity _ => TVariableTypes.ParentEntity,
                WeakEntityAssociation _ => TVariableTypes.WeakEntityAssociation,
                Association association => association.IsBitMask
                                    ? TVariableTypes.BitMaskAssociation
                                    : TVariableTypes.Association,
                _ => TVariableTypes.None,
            };
    }

    public void ModifyEntityField(EntityField entityField)
    {
        entityField.Id = id;
        entityField.Name = persianName;
        entityField.EnName = englishName;
        EntityFieldFlags fieldFlags = EntityFieldFlags.None;
        if (relationType == TVariableTypes.BitMaskAssociation)
            fieldFlags |= EntityFieldFlags.IsBitMask;
        if (notMapped) fieldFlags |= EntityFieldFlags.NotMap;
        //if (notNull)
        //	fieldFlags |= eEntityFieldFlags.NotNull;
        Association association = null;
        Entity relatedEntity = ProjectDefinition.Project.GetEntity(destNamespaceId, destEntityId);
        switch (relationType)
        {
            case TVariableTypes.BaseEntity:
                entityField.BaseEntity = entityField.Entity.AddBaseEntity(relatedEntity, entityField);
                break;
            case TVariableTypes.Composition:
                entityField.Entity.SetCompositionEntity(entityField, relatedEntity);
                break;
            case TVariableTypes.ParentEntity:
                association = entityField.AssociationEntity =
                    entityField.Entity.AddParentEntity(relatedEntity, booleanFieldIdInParent);
                break;
            case TVariableTypes.WeakEntityAssociation:
                association = entityField.AssociationEntity =
                    new WeakEntityAssociation(entityField.Entity, id, persianName, englishName,
                        relatedEntity, constraint, dbConstraintNameMap, fieldFlags);
                break;
            case TVariableTypes.BitMaskAssociation:
            case TVariableTypes.Association:
                association = entityField.AssociationEntity =
                    new Association(entityField.Entity, id, persianName, englishName,
                        relatedEntity, constraint, dbConstraintNameMap, onUpdateBehaviour ?? RelationDeleteUpdateBehavior.DontCheck,
                        onUpdateBehaviour ?? RelationDeleteUpdateBehavior.DontCheck, fieldFlags);
                break;
        }

        if (association != null)
        {
            association.Maps = maps?.Select(map => map.ToMap()).ToList();
        }
    }
}
