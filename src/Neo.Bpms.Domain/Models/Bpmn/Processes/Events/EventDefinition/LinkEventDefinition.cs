using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
/// <summary>
/// Catch only in intermediate
/// Throw only in intermediate
/// </summary>
public class LinkEventDefinition(BpmnDefinitions parent, string id, string name) : EventDefinition(parent, id, Event.eEventType.Link, name), ICatchEventDefinition, IThrowEventDefinition
{
    //public string name;

    /// <summary>
    /// Used to reference the corresponding 'catch' or 'target' LinkEventDefinition, when this LinkEventDefinition represents a 'throw' or 'source' LinkEventDefinition.
    /// </summary>
    public List<string> sources;

    /// <summary>
    /// Used to reference the corresponding 'throw' or 'source' LinkEventDefinition, when this LinkEventDefinition represents a 'catch' or 'target' LinkEventDefinition.
    /// </summary>
    public string target;

    public override string Code => target ?? base.Code;
}
