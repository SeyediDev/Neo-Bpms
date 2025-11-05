namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

public class UserSelections : Dictionary<string, UserSelection> { }
public class UserSelection
{
    public IdentityUser User { get; set; }
    public long UserGroupId { get; set; }
    public string UserGroupName { get; set; }
}
