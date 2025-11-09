using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

namespace Neo.Bpms.UI.MVC.ControllersMethods.PostForms;

public partial class PostForm
{
    public async Task PostCreateForm(PostFormData postFormData, long? wid, string taskId, string processId,
        IdentityUser user, eCreateType createType, long userGroupId, CancellationToken cancellationToken = default)
    {
        postFormData.WorkItemId = wid;
        postFormData.structure = formStructRoutines.GetCreateStructure(postFormData.Culture, postFormData.form.NamespaceId,
            postFormData.form.EntityId, postFormData.form.FormSubjectId, postFormData.form.Id, postFormData.form, user);
        if (postFormData.structure == null || postFormData.form == null)
        {
            _ = postFormData.errors.Add("", Messages.PageNotFound);
            return;
        }

        if (!string.IsNullOrEmpty(postFormData.form.ApplyServiceOperation) &&
            postFormData.form.ApplyFormOperationType == FormOperationType.Apply)
        {
            postFormData.errors = await CallApplyServiceOperation(postFormData, user);
        }
        else
        {
            bool saved = await ApplyCreate(postFormData, postFormData.form.NamespaceId, postFormData.form.EntityId,
                postFormData.form.FormSubjectId, wid, taskId, processId, user, createType,
                postFormData.form.FormType, userGroupId, cancellationToken);
            if (saved && !postFormData.errors.Any() &&
                !string.IsNullOrEmpty(postFormData.form.ApplyServiceOperation) &&
                postFormData.form.ApplyFormOperationType == FormOperationType.AfterApply)
            {
                postFormData.errors = await CallApplyServiceOperation(postFormData, user);
            }
        }
    }

    private async Task<bool> ApplyCreate(PostFormData postFormData, string namespaceId, string entityId, string formSubjectId, long? wid,
        string taskId, string processId, IdentityUser user, eCreateType createType, Form.eFormType formType,
        long userGroupId, CancellationToken cancellationToken=default)
    {
        postFormData.structure.FormType = formType;

        bool saved;
        TriggerTypeId triggerTypeId = postFormData.form.TriggerTypeId(createType == eCreateType.Apply);
        AuditTrail auditTrail = new(triggerTypeId, $"{triggerTypeId} Form {postFormData.form.Id} in entity {entityId}",
            user, userGroupId)
        {
            MetaEntityId = postFormData.form.entity.DbId,
            MetaFormId = postFormData.form.DbId,
            FormId = postFormData.form.Id,
            FlowNodeInstanceId = wid,
        };
        if (wid != null)
        {
            string ids = GetEntityPkvFromActivityInstance(wid);
            _ = postFormData.FormData.SetField("Id", ids ?? postFormData.FormData.GetString("Id"));
            _ = postFormData.FormData.SetField("Ids", ids ?? postFormData.FormData.GetString("Ids"));
            saved = await applyFormData.UpdateRecord(auditTrail, namespaceId, entityId, postFormData.form, postFormData.FormData, postFormData.FormData, null, postFormData.errors, cancellationToken: cancellationToken);
            postFormData.EntityPkv = ids;
        }
        else
        {
            saved = await applyFormData.CreateRecord(auditTrail, namespaceId, entityId, postFormData.form, postFormData.FormData, null, postFormData.errors, cancellationToken: cancellationToken);
        }
        if (saved)
        {
            auditTrail.EntityPkv = postFormData.FormData.GetString("Id") ?? postFormData.FormData.GetString("Ids"); //todo pk
            postFormData.EntityPkv = auditTrail.EntityPkv;
            await applyFormData.UpdateTables(auditTrail, namespaceId, entityId, formSubjectId, postFormData.FormData,
                postFormData.EntityPkv, postFormData.Culture, postFormData.structure.Tables,
                postFormData.errors, cancellationToken);
        }

        if (postFormData.IsValid && postFormData.structure.FormType == Form.eFormType.ProcessCreate)
        {
            RunBpmnEngineOnCreate(postFormData, wid, taskId, processId, createType, auditTrail);
        }
        DataStorage.SaveAudit(auditTrail);
        return saved;
    }

    private void RunBpmnEngineOnCreate(PostFormData postFormData, long? wid, string taskId, string processId,
        eCreateType createType, AuditTrail auditTrail)
    {
        switch (createType)
        {
            case eCreateType.Save:
                if (wid != null)
                {
                    _ = BpmsEngine.ChangeStateByUser(auditTrail, processId, null, taskId,
                        wid.Value, postFormData.WorkDescription, UserTaskInstanceStateId.AllocatedToASingleResource);
                    postFormData.RedirectToWorkItems = true;
                }
                else if (BpmsEngine.CreateProcessInstanceAndCreateUserTask(auditTrail, processId,
                    null, taskId, out wid, postFormData.EntityPkv, postFormData.InputData, postFormData.WorkDescription))
                {
                    postFormData.RedirectToWorkItems = true;
                }

                break;
            case eCreateType.Apply:
                if (wid != null)
                {
                    try
                    {
                        if (BpmsEngine.CompleteTaskByUser(auditTrail,
                            processId, null, taskId, wid.Value, postFormData.WorkDescription))
                        {
                            postFormData.RedirectToWorkItems = true;
                        }
                    }
                    catch (Exception e)
                    {
                        postFormData.AddError(e);
                    }
                }
                else
                {
                    if (BpmsEngine.CreateProcessInstanceAndCompleteUserTask(auditTrail, processId,
                        null, taskId, postFormData.EntityPkv, postFormData.InputData, postFormData.WorkDescription,
                        TimeSpan.Zero, out wid))
                    {
                        postFormData.RedirectToWorkItems = true;
                    }
                }

                break;
        }

        auditTrail.FlowNodeInstanceId = wid;
        postFormData.WorkItemId = wid;
    }
}
