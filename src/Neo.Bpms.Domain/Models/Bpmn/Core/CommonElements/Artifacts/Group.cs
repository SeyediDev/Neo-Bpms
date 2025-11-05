namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Artifacts;

/// <summary>
/// a Group is a visual depiction of a single CategoryValue.
/// The graphical elements within the Group will be assigned the CategoryValue of the Group.
/// A Group is a rounded corner rectangle that MUST be drawn with a solid dashed line. 
/// Groups are not constrained by restrictions of Pools and Lanes.
/// Groups are often used to highlight certain sections of a Diagram without adding additional constraints for performance--as a Sub-Process would. 
/// The highlighted (grouped) section of the Diagram can be separated for reporting and analysis purposes.
/// </summary>
public class Group(IArtifactContainer parent, string id, Category category, CategoryValue value,
    List<FlowElement> categorizedFlowElements = null) : Artifact(parent, id)
{
    /// <summary>
    /// The name of the Category and the value of the CategoryValue separated by delineator "." provides the label for the Group. The graphical elements within the boundaries of the Group will be assigned the CategoryValue
    /// </summary>
    public CategoryValue value = value;

    public Category category = category;

    /// <summary>
    /// all of the elements (e.g., Events, Activities, Gateways, and Artifacts) that are within the boundaries of the Group.
    /// </summary>
    public List<FlowElement> categorizedFlowElements = categorizedFlowElements;
}
