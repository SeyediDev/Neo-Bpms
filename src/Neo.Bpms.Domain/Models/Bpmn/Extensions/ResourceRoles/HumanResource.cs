namespace Neo.Bpms.Domain.Models.Bpmn.Extensions.ResourceRoles;

public class HumanResource(BpmnDefinitions def, string id, string name, HumanResourceType type) : Resource(def, id, name)
{
    public IEnumerable<string> UserClaimRestrictions =>
        resourceParameters?.Where(resourceParameter =>
                resourceParameter.type == ResourceParameter.eType.Claim)
            .Select(resourceParameter => resourceParameter.Name);//todo name?

    public HumanResourceType type = type;
}

public class UserClaimRestriction
{
    public string ClaimType;
    public string ClaimValueProperty;
}

public enum HumanResourceType
{
    User,
    UserGroup,
}