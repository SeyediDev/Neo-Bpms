namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;

/// <summary>
/// A Sequence Flow is used to show the order of Flow Elements in a Process or a Choreography. 
/// Each Sequence Flow has only one source and only one target. The source and target MUST be from the set of the following Flow Elements: 
/// Events (Start, Intermediate, and End), Activities (Task and Sub-Process; for Processes), Choreography Activities (Choreography Task and Sub-Choreography; for Choreographies), and Gateways.
/// A Sequence Flow can optionally define a condition Expression, indicating that the token will be passed down the Sequence Flow only if the Expression evaluates to true. 
/// This Expression is typically used when the source of the Sequence Flow is a Gateway or an Activity.
/// 
/// A conditional outgoing Sequence Flow from an Activity MUST be drawn with a mini-diamond marker at the beginning of the connector.
/// 
/// A Sequence Flow that has an Exclusive, Inclusive, or Complex Gateway or an Activity as its source can also be defined with as default. 
/// Such a Sequence Flow will have a marker to show that it is a default flow. The default Sequence Flow is taken (a token is passed) 
/// only if all the other outgoing Sequence Flows from the Activity or Gateway are not validA Sequence Flow that has an Exclusive, 
/// Inclusive, or Complex Gateway or an Activity as its source can also be defined with as default. Such a Sequence Flow will have 
/// a marker to show that it is a default flow. The default Sequence Flow is taken (a token is passed) only if all the other outgoing 
/// Sequence Flows from the Activity or Gateway are not valid
/// </summary>
public class SequenceFlow : FlowElement
{
    ///// <summary>
    ///// not in BPMN
    ///// </summary>
    //enum eFlowTypes { SequenceFlow, DataAssociation };

    /// <summary>
    /// The FlowNode that the Sequence Flow is connecting from.
    /// For a Process: Of the types of FlowNode, only Activities, Gateways, and Events can be the source. However, Activities that are Event Sub-Processes are not allowed to be a source.
    /// For a Choreography: Of the types of FlowNode, only Choreography Activities, Gateways, and Events can be the source.
    /// </summary>
    public FlowNode sourceRef;

    /// <summary>
    /// The FlowNode that the Sequence Flow is connecting to.
    /// For a Process: Of the types of FlowNode, only Activities, Gateways, and Events can be the target. However, Activities that are Event Sub-Processes are not allowed to be a target.
    /// For a Choreography: Of the types of FlowNode, only Choreography Activities, Gateways, and Events can be the target.
    /// </summary>
    public FlowNode targetRef;

    /// <summary>
    /// An optional boolean Expression that acts as a gating condition. A token will only be placed on this Sequence Flow if this conditionExpression evaluates to true.
    /// </summary>
    public BpmnExpression conditionExpression;

    /// <summary>
    /// An optional boolean value specifying whether Activities or Choreography Activities not in the model 
    /// containing the Sequence Flow can occur between the elements connected by the Sequence Flow. 
    /// If the value is true, they MAY NOT occur. If the value is false, they MAY occur. 
    /// Also see the isClosed attribute on Process, Choreography, and Collaboration. 
    /// When the attribute has no value, the default semantics depends on the kind of model containing Sequence Flows:
    /// • For non-executable Processes (public Processes and non-executable private Processes) and Choreographies no value has the same semantics as if the value were false.
    /// • For an executable Processes no value has the same semantics as if the value were true.
    /// • For executable Processes, the attribute MUST NOT be false.
    /// </summary>
    public bool isImmediate;//todo not used in BPMS Engine


    public SequenceFlow(IFlowElementsContainer flowElementsContainer, string id, string name, FlowNode sourceRef,
        FlowNode targetRef, BpmnExpression conditionExpression, bool isImmediate = true) :
        base(flowElementsContainer, id, name, eFlowElementType.SequenceFlow)
    {
        this.sourceRef = sourceRef;
        this.targetRef = targetRef;
        this.conditionExpression = conditionExpression;
        this.isImmediate = isImmediate;
        sourceRef.outgoing.Add(this);
        targetRef.incoming.Add(this);
    }

    public SequenceFlow(IFlowElementsContainer flowElementsContainer, string id, string name, FlowNode sourceRef,
        FlowNode targetRef,
        bool isImmediate = true) :
        this(flowElementsContainer, id, name, sourceRef, targetRef, null, isImmediate)
    {
    }

    public int StateId { get; set; }
}
