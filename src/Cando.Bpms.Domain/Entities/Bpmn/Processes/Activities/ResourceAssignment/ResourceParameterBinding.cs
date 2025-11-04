using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Resources;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;

/// <summary>
/// Resources support query parameters that are passed to the Resource query at runtime. 
/// Parameters MAY refer to Task instance data using Expressions. 
/// During Resource query execution, an infrastructure can decide which of the Parameters defined by the Resource are used. 
/// It MAY use zero (0) or more of the Parameters specified. It MAY also override certain Parameters with values defined during Resource deployment. 
/// The deployment mechanism for Tasks and Resources is out of scope for this specification. 
/// Resource queries are evaluated to determine the set of Resources, e.g., people, assigned to the Activity. 
/// Failed Resource queries are treated like Resource queries that return an empty result set. 
/// Resource queries return one Resource or a set of Resources.
/// </summary>
public class ResourceParameterBinding(ResourceRole resourceRole, string id, ResourceParameter parameterRef, BpmnExpression expression) : BaseElement(resourceRole, id)
{
    /// <summary>
    /// Reference to the parameter defined by the Resource.
    /// </summary>
    public ResourceParameter parameterRef = parameterRef;
    /// <summary>
    /// The Expression that evaluates the value used to bind the ResourceParameter.
    /// </summary>
    public BpmnExpression expression = expression;
    public FormalExpression FormalExpression => expression as FormalExpression;
}
