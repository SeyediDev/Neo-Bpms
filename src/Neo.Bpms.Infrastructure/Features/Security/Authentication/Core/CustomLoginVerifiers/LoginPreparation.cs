namespace Neo.Bpms.Infrastructure.Features.Security.Authentication.Core.CustomLoginVerifiers;

public class LoginPreparation
{
    public IDictionary<string, string> SessionsToBeSet { get; set; }
    public IDictionary<string, string> PassingToLoginForm { get; set; }
}