namespace Neo.Bpms.UI.MVC.Controllers;

public class ErrorController : ControllerBaseMVC
{
    public ActionResult Index()
    {
        ControllerContext.RouteData.Values.TryGetValue("ErrorMsg", out object cMsg);
        ViewBag.Error = cMsg.ToString();
        return RedirectToAction("ErrorDetails", "Error", new { ErrorMsg = cMsg });
        //return View();
    }

    public ActionResult ErrorDetails(string errorMsg, bool inIframe = false)
    {
        Response.StatusCode = 500;

        ViewBag.ErrorMsg = errorMsg;
        SetInIframe(inIframe);
        return View();
    }

    public ActionResult NotFound(bool inIframe = false)
    {
        //		    Response.StatusCode = 404;
        //		    Response.TrySkipIisCustomErrors = true;
        //		    Server.ClearError();
        //		    Response.TrySkipIisCustomErrors = true;
        SetInIframe(inIframe);
        try
        {
            GetUser();
        }
        catch
        {
            // ignored. Just needed ViewBag.user if available
        }

        Response.ContentType = "text/html; charset=UTF-8";
        return View("NotFound");
    }

    public ActionResult AccessDenied(bool inIframe = false)
    {
        Response.StatusCode = 401;
        SetInIframe(inIframe);
        try
        {
            GetUser();
        }
        catch
        {
            // ignored. Just needed ViewBag.user if available
        }

        return View();
    }

    public ActionResult Definitions()
    {
        return View("DefinitionsError");
    }
}
