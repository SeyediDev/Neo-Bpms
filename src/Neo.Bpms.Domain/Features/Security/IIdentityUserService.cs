using Neo.Bpms.Domain.Models.Security.Authentication;

namespace Neo.Bpms.Domain.Features.Security;

public interface IIdentityUserService
{
    Task<IdentityUser> GetIdentityUserAsync(string userName, CancellationToken cancellationToken=default);
    Task<List<IdentityRole>> GetIdentityRoles();
}
