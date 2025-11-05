namespace Neo.Bpms.Infrastructure.Features.Security;

public class ExternalLoginIntegratorParams : ISsoIntegratorParams
{
    public string RedirectUrl(string baseUrl) => baseUrl + "/Account/LoginCallback";
}
