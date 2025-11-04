using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Model.BPMN.Processes;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

/// <summary>
/// A Pool is the graphical representation of a Participant in a Collaboration. 
/// A Participant can be a specific PartnerEntity (e.g., a company) or can be a more general PartnerRole (e.g., a buyer, seller, or manufacturer). 
/// A Pool MAY or MAY NOT reference a Process. A Pool is NOT REQUIRED to contain a Process, i.e., it can be a “black box.”
/// 
/// The Sequence Flows can cross the boundaries between Lanes of a Pool but cannot cross the boundaries of a Pool. 
/// 
/// A Collaboration can contain two (2) or more Pools (i.e., Participants). 
/// However, a Process that represents the work performed from the point of view of the modeler or the modeler’s organization can be considered “internal” and is NOT REQUIRED to be surrounded by the boundary of the Pool, while the other Pools in the Diagram MUST have their boundary.
/// 
/// 
/// The meaning of the Lanes is up to the modeler.
/// 
/// BPMN does not specify the usage of Lanes. Lanes are often used for such things as internal roles (e.g., Manager,Associate), 
/// systems (e.g., an enterprise application), an internal department (e.g., shipping, finance), etc. In addition, Lanes can be nested (see Figure 10.125) 
/// or defined in a matrix. For example, there could be an outer set of Lanes for company departments and then an inner set of Lanes for roles within each department.
/// </summary>
public class Lane(IFlowElementsContainer flowElementsContainer, string id, string name) : BaseElement(flowElementsContainer as BaseElement, id, name), IResourceRoleContainer
{
    //public string name;

    /// <summary>
    /// A reference to a LaneSet element for embedded Lanes.
    /// </summary>
    public LaneSet childLaneSet;

    /// <summary>
    /// A reference to a BaseElement that specifies the partition value and partition type. 
    /// Using this partition element a BPMN compliant tool can determine the FlowElements that have to be partitioned in this Lane.
    /// </summary>
    public BaseElement partitionElement;

    /// <summary>
    /// A reference to a BaseElement that specifies the partition value and partition type. 
    /// Using this partition element a BPMN compliant tool can determine the FlowElements that have to be partitioned in this Lane.
    /// </summary>
    public string partitionElementRef;

    /// <summary>
    /// The list of FlowNodes partitioned into this Lane according to the partitionElement defined as part of the Lane element.
    /// </summary>
    public List<string> flowNodeRefs;

    /// <summary>
    /// its not in bpmn.2 standard.
    /// </summary>
    public List<ResourceRole> resources { get; set; }

    public void AddResourceRole(ResourceRole resourceRole)
    {
        resources ??= [];
        resources.Add(resourceRole);
    }

    /// <summary>
    /// its not in bpmn.2 standard.
    /// </summary>
    public Performer defaultPerformer
    {
        get { return resources?.FirstOrDefault() as Performer; }
        set { resources = [value]; }
    }

    public Process process => Parent as Process;

    public void addFlowElement(string flowNodeRef)
    {
        flowNodeRefs ??= [];
        flowNodeRefs.Add(flowNodeRef);
    }
}