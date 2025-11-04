using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.MessageFlows;

namespace Neo.Bpms.Domain.Entities.Bpmn.Choreographies;

public class ChoreographyTask(Choreography choreography, string id, string name) : ChoreographyActivity(choreography, id, name)
{
    public List<MessageFlow> messageFlowRef = [];
}
