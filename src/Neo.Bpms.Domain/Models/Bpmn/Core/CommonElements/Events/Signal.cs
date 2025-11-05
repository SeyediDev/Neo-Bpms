using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;

namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

public class Signal(BpmnDefinitions bpmn, string id, string name, ItemDefinition structureRef) : RootElement(bpmn, id, name), IStructureDefinition
{
    //		public string name;
    public object structureRef { get; set; } = structureRef;

    public EntityField entityField => structureRef as EntityField;//not means
    public Entity structure => (structureRef as ItemDefinition)?.structure ?? structureRef as UiEntity ?? structureRef as Entity ?? entityField?.Entity;

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not Signal newItem) return;
        Name = newItem.Name;
        structureRef = newItem.structureRef;
        CloneBase(newItem);
    }
}
