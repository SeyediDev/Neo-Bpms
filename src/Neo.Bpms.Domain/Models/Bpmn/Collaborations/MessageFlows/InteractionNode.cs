using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Task = Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.Task;

namespace Neo.Bpms.Domain.Models.Bpmn.Collaborations.MessageFlows;

/// <summary>
/// The InteractionNode element is used to provide a single element as the source and target Message Flow associations (see Figure 9.14, above) instead of the individual associations of the elements that can connect to Message Flows. 
/// Only the Pool/Participant, Activity, and Event elements can connect to Message Flows. 
/// The InteractionNode element is also used to provide a single element for source and target of Conversation Links
/// </summary>
public class InteractionNode
{
    private BaseElement _node;
    public InteractionNode(Event ev)
    {
        _node = ev;
        nodeType = eType.Event;
    }
    public InteractionNode(Task task)
    {
        _node = task;
        nodeType = eType.Task;
    }
    public InteractionNode(Participant participant)
    {
        _node = participant;
        nodeType = eType.Participant;
    }
    public eType nodeType;
    public Event @event => _node as Event;
    public Task task => _node as Task;
    public Participant participant => _node as Participant;
    public string id => _node?.Id;
    public enum eType
    {
        Event,
        Task,
        Participant
    }
}
