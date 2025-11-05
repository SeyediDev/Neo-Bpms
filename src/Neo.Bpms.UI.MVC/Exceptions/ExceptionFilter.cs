using Neo.Domain.Features.Client;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Neo.Bpms.UI.MVC.Filters;

public class ExceptionFilter : ExceptionFilterAttribute
{
    public override Task OnExceptionAsync(ExceptionContext context)
    {
        HandleException(context);
        return Task.CompletedTask;
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        string message;
        switch (context.Exception)
        {
            case UnauthenticatedUserException:
                if (context.HttpContext.Items["InIframe"] as bool? ?? false)
                {
                    ShowErrorPage(context, Messages.PleaseLoginAgain);
                    return;
                }

                string externalLoginPage =
                    DependencyInjectionHolder.Instance.SsoIntegrator?.LoginPageUrl(context.HttpContext.Request
                        .GetBaseUrl());
                context.Result = externalLoginPage == null
                    ? new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"controller", "Account"},
                        {"action", "Login"},
                        {"returnUrl", context.HttpContext.Request.GetDisplayUrl()}
                    })
                    : new RedirectResult(externalLoginPage, false);

                return;
            case ValidationException:
                context.HttpContext.Response.StatusCode = 400;
                message = context.Exception.Message;
                break;
            case UnauthorizedAccessException:
                context.HttpContext.Response.StatusCode = 403;
                message = context.Exception.Message;
                break;
            default:
                {
                    ILogger logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ExceptionFilter>>();
                    context.HttpContext.Response.StatusCode = 500;
                    if (context.Exception?.GetType() == typeof(Exception)
                        || context.Exception is HttpException)
                    {
                        message = context.Exception.Message;
                    }
                    else
                    {
                        logger.LogError(context.Exception, context.Exception?.ToString());
                        message = Messages.SystemError+" "+ context.Exception?.ToString();
                    }

                    if (context.Exception is HttpException { ErrorCode: > 0 and < 600 } httpException)
                    {
                        context.HttpContext.Response.StatusCode = httpException.ErrorCode;
                    }
                    logger.LogError(context.Exception, message);
                    break;
                }
        }

        if (IsAjax(context.HttpContext))
        {
            context.HttpContext.Response.ContentType = "text/plain; charset=UTF-8";
            context.HttpContext.Response.Body.Write(Encoding.UTF8.GetBytes(message));
        }
        else
        {
            ShowErrorPage(context, message);
        }
    }

    private bool IsAjax(HttpContext context)
    {
        return context.Request.Headers["x-requested-with"] == "XMLHttpRequest";
    }

    private void ShowErrorPage(ExceptionContext context, string s)
    {
        IRequesterUser requesterUser = context.HttpContext.RequestServices.GetRequiredService<IRequesterUser>();
        ViewDataDictionary viewData = new(new EmptyModelMetadataProvider(),
            new ModelStateDictionary())
        {
            new("ErrorMsg", s),
            new("user", new IdentityUser
            {
                Id = requesterUser.Id?.ToString(),
                FirstName = requesterUser.Mobile,
                LastName = requesterUser.Lang,
                IsAdmin = false,
            }),
            new("InIframe", context.HttpContext.Items["InIframe"])
        };
        context.Result = new ViewResult
        {
            ViewName = context.HttpContext.Response.StatusCode == 403
                ? "~/Views/Error/AccessDenied.cshtml"
                : "~/Views/Error/ErrorDetails.cshtml",
            ViewData = viewData
        };
    }
}
