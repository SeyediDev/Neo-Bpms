using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Export;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel;

public class FormExcelTemplateGenerator(Form form, CommonFormStructure structure, IdentityUser user,
    string culture, bool isRightToLeft, ElasticObject filterValues, ExportType exportType)
{
    public MemoryStream CreateExcelTemplate(
        FormStructRoutines formStructRoutines,
        FormDataRoutines formDataRoutines,
        out string fileName, CancellationToken cancellationToken)
    {
        ExportExcelForm excelExport = new(form, structure, filterValues, user, form.FormType == Form.eFormType.Edit,
            culture, isRightToLeft, exportType, cancellationToken);
        return excelExport.Export(formStructRoutines, formDataRoutines, out fileName);
    }
}
