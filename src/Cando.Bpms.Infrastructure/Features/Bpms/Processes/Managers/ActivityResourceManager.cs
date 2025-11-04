using Neo.Bpms.Domain.Entities.Bpmn.Extensions.ResourceRoles;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers;

public class ActivityResourceManager
{
    public ActivityResourceManager(string processId, string versionId, string activityId,
        IdentityUser user, long aiId, int pageNo, int pageSize, ExecutionInstance execution)
    {
        _user = user;
        _pageNo = pageNo;
        _pageSize = pageSize;
        ProcessRunTime processRunTime = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.GetProcessRuntime(processId);
        _processVersion = processRunTime?.GetVersion(versionId);
        if (_processVersion == null)
            throw new Exception($"Invalid process Id {processId}");
        FlowNodeRunTime node = null;
        _processVersion?.nodes.TryGetValue(activityId, out node);
        _activityRunTime = node as ActivityRuntime;
        if (_activityRunTime == null)
            throw new Exception($"Invalid activityAuthorization Id {activityId} in process {processId}");
        _index = -1;


        try
        {
            _ai = DataStorage.FetchActivityInstance(aiId, execution);
        }
        catch
        {
            throw new Exception($"Invalid ActivityInstanceId {aiId} in activityAuthorization Id {activityId} in process {processId}");
        }
    }

    public bool LoadNormally => _processVersion != null && _activityRunTime != null && _ai != null;

    private readonly IdentityUser _user;
    private readonly ProcessVersionRuntime _processVersion;
    private readonly ActivityRuntime _activityRunTime;
    private readonly FlowNodeInstance _ai;
    private readonly IDictionary<string, UserViewModel> _users = new Dictionary<string, UserViewModel>();
    private readonly int _pageNo;
    private readonly int _pageSize;
    private int _index;

    public IList<UserViewModel> GetUsers()
    {
        GetResourcesUsers(_processVersion?.definition?.resources?.OfType<ProcessManagerResource>().ToList());
        if (!_users.ContainsKey(_user.Id))
            _users.Clear(); // اگر من مدیر نیستم نتوانم کار را به مدیری اختصاص دهم.
        GetResourcesUsers(_activityRunTime?.Activity?.ActivityAndLaneResources);
        return [.. _users.Values];
    }

    private void GetResourcesUsers(IEnumerable<ResourceRole> resources)
    {
        foreach (ResourceRole resource in resources ?? [])
        {
            GetResourceUsers(resource);
            if (_index > _pageNo * _pageSize)
                break;
        }
    }
    private void GetResourceUsers(ResourceRole resource)
    {
        if (_activityRunTime is not UserTaskRuntime userTaskRunTime) return;
        UserSelections users = [];
        userTaskRunTime.DistributeWorkToResource(_ai as UserTaskInstance, users, resource);
        foreach (UserSelection userSelection in users.Values)
        {
            IdentityUser user = userSelection.User;
            if (string.IsNullOrEmpty(user?.Id)) continue;
            if (_users.ContainsKey(user.Id)) continue;
            _index++;
            if (_index < (_pageNo - 1) * _pageSize)
                continue;
            if (_index > _pageNo * _pageSize)
                break;
            _users.Add(user.Id, new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Name = user.FirstName,
                Family = user.LastName,
                AvatarId = user.AvatarPicture
            });
        }
    }
}
