using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;

/// <summary>
/// A Business Rule Task provides a mechanism for the Process to provide input to a Business Rules Engine 
/// and to get the output of calculations that the Business Rules Engine might provide. 
/// The InputOutputSpecification of the Task will allow the Process to send data to and receive 
/// data from the Business Rules Engine.
/// </summary>
public class BusinessRuleTask(IFlowElementsContainer flowElementsContainer,
    string id, string name) : Task(flowElementsContainer, id, name, eActivityType.BusinessRuleTask)
{
    /// <summary>
    /// This attribute specifies the technology that will be used to implement the Business Rule Task. 
    /// Valid values are "##unspecified" for leaving the implementation technology open, "##WebService" 
    /// for the Web service technology or a URI identifying any other technology or coordination protocol.
    /// The default technology for this task is unspecified.
    /// </summary>
    public string implementation = "##unspecified";
    //public List<Decision> decisions = new List<Decision>();
    //public void addDecision(Decision decision)
    //{
    //	decisions.Add(decision);
    //}
}
