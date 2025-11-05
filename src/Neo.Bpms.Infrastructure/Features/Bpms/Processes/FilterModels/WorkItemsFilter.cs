using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.FilterModels;

public class WorkItemsFilter
{
    public string __ProcessId { get; set; }
    public string ProcessId => __ProcessId;

    public string __ProcessVersionId { get; set; }
    public string ProcessVersionId => __ProcessVersionId;

    public string UserId { get; set; }

    public WorkItemsPageType __PageType { get; set; } = WorkItemsPageType.MyWorkItems;
    public WorkItemsPageType PageType => __PageType;

    public string __Activity { get; set; }
    public string Activity => __Activity;

    public string __Description { get; set; }
    public string? Description => __Description;

    public int Page { get; set; } = 1;

    //        public string __HostUser { get; set; }
    //        public string HostUser => __HostUser;

    public bool __Offered { get; set; }
    public bool Offered => __Offered;

    public bool __Allocated { get; set; }
    public bool Allocated => __Allocated;

    public bool __Suspended { get; set; }
    public bool Suspended => __Suspended;

    public bool __Created { get; set; } = true;
    public bool Created => __Created;

    public bool __Finished { get; set; }
    public bool Finished => __Finished;

    public bool __JustRestorableTasks { get; set; } = false;
    public bool JustRestorableTasks => __JustRestorableTasks;

    //        public int timeState { get; set; } = 0;

    //        public int Importance { get; set; } = 0;

    //	    public int Confidentiality { get; set; } = 0;

    public string __NamespaceId { get; set; }
    public string NamespaceId => __NamespaceId;
    public string __EntityId { get; set; }
    public string EntityId => __EntityId;

    public string __EntityPkv { get; set; }
    public string EntityPkv => __EntityPkv;

    public string __FromCreationDateText { get; set; }
    public string FromCreationDateText => __FromCreationDateText;
    public DateTime? FromCreationDate { get; set; }

    public string __ToCreationDateText { get; set; }
    public string ToCreationDateText => __ToCreationDateText;
    public DateTime? ToCreationDate { get; set; }
    public long UserGroupId { get; set; }

    public void SetDefaultValues()
    {
        __Offered = true;
        __Allocated = true;
        __Suspended = true;
    }

    public static WorkItemsFilter GetDefaultWorkItemsFilter(string processId, string processVersionId, string userId, int page)
    {
        return new WorkItemsFilter
        {
            __ProcessId = processId,
            __ProcessVersionId = processVersionId,
            Page = page,
            UserId = userId,
            __Offered = true,
            __Allocated = true,
            __Suspended = true,
            __Created = true
        };
    }
}
