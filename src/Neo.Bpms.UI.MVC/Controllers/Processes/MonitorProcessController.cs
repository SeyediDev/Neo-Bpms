using Neo.Bpms.Infrastructure.Features.Bpms.Processes;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class MonitorController : ControllerBaseMVC
{
    public ActionResult Process(string processId)
    {
        IdentityUser user = GetUser();
        if (!AccessServices.CheckControllerActionAccess(user, "Monitor", "Process"))
            throw new HttpException(Messages.PageAccessDenied);

        SetPagePackId("/Monitor/Process");
        ViewBag.user = user;

        List<ProcessViewModel> res = ProcessViewModelManager.GetProcessViewModels(processId);
        return View(res);
    }

    public ActionResult ProcessInstances()
    {
        IdentityUser user = GetUser(User);
        return user == null || !User.Identity.IsAuthenticated
            ? throw new HttpException("خطا")
            : !AccessServices.CheckControllerActionAccess(user, "Monitor", "ProcessInstance")
            ? throw new HttpException(Messages.PageAccessDenied)
            : (ActionResult)View();
    }

    public JsonResult ProcessInstancesData(long? specificPi)
    {
        IdentityUser user = GetUser(User);
        if (user == null || !User.Identity.IsAuthenticated)
            throw new HttpException("خطا");
        if (!AccessServices.CheckControllerActionAccess(user, "Monitor", "ProcessInstance"))
            throw new HttpException(Messages.PageAccessDenied);
        IEnumerable<ProcessInstanceViewModel> res = ProcessInstanceViewModelManager.GetProcessInstancesViewModels(specificPi);
        return Json(res);
    }
    //        public JsonResult ActivityInstancesData()
    //	    {
    //	        var user = GetUser(HttpContext, User);
    //	        if (user == null || !User.Identity.IsAuthenticated)
    //	            throw new HttpException("خطا");
    //	        if (!AccessServices.CheckControllerActionAccess(user, "Monitor", "ProcessInstance"))
    //	            throw new HttpException(Messages.PageAccessDenied);
    //	        ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).repository.
    //	        return Json();
    //	    }
}
