using System.Security.Principal;
using Neo.Domain.Features.Client;

namespace Neo.Bpms.UI.MVC.Controllers.Public;
[ExceptionFilter]
public class ControllerBaseMVC : ControllerBaseInjection
{
    /// <summary>
    /// responsible to redirect to Error page.
    /// </summary>		
    /// <returns></returns>
    protected ActionResult Error(string error)
    {
        return View("~/Views/Error/Error.cshtml",
            new FormErrorViewModel() { ErrorMessage = error });
    }

    /// <summary>
    /// get User (Active Directory user)
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    protected IdentityUser GetUser(ClaimsPrincipal claimsPrincipal)
    {
        if(claimsPrincipal.Identity is null || !claimsPrincipal.Identity.IsAuthenticated )
        {
            throw new UnauthenticatedUserException();
        }
        IRequesterUser requesterUser = HttpContext.RequestServices.GetRequiredService<IRequesterUser>();
        IdentityUser user = (IdentityUser)requesterUser.GetProperty(nameof(IdentityUser));
        return user;
    }

    private static string? GetPrintipalName(IPrincipal principal)
    {
        string name = principal?.Identity?.Name;
        string[] nameArray = name?.Split('\\');
        return (nameArray?.Length ?? 0) < 1 ? null : nameArray[^1];
    }

    protected IdentityUser GetUser()
    {
        IdentityUser user = GetUser(User);
        CheckUserAuthentication(user);
        return user;
    }

    protected void CheckUserAuthentication(IdentityUser user)
    {
        if (user == null || !User.Identity.IsAuthenticated)
        {
            throw new UnauthenticatedUserException();
        }
        //_ = UserAuthenticationService.GetAuthenticationKey(user.Id) ??
        //    throw new UnauthenticatedUserException();
        ViewBag.user = user;
        if (string.IsNullOrEmpty(user.Culture))
        {
            user.Culture = FetchCulture();
        }
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _ = FetchCulture();
    }

    private string FetchCulture()
    {
        string cultureName = null;
        string cultureCookie = Request.Cookies["_culture"];
        if (cultureCookie != null)
        {
            cultureName = cultureCookie;
        }

        cultureName = CultureHelper.GetImplementedCulture(cultureName);

        Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);
        return cultureName;
    }

    protected void SetPagePackId(string value)
    {
        ViewBag.PagePackId = value;
    }
    protected void SetPagePackId(Form form)
    {
        SetPagePackId(form.PagePackId + ";" + form.IndexPagePackId);
    }

    protected void SetInIframe(bool value = true)
    {
        HttpContext.Items.Add("InIframe", value);
    }

    protected (bool authorized, string errorMessage) CheckPostAccess(Form form, out IdentityUser user, out long userGroupId)
    {
        return form == null
                ? throw new Exception("Invalid form id")
                : ((bool authorized, string errorMessage))
                    (!CheckAccess(form, out user, out userGroupId) ? (false, Messages.PageAccessDenied) : (true, ""));
    }

    protected bool CheckAccess(Form form, out IdentityUser user, out long userGroupId)
    {
        userGroupId = 0;
        user = GetUser(User);
        if (user == null)
        {
            if (form.AllowAnonymous)
            {
                ViewBag.anonymous = form.AllowAnonymous;
            }
            else
            {
                CheckUserAuthentication(null);
            }
        }
        else
        {
            CheckUserAuthentication(user);
            if (!AccessServices.CheckFormAccess(user, form, out userGroupId) &&
               !AccessServices.CheckSystemFeatureAccess(user, SystemFeatureId.FormDesign))
            {
                if (form.AllowAnonymous)
                {
                    ViewBag.allowAnonymous = form.AllowAnonymous;
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }

    protected bool CheckAccess(IdentityUser user, Form form)
    {
        return user != null && AccessServices.CheckFormAccess(user, form, out _);
    }

    protected bool CheckAccess(IdentityUser user, Report report)
    {
        return user != null && AccessServices.CheckReportAccess(user, report);
    }

    protected bool CheckAccess(IdentityUser user, Dashboard dashboard)
    {
        return user != null && AccessServices.CheckDashboardAccess(user, dashboard);
    }

    protected bool CheckProcessAccess(IdentityUser user, string processId)
    {
        return user != null && AccessServices.CheckProcessAccess(user, processId);
    }

    protected bool CheckAccess(IdentityUser user, SystemFeatureId systemFeature)
    {
        return user != null && AccessServices.CheckSystemFeatureAccess(user, systemFeature);
    }

    protected bool CheckControllerActionAccess(IdentityUser user, string controllerId, string action)
    {
        return user != null && AccessServices.CheckControllerActionAccess(user, controllerId, action);
    }

    protected void CheckSystemFeatureAccess(SystemFeatureId systemFeature, out IdentityUser user)
    {
        user = GetUser();
        if (!AccessServices.CheckSystemFeatureAccess(user, systemFeature))
        {
            string message = CultureHelper.GetCurrentNeutralCulture() switch
            {
                "fa" => $"شما دسترسی {systemFeature.ToName()} ندارید.",
                //case "en":
                _ => $"You need additional privileges for {systemFeature}",
            };
            throw new HttpException(message);
        }
    }
}
