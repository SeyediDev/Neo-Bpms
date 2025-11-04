using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

/// <summary>
/// An Escalation identifies a business situation that a Process might need to react to.
/// </summary>
public class Escalation(BpmnDefinitions parent, string id, string escalationCode, string name, ItemDefinition structure = null) : RootElement(parent, id, name), IStructureDefinition
{
    //		public string name;

    public object structureRef { get; set; } = structure;

    /// <summary>
    /// For an End Event:
    /// If the result is an Escalation, then the escalationCode MUST be supplied (if the processType attribute of the Process is set to executable) This “throws” the Escalation.
    /// For an Intermediate Escalation within normal flow:
    /// If the trigger is an Escalation, then the escalationCode MUST be entered (if the processType attribute of the Process is set to executable). This “throws” the Escalation.
    /// For an Intermediate Escalation attached to the boundary of an Activity:
    /// If the trigger is an Escalation, then the escalationCode MAY be entered. This Event “catches” the Escalation. If there is no escalationCode, then any Escalation SHALL trigger the Event. If there is an escalationCode, then only an Escalation that matches the escalationCode SHALL trigger the Escalation.
    /// </summary>
    public string escalationCode = escalationCode;

    public Entity structure => (structureRef as ItemDefinition)?.structure ?? structureRef as UiEntity ?? structureRef as Entity ?? field?.Entity;
    public EntityField field => structureRef as EntityField;//not means

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not Escalation newItem) return;
        Name = newItem.Name;
        escalationCode = newItem.escalationCode;
        structureRef = newItem.structureRef;
        CloneBase(newItem);
    }
}
