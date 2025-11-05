using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    [HttpGet]
    public async Task<ActionResult> IframeForm(string NamespaceId,
        string EntityId, string FormId, string Id, string ProcessId, string TaskId, long? wid,
        [ModelBinder(typeof(DynamicActionGetBinder))]
        ElasticObject parameters, string indexFormId, bool isReturnable, CancellationToken cancellationToken)
    {
        SetInIframe();
        (Form form, IdentityUser user, CommonFormStructure structure, PersistenceObject po) formAndThings = await GetFormAndThings(NamespaceId, EntityId, FormId);
        IdentityUser user = formAndThings.user;
        Form form = formAndThings.form;
        CommonFormStructure structure = formAndThings.structure;

        ElasticObject record;
        if (form.FormType.In(Form.eFormType.Create, Form.eFormType.ProcessCreate))
        {
            record = parameters;
        }
        else
        {
            (ElasticObject record, string ids) r = await GetRecord(Id, form, user, CultureHelper.GetCurrentNeutralCulture(),
                structure, parameters, cancellationToken);
            record = r.record;
            Id = r.ids;
            record.MergeValues(parameters);
        }

        MakeIframePageAvailable(Id, form, structure, record, user, ProcessId, TaskId, wid, indexFormId, isReturnable);
        return View(record);
    }

    [HttpPost]
    public async Task<ActionResult> IframeForm(string NamespaceId,
        string EntityId, string FormId,
        [ModelBinder(typeof(DynamicActionBinder))] ElasticObject record,
        string ids, string ProcessId, string TaskId, long? wid, string returnUrl, string indexFormId, bool isReturnable, CancellationToken cancellationToken )
    {
        SetInIframe();
        (Form form, IdentityUser user, CommonFormStructure structure, PersistenceObject po) formAndThings = await GetFormAndThings(NamespaceId, EntityId, FormId);
        IdentityUser user = formAndThings.user;
        Form form = formAndThings.form;
        CommonFormStructure structure = formAndThings.structure;

        ElasticObject clonedRecord = record.Clone();
        PostFormData result = await SubmitForm(form, ids, record, user, ProcessId, TaskId, wid,
            form.FormType == Form.eFormType.ProcessCreate, structure, cancellationToken);
        ExceptionInfos errors = result.errors;
        if (errors != null)
        {
            foreach (ExceptionInfo error in errors)
            {
                ModelState.AddModelError(error.ForField ?? "", error.Exception.Message);
            }
        }

        if (ModelState.IsValid)
        {
            if (!string.IsNullOrEmpty(indexFormId))
            {
                return !string.IsNullOrEmpty(returnUrl)
                        ? Redirect(returnUrl)
                        : RedirectToAction("IframeIndex", new
                        {
                            NamespaceId,
                            EntityId,
                            FormId = indexFormId,
                            Id = ids
                        });
            }

            if (form.FormType == Form.eFormType.Create)
                ViewBag.SuccessMessage = Messages.CreatedSuccessfully;
            else
                ViewBag.SuccessMessage = Messages.DoneSuccessfully;
        }

        (ElasticObject record, string ids) r = await MergeSavedAndPosted(ids, clonedRecord, form, structure, CultureHelper.GetCurrentCulture(), user, cancellationToken);
        record = r.record;
        ids = r.ids;
        MakeIframePageAvailable(ids, form, structure, record, user, ProcessId, TaskId, wid, indexFormId, isReturnable);
        return View(record);
    }

    [HttpGet]
    public ActionResult Redirect(string NamespaceId,
        string EntityId, string FormId, string Id)
    {
        Form form = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId);
        string action = "Edit";

        switch (form.FormType)
        {
            case Form.eFormType.Index:
                action = "Index";
                break;
            case Form.eFormType.Create:
                action = "Create";
                break;
            case Form.eFormType.Edit:
            case Form.eFormType.ProcessCreate:
            case Form.eFormType.WorkItem:
                action = "Edit";
                break;
            case Form.eFormType.Delete:
            case Form.eFormType.VirtualDelete:
                action = "Delete";
                break;
            case Form.eFormType.Detail:
                action = "Details";
                break;
        }

        return RedirectToAction(action, "Form", new
        {
            form.NamespaceId,
            form.EntityId,
            form.FormSubjectId,
            ids = Id
        });
    }

    private void MakeIframePageAvailable(string Id, Form form, CommonFormStructure structure, ElasticObject record,
        IdentityUser user, string processId, string taskId, long? wid, string indexFormId, bool isReturnable)
    {
        FormComboData.SetCombosData(form, structure,
            CultureHelper.GetCurrentNeutralCulture(),
            record, GetLocalParameters(user, record));
        ComboDataRoutines.SetComboDataSelectedId(structure, record, false, form.entity);
        SetIframeFormViewBags(user, structure, Id, processId, taskId, wid, indexFormId, isReturnable);
        SetPagePackId(form);
    }

    private void SetIframeFormViewBags(IdentityUser user,
        CommonFormStructure structure, string id, string processId, string taskId, long? wid, string indexFormId,
        bool isReturnable)
    {
        ViewBag.CanDesign = CanDesignForms(user) && cmmnSettings.Value.ShowFormDesign;
        ViewBag.structure = structure;
        ViewBag.Ids = id;

        ViewBag.ProcessId = processId;
        ViewBag.TaskId = taskId;
        ViewBag.wid = wid;
        ViewBag.IndexFormId = indexFormId;
        ViewBag.IsReturnable = isReturnable;
    }
}
