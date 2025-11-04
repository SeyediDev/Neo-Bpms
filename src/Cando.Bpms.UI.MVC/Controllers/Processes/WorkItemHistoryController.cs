using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.UI.MVC.Controllers;

public class WorkItemHistoryController(WorkItemManager workItemManager) : Controller
{
    [HttpGet]
    public JsonResult GetWorkItemHistory(string namespaceId, string entityId, string processId, long? entityPkv)
    {
        WorkItemsFilter filter = WorkItemsFilter.GetDefaultWorkItemsFilter(processId, null, "", 1);
        filter.__NamespaceId = namespaceId;
        filter.__EntityId = entityId;
        filter.__EntityPkv = entityPkv != null ? entityPkv.ToString() : "";
        filter.__Finished = true;
        IEnumerable<WorkItemHistoryViewModel> result = workItemManager.GetWorkItems(out _,
                         100, WorkItemsQueryType.Process, null,
                         null, filter, null)?.Select(wi => new WorkItemHistoryViewModel(wi))
                     ?? [];
        return Json(result);
    }
}
