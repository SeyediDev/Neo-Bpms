using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.Domain.Entities.Cmmn.Relationship;
using Neo.Bpms.Domain.Entities.Cmmn.ServiceOperations;

namespace Neo.Bpms.Domain.Entities.Cmmn.Entities;

public interface IModelEntity 
{ 
    string Id { get; set; }
}
public class Entity : BaseModelClass, IModelEntity, ISBVRContainer
{
    #region Entity

    public Entity(ModelNamespace model, Type entityType, string name, string entityId, string enName,
        string schema = null, string provider = null)
        : base(model, entityId, name)
    {
        _schema = schema;
        _provider = provider;
        EntityType = entityType;
        entityFields = [];
        EnName = enName;
    }

    public Entity()
    {
        entityFields = [];
    }

    public Type EntityType { get; set; }

    [XmlIgnore] public ModelNamespace model => Parent as ModelNamespace;

    [XmlIgnore] public string NamespaceId => model.Id;

    [XmlIgnore] public EntityAddress EntityAddress => new() { NamespaceId = NamespaceId, EntityId = Id };

    private string _schema;

    public string Schema
    {
        get => _schema ?? model?.Schema;
        set => _schema = value;
    }

    private string _provider;

    public string Provider
    {
        get => _provider ?? model?.Provider;
        set => _provider = value;
    }

    public string ProviderName => Provider ?? "default";

    public string DbTableNameMap { get; set; }

    public string OldDbTableNameMap { get; set; }

    #endregion Entity

    #region fields

    //[XmlIgnore]
    //public EntityMeta metaData;

    [XmlIgnore] public bool CalcEntityFields { get; set; }
    [XmlIgnore] public EntityFields entityFields { get; set; }

    [XmlIgnore] public IEnumerable<EntityField> KeyFields => entityFields?.Values.Where(f => f.IncludeInPkv);
    [XmlIgnore] public long DbId { get; set; }

    public bool AddField(EntityField field)
    {
        entityFields ??= [];
        if (entityFields.ContainsKey(field.Id))
        {
            return false;
        }

        _ = entityFields.TryAdd(field.Id, field);
        return true;
    }

    public EntityField GetField(string fieldId)
    {
        if (fieldId == null || entityFields == null)
        {
            return null;
        }

        _ = entityFields.TryGetValue(fieldId, out EntityField ef);
        return ef;
    }

    public EntityField GetFieldByDbName(string dbName)
    {
        if (dbName == null || entityFields == null)
        {
            return null;
        }

        if (entityFields.TryGetValue(dbName, out EntityField ef) && ef.DbFieldName != dbName)
        {
            ef = null;
        }

        return ef ?? entityFields.Values.FirstOrDefault(f => f.DbFieldName == dbName);
    }

    public void SetEntityAuditFields()
    {
        if (Auditable is null)
        {
            foreach (EntityField field in entityFields?.Values
                                      .Where(fld => fld.Flags == EntityFieldFlags.AuditField) ??
                                  [])
            {
                field.RemoveFieldFlags(EntityFieldFlags.AuditField);
            }

            return;
        }
        if (Auditable == AuditableVersion.V1)
        {
            _ = SetEntityAuditField("AutoAudit_CreatedDate", "تاریخ ایجاد", "AutoAudit_CreatedDate",
                TVariableTypes.DateTime, "AutoAudit_CreatedDate");
            EntityField f = SetEntityAuditField("AutoAudit_CreatedBy", "کاربر پایگاه داده ایجاد کننده", "AutoAudit_CreatedBy",
                TVariableTypes.String, "AutoAudit_CreatedBy");
            f.MaxLen = 128;
            f = SetEntityAuditField("AutoAudit_CreatedByUsr", "شناسه کاربر ایجاد کننده", "AutoAudit_CreatedByUsr",
                TVariableTypes.String, "AutoAudit_CreatedByUsr");
            f.MaxLen = 41;
            _ = SetEntityAuditField("AutoAudit_ModifiedDate", "تاریخ تغییر", "AutoAudit_ModifiedDate", TVariableTypes.DateTime,
                "AutoAudit_ModifiedDate");
            f = SetEntityAuditField("AutoAudit_ModifiedBy", "کاربر پایگاه داده آخرین تغییر", "AutoAudit_ModifiedBy",
                TVariableTypes.String, "AutoAudit_ModifiedBy");
            f.MaxLen = 128;
            f = SetEntityAuditField("AutoAudit_ModifiedByUsr", "شناسه کاربر آخرین تغییر", "AutoAudit_ModifiedByUsr",
                TVariableTypes.String, "AutoAudit_ModifiedByUsr");
            f.MaxLen = 41;
            _ = SetEntityAuditField("AutoAudit_RowVersion", "نسخه تغییرات", "AutoAudit_RowVersion", TVariableTypes.Long,
                "AutoAudit_RowVersion");
        }
    }

    private EntityField SetEntityAuditField(string fieldId, string fieldName, string fieldEnName,
        TVariableTypes fieldType, string fieldDbName)
    {
        if (entityFields.ContainsKey(fieldId))
        {
            _ = entityFields.TryRemove(fieldId, out EntityField entityField);
            if (entityField.IsAutoIncrement())
            {
                entityField.DisableAutoIncrement();
            }
        }

        EntityField field = new(this, fieldId, fieldName + Name, fieldEnName,
            EntityField.GetFieldType(fieldType), fieldType, EntityFieldFlags.AuditField);
        _ = AddField(field);
        field.SetDbFieldNameMap(fieldDbName, null);
        field.Flags = EntityFieldFlags.AuditField;
        return field;
    }

    #endregion fields

    #region indexes

    public List<EntityIndex> indexes = [];

    public EntityIndex AddIndex(string fieldId, string enName, string indexName, bool clustered)
    {
        EntityIndex currentIndex =
            new()
            { Id = fieldId, Name = indexName, EnName = enName, IsUnique = false, Clustered = clustered };
        indexes.Add(currentIndex);
        return currentIndex;
    }

    public EntityIndex AddUniqueIndex(string fieldId, string enName, string indexName, bool clustered)
    {
        EntityIndex currentIndex =
            new()
            { Id = fieldId, Name = indexName, EnName = enName, IsUnique = true, Clustered = clustered };
        indexes.Add(currentIndex);
        return currentIndex;
    }

    #endregion indexes

    #region Basic String

    private List<BasicField> PrivateDisplayString { get; set; } = [];
    public List<BasicField> SelfDisplayStrings => PrivateDisplayString;
    public List<BasicField> DisplayStrings => PrivateDisplayString.Count != 0
        ? PrivateDisplayString
        : ParentEntities?.SelectMany(pr => pr.ParentEntity?.DestEntity?.DisplayStrings).ToList();

    public bool AddBasicField(BasicField basicField)
    {
        PrivateDisplayString.Add(basicField);
        return true;
    }

    public bool RemoveBasicField(BasicField basicField)
    {
        _ = PrivateDisplayString.Remove(basicField);
        return true;
    }

    #endregion Basic String

    #region StateCollection

    public StateDictionary StateCollection;
    public bool IsEntityState => NamespaceId == "Shared" && Id == "EntityStateName";
    public bool IsStateBase => GetStateCollection()?.States?.Any() ?? false;

    public StateDictionary GetStateCollection()
    {
        return StateCollection
               ?? ParentEntities?.FirstOrDefault(p => p.ParentEntity?.DestEntity?.IsStateBase ?? false)
                   ?.ParentEntity?.DestEntity?.GetStateCollection();
    }

    public string StateIdFieldId => IsStateBase ? "StateId" : null;

    public long? FirstActiveStateId => GetStateCollection()?.States?.Values
        .FirstOrDefault(s => (s.category & (uint)EntityStateCategory.ActiveNode) != 0)?.LongId;

    public long? FirstBackupStateId => GetStateCollection()?.States?.Values
        .FirstOrDefault(s => (s.category & (uint)EntityStateCategory.BackupNode) != 0)?.LongId;

    public EntityState GetState(string i)
    {
        return GetStateCollection()?.GetState(i);
    }

    public EntityState AddState(EntityState state)
    {
        StateCollection ??= new StateDictionary();

        return StateCollection.AddState(state);
    }

    public void ReleaseStates()
    {
        StateCollection = null;
        //todo if need to check parents
    }

    #endregion StateCollection

    #region Relationship

    public BaseExtension BaseExtension { get; set; }
    public Dictionary<string, BaseEntity> BaseEntities { get; set; }
    public IEnumerable<EntityField> ParentEntities => entityFields.Values.Where(f => f.ParentEntity != null);

    [XmlIgnore]
    public ConcurrentDictionary<string, DerivedEntity> DerivedEntities =
        new();

    [XmlIgnore]
    public List<CompositionEntity> Compositions =>
        entityFields?.Where(f => f.Value.CompositionEntity != null).Select(f => f.Value.CompositionEntity).ToList();

    [XmlIgnore]
    public List<Association> Associations =>
        entityFields?.Where(f => f.Value.AssociationEntity != null).Select(f => f.Value.AssociationEntity).ToList();

    public bool DerivedEntitiesExtendedMe => (DerivedEntities?.Values.Any(e => e.ReferToEntity.GetType() == typeof(BaseExtension)) ?? false) ||
                   (DerivedEntities?.Any(d => d.Value.SourceEntity?.DerivedEntitiesExtendedMe ?? false) ?? false);


    public Association GetAssociation(string associationId)
    {
        return entityFields?.FirstOrDefault(f => f.Value?.AssociationEntity?.Id == associationId).Value
            ?.AssociationEntity;
    }


    public bool CheckExistsBaseOrParentEntity(Entity entity, out EntityRelationship entityRelationship)
    {
        if (BaseEntities != null && TryGetBaseEntity(entity.NamespaceId, entity.Id, out BaseEntity baseEntity))
        {
            entityRelationship = baseEntity;
            return true;
        }

        if (ParentEntities != null && TryGetParentEntity(entity.NamespaceId, entity.Id, out ParentEntity parentEntity))
        {
            entityRelationship = parentEntity;
            return true;
        }

        entityRelationship = null;
        return false;
    }

    public bool TryGetBaseEntity(string namespaceId, string entityId, out BaseEntity baseEntity)
    {
        if (BaseEntities == null)
        {
            baseEntity = null;
            return false;
        }

        if (BaseEntities.TryGetValue(EntityRelationship.ReferEntityKey(namespaceId, entityId), out baseEntity))
        {
            return true;
        }

        foreach (BaseEntity entity in BaseEntities.Values)
        {
            if (entity.DestEntity == null)
            {
                continue;
            }

            if (entity.DestEntity.TryGetBaseEntity(namespaceId, entityId, out baseEntity))
            {
                return true;
            }
        }

        return false;
    }

    public bool TryGetParentEntity(string namespaceId, string entityId, out ParentEntity parentEntity)
    {
        string key = EntityRelationship.ReferEntityKey(namespaceId, entityId);
        parentEntity = ParentEntities?.FirstOrDefault(p =>
                EntityRelationship.ReferEntityKey(p.ParentEntity.DestNamespaceId, p.ParentEntity.DestEntityId) == key)
            ?.ParentEntity;
        if (parentEntity != null)
        {
            return true;
        }

        foreach (EntityField p in ParentEntities)
        {
            if (p.ParentEntity.DestEntity.TryGetParentEntity(namespaceId, entityId, out parentEntity))
            {
                return true;
            }
        }

        return false;
    }

    public DerivedEntity GetDerivedEntity(string derivedEntity)
    {
        if (DerivedEntities.TryGetValue(derivedEntity, out DerivedEntity ef))
        {
            return ef;
        }

        foreach (DerivedEntity derived in DerivedEntities.Values)
        {
            ef = derived.DestEntity.GetDerivedEntity(derivedEntity);
            if (ef != null)
            {
                return ef;
            }
        }

        return null;
    }

    public BaseEntity AddBaseEntity(Entity baseEntity, EntityField relationshipField)
    {
        if (CheckExistsBaseOrParentEntity(baseEntity, out EntityRelationship entityRelationship))
        {
            return entityRelationship as BaseEntity;
        }

        BaseEntity referToEntity = new(this, baseEntity, relationshipField);
        BaseEntities ??= [];
        _ = BaseEntities.TryAdd(EntityRelationship.ReferEntityKey(baseEntity.NamespaceId, baseEntity.Id), referToEntity);
        AddToDerivedEntities(baseEntity, referToEntity);
        return referToEntity;
    }

    public ParentEntity AddParentEntity(Entity parentEntity, string booleanFieldIdInParent)
    {
        if (CheckExistsBaseOrParentEntity(parentEntity, out EntityRelationship entityRelationship))
        {
            return entityRelationship as ParentEntity;
        }

        ParentEntity referToEntity = new(this, parentEntity, booleanFieldIdInParent);
        AddToDerivedEntities(parentEntity, referToEntity);
        return referToEntity;
    }

    public CompositionEntity SetCompositionEntity(EntityField field, Entity destEntity)
    {
        return field.CompositionEntity = new CompositionEntity(this, field.Id, field.Name, destEntity);
    }

    public void SetBaseExtension(Entity baseEntity, string booleanFieldIdInParentThatPresentMe)
    {
        BaseExtension = new BaseExtension(this, baseEntity, booleanFieldIdInParentThatPresentMe);
        AddToDerivedEntities(baseEntity, BaseExtension);
    }

    private void AddToDerivedEntities(Entity entity, EntityRelationship baseEntity)
    {
        entity.DerivedEntities ??= new ConcurrentDictionary<string, DerivedEntity>();
        _ = entity.DerivedEntities.TryAdd(Id, new DerivedEntity(baseEntity));
    }

    #endregion

    #region DataOperations

    private Dictionary<string, DataOperation> _dataOperations;

    public void addDataOperation(DataOperation dataOperation)
    {
        _dataOperations ??= [];
        if (!_dataOperations.ContainsKey(dataOperation.Name))
        {
            _ = _dataOperations.TryAdd(dataOperation.Name, dataOperation);
        }
    }

    //public DataOperation getDataOperation(int dataOperationId)
    //{
    //	if (dataOperations != null && dataOperations.ContainsKey(dataOperationId))
    //		return dataOperations[dataOperationId];
    //	return null;
    //}
    public DataOperation getDataOperation(string dataOperationName)
    {
        _ = _dataOperations.TryGetValue(dataOperationName, out DataOperation dop);
        return dop;
    }

    #endregion DataOperations

    #region auto calcs

    public AutoCalcList AutoCalcs { get; set; }

    public void InitAutoCalcs()
    {
        AutoCalcs ??= new AutoCalcList();
    }

    #endregion auto calcs

    #region validations

    [XmlIgnore] public List<Validation> Validations;

    public void addValidation(Validation validation)
    {
        Validations ??= [];
        Validations.Add(validation);
    }

    #endregion validations

    #region triggers

    public List<Trigger> triggers;

    public void addTrigger(Trigger trigger)
    {
        triggers ??= [];
        triggers.Add(trigger);
    }

    #endregion triggers

    #region data events

    public List<DataEvent> dataEvents;

    public void addDataEvent(DataEvent dataEvent)
    {
        dataEvents ??= [];
        dataEvents.Add(dataEvent);
    }

    #endregion data events

    #region conformance

    /// <summary>
    /// conformance will be defined in the security models as a function checking conformance of entity for a user
    /// </summary>
    public List<BaseModelClass> Conformances;

    public void AddConformance(BaseModelClass conformance)
    {
        Conformances ??= [];
        Conformances.Add(conformance);
    }

    #endregion conformance

    #region package

    [XmlIgnore] //todo
    public List<Package> packages;

    public void addPackage(Package package)
    {
        packages ??= [];
        packages.Add(package);
    }

    #endregion package

    public AuditableVersion? Auditable { get; set; }
    private bool _dontSync { get; set; }
    public bool DontSync { get => _dontSync || model.DontSync; set => _dontSync = value; }
    public string Collation { get; set; }

    public bool NotMapped { get; set; }
    public bool Partitioned => !string.IsNullOrEmpty(PartitionField);
    public string PartitionField { get; set; }
    public string PartitionScheme { get; set; }
    public string[] PartitionSchemeFileGroups { get; set; }
    
    private string _fileGroup;

    public string FileGroup
    {
        get => _fileGroup ?? ((Schema != "dbo" ? Schema : null) ?? null)?? "PRIMARY";
        set => _fileGroup = value;
    }

    public bool UseEnumData { get; set; }
    public bool IsEnum => model.GetEnums().Values.FirstOrDefault(e => e.Id == Id) != null;

    #region View

    public ViewSetting ViewSetting { get; set; }

    public bool IsView => ViewSetting != null;
    #endregion View

    //data operation read update insert upsert list
    //list with different filter
    //
    [XmlIgnore] public ConcurrentDictionary<string, EntityServiceOperation> Services;

    public void AddServiceOperation(EntityServiceOperation serviceOperation)
    {
        Services ??= [];
        _ = Services.TryAdd(serviceOperation.ServiceName, serviceOperation);
    }

    public EntityServiceOperation GetServiceOperation(EntityServiceOperationType type, string serviceName)
    {
        if (string.IsNullOrEmpty(serviceName))
        {
            return Services?.Values.FirstOrDefault(s => s.EntityServiceOperationType == type);
        }

        EntityServiceOperation serviceOperation = Services.TryGetValue(serviceName, out EntityServiceOperation value) ? value : null;
        return serviceOperation != null && serviceOperation.EntityServiceOperationType != type
            ? throw new Exception($"Invalid request type {type} for service operation {serviceName}")
            : serviceOperation;
    }

    /*public enum eDataModel
    {
        Conceptual,
        Logical,
        Physical,
    }*/
    //public eDataModel dataModel { get; set; } = eDataModel.Physical;
    //public string versionColumn { get; set; }
    //public string discriminatorColumn { get; set; }
    //public string WeightParamId;
    public Dictionary<string, string> GetAssociationMapFields()
    {
        Dictionary<string, string> mapFields = [];
        if (Associations != null)
        {
            foreach (Association association in Associations)
            {
                if (association.Hidden || association.Maps == null)
                {
                    continue;
                }

                foreach (EntityRelationMap map in association.Maps.Where(map => !mapFields.ContainsKey(map.SourceField)))
                {
                    mapFields.Add(map.SourceField, map.SourceField);
                }
            }
        }

        return mapFields;
    }

    public static void FetchEntityNameFromType(out string namespaceId, out string entityId, Type type)
    {
        entityId = type.Name;
        namespaceId = "";
        if (type.Namespace == null)
        {
            return;
        }

        string[] namespaceIds = type.Namespace.Split('.');
        namespaceId = namespaceIds[^1];
    }

    public IEnumerable<EntityField> MappedEntityFields =>
        BaseExtension != null ? [] : EntityExtendedFields;

    private IEnumerable<EntityField> EntityExtendedFields
    {
        get
        {
            Dictionary<string, EntityField> fields = entityFields?.Values.Where(f => f.MappedToDataInThisEntity).ToDictionary(d => d.Id) ??
                         [];
            // ReSharper disable once InvertIf
            if (DerivedEntities != null)
            {
                foreach (BaseExtension derivedExtension in DerivedEntities.Values.Select(e => e.ReferToEntity)
                    .OfType<BaseExtension>())
                {
                    IEnumerable<EntityField> mappedDerivedEntityFields = derivedExtension.SourceEntity.EntityExtendedFields;
                    foreach (EntityField mappedDerivedEntityField in mappedDerivedEntityFields)
                    {
                        if (!fields.ContainsKey(mappedDerivedEntityField.Id))
                        {
                            fields.Add(mappedDerivedEntityField.Id, mappedDerivedEntityField);
                        }
                    }
                }
            }

            return [.. fields.Values];
        }
    }

    public TimeSpan DoSynchronizationInterval { get; set; }

    public bool Equals(Entity e)
    {
        return NamespaceId == e.NamespaceId && Id == e.Id;
    }

    public void SetStateCollection(StateDictionary stateDictionary)
    {
        StateCollection = stateDictionary;
    }

    public virtual object GetForm(string formId)
    {
        return null;
    }

    public List<SBVR> SBVRs { get; set; } = [];
    public void AddSBVR(SBVRAttribute sbvr)
    {
        SBVRs.Add(new SBVR()
        {
            Modality = sbvr.Modality,
            Subject = sbvr.Subject ?? GetType().Name,
            VerbPhrase = sbvr.VerbPhrase,
            Condition = sbvr.Condition
        });
    }
}
public enum AuditableVersion { V1, V2 }
