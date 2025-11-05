using Neo.Bpms.Domain.Entities.Security.Authentication;
using Neo.Bpms.Domain.Repository.Entities;

namespace Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;

public abstract class ConfiguredItem: IConfig
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsPublic { get; set; }
    public string? UserId { get; set; }
    public List<string>? Roles { get; set; }
    public long? FolderId { get; set; }
    public bool IsDefault { get; set; }

    public bool CheckAccess(IdentityUser user)
    {
        if (user.IsAdmin) return true;
        
        if (!IsPublic)
        {
            return user.Id == UserId;
        }
        
        if (Roles == null || Roles.Count == 0)
        {
            return true; // Public with no role restriction
        }
        
        return Roles.Any(roleId => user.Roles?.ContainsKey(roleId) == true);
    }
}
