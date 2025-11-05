using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Features.Security;
using Neo.Bpms.Domain.Model.BPMN.Processes;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Domain.Models.WorkManagement;
using Neo.Bpms.Infrastructure.Features.Bpms.Engine;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;
using Neo.Bpms.UI.MVC.ViewModels.Forms;

namespace Neo.Bpms.UI.MVC.Controllers;

public class ChangeUserModel
{
    public IList<TaskAddressing> Activities { get; set; }
    public string NewUserId { get; set; }
}

public partial class ProcessController
{
    private const int MaxBulkRecords = 5000;
    /// <summary>
    /// any submit action on workItem page will cause this action to get Executed.
    /// check the user is authorized to make post action in work item page, otherwise user get redirected to error page with appropriate message.
    /// </summary>		
    [HttpPost]
    public async Task<ActionResult> DoWorkItems(WorkItemsFilter filter, BulkQueryType queryType,
        List<string> aiList, [ModelBinder(typeof(DynamicActionBinder))] ElasticObject formData,
         string indexFormId, string indexFormFilterValues, string caller, string isApply = "0")
    {
        IdentityUser user = GetUser();
        long userGroupId = FindUserGroupIdForDoWorkItems(user);
        WorkItemsQueryType workItemsQueryType = WorkItemsQueryType.Process;
        if (filter.PageType == WorkItemsPageType.MyWorkItems)
        {
            filter.UserId = user.Id;
            workItemsQueryType = WorkItemsQueryType.MyWorkItems;
        }
        DoFilterConversions(filter);
        bool shouldCompleteTask = isApply == "1";
        UiEntity entity = null;
        Process process = null;
        string wfVersion = string.Empty;
        UserTask userTask = null;
        Dictionary<string, Form> formDic = [];
        switch (queryType)
        {
            case BulkQueryType.Query:
                EntityFilterInfo entityFilter = await GetEntityFilter(filter, indexFormId, indexFormFilterValues, user);
                string culture = CultureHelper.GetCurrentNeutralCulture();
                List<WorkItemViewModel> workItems = workItemManager.GetWorkItems(out _, MaxBulkRecords,
                    workItemsQueryType, user, culture, filter, entityFilter);
                if (workItems == null || workItems.Count == 0)
                    throw new Exception("در فعالیت های انتخاب شده فعالیت نامعتبر وجود دارد.");

                foreach (WorkItemViewModel workItem in workItems)
                {
                    string formKey =
                        $"{workItem.NamespaceId},{workItem.EntityId},{workItem.ProcessId},{workItem.ProcessVersion},{workItem.TaskId}";
                    if (!formDic.TryGetValue(formKey, out Form form))
                    {
                        FetchEntity(workItem.NamespaceId, workItem.EntityId, ref entity);
                        if (entity != null)
                        {
                            FetchUserTask(workItem.ProcessId, workItem.ProcessVersion, workItem.TaskId,
                                ref process, ref wfVersion, ref userTask);
                            if (process != null && userTask != null)
                            {
                                form = entity.GetEntityForm(userTask.FormId);
                                if (form != null)
                                {
                                    if (!CheckAccess(user, form))
                                        form = null;
                                }
                            }
                        }

                        formDic.Add(formKey, form);
                    }
                    if (form == null) continue;

                    await SaveRecordAndCompleteTask(formData, workItem.ProcessId, workItem.ProcessVersion, 
                        workItem.TaskId, workItem.ActivityInstanceId, workItem.EntityPkv,
                        form, shouldCompleteTask, user, userGroupId);
                }

                break;
            case BulkQueryType.List:
                if (aiList == null || aiList.Count == 0) break;
                QueryUtility qAiRecords = QueryUtility<ActivityInstanceRecord>
                    .Where($"{nameof(ActivityInstanceRecord.Id)} In ({string.Join(",", aiList.Select(a => $"'{a}'"))})")
                    .Where($"{nameof(ActivityInstanceRecord.Closed)} != 1")
                    .Where($"{nameof(ActivityInstanceRecord.userTaskStateId)} < {UserTaskInstanceStateId.Completed:D}")
                    .Where($"{nameof(ActivityInstanceRecord.StateId)} < {ActivityInstanceStateId.Completed:D}")
                    .SelectField(nameof(ActivityInstanceRecord.Id))
                    .SelectField(nameof(ActivityInstanceRecord.BPMNFlowNodeId));
                qAiRecords.Include(nameof(ActivityInstanceRecord.ProcessInstance))
                    .Where($"{nameof(ProcessInstanceRecord.StateId)} < {ProcessInstanceStateId.Completed:D}")
                    .SelectField(nameof(ProcessInstanceRecord.EntityPKV));

                List<ElasticObject> aiRecords = qAiRecords.ToList();
                if (aiRecords.Count != aiList.Count)
                    throw new Exception("در فعالیت های انتخاب شده فعالیت نامعتبر وجود دارد.");
                FetchUserTask(filter.ProcessId, filter.ProcessVersionId, filter.Activity, ref process, ref wfVersion, ref userTask);
                foreach (ElasticObject ai in aiRecords ?? Enumerable.Empty<ElasticObject>())
                {
                    long bpmnFlowNodeId = ai.GetLong(nameof(ActivityInstanceRecord.BPMNFlowNodeId));
                    string formKey = bpmnFlowNodeId.ToString();
                    if (!formDic.TryGetValue(formKey, out Form form))
                    {
                        UserTaskRuntime userTaskRuntime = (UserTaskRuntime)((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.GetFlowNodeRunTime(bpmnFlowNodeId);
                        if (userTaskRuntime?.RenderingFormDbId != null)
                        {
                            ProjectDefinition.Forms.TryGetValue(userTaskRuntime.RenderingFormDbId.Value, out form);
                            if (form != null)
                            {
                                if (!CheckAccess(user, form))
                                    form = null;
                            }
                        }

                        formDic.Add(formKey, form);
                    }
                    if (form == null) continue;
                    await SaveRecordAndCompleteTask(formData, process.Id, wfVersion, filter.Activity,
                        ai.Id, ai.GetString(nameof(ProcessInstanceRecord.EntityPKV)),
                        form, shouldCompleteTask, user, userGroupId);
                }
                break;
        }
        string action = string.IsNullOrEmpty(caller)
            ? filter.PageType == WorkItemsPageType.MyWorkItems ? "MyWorkItems" : "WorkItems"
            : caller;
        return RedirectToAction(action, filter); // todo IndexWorkItems, todo either filter parameter or just-needed fields
    }

    //todo contains repeated code (eg with indexworkitems)
    private async Task<EntityFilterInfo> GetEntityFilter(WorkItemsFilter filter, string indexFormId, string indexFormFilterValues, IdentityUser user)
    {
        if (string.IsNullOrEmpty(filter.ProcessId)) return null;
        Process process = ProjectDefinition.Project.GetBpmnDefinition(filter.ProcessId, filter.ProcessVersionId)?.Process;
        if (process == null) return null;
        ElasticObject indexFilterValues = controllerMethods.DecodeFilterValues(indexFormFilterValues);
        Form indexForm = (Form)(ProjectDefinition.Project.GetEntity(process.EntityNamespaceId,
             process.EntityId) as UiEntity)?.getForms()?.FirstOrDefault(f => f.Id == indexFormId) ?? throw new Exception("چنین فرمی یافت نشد.");
        CommonFormStructure structure = await formStructRoutines.GetIndexStructure(CultureHelper.GetCurrentNeutralCulture(),
                 process.EntityNamespaceId, process.EntityId, indexForm.FormSubjectId, indexForm.Id, "", indexForm, user) ?? throw new Exception(Messages.PageNotFound);
        return new EntityFilterInfo(structure, indexForm, indexFilterValues, "");
    }

    private static void FetchUserTask(string processId, string versionId, string taskId,
      ref Process process, ref string processVersionId, ref UserTask userTask)
    {
        if (process?.Id != processId || processVersionId != versionId)
        {
            process = ProjectDefinition.Project.GetBpmnDefinition(processId, versionId)?.Process;
            processVersionId = versionId;
            userTask = null;
        }
        if (userTask?.Id != taskId)
            userTask = process?.GetActivity(taskId) as UserTask;
    }

    private static void FetchEntity(string namespaceId, string entityId, ref UiEntity entity)
    {
        if (entity?.NamespaceId != namespaceId || entity?.Id != entityId)
            entity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
    }

    private async Task SaveRecordAndCompleteTask(ElasticObject formData,
        string processId, string processVersion, string taskId,
        long? aid, string entityPkv, Form form, bool completeTask, IdentityUser user, long userGroupId)
    {
        bool b = true;
        AuditTrail auditTrail = new(TriggerTypeId.SaveRecordAndCompleteTask,
                TriggerTypeId.SaveRecordAndCompleteTask.ToString(), user, userGroupId)
        { FormId = form.Id, MetaFormId = form.DbId, MetaEntityId = form.entity.DbId };
        PostFormData postEditForm = new(null, form, formData);
        if (formData != null)
        {
            ElasticObject keyRecord = FormDataRoutines.GetKeyRecord(form.entity, entityPkv);
            formData.Merge(keyRecord);
            b = await applyFormData.UpdateRecord(auditTrail, form.entity.NamespaceId, form.entity.Id,
                form, formData, keyRecord, null, postEditForm.errors);
        }
        if (b && completeTask && aid != null)
            b = ((BpmsEngine)bpmsEngine).CompleteTaskByUser(auditTrail, processId, processVersion, taskId, aid.Value, postEditForm.WorkDescription);
        if (!b)
        {

        }
        else
            DataStorage.SaveAudit(auditTrail);
    }


    /// <summary>
    /// change work item state by user
    /// </summary>
    /// <returns></returns>
    //		[HttpPost]
    //		public JsonResult ChangeStateByUser(string processId, string taskId, long? wiId, string newState,
    //			[ModelBinder(typeof(DynamicActionBinder))] ElasticObject re)
    //		{
    //			var user = GetUser();
    //			var state = (UserTaskInstanceStateId)Enum.Parse(typeof(UserTaskInstanceStateId), newState);
    //			var b = wiId != null && ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).ChangeStateByUser(processId, null, taskId, wiId.Value, 
    //				        null/*todo کاربر بتواند هنگام تغییر وضعیت شرح را نیز عوض کند*/, user, state);
    //			return Json(b, JsonRequestBehavior.AllowGet);
    //		}

    /// <summary>
    /// change User.
    /// </summary>		
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ChangeUser([FromBody] ChangeUserModel model)
    {
        IdentityUser user = GetUser();
        IdentityUser? newUser=null;
        IIdentityUserService identityUserService = HttpContext.RequestServices.GetRequiredService<IIdentityUserService>();
        
        newUser = await identityUserService.GetIdentityUserAsync(model.NewUserId, default);
        
        if (!(model.Activities?.Any() ?? false))
            throw new ValidationException("کاری برای تخصیص انتخاب نشده است.");
        foreach (TaskAddressing activity in model.Activities)
        {
            long userGroupId = FetchTaskUserGroup(user, activity);
            AuditTrail auditTrail = new(TriggerTypeId.ChangeUser, user.Id, user, userGroupId);
            ExecutionInstance execution = new(auditTrail, null/*todo کاربر بتواند هنگام تغییر کاربر شرح را نیز عوض کند*/);
            ActivityResourceManager activityResourceManager = new(activity.ProcessId, activity.ProcessVersion,
                activity.TaskId, user, activity.ActivityInstanceId, 1, 1000, execution);
            IList<UserViewModel> users = activityResourceManager.GetUsers();
            if (users.All(u => u.Id != model.NewUserId))
                throw new ValidationException(".این کاربر مجاز به انجام این کار نیست");
            if (users.All(u => u.Id != user.Id) && !user.IsAdmin)
                throw new ValidationException("شما مجاز به تخصیص این کار به این کاربر نیستید.");
            if (newUser == null || !((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).ChangeUser(
                activity.ProcessId, activity.ProcessVersion,
                     activity.TaskId, activity.ActivityInstanceId,
                UserTaskInstanceStateId.AllocatedToASingleResource, newUser, userGroupId, execution))
                throw new Exception("تخصیص ناموفق");
        }
        return Json(new UserViewModel
        {
            Id = newUser.Id,
            Name = newUser.FirstName,
            Family = newUser.LastName,
            Username = newUser.UserName,
            AvatarId = newUser.AvatarPicture
        });
    }


    [HttpPost]
    public JsonResult RestoreTask([FromBody] TaskAddressing task)
    {
        IdentityUser user = GetUser();
        // todo Access 
        long userGroupId = FetchTaskUserGroup(user, task);
        return Json(((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).RestoreTask(task, user, userGroupId));
    }
    private long FetchTaskUserGroup(IdentityUser user, TaskAddressing taskAddressing)
    {
        return 0;//todo
    }
    private long FindUserGroupIdForDoWorkItems(IdentityUser user)
    {
        return 0;//todo
    }
}
