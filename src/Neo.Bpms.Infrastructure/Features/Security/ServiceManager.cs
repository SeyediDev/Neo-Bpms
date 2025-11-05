namespace Neo.Bpms.Infrastructure.Features.Security;

public class ServiceManager
{
    private static readonly Lazy<ServiceManager> Lazy = new(() => new ServiceManager());

    public static ServiceManager Instance => Lazy.Value;
    public ISsoIntegrator ExternalLoginIntegrator { get; set; }
    public IAccessServices AccessServices { get; set; }
    public IIdentityUserService IdentityUserService { get; set; }
}
