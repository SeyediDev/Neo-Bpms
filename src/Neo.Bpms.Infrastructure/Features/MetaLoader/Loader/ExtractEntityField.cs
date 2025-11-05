using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.Loader;

public class ExtractEntityField(ProjectContext project, Entity entity)
{
    public static void CalcFieldsFromHierarchiesAndRelationships(ProjectContext project)
    {
        foreach (var entity in project.Namespaces.Values.SelectMany(model =>
            model.GetEntities().Values))
        {

            CalcEntityFields(project, entity);

        }
    }

    private static void CalcEntityFields(ProjectContext project, Entity entity)
    {
        new ExtractEntityField(project, entity).CalcEntityFields();
    }

    private void CalcEntityFields()
    {
        if (entity == null)
        {
        }

        if (entity?.CalcEntityFields ?? true) return;
        entity.CalcEntityFields = true;

        AddBaseEntitiesFields();
        AddParentEntitiesFields();
        SetBaseExtensionAndAddCompositionFields();
    }

    private void AddBaseEntitiesFields()
    {
        if (entity.BaseEntities == null) return;
        foreach (var baseEntityRelationship in entity.BaseEntities.Values)
        {
            CalcEntityFields(project, baseEntityRelationship.DestEntity);
            foreach (var baseEntityField in baseEntityRelationship.DestEntity.entityFields.Values.Where(f => !f.AuditField))
            {
                if (baseEntityField.CheckFlag(EntityFieldFlags.DerivedEntityBooleanField))
                    continue;
                var entityField = entity.GetField(baseEntityField.Id);
                if (entityField == null)
                {
                    entityField = baseEntityField.Clone(entity);
                    entityField.IsRealMember = false;
                    entity.AddField(entityField);

                    if (baseEntityRelationship.DestEntity.AutoCalcs != null)
                    {
                        entity.InitAutoCalcs();
                        foreach (var autoCalc in baseEntityRelationship.DestEntity.AutoCalcs.Calculations.Where(a => a.FieldId == baseEntityField.Id))
                            entity.AutoCalcs.AddAutoCalc(autoCalc.Clone());
                    }
                    if (baseEntityRelationship.DestEntity.DisplayStrings != null)
                    {
                        foreach (var basicField in baseEntityRelationship.DestEntity.DisplayStrings.Where(a => a.FieldId == baseEntityField.Id))
                            entity.AddBasicField(basicField.Clone());
                    }
                }
                entityField.AddReferenceField(baseEntityRelationship, baseEntityField, baseEntityRelationship.RelationshipField);

                if (baseEntityRelationship.DestEntity.indexes != null)
                {
                    foreach (var index in baseEntityRelationship.DestEntity.indexes)
                        entity.indexes.Add(index.Clone());
                }
            }
        }
    }

    private void AddParentEntitiesFields()
    {
        foreach (var parentField in entity.ParentEntities)
        {
            var parentEntity = parentField.ParentEntity;
            CalcEntityFields(project, parentEntity.DestEntity);
            AddBooleanFieldInParent(parentEntity.DestEntity, parentEntity.BooleanFieldIdInParentThatPresentMe);

            foreach (var baseEntityField in parentEntity.DestEntity.entityFields.Values)
            {
                if (baseEntityField.CheckFlag(EntityFieldFlags.DerivedEntityBooleanField))
                    continue;
                var fieldId = baseEntityField.Id;
                var name = baseEntityField.Name;
                var enName = baseEntityField.EnName;
                if (baseEntityField.IncludeInPkv)
                {
                    fieldId = $"{parentEntity.Id}.{baseEntityField.Id}";
                    name = $"{baseEntityField.Name} در {parentEntity.Name}";
                    enName = $"{baseEntityField.EnName} in {parentEntity.EnName}";
                }

                var entityField = entity.GetField(fieldId);
                if (entityField == null)
                {
                    // فیلد مافوق را من ندارم و و یا فیل مافوق کلید است 
                    ExtractedField(baseEntityField, fieldId, name, enName, parentEntity, parentField);
                }
                else
                    entityField.AddReferenceField(parentEntity, baseEntityField, parentField);
            }
        }
    }

    private void SetBaseExtensionAndAddCompositionFields()
    {
        if (entity.BaseExtension != null)
        {
            CalcEntityFields(project, entity.BaseExtension.DestEntity);
            AddBooleanFieldInParent(entity.BaseExtension.DestEntity,
                entity.BaseExtension.BooleanFieldIdInParentThatPresentMe);
        }

        foreach (var entityField in entity.entityFields.Values.Where(f => !f.AuditField))
        {
            var referenceField = entity.BaseExtension?.GetField(entityField.Id);
            if (referenceField != null)
                entityField.AddReferenceField(entity.BaseExtension, referenceField, null);

            if (entityField.AssociationEntity != null)
                CalcEntityFields(project, entityField.AssociationEntity.DestEntity);
            AddCompositionFields(entityField);
        }
    }

    private void AddCompositionFields(EntityField entityField)
    {
        if (entityField.CompositionEntity == null) return;
        foreach (var compositionField in entityField.CompositionEntity.DestEntity.entityFields.Values)
        {
            if (compositionField.IncludeInPkv || compositionField.AuditField) continue;//todo
            ExtractedField(compositionField, $"{entityField.Id}.{compositionField.Id}",
                $"{entityField.Name} {compositionField.Name}",
                $"{compositionField.EnName} of {entityField.EnName}", entityField.CompositionEntity, entityField);
        }
    }

    private void AddBooleanFieldInParent(Entity parentEntity, string booleanFieldIdInParentThatPresentMe)
    {
        var boolField = parentEntity.GetField(booleanFieldIdInParentThatPresentMe);
        if (boolField != null) return;
        boolField = new EntityField(parentEntity,
            booleanFieldIdInParentThatPresentMe, entity.Name,
            booleanFieldIdInParentThatPresentMe, typeof(bool),
            TVariableTypes.BOOL, EntityFieldFlags.DerivedEntityBooleanField)
        {
            IsRealMember = false
        };
        parentEntity.AddField(boolField);

        //boolField.AddReferenceField(relationship, baseField, parentAccociationField);
    }

    private void ExtractedField(EntityField baseField, string fieldId, string name, string enName,
        EntityRelationship relationship, EntityField parentAccociationField)
    {
        var extractedField = baseField.Clone(entity);
        extractedField.IsRealMember = false;
        extractedField.Id = fieldId;
        extractedField.Name = name;
        extractedField.EnName = enName;
        entity.AddField(extractedField);
        extractedField.AddReferenceField(relationship, baseField, parentAccociationField);
    }
}
