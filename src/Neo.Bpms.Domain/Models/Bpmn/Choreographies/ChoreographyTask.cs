using Neo.Bpms.Domain.Models.Bpmn.Collaborations.MessageFlows;

namespace Neo.Bpms.Domain.Models.Bpmn.Choreographies;

public class ChoreographyTask(Choreography choreography, string id, string name) : ChoreographyActivity(choreography, id, name)
{
    public List<MessageFlow> messageFlowRef = [];
}
