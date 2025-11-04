using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

/// <summary>
/// Process models do not exist in isolation and generally participate in larger, more complex business and system
/// development Processes. The intention of the following specification element is to enable BPMN Artifacts to be
/// integrated in these development Processes via the specification of a non-intrusive identity/relationship model between
/// BPMN Artifacts and elements expressed in any other addressable domain model.
/// The ‘identity/relationship’ model it is reduced to the creation of families of typed relationships that enable BPMN and
/// non-BPMN Artifacts to be related in non intrusive manner. By simply defining ‘relationship types’ that can be
/// associated with elements in the BPMN Artifacts and arbitrary elements in a given addressable domain model, it
/// enables the extension and integration of BPMN models into larger system/development Processes.
/// It is that these extensions will enable, for example, the linkage of ‘derivation’ or ‘definition’ relationships between UML
/// artifacts and BPMN Artifacts in novel ways. So, a UML use case could be related to a Process element in the
/// BPMN specification without affecting the nature of the Artifacts themselves, but enabling different integration
/// models that traverse specialized relationships.
/// Simply, the model enables the external specification of augmentation relationships between BPMN Artifacts and
/// arbitrary relationship classification models, these external models, via traversing relationships declared in the external
/// definition allow for linkages between BPMN elements and other structured or non-structured metadata definitions.
/// The UML model for this specification follow a simple extensible pattern as shown below; where named relationships can
/// be established by referencing objects that exist in their given namespaces.
/// </summary>
public class Relationship(BpmnDefinitions bpmnDefinitions, string id, Relationship.RelationshipDirection direction, string type) : BaseElement(bpmnDefinitions, id)
{
    public enum RelationshipDirection { None, Forward, Backward, Both }
    /// <summary>
    /// The descriptive name of the element.
    /// </summary>
    public string type = type;
    /// <summary>
    /// This attribute specifies the direction of the relationship.
    /// </summary>
    public RelationshipDirection direction = direction;
    /// <summary>
    /// defines artifacts that are augmented by the relationship.
    /// </summary>
    public List<Element> sources;
    /// <summary>
    /// defines artifacts used to extend the semantics of the source element(s).
    /// </summary>
    public List<Element> targets;
}
