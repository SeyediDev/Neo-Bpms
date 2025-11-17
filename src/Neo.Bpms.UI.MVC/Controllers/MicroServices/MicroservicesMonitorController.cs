using Neo.Bpms.Infrastructure.Features.Bpms.Engine;
using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.MonitoringInfo;

namespace Neo.Bpms.UI.MVC.Controllers.MicroServices;

public class MicroservicesMonitorController : ControllerBaseMVC
{
    public ActionResult Index()
    {
        IdentityUser user = GetUser(User);
        if (user == null)
            return RedirectToAction("Login", "Account");
        //	        if (!AccessServices.CheckControllerActionAccess(user, "MicroservicesMonitor", "Index"))
        if (!user.IsAdmin)
            throw new UnauthorizedAccessException();
        ViewBag.user = user;
        SetPagePackId("/MicroservicesMonitor/Index");

        return View();
    }

    public JsonResult Data()
    {
        IdentityUser user = GetUser(User);
        //	        if (!AccessServices.CheckControllerActionAccess(user, "MicroservicesMonitor", "Index"))
        if (!(user?.IsAdmin ?? false))
            throw new UnauthorizedAccessException();

        Infrastructure.Features.Bpms.MicroServices.MicroServiceManager microserviceManager = MicroServicesController.GetInterface();
        return Json(microserviceManager.GetResourcesInfo());
    }

    [HttpPost]
    public JsonResult ManualRequeue(string funcName, int count = 1, string machineId = null)
    {
        IdentityUser user = GetUser(User);
        //	        if (!AccessServices.CheckControllerActionAccess(user, "MicroservicesMonitor", "Index"))
        if (!user?.IsAdmin ?? false)
            throw new UnauthorizedAccessException();
        if (!string.IsNullOrEmpty(funcName) && count > 0 && count <= 30)
            ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).RunServiceTasksFromQueue("MicroServiceManager",
                                                                        funcName, count, machineId, false);
        return Json(null);
    }


    public JsonResult DummyData()
    {
        IdentityUser user = GetUser(User);
        //	        if (!AccessServices.CheckControllerActionAccess(user, "MicroservicesMonitor", "Index"))
        if (!user?.IsAdmin ?? false)
            throw new UnauthorizedAccessException();

        IEnumerable<SingleResourceInfo> dummyInfo =
        [
            new SingleResourceInfo
            {
                CurrentRequestId = 1,
                FunctionName = "Catchup",
                IsImmediate = false,
                IsWorking = true,
                MachineId = "1.2.2.2",
                Progress = 43
            },
            new SingleResourceInfo
            {
                CurrentRequestId = 0,
                FunctionName = "Info",
                IsImmediate = true,
                IsWorking = false,
                MachineId = "1.2.2.2",
                Progress = 0
            }
        ];
        return Json(dummyInfo);
    }
}
