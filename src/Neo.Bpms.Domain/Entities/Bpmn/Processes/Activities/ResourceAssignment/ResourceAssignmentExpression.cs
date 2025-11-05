using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;

public class ResourceAssignmentExpression(ResourceRole resourceRole, string id, BpmnExpression expression) : BaseElement(resourceRole, id)
{
    /// <summary>
    /// The element ResourceAssignmentExpression MUST contain an Expression which is used at runtime to assign resource(s) to a ResourceRole element
    /// </summary>
    public BpmnExpression expression = expression;
}