using Neo.Bpms.Domain.Entities.Security.Authentication;
using Neo.Bpms.Domain.Entities.Security.Authorization;

namespace Neo.Bpms.Domain.Features.Security;

public interface IIdentityUserService
{
    Task<IdentityUser> GetIdentityUserAsync(string userName, CancellationToken cancellationToken=default);
    Task<List<IdentityRole>> GetIdentityRoles();
}
