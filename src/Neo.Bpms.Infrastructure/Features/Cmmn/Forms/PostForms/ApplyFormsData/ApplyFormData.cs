using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Features.MetaDefinitions.ProjectDefinitions.Extensions;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;
using Neo.Bpms.Domain.Models.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;

public class ApplyFormData(IApplyFormField applyFormField, IApplyFormDocuments applyFormFiles,
    FormStructRoutines formStructRoutines, FormDataRoutines formDataRoutines) 
    : IApplyFormData
{
    public async Task<bool> CreateRecord(AuditTrail auditTrail, string namespaceId, string entityId,
        Form form, ElasticObject record, IList<string> validFieldIds, ExceptionInfos errors,
        ApplyUtility apply = null, LocalParameters localParameters = null, CancellationToken cancellationToken = default)
    {
        Entity entity = form != null ? form.entity : ProjectDefinition.Project.GetEntity(namespaceId, entityId);
        localParameters ??= GetLocalParamValues(auditTrail, namespaceId, entityId, form, record);
        apply ??= new ApplyUtility(entity, auditTrail, localParameters);
        SetParentRecordIdForCreateForm(record, entity, localParameters, apply);
        if (applyFormField.ApplyFormRecord(apply, entity, form, Form.eFormType.Create, record, record, validFieldIds,
            out errors))
        {
            bool dontGiveOutput = true;
            foreach (EntityField key in form.Entity.KeyFields)
            {
                Association associatedField = form.Entity.Associations.FirstOrDefault(
                    a => a.Maps?.FirstOrDefault(m => m.SourceField == key.Id) != null);
                bool find = record.GetField(key.Id, out _);
                if (!find && associatedField != null)
                {
                    find = record.GetField(associatedField.Id, out _);
                }
                if (!find)
                {
                    dontGiveOutput = false;
                    break;
                }
            }
            if (apply.Insert(record, form?.DataOperation,
                GetConformanceAccessType(Form.eFormType.Create), form?.FormSubjectId, dontGiveOutput))
            {
                string ids = GetIds(record, entity);
                _ = record.SetField("Ids", ids);
                await applyFormFiles.ApplyDocuments(entity, form, record, record, ids, cancellationToken);
                return true;
            }

            ExceptionInfos.AppendErrors(ref errors, apply.Errors);
        }
        else if (errors == null || errors.Count == 0)
        {
            AddError(ref errors, "", "Invalid Record");
        }

        return false;
    }

    public async Task<bool> UpdateRecord(AuditTrail auditTrail, string namespaceId, string entityId,
        Form form, ElasticObject record, ElasticObject keyValues, IList<string> validFieldIds,
        ExceptionInfos errors, bool forVirtualDelete = false,
        ApplyUtility apply = null, LocalParameters localParameters = null, CancellationToken cancellationToken = default)
    {
        if (form == null)
        {
            return false;
        }

        UiEntity entity = form.entity;
        localParameters ??= GetLocalParamValues(auditTrail, namespaceId, entityId, form, record);
        apply ??= new ApplyUtility(entity, auditTrail, localParameters);

        apply.Provider.SetRecordConnectionParams(apply.ConnectionValues, keyValues.GetString(form.Entity.KeyFields?.FirstOrDefault()?.Id));
        bool ret = false;
        Form.eFormType formType = Form.IsBulk(form.FormType) ? form.FormType : Form.eFormType.Edit;
        if (applyFormField.ApplyFormRecord(apply, entity, form, formType, record, keyValues, validFieldIds, out errors))
        {
            errors = null;
            if (apply.Update(record, keyValues, form.DataOperation,
                GetConformanceAccessType(Form.eFormType.Edit), form.FormSubjectId, forVirtualDelete, true))
            {
                ret = true;
            }
            else
            {
                ExceptionInfos.AppendErrors(ref errors, apply.Errors);
            }
        }

        if (ret)
        {
            ret = ApplyAssociationEntities(form, record, auditTrail, ref errors, localParameters);
        }

        if (ret)
        {
            DeleteCascadeRuntime deleteCascade = new(keyValues.Id,
                record.GetLong("StateId"), entity, auditTrail);
            await deleteCascade.CheckAndDeleteCascade(this, formStructRoutines, cancellationToken);
            string ids = GetIds(record, entity);
            await applyFormFiles.ApplyDocuments(entity, form, record, keyValues, ids, cancellationToken);
            ExceptionInfos.AppendErrors(ref errors, deleteCascade.Errors);
        }
        return ret;
    }

    public async Task<bool> DeleteRecord(AuditTrail auditTrail, string namespaceId, string entityId,
        Form form, ElasticObject record,
        ExceptionInfos errors, ApplyUtility apply = null, LocalParameters localParameters = null, CancellationToken cancellationToken = default)
    {
        Entity entity = form != null ? form.entity : ProjectDefinition.Project.GetEntity(namespaceId, entityId);
        localParameters ??= GetLocalParamValues(auditTrail, namespaceId, entityId, form, record);
        apply ??= new ApplyUtility(entity, auditTrail, localParameters);
        if (applyFormField.ApplyFormRecord(apply, entity, form, Form.eFormType.Delete, record, record, null, out errors))
        {
            if (!apply.Delete(record, form?.DataOperation,
                GetConformanceAccessType(Form.eFormType.Delete), form?.FormSubjectId))
            {
                ExceptionInfos.AppendErrors(ref errors, apply.Errors);
            }
            else
            {
                DeleteCascadeRuntime deleteCascade = new(record.Id,
                    record.GetLong("StateId"), entity, auditTrail);
                await deleteCascade.DeleteCascade(this, formStructRoutines, cancellationToken);
                string ids = GetIds(record, entity);
                await applyFormFiles.ApplyDocuments(entity, form, record, record, ids, cancellationToken);
                ExceptionInfos.AppendErrors(ref errors, deleteCascade.Errors);
            }
        }

        return false;
    }

    public async Task<bool> Delete(AuditTrail auditTrail, string namespaceId, string entityId,
        Form form, ElasticObject record, ExceptionInfos errors, CancellationToken cancellationToken = default)
    {
        return form.FormType == Form.eFormType.VirtualDelete
            ? await UpdateRecord(auditTrail, namespaceId, entityId, form, record, record, null, errors, true, cancellationToken: cancellationToken)
            : await DeleteRecord(auditTrail, namespaceId, entityId, form, record, errors, cancellationToken: cancellationToken);
    }

    public void ConvertPostedElasticToRecord(ref ElasticObject postedRecord, CommonFormStructure structure)
    {
        static object ConvertPostedTimeSpan(object val)
        {
            return ReformFormData.ReformTimeSpanValueToTicks(val);
        }

        foreach (TableDefinition table in structure.Tables)
        {
            if (table.ControlType == eControlTypeId.MultipleSelectableCombo)
            {
                string keyName = table.GetProperty(eControlPropertyId.MultipleForeignKeyFieldId)?.Value?.ToString();
                List<ElasticObject> idsList = postedRecord.GetString(table.FieldName)
                                      ?.Split(',')
                                      .Select(id =>
                                      {
                                          ElasticObject el = new();
                                          _ = el.SetField(keyName, id);
                                          return el;
                                      })
                                      .ToList() ?? [];
                _ = postedRecord.SetField(table.FieldName, idsList);
            }
            else
            {
                Dictionary<int, ElasticObject> tableListItems = FindTableRecords(postedRecord, table);
                if (tableListItems.Values.Count == 0)
                {
                    continue;
                }

                foreach (ElasticObject item in tableListItems.Values)
                {
                    if (item.GetField("__Deleted", out _))
                    {
                        item.RemoveAttribute("__Deleted");
                    }

                    if (item.GetField("__Ids", out object idsVal))
                    {
                        item.RemoveAttribute("__Ids");
                        _ = item.SetField("Id", idsVal);
                    }

                    IEnumerable<ColumnFieldDefinition> timeSpanFields = table.ColumnInfos?.Where(c =>
                        c.ControlType.In(eControlTypeId.DurationInput, eControlTypeId.None, eControlTypeId.TimeInput)
                        && c.FieldType == TVariableTypes.DayHourMinute);
                    if (timeSpanFields == null)
                    {
                        continue;
                    }

                    foreach (ColumnFieldDefinition column in timeSpanFields)
                    {
                        if (item.GetField(column.ColumnName, out object val))
                        {
                            _ = item.SetField(column.ColumnName, ConvertPostedTimeSpan(val));
                        }
                    }
                }

                _ = postedRecord.SetField(table.FieldName, tableListItems.Values.ToList());
            }
        }
    }

    public async Task<bool> UpdateTables(AuditTrail auditTrail, string namespaceId, string entityId, string formSubjectId,
        ElasticObject record, string ids, string culture,
        List<TableDefinition> tables, ExceptionInfos errors, CancellationToken cancellationToken)
    {
        bool bSave1 = true;
        //Parallel.ForEach(tables, table =>
        foreach (TableDefinition table in tables)
        {
            if (!table.Editable && table.ControlType != eControlTypeId.MultipleSelectableCombo)
            {
                continue;
            }

            Form indexForm = FormStructRoutines.GetForm(table.NamespaceId, table.EntityId, null,
                Form.eFormType.Index, table.FormSubjectId);
            Form createForm = FormStructRoutines.GetForm(table.NamespaceId, table.EntityId, null,
                                      Form.eFormType.Create, table.FormSubjectId) ?? indexForm;
            Form deleteForm = FormStructRoutines.GetForm(table.NamespaceId, table.EntityId, null,
                                      Form.eFormType.Delete, table.FormSubjectId) ?? indexForm;
            ExceptionInfos tablesErrors;
            if (table.ControlType == eControlTypeId.MultipleSelectableCombo)
            {
                (tablesErrors, bSave1) = await UpdateMultipleSelectableCombo(namespaceId, entityId, formSubjectId, record, ids, auditTrail,
                    culture, table, indexForm, createForm, deleteForm, cancellationToken);
            }
            else
            {
                (tablesErrors, bSave1) = await UpdateIndexTable(namespaceId, entityId, formSubjectId, record, ids, auditTrail,
                    culture, table, indexForm, createForm, deleteForm, cancellationToken);
            }

            ExceptionInfos.AppendErrors(ref errors, tablesErrors);
        }
        return bSave1;
    }

    private static bool ApplyAssociationEntities(Form form, ElasticObject record,
        AuditTrail auditTrail, ref ExceptionInfos errors, LocalParameters localParameters)
    {
        Dictionary<string, FormFieldsEntity> formFieldsEntities = [];
        foreach (FormField formField in form.formFields)
        {
            if (formField.FieldOrControlType is not FormField.Type.ColumnField and
                 not FormField.Type.Field)
            {
                continue;
            }

            if (formField.CheckProperty(eControlPropertyId.ReadOnly))
            {
                continue;
            }

            string[] fieldIds = formField.Id.Split('.');
            if (fieldIds.Length <= 1)
            {
                continue;
            }

            string key = string.Join(".", fieldIds.Take(fieldIds.Length - 1));
            if (!formFieldsEntities.TryGetValue(key, out FormFieldsEntity value))
            {
                FormFieldsEntity formFieldsEntity = new();
                EntityField field = formField.Field;
                for (int i = 1; i < fieldIds.Length - 1; i++)
                {
                    field = field?.AssociationEntity?.Entity()?.GetField(fieldIds[i]);
                }

                Entity applyEntity = field?.AssociationEntity?.Entity();
                if (applyEntity == null)
                {
                    continue;
                }

                formFieldsEntity.Field = field;
                formFieldsEntity.UpdateEntity = applyEntity;
                value = formFieldsEntity;
                formFieldsEntities.Add(key, value);
            }

            value.FormFields.Add(formField);
        }

        if (formFieldsEntities.Count == 0)
        {
            return true;
        }

        string recordId = record["ids"]?.ToString();
        if (string.IsNullOrEmpty(recordId))
        {
            recordId = record[form.entity.KeyFields.FirstOrDefault()?.Id ?? "Id"]?.ToString();
        }

        QueryUtility q = new QueryUtility(form.entity, "ApplyFormData.1").AddPkFilter(recordId);
        foreach (KeyValuePair<string, FormFieldsEntity> formFieldsEntity in formFieldsEntities)
        {
            _ = q.SelectField(formFieldsEntity.Key);
        }

        ElasticObject queryRecord = q.FirstOrDefault();
        if (queryRecord == null)
        {
            return false;
        }

        foreach (KeyValuePair<string, FormFieldsEntity> formFieldsEntity in formFieldsEntities)
        {
            string sourceId = formFieldsEntity.Value.Field?.AssociationEntity?.Maps?.FirstOrDefault()?.SourceField;
            string[] overFieldIds = formFieldsEntity.Key.Split('.');
            string fieldId = overFieldIds.Length > 1
                ? string.Join(".", overFieldIds.Take(overFieldIds.Length - 1)) + "." + sourceId
                : sourceId;
            string applyRecordId = queryRecord.GetString(fieldId);
            if (string.IsNullOrEmpty(applyRecordId))
            {
                continue;
            }

            ElasticObject applyRecord = new();
            EntityField keyField = formFieldsEntity.Value.UpdateEntity.KeyFields.FirstOrDefault();
            if (keyField == null)
            {
                continue;
            }

            _ = applyRecord.SetField("ids", applyRecordId);
            _ = applyRecord.SetField(keyField.Id, applyRecordId);
            foreach (FormField formField in formFieldsEntity.Value.FormFields)
            {
                if (!record.GetField(formField.Id, out object v))
                {
                    continue; //need continue;
                }

                EntityField field = formField.Field.GetDotAssociatedFieldWithCompleteId(formField.Id.Split('.'), out _);
                if (field.AssociationEntity == null)
                {
                    _ = applyRecord.SetField(field.Id, v);
                }
                else if (field.AssociationEntity.Maps != null)
                {
                    if (field.AssociationEntity.Maps.Count == 1)
                    {
                        _ = applyRecord.SetField(field.AssociationEntity.Maps[0].SourceField, v);
                    }
                    else
                    {
                        string[] vItems = v?.ToString().Split('#');
                        for (int i = 0; i < field.AssociationEntity.Maps.Count; i++)
                        {
                            _ = applyRecord.SetField(field.AssociationEntity.Maps[i].SourceField,
                                i < (vItems?.Length ?? 0) ? vItems?[i] : null);
                        }
                    }
                }
            }

            ApplyUtility applyUtility = new(formFieldsEntity.Value.UpdateEntity, auditTrail, localParameters);
            if (!applyUtility.Update(applyRecord, applyRecord, null,
                GetConformanceAccessType(Form.eFormType.Edit), null, true))
            {
                ExceptionInfos.AppendErrors(ref errors, applyUtility.Errors);
                return false;
            }
        }

        return true;
    }

    private async Task<(ExceptionInfos errors, bool bSave)> UpdateMultipleSelectableCombo(string namespaceId, string entityId,
        string formSubjectId, ElasticObject record, string ids, AuditTrail auditTrail,
        string culture,
        TableDefinition table, Form indexForm, Form createForm, Form deleteForm, CancellationToken cancellationToken)
    {
        ExceptionInfos errors = null;
        bool bSave = true;
        List<string> idsList = record.GetString(table.FieldName)?.Split(',').ToList() ?? [];
        LocalParameters localParameters = GetLocalParamValues(auditTrail, table.NamespaceId, table.EntityId, indexForm, record);

        string parentIds = FetchAssociationId(namespaceId, entityId, record, ids, table,
            out string associationNamespaceId, out string associationEntityId);
        _ = localParameters.AddOrUpdate("q", record);

        List<ElasticObject> oldTableRecords = await
            formDataRoutines.GetTableRecords(parentIds, culture, localParameters, table, auditTrail?.User, record,
                false, cancellationToken);
        string multipleForeignKeyFieldId = table.GetProperty(eControlPropertyId.MultipleForeignKeyFieldId)?.Value?.ToString();
        ApplyUtility apply = new(indexForm.entity, auditTrail, localParameters);
        apply.BeginBatch();
        foreach (ElasticObject tableRecord in oldTableRecords)
        {
            ExceptionInfos rowErrors = [];
            string foreignKey = tableRecord.GetString(multipleForeignKeyFieldId);
            if (idsList.Any(id => id == foreignKey))
            {
                continue;
            }

            if (auditTrail != null)
            {
                auditTrail.TraceCode = deleteForm?.Id;
            }

            if (deleteForm?.FormType != Form.eFormType.VirtualDelete)
            {
                if (!await DeleteRecord(auditTrail, table.NamespaceId, table.EntityId, deleteForm, tableRecord, rowErrors, apply, localParameters, cancellationToken))
                {
                    bSave = false;
                }
            }
            else
            {
                if (!await UpdateRecord(auditTrail, table.NamespaceId, table.EntityId, deleteForm, tableRecord, tableRecord, null, rowErrors, false, apply, localParameters, cancellationToken))
                {
                    bSave = false;
                }
            }

            ExceptionInfos.AppendErrors(ref errors, rowErrors);
        }

        foreach (string id in idsList)
        {
            if (string.IsNullOrEmpty(id))
            {
                ExceptionInfos.AppendError(ref errors, new ExceptionInfo(table.Alias, new Exception($"Null {multipleForeignKeyFieldId} in {table.Alias}")));
                continue;
            }
            ElasticObject tableRecord = oldTableRecords.FirstOrDefault(r => r.GetString(multipleForeignKeyFieldId) == id);
            if (tableRecord != null)
            {
                continue;
            }

            tableRecord = new ElasticObject
            {
                [multipleForeignKeyFieldId] = id
            };
            if (auditTrail != null)
            {
                auditTrail.TraceCode = createForm?.Id;
            }

            AddParentRecordIdToRecord(tableRecord, associationNamespaceId, associationEntityId, formSubjectId,
                parentIds, table.TableDef.TableAssociation.Id);
            _ = tableRecord.SetField(table.TableDef.TableAssociationId, parentIds);
            ExceptionInfos rowErrors = [];
            if (!await CreateRecord(auditTrail, table.NamespaceId, table.EntityId,
                createForm, tableRecord, null, rowErrors, apply, localParameters, cancellationToken))
            {
                bSave = false;
            }

            ExceptionInfos.AppendErrors(ref errors, rowErrors);
        }

        apply.EndBatch();
        return (errors , bSave);
    }

    private static LocalParameters GetLocalParamValues(object user, string namespaceId, string entityId,
        Form form, ElasticObject record, ElasticObject record2 = null)
    {
        ArgumentNullException.ThrowIfNull(entityId);
        return FormDataRoutines.GetLocalParamValues(user, namespaceId, entityId, form?.Id, record, record2);
    }

    private async Task<(ExceptionInfos errorsList, bool bSave)> UpdateIndexTable(string namespaceId, string entityId, string formSubjectId,
        ElasticObject record, string ids, AuditTrail auditTrail, string culture,
        TableDefinition table, Form indexForm, Form createForm, Form deleteForm, CancellationToken cancellationToken)
    {
        ExceptionInfos errorsList = null;
        bool bSave = true;
        string parentIds = FetchAssociationId(namespaceId, entityId, record, ids, table,
            out string associationNamespaceId, out string associationEntityId);
        LocalParameters localParameters = GetLocalParamValues(auditTrail, namespaceId, entityId, indexForm, record);
        Dictionary<int, ElasticObject> indexList = FindTableRecords(record, table);
        List<ElasticObject> tableRecords = [];
        ApplyUtility apply = new(indexForm.entity, auditTrail, localParameters);
        apply.BeginBatch();
        foreach (KeyValuePair<int, ElasticObject> tableRecordPair in indexList)
        {
            ElasticObject tableRecord = tableRecordPair.Value;
            string tableRecordIds = "";
            bool bDelete = false;
            if (tableRecord.GetField("__Ids", out object v))
            {
                tableRecordIds = v?.ToString() ?? "";
            }

            if (tableRecord.GetField("__Deleted", out v))
            {
                bDelete = v == null || ConvUtill.ToBoolean(v.ToString());
            }

            _ = tableRecord.SetField("NamespaceId", table.NamespaceId);
            _ = tableRecord.SetField("EntityId", table.EntityId);
            _ = tableRecord.SetField("FormSubjectId", table.FormSubjectId);
            ElasticObject recordIds = null;
            Entity entity = ProjectDefinition.Project.GetEntity(table.NamespaceId, table.EntityId);
            if (!string.IsNullOrEmpty(tableRecordIds))
            {
                recordIds = FormDataRoutines.GetKeyRecord(entity, tableRecordIds);
                if (bDelete)
                {
                    _ = tableRecord.MergeValues(recordIds);
                }
            }

            AddParentRecordIdToRecord(tableRecord, associationNamespaceId, associationEntityId, formSubjectId,
                parentIds, table.TableDef.TableAssociation.Id);
            _ = tableRecord.SetField(table.TableDef.TableAssociationId, parentIds);
            ExceptionInfos rowErrors = [];
            if (bDelete)
            {
                if (auditTrail != null)
                {
                    auditTrail.TraceCode = deleteForm?.Id;
                }

                _ = deleteForm?.FormType != Form.eFormType.VirtualDelete
                    ? await DeleteRecord(auditTrail, table.NamespaceId, table.EntityId, deleteForm, tableRecord, rowErrors, apply, localParameters, cancellationToken)
                    : await UpdateRecord(auditTrail, table.NamespaceId, table.EntityId, deleteForm, tableRecord, recordIds, null, rowErrors, false, apply, localParameters, cancellationToken);
            }
            else if (string.IsNullOrEmpty(tableRecordIds))
            {
                if (auditTrail != null)
                {
                    auditTrail.TraceCode = createForm?.Id;
                }

                if (await CreateRecord(auditTrail, table.NamespaceId, table.EntityId, createForm,
                    tableRecord, null, rowErrors, apply, localParameters, cancellationToken))
                {
                    tableRecord["__Ids"] = string.Join(",", table.KeyFields.Select(tableRecord.GetString));
                    tableRecords.Add(tableRecord);
                    _ = record.SetField($"{table.FieldName}[{tableRecordPair.Key}].__Ids", tableRecord["__Ids"]);
                }
            }
            else
            {
                if (auditTrail != null)
                {
                    auditTrail.TraceCode = indexForm.Id;
                }

                if (await UpdateRecord(auditTrail, table.NamespaceId, table.EntityId, indexForm, tableRecord, recordIds, null, rowErrors, false, apply, localParameters, cancellationToken))
                {
                    tableRecords.Add(tableRecord);
                }
            }

            ExceptionInfos.AppendErrors(ref errorsList, rowErrors);
        }

        apply.EndBatch();
        ExceptionInfos tableErrors = [];
        CommonFormStructure indexStructure = await formStructRoutines.GetIndexStructure(culture, indexForm.NamespaceId, indexForm.EntityId,
            indexForm.FormSubjectId, indexForm.Id, null, indexForm, auditTrail?.User);
        foreach (ElasticObject tableRecord in tableRecords)
        {
            await UpdateTables(auditTrail, table.NamespaceId, table.EntityId, table.FormSubjectId,
                tableRecord, (tableRecord["__Ids"] ?? tableRecord["Ids"])?.ToString(),
                culture, indexStructure.Tables, tableErrors, cancellationToken);
        }
        return (errorsList, bSave);
    }

    public static Dictionary<int, ElasticObject> FindTableRecords(ElasticObject record, TableDefinition table)
    {
        Dictionary<int, ElasticObject> indexList = [];
        string tableRecordName = table.FieldName + "[";
        foreach (KeyValuePair<string, ElasticObject> item in record.Attributes)
        {
            if (item.Key.Length < tableRecordName.Length)
            {
                continue;
            }

            if (item.Key[..tableRecordName.Length] != tableRecordName)
            {
                continue;
            }

            short index = Convert.ToInt16(item.Key[tableRecordName.Length..].Split(']')[0]);
            if (!indexList.ContainsKey(index))
            {
                string newTableRecordName = table.FieldName + "[" + index + "].";
                ElasticObject tableRecord = record.GetSubElasticObject(newTableRecordName);
                if (tableRecord == null)
                {
                    continue; // todo exception //نباید اتفاق بیافتد
                }

                indexList.Add(index, tableRecord);
            }
        }

        return indexList;
    }

    private static string FetchAssociationId(string namespaceId, string entityId,
        ElasticObject record, string ids, TableDefinition table, out string associationNamespaceId,
        out string associationEntityId)
    {
        associationEntityId = entityId;
        associationNamespaceId = namespaceId;
        string parentIds = ids;
        if (string.IsNullOrEmpty(table.TableDef.AssociationId))
        {
            return parentIds;
        }

        parentIds = record[table.TableDef.AssociationId]?.ToString();
        Entity entity = ProjectDefinition.Project.GetEntity(namespaceId, entityId);
        EntityField association = entity?.GetField(table.TableDef.AssociationId);
        associationEntityId = association?.AssociationEntity?.DestEntityId ?? entityId;
        associationNamespaceId = association?.AssociationEntity?.DestNamespaceId ?? namespaceId;
        if (!string.IsNullOrEmpty(parentIds))
        {
            return parentIds;
        }

        ElasticObject formRecord = QueryUtility.New(namespaceId, entityId)
            .SelectField(table.TableDef.AssociationId)
            .Find(Convert.ToInt32(ids));
        parentIds = formRecord?.GetString(table.TableDef.AssociationId + "Id");
        if (string.IsNullOrEmpty(parentIds))
        {
            parentIds = formRecord?.GetString(table.TableDef.AssociationId);
        }

        return parentIds;
    }

    private static UserSecurityAccessFlags GetConformanceAccessType(Form.eFormType eFormType)
    {
        return eFormType switch
        {
            Form.eFormType.Index or Form.eFormType.Report or Form.eFormType.Detail or Form.eFormType.SpecificURLForRecord or Form.eFormType.SpecificURL => UserSecurityAccessFlags.Read,
            Form.eFormType.Create or Form.eFormType.ProcessCreate => UserSecurityAccessFlags.Create,
            Form.eFormType.Edit or Form.eFormType.WorkItem or Form.eFormType.ActiveProcessInstance or Form.eFormType.ProcessInstance or Form.eFormType.ProcessCreateOnExistingRecord => UserSecurityAccessFlags.Update,
            Form.eFormType.Delete or Form.eFormType.VirtualDelete => UserSecurityAccessFlags.Delete,
            _ => UserSecurityAccessFlags.Update,
        };
    }

    private static void SetParentRecordIdForCreateForm(ElasticObject record, Entity entity,
        LocalParameters localParameters, ApplyUtility apply)
    {
        object parentNamespaceId = record["__parentNamespaceId"];
        object parentEntityId = record["__parentEntityId"];
        object parentIds = record["__parentIds"];
        string subTableAssociationFeildId = record["__subTableAssociationFieldId"]?.ToString();
        if (parentNamespaceId == null)
        {
            return;
        }

        UiEntity parentEntity =
            ProjectDefinition.Project.GetUiEntity(parentNamespaceId.ToString(), parentEntityId.ToString());
        if (parentEntity == null)
        {
            return;
        }

        EntityField subTableAssociationFeild = entity.GetField(subTableAssociationFeildId);
        if (subTableAssociationFeild?.AssociationEntity == null)
        {
            return;
        }

        string parentKeys = "";
        if (subTableAssociationFeild.AssociationEntity.Maps?.Count == 1)
        {
            parentKeys = SetParentRecordIdForCreateFormToMapField(record, entity, apply, parentIds, parentKeys,
                subTableAssociationFeild.AssociationEntity.Maps[0].SourceField);
        }
        else
        {
            string[] pIds = parentIds.ToString().Split('#');
            int iIds = 0;
            foreach (EntityRelationMap map in subTableAssociationFeild.AssociationEntity.Maps ?? Enumerable.Empty<EntityRelationMap>())
            {
                if (!string.IsNullOrEmpty(parentKeys))
                {
                    parentKeys += "#";
                }

                parentKeys =
                    SetParentRecordIdForCreateFormToMapField(record, entity, apply, pIds[iIds], parentKeys, map.SourceField);
                iIds++;
                if (iIds >= pIds.Length)
                {
                    break;
                }
            }
        }

        _ = localParameters.AddOrUpdate("__parentKeys", parentKeys);
        _ = localParameters.AddOrUpdate("__parentIds", parentIds.ToString());
    }

    private static string SetParentRecordIdForCreateFormToMapField(ElasticObject record, Entity entity,
        ApplyUtility apply, object parentIds, string parentKeys, string sourceFieldId)
    {
        EntityField associationToParentSourceField = entity.GetField(sourceFieldId);
        if (associationToParentSourceField == null)
        {
            return parentKeys;
        }

        parentKeys += associationToParentSourceField.Id;
        _ = record.SetField(associationToParentSourceField.Id, parentIds);
        _ = apply.AddField(associationToParentSourceField.Id, parentIds);
        return parentKeys;
    }

    private static void AddParentRecordIdToRecord(ElasticObject record, string parentNamespaceId,
        string parentEntityId, string parentFormSubjectId, string parentIds, string subTableAssociationFeildId)
    {
        record["__parentNamespaceId"] = parentNamespaceId;
        record["__parentEntityId"] = parentEntityId;
        record["__parentFormSubjectId"] = parentFormSubjectId;
        record["__parentIds"] = parentIds;
        record["__subTableAssociationFieldId"] = subTableAssociationFeildId;
    }

    internal static void AddError(ref ExceptionInfos errors, string forField, string message)
    {
        errors ??= [];
        errors.Add(new ExceptionInfo
        {
            ForField = forField,
            Exception = new Exception(message)
        });
    }

    private static string GetIds(ElasticObject record, Entity entity)
    {
        List<string> idArray = [.. from keyField in entity.entityFields.Values
                                        where keyField.IncludeInPkv
                                        select record.GetString(keyField.Id)];
        string ids = string.Join(",", idArray);
        return ids;
    }

    private class FormFieldsEntity
    {
        public List<FormField> FormFields { get; } = [];
        public Entity UpdateEntity { get; set; }
        public EntityField Field { get; set; }
    }
}
