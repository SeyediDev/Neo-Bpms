using System.Collections;

namespace Neo.Bpms.Domain.Features.Cmmn.CodeFirst;

internal class CSharpObjectToRelationship : CSharpObjectToModel
{

    /// <summary>
    /// Defines the association.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="field"></param>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="enName">Name of the en.</param>
    /// <param name="type">The type.</param>
    /// <param name="attributes">The attributes.</param>
    /// <param name="repository"></param>
    internal static void DefineRelationship(Entity entity, EntityField field, string id, string name,
        string enName, Type type, IEnumerable attributes, INameSpaceRepository repository)
    {
        Entity relatedEntity = repository.GetEntityFromType(type);
        if (relatedEntity == null)
        {
            return;
        }

        EntityFieldFlags fieldFlags = EntityFieldFlags.None;
        foreach (object attr in attributes ?? Enumerable.Empty<object>())
        {
            switch (attr)
            {
                case OAttr_AssociationMap map:
                    if (map.Bitmask)
                    {
                        fieldFlags |= EntityFieldFlags.IsBitMask;
                    }

                    break;
                case System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute _:
                    fieldFlags |= EntityFieldFlags.NotMap;
                    break;
                case RequiredAttribute:
                    fieldFlags |= EntityFieldFlags.NotNull;
                    break;
            }
        }

        RelationDeleteUpdateBehavior deleteBehavior = RelationDeleteUpdateBehavior.Error;
        RelationDeleteUpdateBehavior updateBehavior = RelationDeleteUpdateBehavior.Error;
        string? constraint = null;

        Association association = null;
        foreach (object attr in attributes ?? Enumerable.Empty<object>())
        {

            switch (attr)
            {
                case BaseHierarchicalEntityAttribute _:
                    field.BaseEntity = entity.AddBaseEntity(relatedEntity, field);
                    break;
                case ParentEntityAttribute parentEntityAttribute:
                    association = field.AssociationEntity =
                        entity.AddParentEntity(relatedEntity, parentEntityAttribute.BooleanFieldIdInParent);
                    deleteBehavior = RelationDeleteUpdateBehavior.Cascade;
                    updateBehavior = RelationDeleteUpdateBehavior.Cascade;
                    break;
                case CompositionAttribute _:
                    entity.SetCompositionEntity(field, relatedEntity);
                    break;
                case WeakEntityAssociationAttribute weakEntityAssociationAttribute:
                    association = field.AssociationEntity =
                        new WeakEntityAssociation(entity, id, name, enName,
                            relatedEntity, weakEntityAssociationAttribute.Constraint ?? constraint,
                            weakEntityAssociationAttribute.ConstraintDbName, fieldFlags);
                    deleteBehavior = RelationDeleteUpdateBehavior.Cascade;
                    updateBehavior = RelationDeleteUpdateBehavior.Cascade;
                    break;
                case OAttr_Association associationAttribute:
                    association = field.AssociationEntity =
                        new Association(entity, id, name, enName,
                            relatedEntity, associationAttribute.Constraint?? constraint,
                            associationAttribute.ConstraintDbName, associationAttribute.OnDeleteBehaviour,
                            associationAttribute.OnUpdateBehaviour, fieldFlags)
                        {
                            Hidden = associationAttribute.Hidden,
                        };
                    break;
                case RelationshipConstraintAttribute attribute:
                    constraint = attribute.Constraint;
                    break;
            }
        }
        
        association ??= field.AssociationEntity =
                new Association(entity, id, name, enName, relatedEntity, constraint, null, deleteBehavior, updateBehavior, fieldFlags);

        foreach (object attr in attributes ?? Enumerable.Empty<object>())
        {
            CheckFieldAttributes(field, entity, attr);
            if (attr is OAttr_AssociationMap mapAttribute)
            {
                association.Maps ??= [];
                association.Maps.Add(new EntityRelationMap
                {
                    SourceField = mapAttribute.MyField,
                    DestField = mapAttribute.ObjectField,
                });
            }
        }

        if (association != null && (association.Maps == null || association.Maps.Count == 0))
        {
            EntityField sourceField = entity.GetField(id + "Id");
            if (sourceField != null)
            {
                association.Maps =
                [
                    new()
                    {
                        SourceField = id + "Id",
                        DestField = association.DestEntity?.KeyFields?.FirstOrDefault()?.Id ?? "Id"
                    }
            ];
                if (string.IsNullOrEmpty(sourceField.Name))
                {
                    sourceField.Name = association.Name;
                }
            }
            else if (!association.NotMapped)
            {
                throw new Exception($"در موجودیت {entity.Name} ارتباط " + association.Name + " دارای مپ نیست");
            }
        }
    }
}
