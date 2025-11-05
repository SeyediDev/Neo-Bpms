using Microsoft.AspNetCore.Routing;
using Neo.Bpms.UI.MVC.Controllers;

namespace Neo.Bpms.UI.MVC.Middlewares;

public class ErrorHandlerMiddleware(RequestDelegate next,
    ILogger<ErrorHandlerMiddleware> logger, LinkGenerator linkGenerator)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            /*if (BpmsStartup.HadError() && !context.Request.Path.ToString().EndsWith("/Error/Definitions"))
            {
                RedirectToError(context, null, "Definitions");
            }
            else*/
            {
                await next(context);
                if (context.Response.StatusCode == 404 && !context.Request.Path.StartsWithSegments("/api"))
                {
                    logger.LogInformation("{0} path not found", context.Request.Path);
                    if (context.Request.Path != "/Error/NotFound")
                        RedirectToNotFound(context);
                }
                if (context.Response.StatusCode != 200)
                {

                }
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.ToString());
            try
            {
                RedirectToError(context, exception, nameof(ErrorController.ErrorDetails));
            }
            catch (Exception e)
            {
                logger.LogError(e, e.ToString());
                throw;
            }
        }
    }

    private void RedirectToNotFound(HttpContext context)
    {
        string notFoundPage = linkGenerator.GetPathByAction("NotFound", "Error", new { inIframe = context.Items["InIframe"] });
        context.Response.Redirect(notFoundPage, false);
    }

    private void RedirectToError(HttpContext context, Exception exception, string action)
    {
        string errorDetailsPage = linkGenerator.GetPathByAction(
            action, "Error", new
            {
                errorMsg = Messages.SystemError + "." + exception.Message,
                inIframe = context.Items["InIframe"]
            });
        context.Response.Redirect(errorDetailsPage, false);
    }
}
