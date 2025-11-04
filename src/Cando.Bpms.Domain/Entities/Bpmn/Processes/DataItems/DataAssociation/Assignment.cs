using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.DataAssociation;

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
