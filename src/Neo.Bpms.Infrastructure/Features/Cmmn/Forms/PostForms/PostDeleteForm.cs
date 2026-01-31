using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

namespace Neo.Bpms.UI.MVC.ControllersMethods.PostForms;

public partial class PostForm
{
    public async Task<bool> PostDeleteForm(PostFormData postFormData, 
        long? wid, IdentityUser user, string ids, long userGroupId, CancellationToken cancellationToken = default)
    {
        postFormData.WorkItemId = wid;
        postFormData.Structure = formStructRoutines.GetDeleteStructure(postFormData.Culture, 
            postFormData.Form.NamespaceId, postFormData.Form.EntityId, postFormData.Form.FormSubjectId,
            postFormData.Form.Id, postFormData.Form, user);
        if (postFormData.Structure == null || postFormData.Form == null)
        {
            postFormData.Errors.Add("", Messages.PageNotFound);
            return false;
        }
        if (!string.IsNullOrEmpty(postFormData.Form.ApplyServiceOperation) && 
            postFormData.Form.ApplyFormOperationType == FormOperationType.Apply)
        {
            postFormData.Errors = await CallApplyServiceOperation(postFormData, user);
        }
        else
        {
            await ApplyDelete(postFormData, postFormData.Form.NamespaceId, postFormData.Form.EntityId, postFormData.Form.FormSubjectId, postFormData.Form.Id, user, userGroupId, ids, cancellationToken);
            if (!string.IsNullOrEmpty(postFormData.Form.ApplyServiceOperation) && postFormData.Form.ApplyFormOperationType == FormOperationType.AfterApply)
            {
                postFormData.Errors = await CallApplyServiceOperation(postFormData, user);
            }
        }
        return postFormData.Errors==null || postFormData.Errors.Count == 0;
    }

    private async Task ApplyDelete(PostFormData postFormData,
        string namespaceId, string entityId, string formSubjectId, string formId, IdentityUser user,
        long userGroupId, string ids, CancellationToken cancellationToken)
    {
        ElasticObject record = postFormData.FormData;
        if (record == null)
        {
            postFormData.Errors.Add("", Messages.RecordNotFound);
            return;
        }

        AuditTrail auditTrail = new(TriggerTypeId.DeleteForm, TriggerTypeId.DeleteForm.ToString(),
            user, userGroupId)
        {
            FormId = postFormData.Form.Id
        };
        bool success = await applyFormData.Delete(auditTrail, postFormData.Form.NamespaceId,
            postFormData.Form.EntityId, postFormData.Form, record, postFormData.Errors, cancellationToken);
        if (success)
            DataStorage.SaveAudit(auditTrail);
        else
        {
            postFormData.Errors ??= new ExceptionInfos().Add("", Messages.DeleteFailed);
        }
    }
}
