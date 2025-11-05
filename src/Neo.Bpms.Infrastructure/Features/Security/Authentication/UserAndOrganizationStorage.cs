using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.Security.Authorization;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

namespace Neo.Bpms.Infrastructure.Features.Security.Authentication;
public sealed class UserAndOrganizationStorage : LazyStorage<UserAndOrganizationStorage>
{
    public UserAndOrganizationStorage()
    {
        Load();
        AfterLoad();
    }

    public CanBeDisableStorage<SystemUserGroup, IdentityRole> Roles { get; set; }

    public CheckTimeOutStorageWithStringKey<SystemUser, IdentityUser> Users { get; set; }

    public override List<string> Errors => ErrorsOfStorage(Roles, Users);

    public override void Load()
    {
        Roles = new CanBeDisableStorage<SystemUserGroup, IdentityRole>();
        Users = new CheckTimeOutStorageWithStringKey<SystemUser, IdentityUser>();
    }

    public override void AfterLoad()
    {
        AfterLoad(Roles, Users);
    }

    public IEnumerable<IdentityUser> FindByIds(IEnumerable<string> userIds)
    {
        if (userIds == null)
            return null;
        var list = new Dictionary<string, IdentityUser>();
        foreach (var userId in userIds)
        {
            if (!TryGetUserById(userId, out var user)) continue;
            if (!list.ContainsKey(userId))
                list.Add(userId, user);
        }

        return list.Values;
    }

    private bool TryGetUserById(string userId, out IdentityUser user)
    {
        user = GetUserById(userId);
        return user != null;
    }

    public IdentityUser GetUserById(string userId)
    {
        return string.IsNullOrEmpty(userId) ? null : Users.GetItem(userId);
    }

    public List<IdentityUser> GetUserList()
    {
        return [.. Users.Items.Values];
    }

    public IdentityUser FindByUserName(string userName)
    {
        return Users.GetCategorizedItem(nameof(IdentityUser.UserName), userName);
    }

    public IdentityUser FindNationalCode(string nationalCode)
    {
        return Users.GetCategorizedItem(nameof(IdentityUser.NationalNumber), nationalCode);
    }

    public IdentityUser FindByEmail(string email)
    {
        return Users.GetCategorizedItem(nameof(IdentityUser.Email), email);
    }

    public IdentityRole GetUserGroupByCode(string code)
    {
        return Roles.GetCategorizedItem(nameof(IdentityRole.Code), code);
    }

    public IdentityRole GetUserGroup(long? groupId)
    {
        return Roles.GetItem(groupId);
    }
}
