using static Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Artifacts.Association;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Artifacts;

/// <summary>
/// An Association is used to associate information and Artifacts with Flow Objects. 
/// Text and graphical non-Flow Objects can be associated with the Flow Objects and Flow. 
/// An Association is also used to show the Activity used for compensation.
/// </summary>
public class Association(IArtifactContainer parent, string id,
    string sourceRef, string targetRef,
    AssociationDirection associationDirection = AssociationDirection.None) : Artifact(parent, id)
{
    public AssociationDirection associationDirection = associationDirection;
    public string sourceRef = sourceRef;
    public string targetRef = targetRef;

    public enum AssociationDirection
    {
        None,
        One,
        Both
    }
}
