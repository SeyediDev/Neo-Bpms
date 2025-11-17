using Neo.Bpms.Domain.Models.Cmmn.Relationship;
using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityModels;

public class EntityViewModel
{
    public EntityViewModel()
    {
    }

    public EntityViewModel(Entity entity)
    {
        id = entity.Id;
        namespaceId = entity.NamespaceId;
        name = entity.Name;
        isStateBased = entity.GetStateCollection()?.States?.Count > 0 || !string.IsNullOrEmpty(entity.GetStateCollection()?.EnumId);
        stateEnumId = entity.GetStateCollection()?.EnumId;
        stateCollection = entity.GetStateCollection()?.States?.Values.Select(s => new EnumValueViewModel(s));
        notMapped = entity.NotMapped;
        dontSync = entity.DontSync;
        dbName = entity.DbTableNameMap;
        oldDbName = entity.OldDbTableNameMap;
        dontAudit = entity.Auditable is null;
        partitionField = entity.PartitionField;
        partitionSchema = entity.PartitionScheme;
        fileGroup = entity.FileGroup;
        indexes = entity.indexes?.Select(i => new IndexViewModel(i));//todo
        fields = entity.entityFields?.Values.Where(f => !f.IsRelationShipField).Select(f => new FieldViewModel(f)) ?? [];
        allFields = [];
        BaseEntity baseEntity = entity.BaseEntities?.Values.FirstOrDefault(b => b.RelationshipField == null);
        relationship = new RelationshipViewModel
        {
            extension = entity.BaseExtension != null ? new BaseExtensionViewModel
            {
                destNamespaceId = entity.BaseExtension.DestNamespaceId,
                destEntityId = entity.BaseExtension.DestEntityId,
                booleanFieldIdInParentThatPresentMe = entity.BaseExtension.BooleanFieldIdInParentThatPresentMe
            } : new BaseExtensionViewModel(),
            baseEntity = baseEntity != null ? new DestEntityViewModel
            {
                destNamespaceId = baseEntity.DestNamespaceId,
                destEntityId = baseEntity.DestEntityId
            } : new DestEntityViewModel()
        };
        associations = entity.entityFields?.Values
            .Where(f => f.IsRelationShipField)
            .Select(f => new RelationItemViewModel(f)).ToList();
    }

    public string id { get; set; }
    public string namespaceId { get; set; }
    public string name { get; set; }
    public bool isStateBased { get; set; }
    public string stateEnumId { get; set; } //todo see modify
    public IEnumerable<EnumValueViewModel> stateCollection { get; set; } //todo see modify
    public bool notMapped { get; set; }
    public bool dontSync { get; set; }
    public string dbName { get; set; }
    public string oldDbName { get; set; }
    public bool dontAudit { get; set; }
    public string partitionField { get; set; }
    public string partitionSchema { get; set; }
    public string fileGroup { get; set; }
    public IEnumerable<IndexViewModel> indexes { get; set; }
    public IEnumerable<FieldViewModel> fields { get; set; }
    public IEnumerable<FieldViewModel> allFields { get; set; }
    public RelationshipViewModel relationship { get; set; }
    public IEnumerable<RelationItemViewModel> associations { get; set; }

    public void ModifyEntity(Entity entity)
    {
        entity.Id = id;
        entity.Name = name;
        if (isStateBased)
        {
            if (!string.IsNullOrEmpty(stateEnumId))
            {
                entity.SetStateCollection(new StateDictionary(stateEnumId));
                Enumeration enumeration = entity.model.GetEnum(stateEnumId); //todo enum may not be in the same model
                if (enumeration?.items != null)
                {
                    entity.GetStateCollection().AddStates(enumeration.items.Values.Select(i => new EntityState(entity,
                        Convert.ToInt32(i.Id),
                        i.Name, i.EnName, i.StateCategory)));
                }
            }
            else
            {
                entity.SetStateCollection(new StateDictionary(stateCollection?.Select(si => si.ToEntityState(entity))));
            }
        }
        else if (!isStateBased)
        {
            entity.ReleaseStates();
        }

        entity.NotMapped = notMapped;
        entity.DontSync = dontSync;
        entity.DbTableNameMap = dbName;
        entity.OldDbTableNameMap = oldDbName;
        entity.PartitionField = partitionField;
        entity.PartitionScheme = partitionSchema;
        entity.FileGroup = fileGroup;
        entity.indexes = indexes?.Select(vidx => vidx.ToEntityIndex()).ToList();
        entity.entityFields ??= [];
        foreach (FieldViewModel fieldViewModel in fields ?? [])
        {
            entity.entityFields.TryGetValue(fieldViewModel.prevId ?? "", out EntityField entityField);
            if (entityField == null)
            {
                entityField = new EntityField(entity, fieldViewModel.id,
                    fieldViewModel.persianName, fieldViewModel.englishName,
                    EntityField.GetFieldType(fieldViewModel.type), fieldViewModel.type);
                entity.entityFields.TryAdd(fieldViewModel.id, entityField);
            }
            else
            {
                if (entityField.Id != fieldViewModel.prevId)
                {
                    entity.entityFields.TryRemove(fieldViewModel.prevId ?? "", out EntityField prevEntityField);
                    entity.entityFields?.TryAdd(fieldViewModel.id, entityField);
                }
            }

            fieldViewModel.ModifyEntityField(entityField);
        }

        foreach (string entityEntityFieldId in entity.entityFields?.Select(f => f.Key).ToList())
        {
            FieldViewModel fieldViewModel = fields?.FirstOrDefault(f => f.id == entityEntityFieldId);
            if (fieldViewModel != null) continue;
            entity.entityFields.TryRemove(entityEntityFieldId, out EntityField prevEntityField);
        }

        entity.Auditable = dontAudit ? AuditableVersion.V1 : null;
        entity.SetEntityAuditFields();

        if (relationship.extension.destNamespaceId != null && relationship.extension.destEntityId != null)
        {
            entity.SetBaseExtension(ProjectDefinition.Project.GetEntity(relationship.extension.destNamespaceId, relationship.extension.destEntityId)
                , relationship.extension.booleanFieldIdInParentThatPresentMe);
        }
        else if (relationship.baseEntity.destNamespaceId != null && relationship.baseEntity.destEntityId != null)
            entity.AddBaseEntity(ProjectDefinition.Project.GetEntity(relationship.baseEntity.destNamespaceId, relationship.baseEntity.destEntityId), null);
        SetRelationships(entity, associations);
    }

    private static void SetRelationships(Entity entity, IEnumerable<RelationItemViewModel> relationshipList)
    {
        foreach (RelationItemViewModel association in relationshipList ?? [])
        {
            entity.entityFields.TryGetValue(association.prevId ?? "", out EntityField entityField);
            if (entityField == null)
            {
                Entity destEntity = ProjectDefinition.Project.GetEntity(association.destNamespaceId, association.destEntityId);
                entityField = new EntityField(entity, association.id, association.persianName, association.englishName,
                    destEntity.GetType(), TVariableTypes.Association);
                entity.entityFields.TryAdd(association.id, entityField);
            }
            else
            {
                if (entityField.Id != association.prevId)
                {
                    entity.entityFields.TryRemove(association.prevId ?? "", out _);
                    entity.entityFields?.TryAdd(association.id, entityField);
                }
            }

            association.ModifyEntityField(entityField);
        }
    }
}
