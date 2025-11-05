using Neo.Bpms.Domain.Features.Security.Dto;
using Neo.Domain.Features.Client.Dto;

namespace Neo.Bpms.Infrastructure.Features.Security.Authentication;

public class SsoIntegrator(
    ILogger<SsoIntegrator> logger,
    ISsoIntegratorParams externalLoginIntegratorParams) : ISsoIntegrator
{
    protected virtual string RedirectUrl(string baseUrl)
    {

        logger.LogInformation("Request RedirectUrl 0.0 ");
        return externalLoginIntegratorParams.RedirectUrl(baseUrl);
    }

    public async Task<VerificationResult> VerifyRedirectionAsync(
        IDictionary<string, string> parameters, object inHttpContext)
    {
        //HttpContext httpContext = (HttpContext)inHttpContext;
        string code = parameters["code"];
        string ticket = parameters["ticket"];
        string state = parameters["state"] ?? "1";
        if (string.IsNullOrEmpty(code))
        {
            return CreateResult(null!, "Invalid code");
        }

        logger.LogInformation("RedirectUrl 0.1 {code} {ticket} {state}", code, ticket, state);
        TokenResponseDto token = null!;
        await Task.CompletedTask;
        return CreateResult(token, token == null ? "Unauthenticated user" : "");

        static VerificationResult CreateResult(TokenResponseDto token, string message)
        {
            return new VerificationResult
            {
                Verified = token != null,
                Token = token,
                Message = message
            };
        }
    }

    public string LoginPageUrl(string baseUrl)
    {
        return baseUrl;
    }

    public Task<dynamic> Signout(IDictionary<string, string> parameters)
    {
        //string userId = parameters["userId"];
        return Task.FromResult<dynamic>(null!);
    }
}
