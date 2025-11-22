using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    internal async Task<(ElasticObject record, string ids)> GetRecord(string ids, Form form, IdentityUser user, string culture,
        CommonFormStructure structure, ElasticObject o, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(ids))
        {
            ids = ids.Replace(',', '#');
        }

        if (string.IsNullOrEmpty(form.GetServiceOperation))
        {
            ElasticObject record = await formDataRoutines.GetRecord(form.entity, ids, structure, culture, form, user, cancellationToken) ??
                         new ElasticObject();
            EntityField keyField = form.entity.KeyFields.FirstOrDefault();
            if (string.IsNullOrEmpty(ids) && keyField != null)
            {
                ids = record.GetString(keyField.Id);
            }

            return (record, ids);
        }
        else
        {
            _ = o.SetField("ids", ids);
            return (await CallGetServiceOperation(form, o, user), ids);
        }
    }

    private async Task<ElasticObject> CallGetServiceOperation(Form form, ElasticObject o, IdentityUser user)
    {
        ServiceOperationFormData result = new(o);
        AuditTrail auditTrail = new(TriggerTypeId.Business,
            $"{form.GetServiceOperation} Form {form.Id} in entity {form.entity.Id}", user, 0)
        {
            MetaEntityId = form.entity.DbId,
            MetaFormId = form.DbId,
            FormId = form.Id
        };
        _ = await formServiceOperation.CallGetServiceOperation(result, form, auditTrail);
        return result.Response;
    }

    private async Task<IndexFormData> CallIndexGetServiceOperation(Form form, ElasticObject filterValues
            , string sortField, IdentityUser user, int pageNo, int recordsPerPage)
    {
        filterValues["_PageNo"] = pageNo;
        filterValues["_RecordsPerPage"] = recordsPerPage;
        filterValues["_SortField"] = sortField;
        ServiceOperationFormData serviceResult = new(filterValues);
        AuditTrail auditTrail = new(TriggerTypeId.Business,
            $"{form.ApplyServiceOperation} Form {form.Id} in entity {form.entity.Id}", user, 0)
        {
            MetaEntityId = form.entity.DbId,
            MetaFormId = form.DbId,
            FormId = form.Id
        };
        _ = await formServiceOperation.CallGetServiceOperation(serviceResult, form, auditTrail);
        List<ElasticObject> list = serviceResult.Response["Rows"] as List<ElasticObject>;
        IndexFormData result = new()
        {
            FilterValues = filterValues,
            recordCount = list?.Count ?? 0
        };
        if (list is not null)
        {
            result.Rows = list;
        }
        return result;
    }

    private async Task<PostFormData> SubmitForm(Form form, string ids, ElasticObject record, IdentityUser user,
        string processId, string taskId, long? wid, bool isApply, CommonFormStructure structure, CancellationToken cancellationToken)
    {
        string culture = CultureHelper.GetCurrentNeutralCulture();
        switch (form.FormType)
        {
            case Form.eFormType.Delete:
            case Form.eFormType.VirtualDelete:
                {
                    (ElasticObject record, string ids) r = await GetRecord(ids, form, user, culture, structure, null, cancellationToken);
                    PostFormData postFormData = new(culture, form, r.record) { structure = structure, EntityPkv=ids, WorkItemId=wid };
                    _ = await postForm.PostDeleteForm(postFormData, wid, user, ids, 0, cancellationToken);
                    return postFormData;
                }
            case Form.eFormType.Edit:
            case Form.eFormType.ProcessCreate:
                {
                    PostFormData postFormData = new(culture, form, record) { structure = structure, EntityPkv = ids, WorkItemId = wid };
                    await postForm.PostEditForm(postFormData, ids, wid, taskId, processId, isApply, user, 0, cancellationToken);
                    return postFormData;
                }
            case Form.eFormType.Create:
                {
                    PostFormData postFormData = new(culture, form, record) { structure = structure, EntityPkv = ids, WorkItemId = wid };
                    await postForm.PostCreateForm(postFormData, null, null, null, user, eCreateType.Save, 0, cancellationToken);
                    return postFormData;
                }
            default:
                throw new NotSupportedException();
        }
    }

    private async Task<(Form form, IdentityUser user, CommonFormStructure structure, PersistenceObject po)> 
        GetFormAndThings(string namespaceId, string entityId, string formId,
        Form.eFormType? formType = null, string formSubject = null, string sortFields = null)
    {
        PersistenceObject po = null;
        Form form = FormStructRoutines.GetForm(namespaceId, entityId, formId, formType, formSubject)
            ?? throw new HttpException(404, Messages.PageNotFound);
        IdentityUser user = GetUser();
        if (!CheckAccess(user, form))
        {
            throw new UnauthorizedAccessException(Messages.PageAccessDenied);
        }

        CommonFormStructure structure = form.FormType == Form.eFormType.Index
             ? await formStructRoutines.GetIndexStructure(CultureHelper.GetCurrentNeutralCulture(), namespaceId, entityId, form.FormSubjectId,
                  formId,
                  string.IsNullOrEmpty(sortFields) ?
                          GetIndexPersistence(form)?.SortFields : sortFields,
                  form, user)
             : formStructRoutines.GetCommonFormStructure(CultureHelper.GetCurrentNeutralCulture(),
            form.NamespaceId, form.EntityId, form.Id, form.FormType, form.FormSubjectId, form, user);
        return structure == null ? throw new Exception("FormStructure not found!") :
            (form, user, structure, po);
    }

    private IdentityUser GetUserAndCheckFormAccess(Form form)
    {
        return !CheckAccess(form, out IdentityUser user, out _) ? throw new Exception(Messages.PageAccessDenied) : user;
    }

    /// <summary>
    /// get the previous Id of current record.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    private static string GetPrevId(Entity entity, string ids)
    {
        return Neighbor(entity, ids, false);
    }

    /// <summary>
    /// get next Id of current record.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    private static string GetNextId(Entity entity, string ids)
    {
        return Neighbor(entity, ids, true);
    }

    private static string Neighbor(Entity entity, string ids, bool readNext)
    {
        return string.Empty;
        /*var q = new QueryUtility(entity).SetPage(1, 1);
        var pkvFields = "";
        foreach (var field in entity.entityFields.Values.Where(f => f.IncludeInPkv))
        {
            q.OrderBy(field.Id, readNext ? SortType.Ascending : SortType.Descending);
            pkvFields = string.IsNullOrEmpty(pkvFields)
                ? field.Id
                : "(" + pkvFields + "+','+" + field.Id + ")";
        }

        return q.ActiveStates().Where(pkvFields + (readNext ? ">" : "<") + "'" + ids + "'")
            .SelectFormulaField("ids", pkvFields).FirstOrDefault()?.GetString("ids");
            */
    }

    private ActionResult RedirectToWorkItems(string caller, string processId, int callerPage)
    {
        if (string.IsNullOrEmpty(caller))
        {
            caller = "IndexWorkItems";
        }

        return RedirectToAction(caller, "Process", new WorkItemsFilter
        {
            __ProcessId = processId,
            Page = callerPage,
            __PageType =
                WorkItemsPageType.MyWorkItems //todo what if caller is IndexWorkItems with pageType of WorkItems 
        });
    }

    private ActionResult RedirectToReceipt(string aiId, string piId, string caller, int callerPage)
    {
        return RedirectToAction("TaskSummary", "Process", new
        {
            aiId,
            piId,
            caller,
            callerPage
        });
    }


    /// <summary>
    /// this action has responsibility to redirect users without authentication or suitable authorization to Error page with appropriate message 
    /// </summary>
    /// <param name="error"></param>
    /// <param name="actionName"></param>
    /// <returns></returns>
    private ActionResult Error(string error, string actionName = null, bool inIframe = false)
    {
        SetInIframe(inIframe);
        return View("~/Views/Error/Error.cshtml", new FormErrorViewModel { ErrorMessage = error });
    }
}
