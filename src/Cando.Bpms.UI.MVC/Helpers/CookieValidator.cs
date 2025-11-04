using System.Security.Principal;
using Neo.Domain.Constants;
using Neo.Domain.Features.Client;

namespace Neo.Bpms.UI.MVC.Helpers;

public static class CookieValidator
{
    private static readonly ConcurrentDictionary<string, byte> Blacklist = new();

    public static async Task ValidateCookie(CookieValidatePrincipalContext context)
    {
        ClaimsPrincipal userPrincipal = context.Principal;
        if (userPrincipal == null)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return;
        }
        
        // اگر access_token وجود ندارد، کاربر را reject کن تا به صفحه لاگین برود
        string accessToken = userPrincipal.FindFirst("access_token")?.Value;
        if (string.IsNullOrEmpty(accessToken))
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return;
        }
        
        IIdpService idpService = context.HttpContext.RequestServices.GetRequiredService<IIdpService>();
        bool isValid = true;//TODO DISABLED IDP await idpService.ValidateTokenAsync(accessToken);

        if (!isValid)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return;
        }

        IRequesterUser requesterUser = context.HttpContext.RequestServices.GetRequiredService<IRequesterUser>();
        string userName = GetPrincipalName(userPrincipal);
        IdentityUser user = (IdentityUser)requesterUser.GetProperty(nameof(IdentityUser),
            () =>
            {
                IdentityUser user = new(userName, 
                    userPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value)
                {
                    UserName = userName,
                    FirstName = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == nameof(IdentityUser.FirstName))?.Value,
                    LastName = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == nameof(IdentityUser.LastName))?.Value,
                    NationalNumber = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value,
                    MobileNo = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.MobilePhone)?.Value,
                    PhoneNumber = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.OtherPhone)?.Value,
                    Email = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value,
                    IsAdmin = userPrincipal.IsInRole(Roles.Admin),
                };
                user.SetRoles([.. userPrincipal.Claims]);
                return user;
            });

        static string? GetPrincipalName(IPrincipal principal)
        {
            string name = principal?.Identity?.Name;
            string[] nameArray = name?.Split('\\');
            return (nameArray?.Length ?? 0) < 1 ? null : nameArray[^1];
        }
    }

    public static Task InvalidateCookie(CookieSigningOutContext context)
    {
        return Task.Run(() =>
        {
            //var accessToken = context.HttpContext.Session.GetString("Token_AccessToken");
            //var ticket = context.HttpContext.Session.GetString("Token_SessionTicket");
            string guid = context.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Hash)?.Value;
            if (!string.IsNullOrEmpty(guid))
            {
                _ = Blacklist.TryAdd(guid, 0);
            }
        });
    }

    public static Task OnSignedIn(CookieSignedInContext context)
    {
        return Task.Run(() =>
        {
            //var accessToken = context.HttpContext.Session.GetString("Token_AccessToken");
            //var ticket = context.HttpContext.Session.GetString("Token_SessionTicket");
            string guid = context.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Hash)?.Value;
            //if (!string.IsNullOrEmpty(guid))
            //{
            //    Blacklist.TryAdd(guid, 0);
            //}
        });
    }
    public static Task OnRedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        return Task.Run(() =>
        {
            // اگر در iframe هستیم، صفحه خطا نشان بده
            if (context.HttpContext.Items["InIframe"] as bool? ?? false)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "text/html";
                context.Response.WriteAsync("<html><body><h1>Please login again</h1></body></html>");
                return;
            }

            // در غیر این صورت به صفحه لاگین redirect کن
            string returnUrl = context.Request.Query["returnUrl"].FirstOrDefault() ?? context.Request.Path.Value;
            string loginUrl = $"/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}";
            context.Response.Redirect(loginUrl);
        });
    }

    internal static Task OnRedirectToReturnUrl(RedirectContext<CookieAuthenticationOptions> context)
    {
        return Task.Run(() =>
        {
            // اگر returnUrl وجود دارد، به آن redirect کن
            string returnUrl = context.Request.Query["returnUrl"].FirstOrDefault();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                context.Response.Redirect(returnUrl);
            }
            else
            {
                // در غیر این صورت به صفحه اصلی برو
                context.Response.Redirect("/");
            }
        });
    }

    internal static Task OnSigningIn(CookieSigningInContext context)
    {
        return Task.Run(() =>
        {
            //var accessToken = context.HttpContext.Session.GetString("Token_AccessToken");
            //var ticket = context.HttpContext.Session.GetString("Token_SessionTicket");
            string guid = context.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Hash)?.Value;
            //if (!string.IsNullOrEmpty(guid))
            //{
            //    Blacklist.TryAdd(guid, 0);
            //}
        });
    }
}
