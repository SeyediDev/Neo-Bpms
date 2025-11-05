namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Export;

public class FormExportData
{
    public readonly bool IsMainForm;
    public IndexFormData Result { get; set; }
    public FormExportData(bool isMainForm)
    {
        IsMainForm = isMainForm;
    }
}