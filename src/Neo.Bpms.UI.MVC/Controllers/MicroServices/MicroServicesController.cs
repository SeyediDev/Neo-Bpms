using Neo.Bpms.Domain.Models.Bpmn.Core.Services;
using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices;
using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.Definitions;
using Neo.Bpms.UI.MVC.ViewModels.MicroServices;

namespace Neo.Bpms.UI.MVC.Controllers.MicroServices;

[Route("api/microservices")]
public class MicroServicesController : ControllerBase
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    private readonly MicroServiceManager _microServiceManager = GetInterface();

    internal static MicroServiceManager GetInterface() // todo access
    {
        Interface intfc = ProjectDefinition.Project.GetInterface("MicroServiceManager");
        return intfc.implementation as MicroServiceManager;
    }

    /// <summary>
    /// توسطِ ماشین‌هایی که سرویس‌تسک‌هایِ bpmn را اجرا می‌کنند فراخوانی می‌شود
    /// ماشین‌ها هنگامِ شروع به کار یا زمانی که بی‌پی‌ام‌اس اعلام می‌کند آن‌ها را نمی‌شناسد رجیستر می‌کنند
    /// </summary>
    /// <param name="machineId">
    ///	شناسه‌یِ ماشین
    /// </param>
    /// <param name="functions">
    ///	آرایه‌یِ توابعی که این ماشین ارائه می‌دهد
    /// </param>
    /// <param name="reRegister">
    ///	اگر true باشد به این معناست که ماشین ریست نشده و چون bpms آن را نمی‌شناخته مجددا اقدام به رجیستر کرده
    /// </param>
    /// <returns>
    /// اگر موفق باشد پاسخِ خالی با کدِ 200
    /// در غیرِ این پیام با کدِ خطا
    /// </returns>
    /// <example>
    /// POST http://hostAddress/api/microservices/register?machineId=folan&amp;reRegister=false HTTP/1.1
    /// content-type: application/json
    /// 
    /// [{"FuncName": "Transcode",
    /// "ResourceCount": 2,
    /// "Url": "http://machineip/api/transcode",
    /// "RefreshTime": 100000,
    /// "PollingAddress": null,
    /// "Immediate": false}]
    /// </example>
    [HttpPost]
    [Route("Register")]
    public ActionResult Register(string machineId,
    IList<FunctionInMachineDefinition> functions, bool? reRegister = false)
    {
        if (functions == null || !functions.Any())
        {
            Logger.LogError("Bad register from machineId {0} with empty or null functions", machineId);
            return BadRequest(functions == null ? "functions is null" : "functions is empty");
        }
        Logger.LogInformation("machineId: {0} is trying to (re {1}) register its functions.Function names:{2}", machineId, reRegister, string.Join((string)",", (IEnumerable<string>)functions.Select(f => f.FuncName)));
        bool registerResult = _microServiceManager.TryRegister(machineId, functions, reRegister ?? false);
        if (registerResult)
            Task.Factory.StartNew(() => _microServiceManager.RunFromQueue(machineId), CancellationToken.None,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);
        return Ok();
    }

    /// <summary>
    /// برایِ اعلامِ درصدِ پیشرفت و اتمامِ کارها در توابعی که Immediate نیستند.
    /// </summary>
    /// <param name="machineId">
    ///	شناسه‌یِ ماشین
    /// </param>
    /// <param name="model"></param>
    /// <returns>
    /// اگر موفق باشد کدِ 200
    /// اگر مدل با ساختارِ درستی ارسال نشده باشد کدِ 400
    /// اگر منبع یافت نشود کدِ 404
    /// اگر شناسه‌یِ درخواست با آنچه که در این منبع موردِ انتظار است مطابق نباشد کدِ 409
    /// اگر خطایِ دیگری رخ دهد کدِ 500		
    /// </returns>
    [HttpPost]
    [Route("StateNotify")]
    public ActionResult StateNotify(string machineId, StateNotifyBindingModel model)
    {
        if (model == null)
            return BadRequest();

        try
        {
            ServiceResource resource = _microServiceManager.GetResource(machineId, model.funcName, model.resourceIndex);
            if (resource == null)
            {
                LogResourceNotFound(machineId, model);
                return NotFound();
            }
            if (resource.CurrentRequestId != model.requestId || model.requestId == 0)
            {
                LogConflict(model, resource, machineId);
                return Conflict();
            }
            LogStateNotify(machineId, model, resource);
            _microServiceManager.Notify(resource, model.detail, model.code, model.data,
                model.status, model.progress);
        }
        catch (Exception e)
        {
            Logger.LogError("notify 500 {0}", machineId);
            Logger.LogError(e, e.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        return Ok("ok");//todo remove "ok" if MrNazemi hasn't relied on it
    }

    /// <summary>
    /// برایِ اعلامِ سلامتِ ماشین‌ها به صورتِ دوره‌ای این درخواست ارسال می‌شود.
    /// اگر بیش از 125 ثانیه از آخرین ImAlive  و گذشته باشد، ماشین مرده تلقی می‌شود و به آن کاردهی نمی‌شود
    /// </summary>
    /// <param name="machineId">
    ///	شناسه‌یِ ماشین
    /// </param>
    /// <returns>
    /// اگر موفق باشد کدِ 200		
    /// اگر ماشین یافت نشود کدِ 404 در این صورت لازم است ماشین مجددا رجیستر کند
    /// اگر خطایِ دیگری رخ دهد کدِ 500		
    /// </returns>
    [HttpPost]
    [Route("ImAlive")]
    public ActionResult ImAlive(string machineId)
    {
        Logger.LogTrace("machineId: {0} is telling she's alive.", machineId);
        try
        {
            if (!_microServiceManager.TryUpdateAlivedTime(machineId))
            {
                Logger.LogDebug("Returning not found in response to ImAlive");
                return NotFound();
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "imalive 500 {0}", machineId);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        return Ok();
    }

    private void LogStateNotify(string machineId, StateNotifyBindingModel model, ServiceResource resource)
    {
        if (model.progress != resource.Progress)
            Logger.LogTrace("machineId: {0} is notifying its state for {1} with progress {2} for {3}.", machineId, model.requestId, model.progress, model.funcName);
    }
    private void LogConflict(StateNotifyBindingModel model, ServiceResource resource, string machineId)
    {
        Logger.LogError("conflict occured resourceRequestId: {0}  modelRequestId: {1} modelResourceIndex: {2} machineId {3}", resource.CurrentRequestId, model.requestId, model.resourceIndex, machineId);
    }
    private void LogResourceNotFound(string machineId, StateNotifyBindingModel model)
    {
        Logger.LogTrace("resource not found for {0} {1} {2}.", machineId, model.funcName, model.resourceIndex);
    }
}
