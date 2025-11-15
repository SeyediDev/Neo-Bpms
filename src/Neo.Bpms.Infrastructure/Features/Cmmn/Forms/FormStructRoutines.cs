using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Logic;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms;

public class FormStructRoutines(ILogger<FormStructRoutines> logger,
    FilterConfigBackupRestore filterConfigBackupRestore,
    FolderConfigBackupRestore folderConfigBackupRestore,
    IEnrichFieldsSbvr enrichFieldsSbvr,
    IFormLogicHelper formLogicHelper, IConfiguration configuration)
{
    #region Structure Methods

    public static Form GetForm(string namespaceId, string entityId, string formId,
        Form.eFormType? formType = null, string formSubjectId = null)
    {
        UiEntity uiEntity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        Form form = uiEntity?.GetEntityForm(formId, formType, formSubjectId);
        if (form == null && formType == Form.eFormType.Delete && uiEntity != null)
        {
            form = uiEntity.GetEntityForm(formId, Form.eFormType.VirtualDelete, formSubjectId);
        }

        return form;
    }

    public CommonFormStructure GetCommonStructure(string culture, CommonFormStructure structure, Form form,
        IdentityUser user, string tableName, bool isFilter, bool designMode = false)
    {
        UiEntity entity = form?.entity;
        if (entity == null)
        {
            return null;
        }

        structure.NamespaceId = entity.model.Id;
        structure.EntityId = entity.Id;
        structure.Name = culture == "en" ? form.EnName : form.Name;
        structure.Form_ReportId = form.Id;
        structure.FormType = form.FormType;
        //structure.FormSubjectId = formSubjectId;
        foreach (FormField formField in form.formFields)
        {
            switch (formField.FieldOrControlType)
            {
                case FormField.Type.Field:
                case FormField.Type.FilterField:
                case FormField.Type.Control:
                    GetCommonStructureField(structure, user, formField, culture, isFilter);
                    break;
                case FormField.Type.ColumnField:
                    GetCommonStructureColumnField(structure, user, entity, formField, culture);
                    break;
                case FormField.Type.SubjectField:
                    GetCommonStructureSubject(structure, form, user, formField, culture);
                    break;
                case FormField.Type.SubTable:
                    GetCommonStructureSubTable(structure, form.FormType, user, formField, culture);
                    break;
            }
        }

        foreach (FormField formField in form.formFields)
        {
            switch (formField.FieldOrControlType)
            {
                case FormField.Type.TooltipField:
                case FormField.Type.IconField:
                    GetCommonStructureIconOrTooltipField(structure, formField);
                    break;
            }
        }

        foreach (EntityField field in entity.KeyFields ?? [])
        {
            structure.KeyFields.Add(field.Id);
        }

        if (!designMode)
        {
            formLogicHelper.AddFormLogic(structure, form.UiRules, tableName);

            // Enrich fields with UI Rules as SBVR
            enrichFieldsSbvr.EnrichFieldsWithUIRules(structure);
        }

        return structure;
    }

    private static void GetCommonStructureIconOrTooltipField(CommonFormStructure structure, FormField formField)
    {
        if (formField.Field == null)
        {
            return;
        }

        FormFieldDefinition fd = new(formField.Name);
        FillProperties(fd, formField, formField.Field);
        UIComponentProperty prop = fd.GetProperty(eControlPropertyId.NoEditIcon);
        if (prop == null)
        {
            return;
        }

        string forFieldId = prop.Value?.ToString();
        InputFieldDefinition fld = structure.Fields.FirstOrDefault(f => f.FieldName == forFieldId);
        if (fld != null)
        {
            if (formField.FieldOrControlType == FormField.Type.TooltipField)
            {
                fld.TooltipField = formField.Field.Id;
            }
            else
            {
                fld.IconField = formField.Field.Id;
            }
        }

        ColumnFieldDefinition col = structure.ColumnInfos.FirstOrDefault(f => f.ColumnName == forFieldId);
        if (col != null)
        {
            if (formField.FieldOrControlType == FormField.Type.TooltipField)
            {
                col.TooltipField = formField.Field.Id;
            }
            else
            {
                col.IconField = formField.Field.Id;
            }
        }

        IndexFormSubjectId subj = structure.Subjects.FirstOrDefault(f => f.Name == forFieldId);
        if (subj != null)
        {
            if (formField.FieldOrControlType == FormField.Type.TooltipField)
            {
                subj.TooltipField = formField.Field.Id;
            }
            else
            {
                subj.IconField = formField.Field.Id;
            }
        }
    }

    private void GetCommonStructureSubTable(CommonFormStructure structure, Form.eFormType formType,
        IdentityUser user, FormField formField, string culture)
    {
        ReformFieldDefinition(formField, culture, true, out EntityField field, out string label);
        if ((formField.TableEntity == null || formField.TableAssociation == null)
        && (field == null || field.FieldType != TVariableTypes.List))
        {
            return;
        }

        TableDefinition table = new(label)
        {
            ControlType = formField.ControlTypeId,
            parentControlId = formField.ParentControlId,
            FormFieldType = FormField.Type.SubTable,
            FieldName = formField.Id,
            TableDef = formField,
            FieldType = TVariableTypes.Table
        };
        FillProperties(table, formField, field);
        ForceRemoteDataTemporarily(table, formField);

        if (formType is Form.eFormType.Create or Form.eFormType.ProcessCreate or
            Form.eFormType.ProcessCreateOnExistingRecord)
        {
            table.SetProperty(eControlPropertyId.Editable, true);
        }

        string labelName = label ?? table.PropertyValue(eControlPropertyId.LabelName);
        if (!string.IsNullOrEmpty(labelName))
        {
            table.Alias = labelName;
        }

        if (string.IsNullOrEmpty(table.Label))
        {
            table.Alias = formField.TableEntity.EnName;
        }

        string formSubjectId = table.FormSubjectId;
        const Form.eFormType tableFormType = Form.eFormType.Index;
        Form tableForm = formField.TableEntity.getForm(null, tableFormType, formSubjectId);
        if (tableForm == null)
        {
            logger.LogError("can not find index form {0} of entity {1}", formSubjectId, formField.TableEntity.Id);
            return;
        }

        CommonFormStructure formStructure = GetCommonFormStructure(culture, formField.TableEntity.model.Id,
            formField.TableEntity.Id,
            tableForm.Id, tableFormType, formSubjectId, tableForm, user, formField.Name);
        if (formStructure == null)
        {
            return;
        }

        SetReferEntityProperties(user, formField, table, formSubjectId, formField.TableEntity);
        table.AddProperty(eControlPropertyId.EntityItemId, tableForm.Id);
        table.AddProperty(eControlPropertyId.Subject, tableForm.FormSubjectId);
        table.KeyFields = formStructure.KeyFields;
        table.Columns.AddRange(formStructure.Tables);
        table.Columns.AddRange(formStructure.ColumnInfos);
        table.Subjects = formStructure.Subjects;
        table.Logic = formStructure.Logic;
        table.CombosData = formStructure.CombosData;
        if (formField.ControlTypeId == eControlTypeId.MultipleSelectableCombo)
        {
            string multipleForeignKeyFieldId =
                table.GetProperty(eControlPropertyId.MultipleForeignKeyFieldId)?.Value?.ToString();
            UiEntity multipleForeignKeyEntity = ProjectDefinition.Project.GetUiEntity(table.NamespaceId, table.EntityId);
            EntityField multipleForeignKeyField = multipleForeignKeyEntity?.GetField(multipleForeignKeyFieldId);
            table.MultipleForeignKeyField = new InputFieldDefinition(multipleForeignKeyField?.Name)
            {
                FormFieldType = FormField.Type.Field,
                ControlType = eControlTypeId.ComboBox,
                FieldName = multipleForeignKeyField?.Id
            };
            SetReferEntityProperties(user, null, table.MultipleForeignKeyField, formSubjectId,
                multipleForeignKeyField?.AssociationEntity?.Entity() as UiEntity);
        }

        structure.Fields.Add(table);
    }

    private void GetCommonStructureSubject(CommonFormStructure structure, IEntityPage form,
        IdentityUser user, FormField formField, string culture)
    {
        if (structure.FormType != Form.eFormType.Index)
        {
            return;
        }

        ReformFieldDefinition(formField, culture, false, out EntityField field, out string label);
        string fieldSubjectId = formField.GetProperty(eControlPropertyId.Subject)?.value?.ToString() ?? formField.Id;
        Form detailForm = FetchAccessedForm(form, user, Form.eFormType.Detail, fieldSubjectId);
        Form editForm = FetchAccessedForm(form, user, Form.eFormType.Edit, fieldSubjectId);
        IndexFormSubjectId ifs = new(label)
        {
            Name = formField.Id,
            DetailFormId = detailForm?.Id,
            DetailAction = ResolveFormAction(detailForm, "Details"),
            EditFormId = editForm?.Id,
            EditAction = ResolveFormAction(editForm, "Edit"),
            HasText = field != null
        };
        FillProperties(ifs, formField, field);
        if (ifs.PropertyBoolean(eControlPropertyId.NoEditIcon))
        {
            ifs.EditFormId = null;
        }

        if (ifs.PropertyBoolean(eControlPropertyId.NoDetailsIcon))
        {
            ifs.DetailFormId = null;
        }

        structure.Subjects.Add(ifs);
    }

    private void GetCommonStructureColumnField(CommonFormStructure structure, IdentityUser user,
        UiEntity entity, FormField formField, string culture)
    {
        if (structure.FormType is not Form.eFormType.Index and not Form.eFormType.Report)
        {
            return;
        }

        ReformFieldDefinition(formField, culture, false, out EntityField field, out string label);
        if (field == null)
        {
            return;
        }

        ColumnFieldDefinition ifc = new(label)
        {
            entityId = entity.Id,
            AssociationName = formField.TableAssociation?.Id,
            ColumnName = formField.Id,
            ControlType = formField.ControlTypeId,
            FieldName = formField.Id,
            Descending = false,
            FieldType = field.FieldType
        };
        FillProperties(ifc, formField, field);
        SetReferEntityPropertiesToFormField(user, formField, ifc);
        structure.ColumnInfos.Add(ifc);
    }

    private void GetCommonStructureField(CommonFormStructure structure, IdentityUser user,
        FormField formField, string culture, bool isFilter)
    {
        InputFieldDefinition iff = NewInputFieldDefinition(structure, formField, culture, isFilter);
        FillProperties(iff, formField, formField.Field);
        ForceRemoteDataTemporarily(iff, formField);
        bool isMandatory = iff.PropertyBoolean(eControlPropertyId.Required);
        if (isMandatory && formField.FieldOrControlType == FormField.Type.FilterField)
        {
            structure.hasMandatoryFilter = true;
        }

        SetReferEntityPropertiesToFormField(user, formField, iff);
    }

    private void ForceRemoteDataTemporarily(InputFieldDefinition ifd, FormField formField)
    {
        string allRemoteDataConfig = configuration["AllRemoteData"];
        _ = bool.TryParse(allRemoteDataConfig, out bool shouldForceRemoteData);
        if (!shouldForceRemoteData)
        {
            return;
        }

        if (formField?.Field?.AssociationEntity?.Entity() != null &&
            formField.ControlTypeId.In(eControlTypeId.ComboBox, eControlTypeId.None))
        {
            if (formField.Field.AssociationEntity.Entity().IsEntityState
                || formField.Field.AssociationEntity.Entity().UseEnumData
                || formField.GetProperty(eControlPropertyId.OnDemand) != null
                || formField.GetProperty(eControlPropertyId.RemoteData) != null)
            {
                return;
            }

            ifd.AddProperty(eControlPropertyId.RemoteData, true);
        }

        if (formField?.ControlTypeId == eControlTypeId.MultipleSelectableCombo)
        {
            if (formField.GetProperty(eControlPropertyId.OnDemand) != null
                || formField.GetProperty(eControlPropertyId.RemoteData) != null)
            {
                return;
            }

            ifd.AddProperty(eControlPropertyId.RemoteData, true);
        }
    }

    private void SetReferEntityPropertiesToFormField(IdentityUser user,
        FormField formField, InputFieldDefinition iff)
    {
        string[] fieldIds = formField.Id.Split('.');
        EntityField field = formField.Field;
        if (fieldIds.Length > 1)
        {
            for (int ii = 1; ii < fieldIds.Length; ii++)
            {
                field = field?.AssociationEntity?.Entity()?.GetField(fieldIds[ii]);
            }

            FillProperties(iff, null, field);
        }

        if (field?.AssociationEntity?.Entity() != null)
        {
            SetReferEntityProperties(user, formField, iff, iff.FormSubjectId,
                field.AssociationEntity.Entity() as UiEntity);
        }
    }

    private void SetReferEntityProperties(IdentityUser user, FormField formField,
        FormFieldDefinition fieldDefinition, string formSubjectId, UiEntity entity)
    {
        string referNamespaceId = entity.model.Id;
        string referEntityId = entity.Id;
        fieldDefinition.AddProperty(eControlPropertyId.NamespaceId, referNamespaceId);
        fieldDefinition.AddProperty(eControlPropertyId.EntityId, referEntityId);

        Form createForm = FetchAccessedForm(entity, user, Form.eFormType.Create, formSubjectId);
        fieldDefinition.CreateFormId = createForm?.Id;

        Form editForm = FetchAccessedForm(entity, user, Form.eFormType.Edit, formSubjectId);
        fieldDefinition.EditFormId = editForm?.Id;
        fieldDefinition.EditAction = ResolveFormAction(editForm, "Edit");

        Form deleteForm = FetchAccessedForm(entity, user, Form.eFormType.Delete, formSubjectId);
        fieldDefinition.DeleteFormId = deleteForm?.Id;

        Form detailForm = FetchAccessedForm(entity, user, Form.eFormType.Detail, formSubjectId);
        fieldDefinition.DetailFormId = detailForm?.Id;
        fieldDefinition.DetailAction = ResolveFormAction(detailForm, "Details");

        Form indexForm = FetchAccessedForm(entity, user, Form.eFormType.Index, formSubjectId);
        fieldDefinition.IndexFormId = indexForm?.Id;
        if (formField?.CheckProperty(eControlPropertyId.ReadOnly) == true)
        {
            fieldDefinition.CreateFormId = fieldDefinition.DeleteFormId = fieldDefinition.EditFormId = null;
        }

        if (formField?.CheckProperty(eControlPropertyId.NoEditIcon) == true)
        {
            fieldDefinition.EditFormId = null;
        }

        if (formField?.CheckProperty(eControlPropertyId.NoDetailsIcon) == true)
        {
            fieldDefinition.DetailFormId = null;
        }
    }

    public async Task<CommonFormStructure> GetIndexStructure(string culture,
        string namespaceId, string entityId, string formSubjectId, string formId,
        string sortFieldList, Form form, IdentityUser user)
    {
        CommonFormStructure structure = GetCommonFormStructure(culture, namespaceId, entityId, formId, Form.eFormType.Index,
            formSubjectId, form, user);
        if (structure == null)
        {
            return null;
        }

        if (form != null)
        {
            structure.PassingParameters = form.PassingParameters;
            List<Form> forms = form.entity.getForms().ToList();
            structure.BulkEdits =
                [.. forms.Where(f => f.FormType == Form.eFormType.BulkEdit &&
                                 CompareValue(f.FormSubjectId, structure.FormSubjectId)).Select(f =>
                    new FormLinkId(f.NamespaceId, f.EntityId, f.Id, f.Name))];
            SetBulkProcessLinks(structure, form);
            Form entityForm = form;
            List<ConfiguredFilter> configuredFilters = await filterConfigBackupRestore.Configurations("Form", namespaceId, entityId, form.Id);
            structure.ConfiguredFilters = configuredFilters?.Where(cfg =>
                cfg.CheckAccess(user)).ToList();
            List<ConfiguredFolder> configuredFolders = await folderConfigBackupRestore.Configurations("Form", namespaceId, entityId, form.Id);
            structure.ConfiguredFolders = configuredFolders?.Where(cfg =>
                cfg.CheckAccess(user)).ToList();

            structure.UserGroups = [.. user.Roles.Values.Where(role => form.CheckRole(role.Code))];
        }

        string[] sortFields = string.IsNullOrEmpty(sortFieldList) ? null : sortFieldList.Split('#');

        if (sortFields != null)
        {
            int sortOrderIndex = 1;
            foreach (string sortField in sortFields)
            {
                ColumnFieldDefinition col = structure.ColumnInfos.FirstOrDefault(c => c.ColumnName == sortField.Split(' ')[0]);
                if (col != null)
                {
                    col.SortOrder = sortOrderIndex;
                    string[] t = sortField.Split(' ');
                    if (t.Length > 1 && t[1].ToUpper() == "DESC")
                    {
                        col.Descending = true;
                    }
                }

                sortOrderIndex++;
            }
        }

        Form createForm = FetchAccessedForm(form, user, Form.eFormType.Create);
        structure.CreateFormId = createForm?.Id;

        Form deleteForm = FetchAccessedForm(form, user, Form.eFormType.Delete);
        structure.DeleteFormId = deleteForm?.Id;

        Form editForm = FetchAccessedForm(form, user, Form.eFormType.Edit);
        structure.EditFormId = editForm?.Id;
        structure.EditAction = ResolveFormAction(editForm, "Edit");

        Form detailForm = FetchAccessedForm(form, user, Form.eFormType.Detail);
        structure.DetailFormId = detailForm?.Id;
        structure.DetailAction = ResolveFormAction(detailForm, "Details");
        structure.HasServiceOperation = !string.IsNullOrEmpty(form?.GetServiceOperation) ||
                                        !string.IsNullOrEmpty(form?.ApplyServiceOperation);
        structure.HasTemplate = form?.HasTemplateFile ?? false;
        structure.FormSortType = form?.SortType ?? FormSortType.PassToProvider;
        return structure;
    }

    private Form FetchAccessedForm(Form form, IdentityUser user, Form.eFormType formType)
    {
        return FetchAccessedForm(form?.Entity, user, formType, form?.FormSubjectId);
    }

    private Form FetchAccessedForm(IEntityPage form, IdentityUser user, Form.eFormType formType,
        string formSubjectId)
    {
        return FetchAccessedForm(form?.Entity, user, formType, formSubjectId);
    }

    private static Form FetchAccessedForm(UiEntity entity, IdentityUser user,
        Form.eFormType formType, string formSubjectId)
    {
        if (entity == null)
        {
            return null;
        }

        Form linkForm = entity.GetEntityForm(null, formType, formSubjectId);
        if (linkForm == null && formType == Form.eFormType.Delete)
        {
            linkForm = entity.GetEntityForm(null, Form.eFormType.VirtualDelete, formSubjectId);
        }

        if (linkForm == null)
        {
            foreach (Form.eFormType alternative in GetAlternativeFormTypes(formType))
            {
                linkForm = entity.GetEntityForm(null, alternative, formSubjectId);
                if (linkForm != null)
                {
                    break;
                }
            }
        }

        if (linkForm == null)
        {
            return null;
        }

        bool hasAccess = (user?.CheckFormAccess(entity.NamespaceId, entity.Id, linkForm.FormType,
            formSubjectId, linkForm.Id, linkForm) ?? false) || linkForm.AllowAnonymous;
        return hasAccess ? linkForm : null;
    }

    private string FetchAccessedFormId(Form form, IdentityUser user, Form.eFormType formType)
    {
        return FetchAccessedForm(form, user, formType)?.Id;
    }

    private string FetchAccessedFormId(IEntityPage form, IdentityUser user, Form.eFormType formType,
        string formSubjectId)
    {
        return FetchAccessedForm(form, user, formType, formSubjectId)?.Id;
    }

    private static string FetchAccessedFormId(UiEntity entity, IdentityUser user,
        Form.eFormType formType, string formSubjectId)
    {
        return FetchAccessedForm(entity, user, formType, formSubjectId)?.Id;
    }

    private static IEnumerable<Form.eFormType> GetAlternativeFormTypes(Form.eFormType formType)
    {
        switch (formType)
        {
            case Form.eFormType.Edit:
                yield return Form.eFormType.SpecificURL;
                yield return Form.eFormType.SpecificURLForRecord;
                break;
            case Form.eFormType.Detail:
                yield return Form.eFormType.SpecificURLForRecord;
                break;
        }
    }

    private static string ResolveFormAction(Form linkForm, string defaultAction)
    {
        if (linkForm == null)
        {
            return defaultAction;
        }

        return linkForm.FormType switch
        {
            Form.eFormType.Edit => "Edit",
            Form.eFormType.Detail => "Details",
            Form.eFormType.Delete => "Delete",
            Form.eFormType.Create => "Create",
            Form.eFormType.SpecificURL => "SpecificURL",
            Form.eFormType.SpecificURLForRecord => "SpecificURLForRecord",
            _ => defaultAction
        };
    }

    private static void SetBulkProcessLinks(CommonFormStructure structure, Form form)
    {
        structure.BulkProcessCreates = [];
        foreach (BusinessProcess businessProcess in ProjectDefinition.Project.BusinessProcesses.Values)
        {
            foreach (BusinessProcessVersion businessProcessVersion in businessProcess.Versions.Values.Where(v => v.IsActive))
            {
                if (businessProcessVersion.BpmnDefinitions.GetRootElement(businessProcess.Id) is not Process
                    processDefinition)
                {
                    continue;
                }

                foreach (FlowElement flowElement in processDefinition.flowElements.Values)
                {
                    if (flowElement is not UserTask userTask)
                    {
                        continue;
                    }

                    UiEntity processEntity =
                        ProjectDefinition.Project.GetUiEntity(processDefinition.EntityNamespaceId,
                            processDefinition.EntityId);
                    Form userTaskForm = processEntity?.getForm(userTask.FormId);
                    if (userTaskForm == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(userTask.IndexFormId))
                    {
                        if (!CompareValue(userTask.IndexFormId, form.Id))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!form.EntityId.Equals(processDefinition.EntityId) ||
                            !form.NamespaceId.Equals(processDefinition.EntityNamespaceId))
                        {
                            continue;
                        }

                        if (userTaskForm.FormType != Form.eFormType.ProcessCreateOnExistingRecord)
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(userTaskForm.FormSubjectId) ||
                            !CompareValue(userTaskForm.FormSubjectId, form.FormSubjectId))
                        {
                            continue;
                        }
                    }

                    structure.BulkProcessCreates.Add(new ProcessCreateFormLinkId(
                        processEntity.NamespaceId, processEntity.Id,
                        userTaskForm.Id, userTask.Name)
                    {
                        FormAlias = userTaskForm.Name,
                        ProcessId = businessProcess.Id,
                        ProcessVersion = businessProcessVersion.Id,
                        TaskId = userTask.Id,
                        AssociationFieldId = userTask.AssociationFieldId,
                    });
                }
            }
        }
    }

    private static bool CompareValue(object value1, object value2)
    {
        return (value1?.ToString() ?? "") == (value2?.ToString() ?? "");
    }

    public CommonFormStructure GetDetailsStructure(string culture, string namespaceId, string entityId,
        string formSubjectId, string formId, Form form, IdentityUser user)
    {
        CommonFormStructure structure = GetCommonFormStructure(culture, namespaceId, entityId, formId, Form.eFormType.Detail,
            formSubjectId, form, user);
        return structure;
    }

    public CommonFormStructure GetCreateStructure(string culture, string namespaceId, string entityId,
        string formSubjectId, string formId, Form form, IdentityUser user)
    {
        CommonFormStructure structure = GetCommonFormStructure(culture, namespaceId, entityId, formId, Form.eFormType.Create,
            formSubjectId, form, user);
        return structure;
    }

    public CommonFormStructure GetEditStructure(string culture, string namespaceId, string entityId,
        string formSubjectId, string formId, Form form, IdentityUser user)
    {
        CommonFormStructure structure = GetCommonFormStructure(culture, namespaceId, entityId, formId, Form.eFormType.Edit,
            formSubjectId, form, user);
        return structure;
    }

    public CommonFormStructure GetBulkEditStructure(string culture, string namespaceId, string entityId,
        string formSubjectId, string formId, Form form, IdentityUser user)
    {
        CommonFormStructure structure = GetCommonFormStructure(culture, namespaceId, entityId, formId, Form.eFormType.BulkEdit,
            formSubjectId, form, user);
        return structure;
    }

    public CommonFormStructure GetCreateProcessStructure(string culture, string namespaceId, string entityId,
        string formSubjectId, string formId, Form form, IdentityUser user)
    {
        CommonFormStructure structure = GetCommonFormStructure(culture, namespaceId, entityId, formId,
            Form.eFormType.ProcessCreateOnExistingRecord,
            formSubjectId, form, user);
        return structure;
    }

    public CommonFormStructure GetDeleteStructure(string culture, string namespaceId, string entityId,
        string formSubjectId, string formId, Form form, IdentityUser user)
    {
        CommonFormStructure structure =
            GetCommonFormStructure(culture, namespaceId, entityId, formId,
                form?.FormType ?? Form.eFormType.VirtualDelete,
                formSubjectId, form, user) ??
            GetCommonFormStructure(culture, namespaceId, entityId, formId, form?.FormType ?? Form.eFormType.Delete,
                null, form,
                user);
        return structure;
    }

    #endregion Structure Methods


    #region input field

    public static eControlTypeId GetDefaultControlTypeId(EntityField field, bool isFilter)
    {
        switch (field.FieldType)
        {
            case TVariableTypes.invalid:
                break;
            case TVariableTypes.String:
            case TVariableTypes.ByteArray:
            case TVariableTypes.UChar:
            case TVariableTypes.Char:
                return eControlTypeId.TextInput;
            case TVariableTypes.Short:
            case TVariableTypes.Int:
            case TVariableTypes.Long:
            case TVariableTypes.Decimal:
            case TVariableTypes.UShort:
            case TVariableTypes.ULong:
            case TVariableTypes.Double:
                return eControlTypeId.NumberInput;
            case TVariableTypes.DoubleMinuteSecond:
            case TVariableTypes.HourMinute:
            case TVariableTypes.DayHourMinute:
            case TVariableTypes.DurHourMinute:
                return eControlTypeId.TimeInput;
            // case TVariableTypes.varTable: todo
            case TVariableTypes.DateTime:
            case TVariableTypes.Date:
            case TVariableTypes.DateStr:
                return eControlTypeId.DatePicker;
            case TVariableTypes.StringListItem:
            case TVariableTypes.Association:
                return eControlTypeId.ComboBox;
            case TVariableTypes.StringListBitMask:
                return eControlTypeId.CheckBoxList;
            case TVariableTypes.BOOL:
                return isFilter ? field.Boolean?.FalseTitle == null
                        ? eControlTypeId.BooleanRadioButtons
                        : eControlTypeId.BooleanCombo
                    : string.IsNullOrEmpty(field.Boolean?.FalseTitle) ? eControlTypeId.CheckBox
                    : eControlTypeId.Toggle;
            case TVariableTypes.Link:
                return eControlTypeId.SystemPageLink;
            case TVariableTypes.List:
                return eControlTypeId.IndexTable;
            case TVariableTypes.File:
                return eControlTypeId.File;
        }

        return eControlTypeId.TextInput;
    }

    private InputFieldDefinition NewInputFieldDefinition(FormField formField, string culture, bool isFilter)
    {
        eControlTypeId eControlType = formField.ControlTypeId;
        ReformFieldDefinition(formField, culture, false, out EntityField field, out string label);
        InputFieldDefinition ifd = new(label)
        {
            parentControlId = formField.ParentControlId,
            FormFieldType = formField.FieldOrControlType,
            FieldName = formField.Id ?? ""
        };
        if (field != null)
        {
            enrichFieldsSbvr.EnrichSbvrWithAutoRules(ifd, field, formField);
            
            if (field.AssociationEntity != null)
            {
                if (field.AssociationEntity.CheckFlag(EntityFieldFlags.IsBitMask))
                {
                    eControlType = eControlTypeId.CheckBoxList;
                }
                else if (eControlType == eControlTypeId.None)
                {
                    eControlType = eControlTypeId.ComboBox;
                }
            }

            if (eControlType == eControlTypeId.None)
            {
                eControlType = GetDefaultControlTypeId(field, isFilter);
            }
        }
        else
        {
            ifd.FieldName = formField.ControlId;
        }

        ifd.ControlType = eControlType;
        return ifd;
    }

    public static void ReformFieldDefinition(FormField formField, string culture, bool forSubTable,
        out EntityField field,
        out string label)
    {
        string[] fieldIds = formField.Id.Split('.');
        field = formField.Field;
        label = culture == "en"
            ? formField.Field?.EnName ??
              formField.Field?.Id ?? (forSubTable ? formField.TableEntity.EnName : formField.Name)
            : formField.Name ?? "";
        if (fieldIds.Length > 1)
        {
            for (int ii = 1; ii < fieldIds.Length; ii++)
            {
                field = field?.AssociationEntity?.Entity()?.GetField(fieldIds[ii]);
                label += "-" + (culture == "en" ? field?.EnName : field?.Name);
            }
        }

        FormProperty labelProp = culture == "en"
            ? formField.GetProperty(eControlPropertyId.EnLabelName)
            : formField.GetProperty(eControlPropertyId.LabelName);
        if (labelProp?.value != null)
        {
            label = labelProp.value.ToString();
        }
    }

    public InputFieldDefinition NewInputFieldDefinition(EntityField field, eControlTypeId eControlType,
        FormField.Type formFieldType, bool isFilter)
    {
        InputFieldDefinition ifd = new(field?.Name ?? "")
        {
            parentControlId = null,
            FormFieldType = formFieldType
        };
        if (field != null)
        {
            enrichFieldsSbvr.EnrichSbvrWithAutoRules(ifd, field, null);

            ifd.FieldName = field.Id ?? "";
            if (field.AssociationEntity != null)
            {
                if (field.AssociationEntity.CheckFlag(EntityFieldFlags.IsBitMask))
                {
                    eControlType = eControlTypeId.CheckBoxList;
                }
                else if (eControlType == eControlTypeId.None)
                {
                    eControlType = eControlTypeId.ComboBox;
                }
            }

            if (eControlType == eControlTypeId.None)
            {
                eControlType = GetDefaultControlTypeId(field, isFilter);
            }
        }
        else
        {
            ifd.FieldName = eControlType.ToString();
            //ifd.Label = field.name;
        }

        ifd.ControlType = eControlType;
        return ifd;
    }

    public InputFieldDefinition NewInputFieldDefinition(CommonFormStructure structure,
        FormField field, string culture, bool isFilter)
    {
        InputFieldDefinition ifd = NewInputFieldDefinition(field, culture, isFilter);
        structure.Fields.Add(ifd);
        return ifd;
    }

    private static void FillProperties(FormFieldDefinition iff,
        FormField formField, EntityField entityField)
    {
        if (entityField?.Properties != null)
        {
            foreach (FieldProperty prop in entityField.Properties)
            {
                switch (prop.PropertyId)
                {
                    case EntityFieldPropertyId.DisplayFields:
                        iff.AddProperty(eControlPropertyId.DisplayFields, prop.Value);
                        break;
                    case EntityFieldPropertyId.Multiple:
                        iff.AddProperty(eControlPropertyId.IsMultiple, prop.Value);
                        break;
                    case EntityFieldPropertyId.UseFileServer:
                        iff.AddProperty(eControlPropertyId.UseFileServer, prop.Value);
                        break;
                    case EntityFieldPropertyId.ShowDocumentInPage:
                        iff.AddProperty(eControlPropertyId.ShowDocumentInPage, prop.Value);
                        break;
                    case EntityFieldPropertyId.DocumentType:
                        iff.AddProperty(eControlPropertyId.DocumentType, prop.Value);
                        break;
                    default:
                        iff.AddProperty((eControlPropertyId)prop.PropertyId, prop.Value);
                        break;
                }
            }
        }

        if (entityField?.Required == true && formField != null &&
            formField.FieldOrControlType != FormField.Type.FilterField)
        {
            iff.SetProperty(eControlPropertyId.Required, true);
        }

        if (entityField?.Boolean != null)
        {
            SetBooleanProperties(iff, entityField.Boolean);
        }

        if (formField?.GetProperties() != null)
        {
            foreach (FormProperty prop in formField.GetProperties())
            {
                iff.AddProperty(prop.Id, prop.Value);
            }
        }
    }

    private static void SetBooleanProperties(FormFieldDefinition iff, BooleanEntityField booleanEntityField)
    {
        if (booleanEntityField.TrueTitle != null)
        {
            iff.AddProperty(eControlPropertyId.TrueTitle, booleanEntityField.TrueTitle);
        }

        if (booleanEntityField.FalseTitle != null)
        {
            iff.AddProperty(eControlPropertyId.FalseTitle, booleanEntityField.FalseTitle);
        }

        if (booleanEntityField.AllTitle != null)
        {
            iff.AddProperty(eControlPropertyId.AllTitle, booleanEntityField.AllTitle);
        }

        if (booleanEntityField.NullTitle != null)
        {
            iff.AddProperty(eControlPropertyId.NullTitle, booleanEntityField.NullTitle);
        }
    }

    public CommonFormStructure GetCommonFormStructure(string culture,
        string namespaceId, string entityId, string formId, Form.eFormType formType,
        string formSubjectId, Form form, IdentityUser user, string tableName = null, bool designMode = false)
    {
        UiEntity entity = form?.entity;
        if (entity == null)
        {
            return null;
        }

        CommonFormStructure structure = new()
        {
            NamespaceId = namespaceId,
            EntityId = entityId,
            FormSubjectId = formSubjectId,
            Form_ReportId = formId,
            FormType = formType,
        };
        bool isFilter = formType.In(Form.eFormType.Index, Form.eFormType.Report, Form.eFormType.Dashboard);
        return GetCommonStructure(culture, structure, form, user, tableName, isFilter, designMode);
    }
    #endregion
}

