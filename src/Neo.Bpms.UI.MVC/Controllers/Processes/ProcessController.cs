using Neo.Bpms.Domain.Features.Bpms;
using Neo.Bpms.Domain.Model.BPMN.Processes;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeOperation;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;
using Neo.Bpms.UI.MVC.Features;
using Microsoft.Extensions.Options;
using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.WorkManagement;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Infrastructure.Features.Bpms.Engine;
using Neo.Bpms.Infrastructure.Features.Bpms.Interfaces;

namespace Neo.Bpms.UI.MVC.Controllers;

public class GetActivityListModel
{
    public string ProcessId { get; set; }
    public string VersionNo { get; set; }
}

public partial class ProcessController(IBpmsEngine bpmsEngine, IApplyFormData applyFormData,
    ControllerMethods controllerMethods,
    FormStructRoutines formStructRoutines,
    WorkItemManager workItemManager,
    FilterManager filterController,
    FilterConfigBackupRestore filterConfigBackupRestore, IOptions<CmmnSettings> cmmnSettings
    ) : BpmsController
{
    private const int RecordsPerPage = 15;
    //private readonly IWebHostEnvironment _environment;

    //public ProcessController(IWebHostEnvironment environment)
    //{
    //    _environment = environment;
    //}
    /// <summary>
    /// view work items of process
    /// check if user in valid to view this page, otherwise you get redirected to error page with appropriate message.
    /// </summary>		
    [HttpGet]
    public ActionResult WorkItems(WorkItemsFilter filter, string dummy)
    {
        IdentityUser user = CheckWorkItemsAccess(WorkItemsPageType.WorkItems, filter.ProcessId);
        ViewBag.PageTitle = "کارهای فرآیند ";
        filter.SetDefaultValues();
        filter.__PageType = WorkItemsPageType.WorkItems;
        return GetWorkItems(WorkItemsQueryType.Process, user, filter);
    }

    /// <summary>
    /// any submit action on workItem page will cause this action to get Executed.
    /// check the user is authorized to make post action in workItem page, otherwise user get redirected to error page with appropriate message.
    /// </summary>		
    [HttpPost]
    public ActionResult WorkItems(WorkItemsFilter filter)
    {
        if (filter.PageType == WorkItemsPageType.MyWorkItems)
        {
            return MyWorkItems(filter);
        }

        string processId = filter.ProcessId;
        IdentityUser user = CheckWorkItemsAccess(filter.PageType, processId);
        ViewBag.PageTitle = "کارهای فرآیند ";
        filter.__PageType = WorkItemsPageType.WorkItems;
        return GetWorkItems(WorkItemsQueryType.Process, user, filter);
    }

    /// <summary>
    /// get list of workItems for current user.(logged in user)
    /// check if user in valid to view this page, otherwise you get redirected to error page with appropriate message.
    /// </summary>		
    [HttpGet]
    public ActionResult MyProcesses()
    {
        IdentityUser user = CheckWorkItemsAccess(WorkItemsPageType.MyWorkItems, null);
        ViewBag.PageTitle = "کارهای ورودی ";
        List<ProcessItemsViewModel> list = workItemManager.GetMyProcessesWorkItems(user);
        return list.Count == 1
            ? RedirectToAction("IndexWorkItems", new WorkItemsFilter
            {
                __ProcessId = list[0].ProcessId
            })
            : View(list);
    }

    /// <summary>
    /// get list of workItems for current user.(logged in user)
    /// check if user in valid to view this page, otherwise you get redirected to error page with appropriate message.
    /// </summary>		
    [HttpGet]
    public ActionResult MyWorkItems(WorkItemsFilter filter, string dummy)
    {
        IdentityUser user = CheckWorkItemsAccess(WorkItemsPageType.MyWorkItems, null);
        ViewBag.PageTitle = "کارهای ورودی ";
        filter.SetDefaultValues();
        filter.__PageType = WorkItemsPageType.MyWorkItems;
        filter.UserId = user.Id;
        return GetWorkItems(WorkItemsQueryType.MyWorkItems, user, filter);
    }

    /// <summary>
    /// get list of workItems for current user.(logged in user)
    /// check if user in authorized to post this page, otherwise you get redirected to error page with appropriate message.
    /// </summary>
    [HttpPost]
    public ActionResult MyWorkItems(WorkItemsFilter filter)
    {
        filter.__PageType = WorkItemsPageType.MyWorkItems;
        IdentityUser user = CheckWorkItemsAccess(filter.PageType, filter.ProcessId);
        ViewBag.PageTitle = "کارهای ورودی ";
        filter.UserId = user.Id;
        return GetWorkItems(WorkItemsQueryType.MyWorkItems, user, filter);
    }

    public JsonResult ServiceStates([FromBody] IList<TaskAddressing> tasks)
    {
        List<ServiceStateResult> result = [];
        if (tasks == null)
        {
            return Json(result);
        }

        List<ActivityInstanceRecord> airs = QueryUtility<ActivityInstanceRecord>
            .Where($"Id In('{string.Join(",", tasks.Select(task => task.ActivityInstanceId))}')")
            .ToList<ActivityInstanceRecord>();
        foreach (ActivityInstanceRecordDb air in airs ?? Enumerable.Empty<ActivityInstanceRecordDb>())
        {
            IBPMNOperationRuntime runTime = (IBPMNOperationRuntime)((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.GetFlowNodeRunTime(air.BPMNFlowNodeId);
            OperationResourceRuntime allocatedResource = runTime?.BPMNOperationRuntime.GetAllocatedResource(air.Id);
            try
            {
                result.Add(new ServiceStateResult
                {
                    ActivityInstanceId = air.Id,
                    ServiceTaskState = air.userTaskStateId,
                    Progress = allocatedResource?.Progress
                });
            }
            catch
            {
                result.Add(new ServiceStateResult
                {
                    ActivityInstanceId = air.Id,
                    ServiceTaskState = air.userTaskStateId,
                    Progress = 100
                });
            }
        }

        return Json(result);
    }

    /// <summary>
    /// Get Activities List
    /// </summary>		
    /// <returns></returns>
    [HttpPost]
    public JsonResult GetActivityList([FromBody] GetActivityListModel model)
    {
        _ = GetUser();
        Infrastructure.Features.Bpms.Loader.Repository repository = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository;
        if (string.IsNullOrEmpty(model.ProcessId) || !repository.processesRunTimes.TryGetValue(model.ProcessId, out ProcessRunTime value) ||
            string.IsNullOrEmpty(model.VersionNo) ||
            !value.Versions.ContainsKey(model.VersionNo))
        {
            return Json("ورودی نادرست");
        }

        ComboData cd = new("");
        ProcessVersionRuntime procVersion = value.Versions[model.VersionNo];
        foreach (KeyValuePair<string, FlowNodeRunTime> item in procVersion.nodes)
        {
            if (item.Value is UserTaskRuntime)
            {
                cd.AddRow(new FormDataRow { Ids = item.Key, DisplayValue = item.Value.flowNode.Name });
            }
        }

        return Json(cd);
    }

    [HttpPost]
    public JsonResult GetActivitiesUsers([FromBody] IList<TaskAddressing> activities, [FromQuery] int pageNo, [FromQuery] int pageSize)
    {
        IdentityUser user = GetUser();
        if (!(activities?.Any() ?? false))
        {
            throw new ValidationException("لطفا ابتدا کاری را انتخاب نمایید.");
        }

        HashSet<UserViewModel> activitiesUsers = []; // todo concepts of pageNo and pageSize!
        AuditTrail auditTrail = new(TriggerTypeId.ChangeUser, user.Id, user, 0);
        ExecutionInstance execution = new(auditTrail, null);
        foreach (TaskAddressing activity in activities)
        {
            ActivityResourceManager activityResourceManager = new(activity.ProcessId,
                activity.ProcessVersion, activity.TaskId,
                user, activity.ActivityInstanceId, pageNo, pageSize, execution);
            if (!activityResourceManager.LoadNormally)
            {
                continue;
            }

            IList<UserViewModel> users = activityResourceManager.GetUsers();
            if (activitiesUsers.Count == 0)
            {
                activitiesUsers.UnionWith(users);
            }
            else
            {
                activitiesUsers.IntersectWith(users);
            }
        }

        return Json(activitiesUsers);
    }

    public ActionResult Form(string taskId, string __processId)
    {
        _ = GetUser();
        Form form = GetProcessTaskForm(__processId, taskId);
        return form == null
            ? Error(Messages.PageAccessDenied)
            : RedirectToAction("Edit", "Form",
            new
            {
                form.entity.NamespaceId,
                form.EntityId,
                form.FormSubjectId,
                FormId = form.Id,
                TaskId = taskId,
                ProcessId = __processId
            });
    }

    [HttpGet]
    public ActionResult EmbedFrame(string embedFolder, string defaultPage = "index.html")
    {
        //var path = Path.Combine(_environment.WebRootPath, "EmbedFrame", embedFolder);
        //if (!Directory.Exists(path))
        //    return Error(Messages.PageNotFound);

        //var pathDefaultPage = Path.Combine(path, defaultPage);
        //if (!System.IO.File.Exists(pathDefaultPage))
        //    return Error(Messages.PageNotFound);
        string pathDefaultPage = "http://.:8082/";
        return View("EmbedFrame", pathDefaultPage);
    }

    private static Form GetProcessTaskForm(string processId, string taskId)
    {
        return ProjectDefinition.Project.GetProcessTaskForm(processId, taskId);
    }

    /// <summary>
    /// get whole workItems related to current Filters and current user(logged in user)
    /// </summary>	    
    private ActionResult GetWorkItems(WorkItemsQueryType workItemsQueryType, IdentityUser user,
        WorkItemsFilter filter)
    {
        DoFilterConversions(filter);
        string culture = CultureHelper.GetCurrentNeutralCulture();
        List<WorkItemViewModel> workItems = workItemManager.GetWorkItems(out long recordCount, RecordsPerPage,
        workItemsQueryType, user, culture, filter, null);
        WorkItemsViewModel result = new(workItems, filter, null, recordCount);
        ViewBag.Page = filter.Page;
        ViewBag.recordsPerPage = RecordsPerPage;
        ViewBag.ProcessId = filter.ProcessId;
        BpmnDefinitions bpmnDefinitions = !string.IsNullOrEmpty(filter.ProcessId)
            ? ProjectDefinition.Project.GetBpmnDefinition(filter.ProcessId, filter.ProcessVersionId)
            : null;
        ViewBag.processName = bpmnDefinitions?.Name;
        if (filter.PageType == WorkItemsPageType.MyWorkItems)
        {
            FillProcessList(null);
        }

        Process process = bpmnDefinitions?.GetRootElement(filter.ProcessId) as Process;
        SetActivitiesListViewBag(process);
        return workItemsQueryType == WorkItemsQueryType.MyWorkItems ? View("MyWorkItems", result) : (ActionResult)View("WorkItems", result);
    }

    private void SetActivitiesListViewBag(Process process)
    {
        ViewBag.activityList = new List<FormDataRow>();
        IEnumerable<Activity> activities = FetchActivities(process);
        foreach (Activity item in activities)
        {
            ViewBag.activityList.Add(new FormDataRow { Ids = item.Id, DisplayValue = item.Name });
        }
    }

    private static void DoFilterConversions(WorkItemsFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.FromCreationDateText))
        {
            filter.FromCreationDate = filter.FromCreationDateText.ToDateTimeFromMiladi();
        }

        if (!string.IsNullOrEmpty(filter.ToCreationDateText))
        {
            filter.ToCreationDate = filter.ToCreationDateText.ToDateTimeFromMiladi();
        }
    }

    private void FillProcessList(Process sameEntityProcess)
    {
        ICollection<ProcessRunTime> processRunTimes = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.processesRunTimes.Values;
        ViewBag.processList = processRunTimes
            .Select(p => p.Versions.Values
                .Where(processVersion => VersionDeservesSelection(sameEntityProcess, processVersion))
                .Select(v =>
                    new ProcessComboItem
                    {
                        processId = v.definition.Id,
                        versionNo = v.VersionNo ?? "1.0",
                        displayText = v.definition.Name + " version " + (v.VersionNo ?? "1.0")
                    }
                )
                .ToList())
            .ToList();
    }

    private static bool VersionDeservesSelection(Process sameEntityProcess,
        ProcessVersionRuntime processVersion)
    {
        return (sameEntityProcess == null ||
                (sameEntityProcess.EntityNamespaceId == processVersion.definition?.EntityNamespaceId &&
                 sameEntityProcess.EntityId == processVersion.definition?.EntityId))
               && FetchActivities(processVersion.definition).Any();
    }

    private static IEnumerable<Activity> FetchActivities(Process process)
    {
        List<Activity> activities = [];
        if (process == null)
        {
            return activities;
        }

        activities.AddRange(from item in process.flowElements?.Values ?? Enumerable.Empty<FlowElement>()
                            where item is UserTask or ServiceTask
                            select item as Activity);
        return activities;
    }

    private IdentityUser CheckWorkItemsAccess(WorkItemsPageType pageType, string processId)
    {
        IdentityUser user = GetUser();
        if (pageType == WorkItemsPageType.WorkItems)
        {
            if (string.IsNullOrEmpty(processId))
            {
                if (!CheckControllerActionAccess(user, "Process", "WorkItems"))
                {
                    throw new Exception(Messages.PageAccessDenied);
                }

                SetPagePackId("Process_WorkItems");
            }
            else
            {
                if (!CheckProcessAccess(user, processId))
                {
                    throw new Exception(Messages.PageAccessDenied);
                }

                SetPagePackId("Process_WorkItems_" + processId);
            }
        }
        else
        {
            SetPagePackId("Process_MyWorkItems");
        }

        return user;
    }
}
