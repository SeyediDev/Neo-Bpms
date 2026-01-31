using Neo.Bpms.Domain.Models.Base;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms;

public static class ComboDataRoutines
{
    public static void SetComboDataSelectedId(CommonFormStructure structure,
        ElasticObject mainData, bool recordIsFilterValue)
    {
        if (mainData == null)
        {
            return;
        }

        foreach (InputFieldDefinition field in structure.Fields.Where(field => field.HasProperty(eControlPropertyId.FalseTitle)))
        {
            mainData[field.FieldName] = (int)BooleanEntityField.GetValueItem(recordIsFilterValue, mainData[field.FieldName]);
        }
    }

    public static ComboData GetRecords(Entity baseEntity, Entity entity,
        string culture, string filter, int pageNo, string constraint = null,
        string displayFields = null, LocalParameters localParameters = null,
        string orderBy = null, int topRows = 100000,
        bool onlyActiveStates = true, IEnumerable<UIComponentProperty> properties = null)
    {
        ComboData cd = InitComboData(baseEntity, entity, culture, filter, displayFields, properties, topRows);
        if (entity.IsEntityState)
        {
            return GetEntityStatesAsRecordData(baseEntity, cd, culture);
        }

        if (entity.UseEnumData)
        {
            return GetEntityStatesAsRecordData(entity, cd, culture);
        }

        QueryUtility q = new(entity, "ComboDataRoutines.1");
        try
        {
            AddKeyFields(entity, q);
            if (!string.IsNullOrEmpty(displayFields))
            {
                AddDisplayFields(displayFields, orderBy, q);
            }
            else
            {
                AddBasicStringFields(entity, culture, orderBy, q);
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                _ = q.AddFilter(filter);
            }

            if (!string.IsNullOrWhiteSpace(constraint))
            {
                _ = q.AddFilter(constraint);
            }
            if (entity.EntityType.IsInBaseInterface<ISoftDelete>())
            {
                q.Where($"({nameof(ISoftDelete.IsDeleted)} == false) Or ({nameof(ISoftDelete.IsDeleted)} == null)");
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                _ = q.OrderBy(orderBy);
            }

            if (onlyActiveStates)
            {
                _ = q.ActiveStates();
            }

            AddPropertiesFields(cd.Properties, q);
            _ = q.SetPage(pageNo, topRows);
            q.ForEach(localParameters, r =>
            {
                FormDataRow dataRow = GetDataRow(culture, displayFields, entity, r, cd.Properties);
                cd.AddRow(dataRow);
            });
        }
        catch
        {
            q.Release();
        }
        return cd;
    }

    public static ComboData InitComboData(Entity baseEntity, Entity entity,
        string culture, string filter, string displayFields, 
        IEnumerable<UIComponentProperty> properties, int count)
    {
        ComboData cd = new(displayFields)
        {
            NamespaceId = entity?.NamespaceId ?? baseEntity?.NamespaceId,
            EntityId = entity?.Id ?? baseEntity?.Id,
            Filter = filter,
            Culture = culture,
            Properties = properties?.Where(p => p.PropertyId.In(
                  eControlPropertyId.SuccessWhen
                , eControlPropertyId.DangerWhen
                , eControlPropertyId.WarningWhen
                , eControlPropertyId.InfoWhen
                , eControlPropertyId.ActiveWhen)).ToList(),
            Count = count
        };
        if (baseEntity != null && entity?.IsEntityState is true)
        {
            cd.NamespaceId += "." + baseEntity.NamespaceId;
            cd.EntityId += "." + baseEntity.Id;
        }

        return cd;
    }

    private static ComboData GetEntityStatesAsRecordData(Entity baseEntity, ComboData cd, string culture)
    {
        foreach (EntityState state in baseEntity?.GetStateCollection()?.States?.Values ?? Enumerable.Empty<EntityState>())
        {
            FormDataRow fdr = new()
            {
                Ids = state.Id,
                DisplayValue = culture == "en" ? state.enName : state.Name
            };
            cd.AddRow(fdr);
        }

        return cd;
    }

    private static void AddKeyFields(Entity entity, QueryUtility q)
    {
        foreach (EntityField field in entity.KeyFields)
        {
            _ = q.SelectField(field.Id);
        }
    }

    private static void AddBasicStringFields(Entity entity, string culture, string orderBy, QueryUtility q)
    {
        bool bFirstBasic = true;
        bool isEnum = entity.IsEnum;
        foreach (BasicField field in entity.DisplayStrings)
        {
            if (!field.CheckCulture(culture))
            {
                continue;
            }

            if (!string.IsNullOrEmpty(field.OtherFieldThatIgnoreMe))
            {
                bFirstBasic = AddBasicStringField(entity, culture, orderBy, q, field, field.OtherFieldThatIgnoreMe, isEnum, bFirstBasic);
            }

            bFirstBasic = AddBasicStringField(entity, culture, orderBy, q, field, field.FieldId, isEnum, bFirstBasic);
        }
    }

    private static bool AddBasicStringField(Entity entity, string culture, string orderBy,
        QueryUtility q, BasicField field, string fieldId, bool isEnum, bool bFirstBasic)
    {
        EntityField entityField = entity.GetField(fieldId);
        if (entityField == null)
        {
            return bFirstBasic;
        }

        if (entityField.AssociationEntity != null)
        {
            QueryUtility.SubQueryDefinition qRelation = q.LeftOuterJoin(entityField.AssociationEntity.Entity(), null);
            qRelation?.SetMapping(entityField);
            if (qRelation != null)
            {
                foreach (BasicField item in entityField.AssociationEntity.Entity().DisplayStrings)
                {
                    if (!item.CheckCulture(culture))
                    {
                        continue;
                    }

                    EntityField rf = entityField.AssociationEntity.Entity().GetField(item.Key);
                    _ = qRelation.Query.SelectField(item.Key, entityField.Id + "." + item.Key);
                    if (!isEnum && string.IsNullOrWhiteSpace(orderBy) && bFirstBasic)
                    {
                        _ = qRelation.Query.OrderBy(rf?.AssociationEntity?.Maps?[0].SourceField ?? item.Key);
                        bFirstBasic = false;
                    }
                }
            }
        }
        else
        {
            _ = q.SelectField(field.Key);
            if (!isEnum && string.IsNullOrWhiteSpace(orderBy) && bFirstBasic)
            {
                _ = q.OrderBy(field.Key);
                bFirstBasic = false;
            }
        }

        return bFirstBasic;
    }

    private static void AddDisplayFields(string displayFields, string orderBy, QueryUtility queryUtility)
    {
        bool bFirst = true;
        string[] list = displayFields.Split(',');
        foreach (string descriptionField in list)
        {
            _ = queryUtility.SelectField(descriptionField, descriptionField);
            if (string.IsNullOrWhiteSpace(orderBy) && bFirst)
            {
                _ = queryUtility.OrderBy(descriptionField.Split('.')[0]);
            }

            bFirst = false;
        }
    }

    public static void GetComboRecords(ComboData comboData, string culture)
    {
        List<string> recordsById = comboData.RecordsById?
            .Where(r => r.Value == null && !string.IsNullOrEmpty(r.Key))
            .Select(r => r.Key).ToList();
        if (recordsById == null || recordsById.Count == 0)
        {
            return;
        }

        if (comboData.NamespaceId?.StartsWith("Shared") == true && comboData.EntityId?.StartsWith("EntityStateName") == true)
        {
            Entity mainEntity = ProjectDefinition.Project.GetEntity(comboData.NamespaceId.Split('.')[1],
                comboData.EntityId.Split('.')[1]);
            foreach (string recordById in recordsById)
            {
                EntityState state = mainEntity?.GetState(recordById);
                if (state != null)
                {
                    comboData.AddRow(new FormDataRow
                    {
                        Ids = state.Id,
                        DisplayValue = state.Name
                    }, true);
                }
            }

            goto gotoAddInvalidRecords;
        }

        // اگر NamespaceId یا EntityId null باشد (مثل enum)، نمی‌توانیم query بزنیم
        if (string.IsNullOrEmpty(comboData.NamespaceId) || string.IsNullOrEmpty(comboData.EntityId))
        {
            goto gotoAddInvalidRecords;
        }
        
        Entity entity = ProjectDefinition.Project.GetEntity(comboData.NamespaceId, comboData.EntityId);
        EntityField keyField = entity.KeyFields.FirstOrDefault();
        string keyFieldId = keyField?.Id;
        string filter =
            $"{keyFieldId} In ({string.Join(",", keyField?.IsLongParam() ?? false ? recordsById : recordsById.Select(i => $"('{i}')"))})";
        QueryUtility q = new(entity, "ComboDataRoutines.2");
        AddKeyFields(entity, q);
        if (!string.IsNullOrEmpty(comboData.DisplayFields))
        {
            AddDisplayFields(comboData.DisplayFields, "", q);
        }
        else
        {
            AddBasicStringFields(entity, culture, "", q);
        }

        Dictionary<string, EntityState> states = q.Entity.GetStateCollection()?.States;
        if (states?.Count > 0)
        {
            _ = q.SelectField("StateId");
        }

        _ = q.Where(filter);
        AddPropertiesFields(comboData.Properties, q);
        q.ForEach(null, record =>
        {
            FormDataRow dataRow = GetDataRow(culture, comboData.DisplayFields, entity, record, comboData.Properties);
            if (states?.Count > 0 && dataRow != null)
            {
                string stateId = record.GetString("StateId");
                if (!string.IsNullOrEmpty(stateId))
                {
                    EntityState state = states.TryGetValue(stateId, out EntityState value) ? value : null;
                    if ((state?.category & (uint)EntityStateCategory.BackupNode) ==
                         (uint)EntityStateCategory.BackupNode)
                    {
                        dataRow.DisplayValue += " رکورد حذف شده ";
                    }
                }
                else
                {
                    dataRow.DisplayValue += " رکورد بدون وضعیت ";
                }
            }

            comboData.AddRow(dataRow, true);
        });
        q.ReleaseQuery();
    gotoAddInvalidRecords:
        AddInvalidRecords(comboData);
    }

    private static void AddPropertiesFields(List<UIComponentProperty> properties, QueryUtility q)
    {
        if (properties == null)
        {
            return;
        }

        foreach (UIComponentProperty property in properties)
        {
            _ = q.SelectFormulaField(property.PropertyQueryName, $"IF(({property.Value}),1,0)");
        }
    }

    private static void AddInvalidRecords(ComboData comboData)
    {
        foreach (string rowId in comboData.RecordsById?.Where(r => r.Value == null && !string.IsNullOrEmpty(r.Key))
                                         .Select(r => r.Key)
                                         .ToList() ?? Enumerable.Empty<string>())
        {
            if (rowId == "0")
            {
                continue; //todo
            }

            comboData.AddRow(new FormDataRow
            {
                Ids = rowId,
                DisplayValue = $"<<<رکورد نامعتبر {rowId}>>>",
            }, true);
        }
    }

    private static FormDataRow GetDataRow(string culture, string displayFields,
        Entity entity, ElasticObject record, List<UIComponentProperty> propertiesDefinitions)
    {
        List<object> dvs = [];
        List<object> ids = [.. entity.KeyFields.Select(field => record.GetField(field.Id, out object obj) ? obj : null)];
        if (!string.IsNullOrEmpty(displayFields))
        {
            GetDataRowFromDisplayFields(displayFields, entity, record, dvs);
        }
        else
        {
            GetDataRowFromBasicString(culture, entity, record, dvs);
        }

        FormDataRow dataRow = new()
        {
            Ids = string.Join(",", ids),
            DisplayValue = string.Join(" - ", dvs)
        };
        if (propertiesDefinitions != null)
        {
            foreach (UIComponentProperty propertyDefinition in propertiesDefinitions)
            {
                dataRow.AddProperty(propertyDefinition.PropertyId, record[propertyDefinition.PropertyQueryName]);
            }
        }

        return dataRow;
    }

    private static void GetDataRowFromDisplayFields(string displayFields, Entity entity, ElasticObject record,
        List<object> dvs)
    {
        string[] list = displayFields.Split(',');
        foreach (string descriptionField in list)
        {
            if (record.GetField(descriptionField, out object obj))
            {
                string[] associationItems = descriptionField.Split('.');
                Entity associationEntity = entity;
                EntityField entityField = null;
                foreach (string associationItem in associationItems)
                {
                    entityField = associationEntity?.GetField(associationItem);
                    associationEntity = entityField?.AssociationEntity?.Entity();
                }

                if (entityField != null)
                {
                    obj = NormalizeValue(entityField, obj);
                }
            }

            dvs.Add(obj);
        }
    }

    private static void GetDataRowFromBasicString(string culture, Entity entity, ElasticObject record, List<object> dvs)
    {
        foreach (BasicField field in entity.DisplayStrings)
        {
            if (!field.CheckCulture(culture))
            {
                continue;
            }

            object obj;
            if (!string.IsNullOrEmpty(field.OtherFieldThatIgnoreMe))
            {
                if (record.GetField(field.OtherFieldThatIgnoreMe, out obj))
                {
                    continue;
                }
            }
            EntityField entityField = entity.GetField(field.Key);
            if (entityField == null)
            {
                continue;
            }

            if (entityField.AssociationEntity != null)
            {
                foreach (BasicField basicField in entityField.AssociationEntity.Entity().DisplayStrings.Where(b => b.CheckCulture(culture))
                )
                {
                    if (!string.IsNullOrEmpty(basicField.OtherFieldThatIgnoreMe))
                    {
                        if (record.GetField(entityField.Id + "." + basicField.OtherFieldThatIgnoreMe, out obj))
                        {
                            continue;
                        }
                    }
                    if (record.GetField(entityField.Id + "." + basicField.Key, out obj))
                    {
                        EntityField srcField = entityField.AssociationEntity.Entity().GetField(basicField.Key);
                        obj = NormalizeValue(srcField, obj);
                        dvs.Add(obj);
                    }
                    else if (!basicField.IgnoreIfNull)
                    {
                        dvs.Add(null);
                    }
                }
            }
            else
            {
                if (record.GetField(field.Key, out obj))
                {
                    obj = NormalizeValue(entityField, obj);
                    dvs.Add(obj);
                }
                else if (!field.IgnoreIfNull)
                {
                    dvs.Add(null);
                }
            }
        }
    }

    private static object NormalizeValue(EntityField srcField, object obj)
    {
        switch (srcField?.FieldType)
        {
            case TVariableTypes.Date:
            case TVariableTypes.DateTime:
                if (obj is not DateTime)
                {
                    return obj;
                }

                DateTime dt = (DateTime)obj;
                string dStr;
                if (ProjectDefinition.Project.DefaultCalendar == "miladi")
                {
                    dStr = dt.Year.ToString() + '/' + dt.Month.ToString("d2") + '/' + dt.Day.ToString("d2");
                }
                else
                {
                    PersianCalendar pc = new();
                    dStr = pc.GetYear(dt).ToString() + '/' + pc.GetMonth(dt) + '/' +
                             pc.GetDayOfMonth(dt);
                }

                return dStr;
            case TVariableTypes.BOOL:
                if (string.IsNullOrEmpty(srcField.Boolean?.FalseTitle))
                {
                    return obj;
                }

                if (obj == null)
                {
                    return srcField.Boolean.NullTitle;
                }

                if (ConvUtill.ToBoolean(obj))
                {
                    return srcField.Boolean.TrueTitle;
                }

                return srcField.Boolean.FalseTitle;
            default:
                return obj;
        }
    }
}
