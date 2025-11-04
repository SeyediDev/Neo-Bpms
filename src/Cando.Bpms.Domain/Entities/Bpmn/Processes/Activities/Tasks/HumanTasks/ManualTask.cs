using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;

/// <summary>
/// A Manual Task is a Task that is expected to be performed without the aid of any business process execution engine or any application. 
/// An example of this could be a telephone technician installing a telephone at a customer location.
/// A Manual Task is neither executed by nor managed by a business process runtime.
/// 
/// The User Task can be implemented using different technologies, specified by the implementation attribute. 
/// Besides the Web service technology, any technology can be used. 
/// A User Task for instance can be implemented using WSHumanTask by setting the implementation attribute to “http://docs.oasis-open.org/ns/bpel4people/ws-humantask/protocol/200803.”
/// </summary>
public class ManualTask(IFlowElementsContainer flowElementsContainer,
    string id, string name) : Task(flowElementsContainer, id, name, eActivityType.ManualTask)
{
    public bool ControlBySystem;
}
