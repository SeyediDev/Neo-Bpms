using Neo.Bpms.Domain.Features.MetaDefinitions.ProjectDefinitions.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.CombosData;

public static class FormComboData
{
    public static void SetCombosData(Form form, CommonFormStructure structure, string culture,
        ElasticObject record, LocalParameters localParameters)
    {
        ConcurrentDictionary<string, ComboData> combosData = new();
        SetFormCombosData(form, combosData, culture, record, false, false, localParameters);
        Parallel.ForEach(structure.Tables, table =>
        {
            Form tableForm = table.TableDef.TableEntity.getForm(null, Form.eFormType.Index, table.FormSubjectId);
            if (tableForm == null)
                return;
            List<ElasticObject> tableRecords = record?[table.FieldName] as List<ElasticObject> ??
                               Enumerable.Empty<ElasticObject>().ToList();
            if (table.Editable && tableRecords.Count == 0)
                SetFormCombosData(tableForm, combosData, culture, null, true, table.Editable, localParameters);
            foreach (ElasticObject tableRecord in tableRecords)
                SetFormCombosData(tableForm, combosData, culture, tableRecord, true, table.Editable, localParameters);
        });
        Parallel.ForEach(combosData.Values, comboData => ComboDataRoutines.GetComboRecords(comboData, culture));
        structure.CombosData = combosData;
        foreach (TableDefinition table in structure.Tables)
            table.CombosData = combosData;
    }
    private static void SetFormCombosData(Form form, ConcurrentDictionary<string, ComboData> combosData,
        string culture, ElasticObject record, bool isSubTable, bool subIsEditable,
        LocalParameters localParameters)
    {
        Parallel.ForEach(form.formFields, formField =>
        {
            switch (formField.FieldOrControlType)
            {
                case FormField.Type.ColumnField:
                case FormField.Type.Field:
                case FormField.Type.FilterField:
                case FormField.Type.Control:
                case FormField.Type.SubTable:
                    SetFormFieldComboData(form, combosData, culture, record,
                        isSubTable, subIsEditable, formField, localParameters);
                    break;
            }
        });
    }

    private static void SetFormFieldComboData(Form form,
        ConcurrentDictionary<string, ComboData> combosData, string culture, ElasticObject record,
        bool isSubTable, bool subIsEditable, FormField formField, LocalParameters localParameters)
    {
        if(formField.Field?.IsBoolParam() is true)
        {
            return;
        }
        if (isSubTable && formField.FieldOrControlType == FormField.Type.FilterField) return;
        string[] fieldIds = formField.Id.Split('.');
        EntityField field = formField.Field;
        string dotAssociatedFieldId = null;
        if (fieldIds.Length > 1)
        {
            field = field.GetDotAssociatedFieldWithCompleteId(fieldIds, out dotAssociatedFieldId);
        }
        var isEnum = field?.AssociationEntity?.Entity() == null && field?.IsEnum is true;
        if (field?.AssociationEntity?.Entity() == null )
        {
            if (field?.IsEnum is not true)
            {
                return;
            }
        }
        bool isReadOnly = (!isSubTable || !subIsEditable) &&
                         form.AllFieldsAreReadOnly() && formField.FieldOrControlType != FormField.Type.FilterField;
        bool isRemoteData = formField.CheckProperty(eControlPropertyId.RemoteData);
        if (!isRemoteData && field?.AssociationEntity?.Entity() != null)
        {
            isRemoteData = true;
            formField.AddProperty(eControlPropertyId.RemoteData, true);
        }
        bool isOnDemand = formField.CheckProperty(eControlPropertyId.OnDemand) &&
                         !formField.CheckProperty(eControlPropertyId.DefaultValue);
        bool isMandatory = formField.CheckProperty(eControlPropertyId.Required);
        if (!isReadOnly && formField.CheckProperty(eControlPropertyId.ReadOnly))
            isReadOnly = true;
        string displayFields = formField.Property(eControlPropertyId.DisplayFields);
        if (string.IsNullOrEmpty(displayFields))
            displayFields = field.Property(EntityFieldPropertyId.DisplayFields);
        const int count = 20, pageNo = 1;
        ComboData comboData = combosData.TryGetValue(formField.Id, out ComboData value)
            ? value :
            ComboDataRoutines.InitComboData(form.entity,
                field.AssociationEntity?.Entity(), culture, field.AssociationEntity?.Constraint,
                displayFields, formField.GetProperties()?.Select(p => (UIComponentProperty)p),
                count);
        if (isEnum || (!isOnDemand && !isRemoteData && !isReadOnly && !comboData.GetQuery))
        {
            LocalParameters lp = [];
            lp.Set(localParameters);
            lp.AddOrUpdate("q", record);

            ComboData newComboData = null;
            if (field?.AssociationEntity?.Entity() == null && field?.IsEnum is true)
            {
                newComboData = comboData;
                var enumType = field.CSharpType;
                if(Nullable.GetUnderlyingType(field.CSharpType) !=null)
                {
                    enumType = Nullable.GetUnderlyingType(field.CSharpType);
                }
                var enumItems = Enum.GetValues(enumType);
                var enumNames = Enum.GetNames(enumType);
                for (int i=0; i< enumNames.Length; i++)
                {
                    var enumItem = enumItems.GetValue(i);
                    var enumItemId = (int)enumItems.GetValue(i);
                    var displayValue = FormDataRoutines.GetEnumText(enumType, enumItem, culture)?? enumNames[i];
                    newComboData.AddRow(new FormDataRow()
                    {
                        Ids = enumItemId.ToString(),
                        DisplayValue = displayValue
                    });
                }
            }
            else
            {
                newComboData = ComboDataRoutines.GetRecords(form.entity,
                    field.AssociationEntity.Entity(), culture, formField.Property(eControlPropertyId.FilterFormula),
                    pageNo, field.AssociationEntity.Constraint,
                    displayFields, lp, null, count, true, formField.GetProperties());
            }
            newComboData.GetQuery = true;
            if (!combosData.ContainsKey(formField.Id))
                combosData.TryAdd(formField.Id, newComboData);
            else if(field?.AssociationEntity?.Entity() != null )
            {
                combosData[formField.Id] = newComboData;
                foreach (FormDataRow row in comboData.Rows)
                    newComboData.AddRow(row);
                foreach (KeyValuePair<string, FormDataRow> row in comboData.RecordsById.Where(r => r.Value == null))
                    if (!newComboData.RecordsById.ContainsKey(row.Key))
                        newComboData.RecordsById.TryAdd(row.Key, null);
            }

            comboData = newComboData;
        }

        if (!combosData.ContainsKey(formField.Id))
            combosData.TryAdd(formField.Id, comboData);
        if (record != null && !field.AssociationEntity?.CheckFlag(EntityFieldFlags.IsBitMask) is true)
        {
            string selectedIds = "";
            foreach (EntityRelationMap map in field.AssociationEntity.Maps)
            {
                if (!string.IsNullOrEmpty(selectedIds))
                    selectedIds += "#";
                object v = string.IsNullOrEmpty(dotAssociatedFieldId)
                    ? record[map.SourceField]
                    : record[dotAssociatedFieldId];
                selectedIds +=
                    v is string
                        ? string.IsNullOrEmpty(v as string)
                            ? "null"
                            : v
                        : v?.ToString() ?? "null";
            }

            if (!string.IsNullOrEmpty(selectedIds) && selectedIds != "null")
                if (!comboData.RecordsById.ContainsKey(selectedIds))
                    comboData.RecordsById.TryAdd(selectedIds, null);
        }

        FormProperty defaultValue = formField.GetProperty(eControlPropertyId.DefaultValue);
        if (defaultValue != null)
        {
            string selectedIds = defaultValue.value?.ToString() ?? "";
            if (!string.IsNullOrEmpty(selectedIds) && selectedIds != "null")
                if (!comboData.RecordsById.ContainsKey(selectedIds))
                    comboData.RecordsById.TryAdd(selectedIds, null);
        }
    }
}
