using System.Net.Mime;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormTemplate;
using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.FormModels;
using static Neo.Bpms.Domain.Model.UI.Forms.Form;


namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

public partial class MetaDesignController
{
    [HttpGet]
    public JsonResult Forms(string namespaceId, string entityId, bool onlyProcessForms = false,
        bool onlyIndexForms = false)
    {
        CheckEntityDesignAccess(false);

        IEnumerable<Form> forms = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?.getForms();
        if (onlyProcessForms)
            forms = forms?.Where(f => f.FormType == eFormType.WorkItem ||
                                      f.FormType == eFormType.ProcessCreateOnExistingRecord ||
                                      f.FormType == eFormType.ActiveProcessInstance ||
                                      f.FormType == eFormType.ProcessCreate);
        if (onlyIndexForms)
            forms = forms?.Where(f => f.FormType == eFormType.Index);

        return Json(
            forms?.OrderBy(f => f.FormSubjectId).ThenBy(f => f.Id).Select(f => new FormRecognizer(f)));
    }

    [HttpGet]
    public JsonResult Form(string namespaceId, string entityId, string formId)
    {
        CheckEntityDesignAccess(false);

        Form form = FetchForm(namespaceId, entityId, formId);
        return Json(new FormViewModel(form));
    }


    [HttpPut]
    public JsonResult Form([FromBody] FormViewModel formViewModel, string prevFormId)
    {
        CheckEntityDesignAccess(true);

        Form entityForm = FetchForm(formViewModel.namespaceId, formViewModel.entityId, prevFormId);
        UiEntity entity = entityForm.entity;
        formViewModel.Modify(entityForm);
        if (entityForm.Id != prevFormId)
        {
            ProjectEntityForm.Remove(formViewModel.namespaceId, formViewModel.entityId, prevFormId);

            //entity.DeleteForm(prevFormId);
            entity.AddForm(entityForm);
        }

        ProjectEntityForm.Save(entityForm);
        return Json(new { Success = true });
    }

    [HttpDelete]
    public async Task<JsonResult> Form(string namespaceId, string entityId, string formId, int? justToMakeADifference, CancellationToken cancellationToken)
    {
        CheckEntityDesignAccess(true);

        await DeleteFormConfigs(namespaceId, entityId, formId, cancellationToken);

        ProjectEntityForm.Remove(namespaceId, entityId, formId);

        return Json(new { Success = true });
    }
    
    public async Task DeleteFormConfigs(string namespaceId, string entityId, string formId, CancellationToken cancellationToken)
    {
        UiEntity entity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        Form entityForm = entity?.GetEntityForm(formId);
        if (entityForm == null)
            return;
        await filterConfigBackupRestore.RemoveConfigs(namespaceId, entityId, formId, null, null, null, cancellationToken);
        await folderConfigBackupRestore.RemoveConfigs(namespaceId, entityId, formId, null, null, null, cancellationToken);
    }

    [HttpPost]
    public JsonResult NewForm(string namespaceId, string entityId)
    {
        CheckEntityDesignAccess(true);

        UiEntity uiEntity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        if (uiEntity == null)
            return Json(new { Success = false });
        string formId = GenerateNewId(namespaceId, "Form");
        //todo formType ?
        Form form = new(uiEntity, formId, "فرم " + uiEntity.Name, $"{uiEntity.EnName} Index Form", eFormType.Index);
        uiEntity.AddForm(form);
        ProjectEntityForm.Save(form);
        return Json(new FormRecognizer(form));
    }

    [HttpPost]
    public JsonResult CloneForm(string namespaceId, string entityId, string formId)
    {
        CheckEntityDesignAccess(true);

        UiEntity entity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        Form form = (entity?.GetEntityForm(formId)) ?? throw new Exception("چنین فرمی موجود نیست");
        Form cloneForm = form.Clone();
        ProjectEntityForm.Save(cloneForm);
        entity.AddForm(cloneForm, true);
        return Json(new FormRecognizer(cloneForm));
    }


    [HttpPost]
    public JsonResult GetFormTemplateFiles(string namespaceId, string entityId, string formId)
    {
        CheckEntityDesignAccess(true);

        Form form = FetchForm(namespaceId, entityId, formId);
        return Json(form);
    }


    [HttpPost]
    public JsonResult UploadFormTemplateFile(string namespaceId, string entityId, string formId)
    {
        CheckEntityDesignAccess(true);

        Form form = FetchForm(namespaceId, entityId, formId);
        return Json(form);
    }

    [HttpPost]
    public JsonResult AutoDesignForm(string namespaceId, string entityId, string formId)
    {
        CheckEntityDesignAccess(true);

        Form form = FetchForm(namespaceId, entityId, formId);
        form.Includes = null;
        form.formFields.Clear();
        Dictionary<string, EntityField> entityFields = PrepairEntityFields(form);

        foreach (EntityField entityField in entityFields?.Values ?? Enumerable.Empty<EntityField>())
        {
            if (form.FormType == eFormType.Index)
            {
                form.AddFormField(new FormField(form, entityField)
                {
                    FieldOrControlType = FormField.Type.FilterField
                });
            }

            form.AddFormField(new FormField(form, entityField)
            {
                FieldOrControlType = form.FormType == eFormType.Index
                    ? FormField.Type.ColumnField
                    : FormField.Type.Field
            });
        }

        return Json(new { Success = true });
    }

    private static Dictionary<string, EntityField> PrepairEntityFields(Form form)
    {
        Dictionary<string, EntityField> entityFields = [];
        foreach (EntityField entityField in form.entity?.entityFields?.Values ?? Enumerable.Empty<EntityField>())
        {
            if (form.entity?.Associations.Any(a => a.Maps.FirstOrDefault()?.SourceField == entityField.Id) ?? false)
                continue;
            if (entityField.AuditField)
                continue;
            if (entityField.IsAutoIncrement() || entityField.Id == "StateId")
                continue;
            entityFields.Add(entityField.Id, entityField);
        }
        return entityFields;
    }

    public ActionResult CreateTemplateFile(string namespaceId, string entityId, string formId)
    {//todo out put is not complete
        CheckEntityDesignAccess(true);

        Form form = FetchForm(namespaceId, entityId, formId);
        FormTemplate template = new(form, CultureHelper.GetCurrentNeutralCulture());
        MemoryStream stream = new();

        template.Export(formStructRoutines, stream);
        ContentDisposition cd = new()
        {
            FileName = template.FileName + ".html",
            Inline = false
        };
        Response.Headers.Append("Content-Disposition", cd.ToString());
        stream.Seek(0, SeekOrigin.Begin);
        return new FileStreamResult(stream, "text/html");
    }

    private static Form FetchForm(string namespaceId, string entityId, string formId)
    {
        Form form = (ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?.GetEntityForm(formId))
            ?? throw new Exception("چنین فرمی موجود نیست");
        return form;
    }
}
