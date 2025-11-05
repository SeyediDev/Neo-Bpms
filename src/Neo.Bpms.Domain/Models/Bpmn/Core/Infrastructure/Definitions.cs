using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams;

namespace Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;

/// <summary>
/// The Definitions class is the outermost containing object for all BPMN elements. It defines the scope of visibility and the namespace for all contained elements. The interchange of BPMN files will always be through one or more Definitions.
/// </summary>
public class BpmnDefinitions(string id, string name, string targetNamespace, string exporter, string exporterVersion)
    : BaseElement(null, id, name)
{
    //		public string id;
    //		public string name;

    /// <summary>
    /// follows the convention established by XML Schema
    /// </summary>
    public string targetNamespace = targetNamespace;

    /// <summary>
    /// default: XPath
    /// “http://www.w3.org/1999/XPath”
    /// </summary>
    public string expressionLanguage = "http://www.w3.org/1999/XPath";

    /// <summary>
    /// default: XMLSchema
    /// "http://www.w3.org/2001/XMLSchema"
    /// </summary>
    public string typeLanguage = "http://www.w3.org/2001/XMLSchema";

    private readonly Dictionary<string, RootElement> rootElements = [];
    public List<BPMNDiagram> diagrams;

    /// <summary>
    /// import externally defined elements and make them available for use by elements within this Definitions
    /// </summary>
    public List<Import> imports;

    public List<Extension> extensions;

    /// <summary>
    /// enables the extension and integration of BPMN models into larger system/development Processes
    /// </summary>
    public List<Relationship> relationships;

    /// <summary>
    /// the tool that is exporting the bpmn model file
    /// </summary>
    public string exporter = exporter;

    public string exporterVersion = exporterVersion;

    public Process Process => GetRootElement(Id) as Process;

    public ErrorInformationList ErrorInfos { get; set; } = [];

    public void AddRootElement(RootElement re)
    {
        rootElements.Add(re.Id, re);
    }

    public RootElement GetRootElement(string rootElementId)
    {
        if (string.IsNullOrEmpty(rootElementId))
            return null;
        IEnumerable<Message> messages = rootElements.Values.OfType<Message>();
        return !rootElements.TryGetValue(rootElementId, out RootElement re) ? null : re;
    }

    public bool RemoveRootElement(string rootElementId)
    {
        return rootElements.Remove(rootElementId ?? "");
    }

    public IEnumerable<RootElement> GetRootElements()
    {
        return rootElements?.Values;
    }

    public ItemDefinition AddOrGetItemDefinition(bool isCollection, Entity entity)
    {
        if (entity == null) return null;
        string itemDefinitionId = $"ItemDefinition.teta:Entity.{entity.NamespaceId}.{entity.Id}";
        if (GetRootElement(itemDefinitionId) is ItemDefinition itemDefinition)
            return itemDefinition;
        itemDefinition = new ItemDefinition(this, itemDefinitionId, isCollection, entity);
        AddRootElement(itemDefinition);
        return itemDefinition;
    }

    public ItemDefinition AddOrGetItemDefinition(bool isCollection, EntityField entityField)
    {
        if (entityField == null) return null;
        string itemDefinitionId = $"ItemDefinition.teta:EntityField.{entityField.Entity.NamespaceId}.{entityField.Entity.Id}.{entityField.Id}";
        if (GetRootElement(itemDefinitionId) is ItemDefinition itemDefinition)
            return itemDefinition;
        itemDefinition = new ItemDefinition(this, itemDefinitionId, isCollection, entityField);
        AddRootElement(itemDefinition);
        return itemDefinition;
    }
}
