namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataAssociation;

/// <summary>
/// The Assignment class is used to specify a simple mapping of data elements using a specified Expression language.
/// </summary>
public class Assignment(DataAssociation dataAssociation, string id, BpmnExpression from, BpmnExpression to) : BaseElement(dataAssociation, id)
{
    /// <summary>
    /// The Expression that evaluates the source of the Assignment
    /// </summary>
    public BpmnExpression from = from;

    /// <summary>
    /// The Expression that defines the actual Assignment operation and the target data element.
    /// </summary>
    public BpmnExpression to = to;
}
