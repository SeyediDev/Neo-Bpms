global using Neo.Bpms.Domain.Model.BPMN.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

public interface ICatchEventDefinition
{
}

public interface IThrowEventDefinition
{
}

public abstract class EventDefinition(BpmnDefinitions parent, string id, Event.eEventType type, string name = null) 
    : RootElement(parent, id, name)
{
    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not EventDefinition newItem)
        {
            return;
        }

        type = newItem.type;
        //todo for each childred ?
    }

    public Event.eEventType type { get; set; } = type;
    public virtual string Code => Id;
}
