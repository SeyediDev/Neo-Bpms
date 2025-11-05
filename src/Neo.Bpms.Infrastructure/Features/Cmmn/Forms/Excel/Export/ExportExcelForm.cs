using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Export;
public class ExportExcelForm(Form form, CommonFormStructure formStructure, ElasticObject filterValues,
    IdentityUser user, bool setData, string culture, bool isRightToLeft, ExportType exportType,
    CancellationToken cancellationToken)
{
    public Form Form = form;
    public CommonFormStructure FormStructure = formStructure;

    public string FileName { get; set; }
    public bool IsRightToLeft { get; set; } = isRightToLeft;
    public string Culture { get; set; } = culture;
    private Dictionary<string, FormSheet> FormSheets { get; } = [];

    public MemoryStream Export(FormStructRoutines formStructRoutines, FormDataRoutines formDataRoutines, out string fileName)
    {
        MemoryStream stream = new();
        using (ExcelPackage package = new(stream))
        {
            CreateFormSheet(package, Form, FormStructure, false, "MainForm", null, out int colCount);
            CreateTablesSheets(formStructRoutines, package);
            SetWorkbookProperties(package);
            if (setData && Form.FormType == Form.eFormType.Edit)
            {
                ExportDataManager data = new(filterValues, user, cancellationToken, Culture, FormSheets);
                data.QueryData(formDataRoutines);
                CreateRecordRows(data, colCount);
            }

            fileName = FileName = $"{Form.entity.Id}.{Form.Id}";
            package.Save();
        }

        return stream;
    }

    private void CreateRecordRows(ExportDataManager data, int colCount)
    {
        foreach (KeyValuePair<string, FormExportData> d in data.Data)
        {
            if (FormSheets.TryGetValue(d.Key, out FormSheet formSheet))
                formSheet.CreateRecords(d.Value, colCount);
        }
    }

    private void CreateTablesSheets(FormStructRoutines formStructRoutines, ExcelPackage package)
    {
        foreach (TableDefinition table in FormStructure.Tables)
        {
            if (table.ControlType != eControlTypeId.IndexTable)
                continue;
            Form tableForm = FormStructRoutines.GetForm(table.NamespaceId, table.EntityId, null,
                GetFormType(), table.FormSubjectId);
            if (tableForm == null)
                continue;
            CommonFormStructure formStructure = formStructRoutines.GetCommonFormStructure(Culture, tableForm.NamespaceId,
                tableForm.EntityId, tableForm.Id, tableForm.FormType, tableForm.FormSubjectId, tableForm, user);
            //todo change isChild to association and pass association field
            CreateFormSheet(package, tableForm, formStructure, true, table.FieldName,
                table.TableDef.TableAssociation.Maps.FirstOrDefault()?.SourceField, out _);
        }
    }

    private Form.eFormType? GetFormType()
    {
        return exportType switch
        {
            ExportType.Create => (Form.eFormType?)Form.eFormType.Create,
            ExportType.Edit => (Form.eFormType?)Form.eFormType.Edit,
            ExportType.Delete => null,//todo delete or virtual delete
            _ => null,
        };
    }

    private void CreateFormSheet(ExcelPackage package, Form form, CommonFormStructure formStructure,
        bool isChild, string fieldName, string sourceField, out int colCount)
    {
        FormSheet formSheet = new(package, form, formStructure, IsRightToLeft, isChild, sourceField);
        colCount = formSheet.CreateFormSheet(setData);
        FormSheets.Add(fieldName, formSheet);
    }

    private void SetWorkbookProperties(ExcelPackage package)
    {
        package.Workbook.Properties.Title = FormStructure.Name;
        package.Workbook.Properties.Author = "کندو - سامانه مدیریت فرآیند";
        package.Workbook.Properties.Company = "کندو";
    }
}
