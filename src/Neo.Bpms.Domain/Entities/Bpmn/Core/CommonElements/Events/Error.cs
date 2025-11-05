using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

/// <summary>
/// An Error represents the content of an Error Event or the Fault of a failed Operation
/// An ItemDefinition is used to specify the structure of the Error. 
/// An Error is generated when there is a critical problem in the processing of an Activity or when the execution of an Operation failed.
/// </summary>
public class Error(BpmnDefinitions parent, string id, string name, string errorCode, ItemDefinition structureRef) : RootElement(parent, id, name), IStructureDefinition
{
    //The descriptive name of the Error.
    //public string name;

    public object structureRef { get; set; } = structureRef;

    /// <summary>
    /// For an End Event:
    /// If the result is an Error, then the errorCode MUST be supplied (if the processType attribute of the Process is set to executable) This “throws” the Error.
    /// For an Intermediate Event within normal flow:
    /// If the trigger is an Error, then the errorCode MUST be entered (if the processType attribute of the Process is set to executable). This “throws” the Error.
    /// For an Intermediate Event attached to the boundary of an Activity:
    /// If the trigger is an Error, then the errorCode MAY be entered. This Event “catches” the Error. If there is no errorCode, then any error SHALL trigger the Event. If there is an errorCode, then only an Error that matches the errorCode SHALL trigger the Event.
    /// </summary>
    public string errorCode = errorCode;

    public EntityField entityField => structureRef as EntityField;
    public Entity structure => (structureRef as ItemDefinition)?.structure ?? structureRef as UiEntity ?? structureRef as Entity ?? entityField?.Entity;

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not Error newItem) return;
        Name = newItem.Name;
        errorCode = newItem.errorCode;
        structureRef = newItem.structureRef;
        CloneBase(newItem);
    }
}
