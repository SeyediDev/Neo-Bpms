namespace Neo.Bpms.Domain.Features.Cmmn.CodeFirst;

public class CSharpObjectToEntity(INameSpaceRepository nameSpaceRepository) : CSharpObjectToModel
{
    /// <summary>
    /// Defines the entity from code.
    /// </summary>
    /// <param name="modelNamespace">The namespace.</param>
    /// <param name="entityType">The type.</param>
    /// <param name="definedBefore"></param>
    /// <returns></returns>
    public Entity DefineEntity(ModelNamespace modelNamespace, Type entityType, out bool definedBefore)
    {
        object[] customAttributes = entityType.GetTypeInfo().GetCustomAttributes(true);
        NotInMetaAttribute notInMeta = GetAttribute<NotInMetaAttribute>(entityType.GetTypeInfo().GetCustomAttributes(false));
        definedBefore = false;
        if (notInMeta != null)
        {
            return null;
        }

        Entity entity = nameSpaceRepository.GetEntity(entityType, out string entityKey);
        if (entity != null)
        {
            definedBefore = true;
            return entity;
        }
        Entity.FetchEntityNameFromType(out string namespaceId, out string entityId, entityType);
        if (modelNamespace != null && entityKey != $"{modelNamespace.Id}.{entityId}")
        {
            modelNamespace = null;
        }
        modelNamespace ??= nameSpaceRepository.GetModel(namespaceId) ??
                nameSpaceRepository.AddNamespace(namespaceId, namespaceId, null);

        string id = entityType.Name;
        EFAttr_Id ids = new()
        {
            Name = entityType.Name,
            EnName = entityType.Name
        };
        ExtractIds(ids, customAttributes);
        entity = nameSpaceRepository.NewEntity(modelNamespace, entityType, ids.Name, id, ids.EnName);
        modelNamespace.AddEntity(entity);
        _ = nameSpaceRepository.Entities.TryAdd(entityKey, entity);
        entity.DbTableNameMap = ids.DBName;
        entity.OldDbTableNameMap = ids.OldDbName;
        GetEntityMetaFromAttributes(entity, customAttributes);
        DefineRelationshipEntity(entityType, entity);
        return entity;
    }

    private void DefineRelationshipEntity(Type type, Entity entity)
    {
        Entity baseEntity = FetchBaseEntity(type);
        if (baseEntity != null)
        {
            object[] entityAttributes = type.GetTypeInfo().GetCustomAttributes(true);
            BaseExtensionAttribute extensionAttribute = GetAttribute<BaseExtensionAttribute>(entityAttributes);
            if (extensionAttribute != null)
            {
                entity.SetBaseExtension(baseEntity, extensionAttribute.BooleanFieldIdInParentThatPresentMe);
            }
            else
            {
                _ = entity.AddBaseEntity(baseEntity, null);
            }
        }

        foreach (MemberInfo item in ReflectionField.Members(type))
        {
            DefineMemberRelationshipEntity(ReflectionField.FetchMemberType(item));
        }
    }

    private Entity FetchBaseEntity(Type type)
    {
        Type baseType = type.BaseType;
        Entity baseEntity = null;
        if (baseType != null && ReflectionTools.IsClass(baseType) && !baseType.IsAbstract)
        {
            baseEntity = DefineEntityFromType(baseType);
            baseEntity ??= FetchBaseEntity(baseType);
        }
        return baseEntity;
    }

    private void DefineMemberRelationshipEntity(Type type)
    {
        if (!ReflectionTools.IsClass(type))
        {
            return;
        }

        if (ReflectionTools.IsGenericList(type))
        {
            Type t = type.GetGenericArguments().FirstOrDefault();
            if (ReflectionTools.IsClass(t))
            {
                _ = DefineEntityFromType(t);
            }
        }
        if (type == null || type.TryGetInterfaceGenericParameters(typeof(ICollection<>), out _)||
           type.FullName.StartsWith("System.Collections.Generic.ICollection") )
        {
            return;
        }

        if (type == typeof(EntityState))
        {
            return;
        }

        _ = DefineEntityFromType(type);
    }

    private Entity DefineEntityFromType(Type type)
    {
        ModelNamespace modelNamespace = nameSpaceRepository.GetNamespaceFromType(type);
        if (modelNamespace == null)
        {

        }
        Entity entity = DefineEntity(modelNamespace, type, out bool definedBefore);
        if (entity != null && !definedBefore && type.IsAbstract)
        {
            entity.NotMapped = true;
        }

        return entity;
    }

    /// <summary>
    /// Gets the entity meta from attributes.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="attributes">The attributes.</param>
    private void GetEntityMetaFromAttributes(Entity entity, IEnumerable<object> attributes)
    {
        Trigger trigger = null!;
        EAttr_Audit atrAudit = null!;
        foreach (object attr in attributes)
        {
            if (attr is System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute ||
                entity.model.Id == "ProcessEntities")
            {
                entity.DontSync = true;
                entity.NotMapped = true;
            }

            switch (attr)
            {
                case DisplayNameAttribute at:
                    entity.Name = at.DisplayName;
                    break;
                case DbMapAttribute at:
                    entity.DbTableNameMap = at.DBName;
                    entity.OldDbTableNameMap = at.OldDbName;
                    break;
                case OldDbMapAttribute at:
                    entity.OldDbTableNameMap = at.OldDbName;
                    break;
                case SBVRAttribute sbvr:
                    entity.AddSBVR(sbvr);
                    break;
                case EAttr_DataEvent dataEvent:
                    entity.addDataEvent(new DataEvent(entity, dataEvent.Id, dataEvent.Name, Parser.Parse(dataEvent.Condition)));
                    break;
                case EAttr_DefaultOrderBy _:
                    //TODO
                    break;
                case Models.Attributes.EntityAttributes.EntityIndex attrIndex:
                    {
                        Models.Cmmn.Entities.EntityIndex idx = attrIndex.IsUnique
                            ? entity.AddUniqueIndex(attrIndex.Id, attrIndex.EnName, attrIndex.Name, attrIndex.Clustered)
                            : entity.AddIndex(attrIndex.Id, attrIndex.EnName, attrIndex.Name, attrIndex.Clustered);
                        if (idx != null)
                        {
                            if (string.IsNullOrEmpty(attrIndex.Fields))
                            {
                                idx.Fields.Add(new IndexField { FieldName = attrIndex.EnName });
                            }
                            else
                            {
                                string[] idxFields = attrIndex.Fields.Split(',');
                                foreach (string idxField in idxFields)
                                {
                                    string[] field = idxField.Split('#');
                                    if (field.Length > 1 && (field[1] == "I" || field[1] == "Included"))
                                    {
                                        idx.Fields.Add(new IndexField { FieldName = field[0], IsIncluded = true });
                                    }
                                    else
                                    {
                                        idx.Fields.Add(new IndexField { FieldName = field[0] });
                                    }
                                }
                            }
                        }

                        break;
                    }

                case EAttr_State attrState:
                    _ = entity.AddState(new EntityState(entity, attrState.Id, attrState.Name, attrState.EnName, attrState.Category)
                    {
                        stateFieldName = attrState.StateFieldName
                    });
                    break;
                case States states:
                    AddStatesToEntity(entity, states.EnumType);
                    break;
                case EAttr_Audit audit:
                    atrAudit = audit;
                    break;
                case SchemaAttribute schema:
                    entity.Schema = schema.Name;
                    break;
                case FileGroupAttribute fileGroup:
                    entity.FileGroup = fileGroup.Name;
                    break;
                case PartitionAttribute attrPartition:
                    entity.PartitionField = attrPartition.PartitionFieldId;
                    entity.PartitionScheme = attrPartition.PartitionScheme;
                    entity.PartitionSchemeFileGroups = attrPartition.FileGroups;
                    break;
                case EAttr_View view:
                    entity.ViewSetting = new ViewSetting
                    {
                        Query = view.Query,
                        IsDbQuery = view.IsDbQuery
                    };
                    break;
                case DontSync _:
                    entity.DontSync = true;
                    break;
                case EAttr_Collation collation:
                    entity.Collation = collation.ToString();
                    break;
                case EAttr_Trigger attrTrigger:
                    {
                        Entity tEntity = nameSpaceRepository.GetEntity(attrTrigger.TriggeredEntityNamespace, attrTrigger.TriggeredEntityId);
                        DataOperation dop = tEntity?.getDataOperation(attrTrigger.TriggeredOperationId);
                        if (dop != null)
                        {
                            trigger = new Trigger
                            {
                                condition = Parser.ParseTree(attrTrigger.Condition),
                                triggeredOperation = dop
                            };
                            entity.addTrigger(trigger);
                        }

                        break;
                    }

                case EAttr_TriggerParameter parameter:
                    trigger?.addParameter(!string.IsNullOrEmpty(parameter.SourceFieldId)
                        ? new Trigger.Parameter(parameter.TrigFieldId, parameter.SourceFieldId)
                        : new Trigger.Parameter(parameter.TrigFieldId, Parser.ParseTree(parameter.Formula)));
                    break;
                case EFAttr_Conformance at:
                    entity.AddConformance(new Conformance(entity, null, at.Name, at.Name, at.Access, at.Level, at.Subject,
                        Parser.Parse(at.ConformanceExpression), new ExceptionInformation(at.Error.DataEngine, at.Error.Name)
                        {
                            ErrorText = at.Error.ErrorText,
                            ErrorFormula = at.Error.ErrorFormula,
                            EnErrorText = at.Error.EnErrorText,
                            EnErrorFormula = at.Error.EnErrorFormula
                        }));
                    break;
                case DoSynchronizationIntervalAttribute time:
                    entity.DoSynchronizationInterval = time.Interval;
                    break;
                case DataProviderAttribute dataProvider:
                    entity.Provider = dataProvider.Name;
                    break;
            }
        }

        if (entity.model.Id != "ProcessEntities" && atrAudit?.dontAudit != true && entity.ViewSetting == null)
        {
            entity.Auditable = AuditableVersion.V1;
        }
        if (entity.EntityType.IsInBaseInterface<IDomainEventEntity>())
        {
            entity.Auditable = entity.EntityType.IsInBaseInterface<IBaseAuditableEntity>() ? AuditableVersion.V2 : null;
        }
        if (entity.Auditable is not null)
        {
            entity.SetEntityAuditFields();
        }
    }

    private static void AddStatesToEntity(Entity entity, Type enumType)
    {
        if (!enumType.IsEnum)
        {
            return;
        }

        Array values = Enum.GetValues(enumType);
        string[] names = Enum.GetNames(enumType);
        int i = 0;
        Enumeration @enum = entity.model.GetEnum(enumType.Name);
        entity.SetStateCollection(new StateDictionary(@enum?.Id));
        foreach (object value in values)
        {
            Enum eValue = (Enum)value;
            EAttr_State state = eValue.GetAttribute<EAttr_State>();
            _ = entity.AddState(new EntityState(entity, eValue.ToInt(), state?.Name ?? names[i], names[i],
                state?.Category ?? EntityStateCategory.ActiveNode)
            {
                stateFieldName = state?.StateFieldName!
            });
            i++;
        }
    }
}
