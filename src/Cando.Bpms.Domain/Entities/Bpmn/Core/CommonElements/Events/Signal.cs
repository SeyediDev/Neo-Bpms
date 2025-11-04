using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

public class Signal(BpmnDefinitions bpmn, string id, string name, ItemDefinition structureRef) : RootElement(bpmn, id, name), IStructureDefinition
{
    //		public string name;
    public object structureRef { get; set; } = structureRef;

    public Entity structure => (structureRef as ItemDefinition)?.structure ?? structureRef as UiEntity ?? structureRef as Entity ?? field?.Entity;
    public EntityField field => structureRef as EntityField;//not means

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not Signal newItem) return;
        Name = newItem.Name;
        structureRef = newItem.structureRef;
        CloneBase(newItem);
    }
}
