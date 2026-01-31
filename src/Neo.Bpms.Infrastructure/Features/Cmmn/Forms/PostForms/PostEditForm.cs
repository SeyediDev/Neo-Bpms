using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

namespace Neo.Bpms.UI.MVC.ControllersMethods.PostForms;

public partial class PostForm
{
    public async Task PostEditForm(PostFormData postFormData,
        string entityPkv, long? wid, string taskId, string processId,
        bool isApply, IdentityUser user, long userGroupId, CancellationToken cancellationToken = default)
    {
        if (!LoadForm(postFormData, user))
        {
            return;
        }

        if (!string.IsNullOrEmpty(postFormData.Form.ApplyServiceOperation) && 
            postFormData.Form.ApplyFormOperationType == FormOperationType.Apply)
        {
            postFormData.Errors = await CallApplyServiceOperation(postFormData, user);
        }
        else
        {
            await ApplyEdit(postFormData, postFormData.Form.NamespaceId, postFormData.Form.EntityId, 
                postFormData.Form.FormSubjectId, postFormData.EntityPkv, wid, taskId, processId, user, isApply, userGroupId, cancellationToken);
            if (!string.IsNullOrEmpty(postFormData.Form.ApplyServiceOperation) && 
                postFormData.Form.ApplyFormOperationType == FormOperationType.AfterApply)
            {
                postFormData.Errors = await CallApplyServiceOperation(postFormData, user);
            }
        }
    }

    private async Task ApplyEdit(PostFormData postFormData, string namespaceId, string entityId, string formSubjectId, 
        string entityPkv,
        long? wid, string taskId, string processId, IdentityUser user, bool isApply,
        long userGroupId, CancellationToken cancellationToken)
    {
        postFormData.WorkItemId = wid;
        if (!string.IsNullOrEmpty(taskId) && wid != null)
        {
            if (!CheckWorkItemState(postFormData, ref entityPkv, wid.Value, taskId))
            {
                return;
            }
        }

        postFormData.EntityPkv = postFormData.EntityPkv;
        TriggerTypeId triggerTypeId = postFormData.Form.TriggerTypeId(isApply);
        AuditTrail auditTrail =
            new(triggerTypeId, $"{triggerTypeId}postFormData.form {postFormData.Form.Id} in entity {entityId}", user, userGroupId)
            {
                MetaEntityId = postFormData.Form.entity.DbId,
                MetaFormId = postFormData.Form.DbId,
                FormId = postFormData.Form.Id,
                FlowNodeInstanceId = wid,
                TraceCode = taskId,
                EntityPkv = postFormData.EntityPkv
            };
        await UpdateForm(postFormData, namespaceId, entityId, formSubjectId, taskId, auditTrail, cancellationToken);
        if (postFormData.IsValid)
        {
            RunBpmnEngineOnEdit(postFormData, auditTrail, taskId, processId, isApply);
        }

        DataStorage.SaveAudit(auditTrail);
    }

    private bool LoadForm(PostFormData postFormData, IdentityUser user)
    {
        postFormData.Structure = formStructRoutines.GetEditStructure(postFormData.Culture, postFormData.Form?.NamespaceId,
            postFormData.Form?.EntityId, postFormData.Form?.FormSubjectId, postFormData.Form?.Id, postFormData.Form, user);
        if (postFormData.Structure == null || postFormData.Form == null)
        {
            _ = postFormData.Errors.Add("", Messages.PageNotFound);
            return false;
        }

        postFormData.Structure.FormType = postFormData.Form?.FormType ?? Form.eFormType.Edit;
        return true;
    }

    private async Task UpdateForm(PostFormData postFormData, string namespaceId, string entityId, 
        string formSubjectId, string taskId, AuditTrail auditTrail, CancellationToken cancellationToken)
    {
        bool updateRecord;
        if (!string.IsNullOrEmpty(taskId) && string.IsNullOrEmpty(postFormData.EntityPkv))
        {
            updateRecord = await applyFormData.CreateRecord(auditTrail, 
                namespaceId, entityId, postFormData.Form, postFormData.FormData, null, 
                postFormData.Errors, null, null, cancellationToken);
            EntityField keyField = postFormData.Form.Entity.KeyFields.FirstOrDefault();
            postFormData.EntityPkv = keyField != null
                ? postFormData.FormData.GetString(keyField.Id)
                : postFormData.FormData.GetString("Id") ?? postFormData.FormData.GetString("Ids"); //todo
        }
        else
        {
            updateRecord = await applyFormData.UpdateRecord(auditTrail, namespaceId, entityId, 
                postFormData.Form, postFormData.FormData, 
                FormDataRoutines.GetKeyRecord(postFormData.Form.entity, postFormData.EntityPkv), 
                null, postFormData.Errors, cancellationToken: cancellationToken);
        }

        auditTrail.EntityPkv = postFormData.EntityPkv;
        if (updateRecord && !(postFormData.Errors?.Any() ?? false))
        {
            await applyFormData.UpdateTables(auditTrail, namespaceId, entityId, formSubjectId, 
                postFormData.FormData, postFormData.EntityPkv, postFormData.Culture, 
                postFormData.Structure.Tables, postFormData.Errors, cancellationToken);
        }
    }

    private bool CheckWorkItemState(PostFormData postFormData, ref string ids, long wid, string taskId)
    {
        ActivityInstanceRecordDb ai = BpmsEngine.FetchActivityInstanceRecord(wid);
        if (ai == null ||
            ai.Closed ||
            ai.userTaskStateId > UserTaskInstanceStateId.Completed ||
            ai.StateId >= (long)ActivityInstanceStateId.Completed)
        {
            postFormData.Errors.Add(null, Messages.WorkAlreadyDone);
            return false;
        }

        ProcessInstanceRecordDb pi = BpmsEngine.FetchProcessInstanceRecord(ai.ProcessInstanceId);
        postFormData.EntityPkv = pi?.EntityPKV;
        if (string.IsNullOrEmpty(postFormData.EntityPkv))
        {
            logger.LogCritical("Edit get 208 {ids} {EntityPkv} {taskId} {wid}", ids, postFormData.EntityPkv, taskId, wid);
            postFormData.Errors.Add(null, "این پرونده در این فرآیند قابل اجرا نیست.");
            return false;
        }

        if (string.IsNullOrEmpty(ids))
        {
            ids = postFormData.EntityPkv;
        }
        else if (ids != postFormData.EntityPkv)
        {
            logger.LogCritical("Edit get 208 {ids} {EntityPkv} {taskId} {wid}", ids, postFormData.EntityPkv, taskId, wid);
            postFormData.Errors.Add(null, "این کار برای این پرونده نیست.");
            return false;
        }
        return true;
    }

    private void RunBpmnEngineOnEdit(PostFormData postFormData, AuditTrail auditTrail, string taskId, string processId, bool isApply)
    {
        switch (postFormData.Structure.FormType)
        {
            case Form.eFormType.ProcessCreate when isApply:
                    if (postFormData.WorkItemId != null)
                    {
                        try
                        {
                            BpmsEngine.CompleteTaskByUser(auditTrail, processId, null, taskId, postFormData.WorkItemId.Value,
                                postFormData.WorkDescription);
                            postFormData.RedirectToWorkItems = true;
                        }
                        catch (Exception e)
                        {
                            postFormData.AddError(e);
                        }
                    }
                    else
                    {
                        if (BpmsEngine.CreateProcessInstanceAndCompleteUserTask(auditTrail,
                            processId, null, taskId, postFormData.EntityPkv, postFormData.InputData, postFormData.WorkDescription,
                            TimeSpan.Zero, out long? wid))
                        {
                            postFormData.RedirectToWorkItems = true;
                            postFormData.WorkItemId = wid;
                        }
                    }
                    break;
            case Form.eFormType.ProcessCreate when postFormData.WorkItemId != null:
                BpmsEngine.ChangeStateByUser(auditTrail, processId, null, taskId, postFormData.WorkItemId.Value,
                    postFormData.WorkDescription, UserTaskInstanceStateId.AllocatedToASingleResource);
                postFormData.RedirectToWorkItems = true;
                break;
            case Form.eFormType.ProcessCreate:
                {
                    if (BpmsEngine.CreateProcessInstanceAndCreateUserTask(auditTrail, processId,
                        null, taskId, out long? wid, postFormData.EntityPkv, postFormData.InputData, postFormData.WorkDescription))
                    {
                        postFormData.RedirectToWorkItems = true;
                        postFormData.WorkItemId = wid;
                    }
                    break;
                }

            case Form.eFormType.ProcessCreateOnExistingRecord when isApply:
                {
                    if (BpmsEngine.CreateProcessInstanceAndCompleteUserTask(auditTrail, processId,
                        null, taskId, postFormData.EntityPkv, postFormData.InputData, postFormData.WorkDescription, TimeSpan.Zero, out long? wid))
                    {
                        postFormData.RedirectToWorkItems = true;
                        postFormData.WorkItemId = wid;
                    }
                    break;
                }

            case Form.eFormType.ProcessCreateOnExistingRecord:
                {
                    if (BpmsEngine.CreateProcessInstanceAndCreateUserTask(auditTrail, processId,
                        null, taskId, out long? wid, postFormData.EntityPkv, postFormData.InputData, postFormData.WorkDescription))
                    {
                        postFormData.RedirectToWorkItems = true;
                        postFormData.WorkItemId = wid;
                    }

                    break;
                }

            default:
                {
                    if (postFormData.Form.FormType == Form.eFormType.WorkItem && postFormData.WorkItemId != null)
                    {
                        if (isApply)
                        {
                            try
                            {
                                BpmsEngine.CompleteTaskByUser(auditTrail, processId, null, taskId, postFormData.WorkItemId.Value,
                                    postFormData.WorkDescription);
                                postFormData.RedirectToWorkItems = true;
                            }
                            catch (Exception e)
                            {
                                postFormData.AddError(e);
                            }
                        }
                        else
                        {
                            BpmsEngine.ChangeStateByUser(auditTrail, processId, null, taskId,
                                postFormData.WorkItemId.Value, postFormData.WorkDescription, UserTaskInstanceStateId.AllocatedToASingleResource);
                        }
                    }

                    break;
                }
        }
    }
}
