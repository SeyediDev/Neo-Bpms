using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

namespace Neo.Bpms.UI.MVC.ControllersMethods.PostForms;

public partial class PostForm
{
    public async Task<bool> PostDeleteForm(PostFormData postFormData, 
        long? wid, IdentityUser user, string ids, long userGroupId, CancellationToken cancellationToken = default)
    {
        postFormData.WorkItemId = wid;
        postFormData.structure = formStructRoutines.GetDeleteStructure(postFormData.Culture, 
            postFormData.form.NamespaceId, postFormData.form.EntityId, postFormData.form.FormSubjectId,
            postFormData.form.Id, postFormData.form, user);
        if (postFormData.structure == null || postFormData.form == null)
        {
            postFormData.errors.Add("", Messages.PageNotFound);
            return false;
        }
        if (!string.IsNullOrEmpty(postFormData.form.ApplyServiceOperation) && 
            postFormData.form.ApplyFormOperationType == FormOperationType.Apply)
        {
            postFormData.errors = await CallApplyServiceOperation(postFormData, user);
        }
        else
        {
            await ApplyDelete(postFormData, postFormData.form.NamespaceId, postFormData.form.EntityId, postFormData.form.FormSubjectId, postFormData.form.Id, user, userGroupId, ids, cancellationToken);
            if (!string.IsNullOrEmpty(postFormData.form.ApplyServiceOperation) && postFormData.form.ApplyFormOperationType == FormOperationType.AfterApply)
            {
                postFormData.errors = await CallApplyServiceOperation(postFormData, user);
            }
        }
        return postFormData.errors==null || postFormData.errors.Count == 0;
    }

    private async Task ApplyDelete(PostFormData postFormData,
        string namespaceId, string entityId, string formSubjectId, string formId, IdentityUser user,
        long userGroupId, string ids, CancellationToken cancellationToken)
    {
        ElasticObject record = postFormData.FormData;
        if (record == null)
        {
            postFormData.errors.Add("", Messages.RecordNotFound);
            return;
        }

        AuditTrail auditTrail = new(TriggerTypeId.DeleteForm, TriggerTypeId.DeleteForm.ToString(),
            user, userGroupId)
        {
            FormId = postFormData.form.Id
        };
        bool success = await applyFormData.Delete(auditTrail, postFormData.form.NamespaceId,
            postFormData.form.EntityId, postFormData.form, record, postFormData.errors, cancellationToken);
        if (success)
            DataStorage.SaveAudit(auditTrail);
        else
        {
            postFormData.errors ??= new ExceptionInfos().Add("", Messages.DeleteFailed);
        }
    }
}
