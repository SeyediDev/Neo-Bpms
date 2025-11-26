namespace Neo.Bpms.Domain.Features.Security;

public interface IClubRolesService
{
    List<ClubRoleInfo> GetClubRoles();
}

public class ClubRoleInfo
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

