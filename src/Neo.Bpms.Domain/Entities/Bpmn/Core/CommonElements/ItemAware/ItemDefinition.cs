using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;

/// <summary>
/// BPMN elements, such as DataObjects and Messages, represent items that are manipulated, transferred, transformed, 
/// or stored during Process flows. These items can be either physical items, such as the mechanical part of a vehicle, 
/// or information items such the catalog of the mechanical parts of a vehicle.
/// 
/// An important characteristics of items in Process is their structure. 
/// BPMN does not require a particular format for this data structure, but it does designate XML Schema as its default. 
/// The structure attribute references the actual data structure.
/// The default format of the data structure for all elements can be specified in the Definitions element using the typeLanguage attribute.
/// 
/// When an ItemDefinition is defined it is contained in Definitions.
/// </summary>
public class ItemDefinition(BpmnDefinitions parent, string id, bool isCollection, object structureRef = null) 
    : RootElement(parent, id), IStructureDefinition
{
    /// <summary>
    /// specifies the nature of an item which can be a physical or an information item
    /// </summary>
    public eItemKind itemKind = eItemKind.Information;

    /// <summary>
    /// The concrete data structure to be used.
    /// Structure definitions are always defined as separate entities, so they cannot be inlined in one of their usages. 
    /// You will see that in every mention of structure definition there is a “reference” to the element. 
    /// This is why this class inherits from RootElement.
    /// </summary>
    public object structureRef { get; set; } = structureRef;

    /// <summary>
    /// Identifies the location of the data structure and its format. 
    /// If the importType attribute is left unspecified, the typeLanguage specified in the Definitions that contains this ItemDefinition is assumed
    /// 
    /// An ItemDefinition element can specify an import reference where the proper definition of the structure is defined.
    /// </summary>
    public Import import;

    /// <summary>
    /// Setting this flag to true indicates that the actual data type is a collection.
    /// If this attribute is set to “true,” but the actual type is not a collection type, the model is considered as invalid.
    /// </summary>
    public bool isCollection { get; set; } = isCollection;

    public EntityField entityField => structureRef as EntityField;
    public Entity structure => structureRef as UiEntity ?? structureRef as Entity ?? entityField?.Entity;

    public enum eItemKind
    {
        Information,
        Physical
    }

    public override void Copy(RootElement newRootElement)
    {
        ItemDefinition itemDefinition = (ItemDefinition)newRootElement;
        if (itemDefinition == null) return;
        itemDefinition.itemKind = itemKind;
        itemDefinition.structureRef = structureRef;
        itemDefinition.import = import;
        itemDefinition.isCollection = isCollection;
    }
}
