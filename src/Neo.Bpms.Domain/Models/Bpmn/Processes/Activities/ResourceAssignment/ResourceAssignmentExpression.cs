namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.ResourceAssignment;

public class ResourceAssignmentExpression(ResourceRole resourceRole, string id, BpmnExpression expression) : BaseElement(resourceRole, id)
{
    /// <summary>
    /// The element ResourceAssignmentExpression MUST contain an Expression which is used at runtime to assign resource(s) to a ResourceRole element
    /// </summary>
    public BpmnExpression expression = expression;
}