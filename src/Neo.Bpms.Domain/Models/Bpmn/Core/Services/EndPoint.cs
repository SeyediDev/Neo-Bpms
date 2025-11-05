namespace Neo.Bpms.Domain.Models.Bpmn.Core.Services;

/// <summary>
/// The actual definition of the service address is out of scope of BPMN 2.0
/// 
/// The EndPoint element MAY be extended with endpoint reference definitions introduced in other specifications (e.g., WS-Addressing).
/// EndPoints can be specified for Participants.
/// </summary>
public partial class EndPoint(BpmnDefinitions parent, string id) : RootElement(parent, id)
{
    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not EndPoint newItem) return;
    }
}
