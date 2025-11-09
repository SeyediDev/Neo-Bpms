namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
/// <summary>
/// Catch only in boundary interrupting
/// Throw only in end
/// </summary>
public class CancelEventDefinition(BpmnDefinitions parent, string id) : EventDefinition(parent, id, Event.eEventType.Cancel), ICatchEventDefinition, IThrowEventDefinition
{
}
