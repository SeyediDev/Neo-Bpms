using System.Net.Mime;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Export;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    [HttpGet]
    public ActionResult GenerateExcelTemplate(CancellationToken cancellationToken, string NamespaceId,
        string EntityId, string FormSubjectId, string FormId, string SortFields, string newPage,
        [ModelBinder(typeof(DynamicActionGetBinder))] ElasticObject FilterValues, string calendar,
        ExportType exportType)
    {
        IdentityUser user = GetUser();

        Form indexForm = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Index, FormSubjectId);
        if (indexForm == null)
            return Error(Messages.PageNotFound, "Index");

        Form form = null;
        switch (exportType)
        {
            case ExportType.Create:
                form = FormStructRoutines.GetForm(NamespaceId, EntityId, null, Form.eFormType.Create, indexForm.FormSubjectId);
                break;
            case ExportType.Edit:
                form = FormStructRoutines.GetForm(NamespaceId, EntityId, null, Form.eFormType.Edit, indexForm.FormSubjectId);
                break;
            case ExportType.Delete:
                throw new NotImplementedException();
        }
        if (form == null)
            return Error(Messages.FormNotFound, "Form");
        if (!CheckAccess(user, form))
            return Error(Messages.PageAccessDenied, "GenerateExcelTemplate");

        string culture = CultureHelper.GetCurrentNeutralCulture();

        CommonFormStructure structure = formStructRoutines.GetCommonFormStructure(culture, form.NamespaceId, form.EntityId, form.Id, form.FormType,
            form.FormSubjectId, form, user);

        FormExcelTemplateGenerator exportFormExcel = new(form, structure, user, culture,
            CultureHelper.IsRightToLeft(), FilterValues, exportType);
        MemoryStream stream = exportFormExcel.CreateExcelTemplate(formStructRoutines, formDataRoutines, out string fileName, cancellationToken);
        ContentDisposition cd = new()
        {
            FileName = fileName + ".xlsx",
            Inline = false
        };
        Response.Headers.Append("Content-Disposition", cd.ToString());
        stream.Seek(0, SeekOrigin.Begin);
        return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [HttpPost]
    public async Task<JsonResult> BulkImportFromExcelFile(IFormExcelImporter formExcelImporter, 
        string namespaceId, string entityId, string formId, string formSubjectId, CancellationToken cancellationToken)
    {
        IdentityUser user = GetUser();
        Form indexForm = FormStructRoutines.GetForm(namespaceId, entityId, formId, Form.eFormType.Index, formSubjectId) ?? throw new HttpException(404, Messages.PageNotFound);
        if (!CheckAccess(user, indexForm))
            throw new UnauthorizedAccessException(Messages.PageAccessDenied);
        IFormFile file = null;
        if (Request.Form.Files.Count == 1)
            file = Request.Form.Files[0];
        if (file == null)
            throw new Exception(Messages.NoSelectedFile);

        string culture = CultureHelper.GetCurrentNeutralCulture();
        formExcelImporter.Init(user, culture, namespaceId, entityId, formSubjectId);
        OperationResult result = await formExcelImporter.Import(file.OpenReadStream(), file.ContentType, MaxRecords, cancellationToken);

        return Json(result);
    }

    private const long MaxRecords = 5000;
}
