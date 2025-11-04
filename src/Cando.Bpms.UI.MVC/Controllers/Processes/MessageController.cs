using Neo.Bpms.Domain.Entities.Base.Audit;

namespace Neo.Bpms.UI.MVC.Controllers;

[IgnoreAntiforgeryToken]
public class MessageController : ControllerBaseMVC
{
    [HttpPost]
    public JsonResult Catch(string processParticipantName, string messageName, [FromBody] LocalParameters recordDictionary)
    {
        GetUser();
        //throw new NotImplementedException("Authorization");
        AuditTrail auditTrail = new(TriggerTypeId.ProcessParticipant, processParticipantName);
        ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).MessageReceived(messageName, recordDictionary, auditTrail);
        DataStorage.SaveAudit(auditTrail);
        return Json(new { ok = true });
    }
}
