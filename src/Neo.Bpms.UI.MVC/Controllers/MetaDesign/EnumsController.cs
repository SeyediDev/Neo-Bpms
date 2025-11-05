using Neo.Bpms.Domain.Models.Base.Utils;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

//todo caching and updating an enum? works on controller?
[ResponseCache(Duration = int.MaxValue, Location = ResponseCacheLocation.Client)]
public class EnumsController : ControllerBaseMVC
{
    [HttpGet]
    public JsonResult FormControllerEventTypes()
    {
        IEnumerable<IdValueEnum> result = Enum.GetValues(typeof(UIRuleEvent.eEventType))
            .Cast<UIRuleEvent.eEventType>()
            .Select(e => new IdValueEnum(e));
        return Json(result);
    }

    [HttpGet]
    public JsonResult FormControllerTaskTypes()
    {
        IEnumerable<IdValueEnum> result = Enum.GetValues(typeof(UIRuleTask.eTaskType))
            .Cast<UIRuleTask.eTaskType>()
            .Select(e => new IdValueEnum(e));
        return Json(result);
    }

    [HttpGet]
    public JsonResult FormPropertyTypes()
    {
        IEnumerable<IdValueEnum> result = Enum.GetValues(typeof(eControlPropertyId))
            .Cast<eControlPropertyId>()
            .Select(e => new IdValueEnum(e));
        return Json(result);
    }

    [HttpGet]
    public JsonResult FormControlTypes()
    {
        var result = Enum.GetValues(typeof(eControlTypeId))
            .Cast<eControlTypeId>()
            .Select(e => new { Id = e.ToString(), Name = e.ToName(), ControlGroups = e.GetControlGroup()?.Select(cg => cg.ToString()) });
        return Json(result);
    }
}
