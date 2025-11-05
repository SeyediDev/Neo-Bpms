using Neo.Bpms.Domain.Models.Cmmn.UI.Forms.UIRules;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Logic;

public class FormLogicEventDefinition
{
    public UIRuleEvent.eEventType EventType { get; set; }
    public string Source { get; set; }
}
