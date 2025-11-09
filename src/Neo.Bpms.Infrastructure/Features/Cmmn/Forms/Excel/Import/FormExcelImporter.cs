using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Resources;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;

public interface IFormExcelImporter
{
    void Init(IdentityUser user, string culture,
        string namespaceId, string entityId, string formSubjectId);
    Task<OperationResult> Import(Stream inputStream, string fileContentType, long maxRecords, CancellationToken cancellationToken);
}

public partial class FormExcelImporter(IApplyFormData applyFormData, 
    FormServiceOperation formServiceOperation,
    FormStructRoutines formStructRoutines
    ) : IFormExcelImporter
{
    public void Init(IdentityUser user, string culture,
        string namespaceId, string entityId, string formSubjectId)
    {
        _user = user;
        _culture = culture;
        _namespaceId = namespaceId;
        _entityId = entityId;
        if (formSubjectId == "undefined")
        {
            formSubjectId = null;
        }

        _formSubjectId = formSubjectId;
    }

    private IdentityUser _user;
    private string _culture;
    private string _namespaceId;
    private string _entityId;
    private string _formSubjectId;
    private string Name(CommonFormStructure formStructure)
    {
        return formStructure.Name.Length > 31 ? formStructure.Name[..31] : formStructure.Name;
    }

    private Form Form { get; set; }
    private CommonFormStructure FormStructure { get; set; }
    private ImportType ImportType { get; set; }

    private Dictionary<string, HeaderColumn> _headerColumns = [];
    private Dictionary<string, SubtableSheetImporter> SubTableSheets { get; set; }
    private Dictionary<string, MultiSelectCellImporter> FormMultiCombos { get; set; }

    private Dictionary<string, TableDefinition> MultiCombos => FormStructure.Tables
        .Where(t => t.ControlType == eControlTypeId.MultipleSelectableCombo)
        .ToDictionary(t => t.FieldName);

    public async Task<OperationResult> Import(Stream inputStream, string fileContentType, long maxRecords, CancellationToken cancellationToken)
    {
        OperationResult result = new();

        try
        {
            using ExcelPackage excelPackage = new(inputStream);
            ExcelWorkbook workbook = excelPackage.Workbook;

            GetForm(workbook.Worksheets[0].Name);
            RelationsData relationsData = [];
            List<MainExcelRecord> records = ReadData(workbook, result, relationsData, maxRecords);

            if (Form.ApplyFormOperationType != FormOperationType.Apply)
            {
                relationsData.QueryRelations(_culture);
                relationsData.CreateRelations(records, result, workbook.Worksheets[0].Name, _user, _culture);
            }

            await SaveRecords(records, result, cancellationToken);
        }
        catch (ExcelImportException e)
        {
            result.AddFatal(e.Message);
        }
        catch (Exception)
        {
            result.AddFatal(Texts.CouldNotReadExcelFile);
        }

        return result;
    }

    private void GetForm(string sheetName)
    {
        static ImportType GetImportType(Form.eFormType formType)
        {
            return formType switch
            {
                Form.eFormType.Create => ImportType.Insert,
                Form.eFormType.Edit => ImportType.Update,
                _ => throw new ArgumentOutOfRangeException(nameof(formType), formType, null),
            };
        }

        bool ValidateForm(Form form1)
        {
            if (form1 != null)
            {
                CommonFormStructure structure = formStructRoutines.GetCommonFormStructure(_culture, form1.NamespaceId, form1.EntityId, form1.Id,
                    form1.FormType, form1.FormSubjectId, form1, _user);
                string name = Name(structure);
                if (!_user.CheckFormAccess(_namespaceId, _entityId, form1.FormType, form1.FormSubjectId, form1.Id, form1))
                {
                    throw new UnauthorizedAccessException();
                }

                if (name == sheetName)
                {
                    Form = form1;
                    FormStructure = structure;
                    ImportType = GetImportType(form1.FormType);
                    Initialize();
                    return true;
                }
            }
            return false;
        }

        Form form = FormStructRoutines.GetForm(_namespaceId, _entityId, null, Form.eFormType.Create, _formSubjectId);
        if (ValidateForm(form))
        {
            return;
        }

        form = FormStructRoutines.GetForm(_namespaceId, _entityId, null, Form.eFormType.Edit, _formSubjectId);
        if (ValidateForm(form))
        {
            return;
        }

        throw new ExcelImportException(Texts.ExcelFirstSheetIsInvalid);
    }

    private List<MainExcelRecord> ReadData(ExcelWorkbook workbook, OperationResult result,
        RelationsData relationsData, long maxRecords)
    {
        List<MainExcelRecord> records = [];
        ExcelWorksheet mainWorksheet = workbook.Worksheets[0];

        _headerColumns = new ReadSheetHeader(Form.Entity, FormStructure).ReadEPPlus(mainWorksheet, result, true);
        if (_headerColumns == null || _headerColumns.Count == 0)
        {
            throw new ExcelImportException(Texts.ExcelMainSheetHeaderIsInvalid);
        }

        long totalRecords = ReadMainSheetData(mainWorksheet, result, relationsData, records, maxRecords);
        result.AddSheetOverviewResult(FormStructure.Name, totalRecords, totalRecords - records.Count);
        ReadSubTables(workbook, result, relationsData, records, maxRecords);
        return records;
    }

    private long ReadMainSheetData(ExcelWorksheet worksheet, OperationResult result,
        RelationsData relationsData, List<MainExcelRecord> records, long maxRecords)
    {
        int rowCount = worksheet.Dimension?.Rows ?? 0;
        long rowsToProcess = Math.Min(rowCount, maxRecords);

        for (int row = 1; row <= rowsToProcess; row++)
        {
            ReadRecordFromMainSheet(worksheet, row, result, relationsData, records);
        }

        return rowsToProcess;
    }

    private void ReadRecordFromMainSheet(ExcelWorksheet worksheet, int row,
        OperationResult result, RelationsData relationsData, List<MainExcelRecord> records)
    {
        MainExcelRecord record = new();
        bool isValid = true;

        foreach (HeaderColumn item in _headerColumns.Values)
        {
            ExcelRange cell = worksheet.Cells[row, item.Index];

            if (MultiCombos.ContainsKey(item.FieldName))
            {
                if (FormMultiCombos.TryGetValue(item.FieldName, out MultiSelectCellImporter mc))
                {
                    mc.ReadDataEPPlus(cell, record, item, relationsData);
                }

                continue;
            }

            ExcelColumnValue value = ExcelColumnValue.GetColumnValueEPPlus(cell, item, relationsData, result, out bool isCellValid);
            if (!isCellValid)
            {
                isValid = false;
                continue;
            }

            // بقیه منطق مشابه قبل
        }

        if (isValid)
        {
            records.Add(record);
        }
    }

    private void ReadSubTables(ExcelWorkbook workbook, OperationResult result,
        RelationsData relationsData, List<MainExcelRecord> records, long maxRecords)
    {
        IEnumerable<TableDefinition> tables = FormStructure.Tables.Where(t => t.ControlType != eControlTypeId.MultipleSelectableCombo);
        if (!tables.Any())
        {
            return;
        }

        foreach (ExcelWorksheet worksheet in workbook.Worksheets.Skip(1))
        {
            if (SubTableSheets.TryGetValue(worksheet.Name, out SubtableSheetImporter table))
            {
                table.ImportFromSubTableSheetEPPlus(worksheet, result, relationsData, records, maxRecords);
            }
            else
            {
                throw new ExcelImportException($"{Texts.InvalidExcelSheetName} : {worksheet.Name}");
            }
        }
    }

    private void Initialize()
    {
        foreach (TableDefinition table in FormStructure.Tables)
        {
            if (table.ControlType == eControlTypeId.MultipleSelectableCombo)
            {
                Form tableForm = FormStructRoutines.GetForm(table.NamespaceId, table.EntityId,
                        GetSubIndexTableFormId(table),
                        GetFormType(), table.FormSubjectId);
                EntityField field = tableForm.Entity.GetField(table.TableDef?.GetProperty(
                        eControlPropertyId.MultipleForeignKeyFieldId)?.Value?.ToString());
                if (field == null)
                {
                    continue;
                }

                MultiSelectCellImporter mc = new(table.TableDef, field);
                FormMultiCombos ??= [];
                if (FormMultiCombos.ContainsKey(table.FieldName))
                {
                    continue;
                }

                FormMultiCombos.Add(table.FieldName, mc);
            }
            else if (table.ControlType == eControlTypeId.IndexTable && table.Editable)
            {
                SubTableSheets ??= [];
                Form tableForm = FormStructRoutines.GetForm(table.NamespaceId, table.EntityId,
                        GetSubIndexTableFormId(table),
                        GetFormType(), table.FormSubjectId);

                CommonFormStructure formStructure = formStructRoutines.GetCommonFormStructure(_culture, tableForm.NamespaceId,
                        tableForm.EntityId, tableForm.Id, tableForm.FormType, tableForm.FormSubjectId, tableForm, _user);

                SubtableSheetImporter sheetForm = new(tableForm, formStructure, table.TableDef);

                if (SubTableSheets.ContainsKey(sheetForm.Name))
                {
                    continue;
                }

                SubTableSheets.Add(sheetForm.Name, sheetForm);
            }
        }
    }

    private async Task SaveRecords(IEnumerable<MainExcelRecord> records, OperationResult result, CancellationToken cancellationToken)
    {
        switch (ImportType)
        {
            case ImportType.Insert:
                foreach (MainExcelRecord record in records)
                {
                    bool isForApply = !string.IsNullOrEmpty(Form.ApplyServiceOperation) &&
                                         Form.ApplyFormOperationType == FormOperationType.Apply;
                    ElasticObject formData = EstablishFormData(record, isForApply);
                    if (formData == null)
                    {
                        continue;
                    }

                    ExceptionInfos errors;

                    AuditTrail auditTrail = GetAuditTrail();

                    ServiceOperationFormData serviceOperationFormData;
                    if (isForApply)
                    {
                        serviceOperationFormData = new ServiceOperationFormData(formData);
                        errors = await formServiceOperation.CallApplyServiceOperation(serviceOperationFormData, Form,
                            auditTrail);
                    }
                    else
                    {
                        (bool Saved, ExceptionInfos errors) res = await Apply(auditTrail, formData, cancellationToken);
                        errors = res.errors;
                        if (res.Saved && !string.IsNullOrEmpty(Form.ApplyServiceOperation) &&
                            Form.ApplyFormOperationType == FormOperationType.AfterApply)
                        {
                            serviceOperationFormData = new ServiceOperationFormData(formData);
                            errors = await formServiceOperation.CallApplyServiceOperation(serviceOperationFormData, Form,
                                auditTrail);
                        }
                    }

                    if (errors.Any())
                    {
                        foreach (ExceptionInfo error in errors)
                        {
                            result.AddApplyError(error.ForField, error.Exception.Message);
                        }
                    }
                }
                break;
            case ImportType.Update:
                foreach (MainExcelRecord record in records)
                {
                    bool isForApply = !string.IsNullOrEmpty(Form.ApplyServiceOperation) && Form.ApplyFormOperationType == FormOperationType.Apply;
                    ElasticObject formData = EstablishFormData(record, isForApply);
                    if (formData == null)
                    {
                        continue;
                    }

                    ExceptionInfos errors = [];
                    TriggerTypeId triggerTypeId = Form.TriggerTypeId(false);
                    AuditTrail auditTrail =
                            new(triggerTypeId, $"{triggerTypeId} Form {Form.Id} in entity {Form.entity.Id}", _user, 0)
                            {
                                MetaEntityId = Form.entity.DbId,
                                MetaFormId = Form.DbId,
                                FormId = Form.Id,
                                EntityPkv = record.PrimaryKeyValue.ToString()
                            };
                    ServiceOperationFormData serviceOperationFormData;
                    if (isForApply)
                    {
                        serviceOperationFormData = new ServiceOperationFormData(formData);
                        errors = await formServiceOperation.CallApplyServiceOperation(serviceOperationFormData, Form, auditTrail);
                    }
                    else
                    {
                        bool saved = await applyFormData.UpdateRecord(auditTrail, Form.NamespaceId, Form.EntityId, Form, formData, FormDataRoutines.GetKeyRecord(Form.entity, record.PrimaryKeyValue.ToString()), null, errors, cancellationToken: cancellationToken);

                        auditTrail.EntityPkv = record.PrimaryKeyValue.ToString();
                        if (saved && !(errors?.Any() ?? false))
                        {
                            saved = await applyFormData.UpdateTables(auditTrail, Form.NamespaceId, Form.EntityId, Form.FormSubjectId, formData,
                                record.PrimaryKeyValue.ToString(), _culture, FormStructure.Tables, errors, cancellationToken);
                        }
                        if (saved && !string.IsNullOrEmpty(Form.ApplyServiceOperation) &&
                            Form.ApplyFormOperationType == FormOperationType.AfterApply)
                        {
                            serviceOperationFormData = new ServiceOperationFormData(formData);
                            errors = await formServiceOperation.CallApplyServiceOperation(serviceOperationFormData, Form,
                                auditTrail);
                        }
                    }

                    if (errors?.Any() ?? false)
                    {
                        foreach (ExceptionInfo error in errors)
                        {
                            result.AddApplyError(error.ForField, error.Exception.Message);
                        }
                    }
                    DataStorage.SaveAudit(auditTrail);
                }
                break;
            case ImportType.Delete:
            case ImportType.Sync:
                throw new NotImplementedException();
        }
    }

    private AuditTrail GetAuditTrail()
    {
        TriggerTypeId triggerTypeId = Form.TriggerTypeId(true);

        AuditTrail auditTrail = new(triggerTypeId,
                $"Excel Import {triggerTypeId} Form {Form.Id} in entity {Form.EntityId}",
                _user, 0)
        {
            MetaEntityId = Form.entity.DbId,
            MetaFormId = Form.DbId,
            FormId = Form.Id,
            FlowNodeInstanceId = 0
        };
        return auditTrail;
    }

    private async Task<(bool Saved, ExceptionInfos errors)> Apply(AuditTrail auditTrail,
        ElasticObject formData, CancellationToken cancellationToken)
    {
        ExceptionInfos errors = null;
        bool saved = await applyFormData.CreateRecord(auditTrail, Form.NamespaceId, Form.EntityId, Form, formData, null,
                errors, null, null, cancellationToken);
        if (saved)
        {
            string entityPkv = formData.GetString("Id") ?? formData.GetString("Ids");
            saved = await applyFormData.UpdateTables(auditTrail, Form.NamespaceId, Form.EntityId, Form.FormSubjectId, formData,
                entityPkv, _culture, FormStructure.Tables, errors, cancellationToken);
        }

        return (saved, errors);
    }

    private ElasticObject EstablishFormData(MainExcelRecord record, bool isForApply)
    {
        ElasticObject e = new();
        if (record.ColumnValues == null)
        {
            return null;
        }

        foreach (ExcelColumnValue column in record.ColumnValues.Values)
        {
            if (column.Header.Field == null)
            {
                continue;
            }

            e[column.Header.Field.Id] = GetColumnValue(column, isForApply);
        }

        if (record.SubTableRecords != null && record.SubTableRecords.Count != 0)
        {
            EstablishSubTablesData(e, record);
        }

        if (record.MultiComboRecords != null && record.MultiComboRecords.Count != 0)
        {
            EstablishMultiComboRecordsData(e, record);
        }

        return e;
    }

    private void EstablishMultiComboRecordsData(ElasticObject e, MainExcelRecord record)
    {
        foreach (KeyValuePair<string, List<ExcelRecord>> comboRecord in record.MultiComboRecords)
        {
            IEnumerable<string> values = comboRecord.Value.SelectMany(v => v.ColumnValues.Values)
                .Select(v2 => v2.RelationId.Ids.FirstOrDefault()?.Value?.ToString());
            e[comboRecord.Key] = string.Join(",", values);
        }
    }

    private void EstablishSubTablesData(ElasticObject e, MainExcelRecord record)
    {
        foreach (KeyValuePair<string, List<ExcelRecord>> recordSubTableRecord in record.SubTableRecords)
        {
            int counter = 0;
            string keyField = Form.Entity.KeyFields.FirstOrDefault()?.Id;

            foreach (ExcelRecord excelRecord in recordSubTableRecord.Value)
            {
                foreach (ExcelColumnValue columnValue in excelRecord.ColumnValues.Values)
                {
                    if (columnValue.Header.FieldName == keyField)
                    {
                        e[recordSubTableRecord.Key + $"[{counter}].__Ids"] = columnValue.Value;
                        continue;
                    }

                    e[recordSubTableRecord.Key + $"[{counter}].{columnValue.Header.FieldName}"] =
                        GetColumnValue(columnValue);
                }

                e[recordSubTableRecord.Key + $"[{counter}].__Deleted"] = excelRecord.IsDelete ? true : (object)false;

                counter++;
            }
        }
    }

    private static object GetColumnValue(ExcelColumnValue column, bool isForApply = false)
    {
        if (column.RelationId != null)
        {
            string rId = FindRelationId(column);
            return isForApply && string.IsNullOrEmpty(rId) ? column.Value : rId;
        }
        return column.Value;
    }

    private static string FindRelationId(ExcelColumnValue column)
    {
        return column.RelationId.Ids.Count == 1 ? (column.RelationId.Ids.FirstOrDefault()?.Value) : throw new NotImplementedException();
    }

    private string GetSubIndexTableFormId(TableDefinition table)
    {
        return ImportType switch
        {
            ImportType.Insert => table.CreateFormId,
            ImportType.Update => table.EditFormId,
            ImportType.Delete or ImportType.Sync => null,
            _ => null,
        };
    }

    private Form.eFormType? GetFormType()
    {
        return ImportType switch
        {
            ImportType.Insert => (Form.eFormType?)Form.eFormType.Create,
            ImportType.Update => (Form.eFormType?)Form.eFormType.Edit,
            ImportType.Delete or ImportType.Sync => null,
            _ => null,
        };
    }
}
