using Neo.Bpms.Domain.Models.Bpmn.Core.Foundation;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

internal static class BaseElementXmlConvertor
{
    internal static void NamespaceExport(dynamic node, string @namespace = null)
    {
        node.Namespace = @namespace == "teta"
            ? "http://www.bmi.ir/schemas/BPMN20Extensions/MODEL"
            : "http://www.omg.org/spec/BPMN/20100524/MODEL";
    }

    internal static void ExportRef(dynamic node, BaseElement baseElement, string @namespace = null)
    {
        NamespaceExport(node, @namespace);
        node.InternalValue = baseElement.Id;
    }


    internal static void Export(dynamic node, BaseElement baseElement, string @namespace = null, bool exportId = true)
    {
        NamespaceExport(node, @namespace);
        //if(exportId)
        node.id = baseElement.Id;
        ExtensionDefinitionsExport(node, baseElement);
        //todo values
        DocumentationXmlConvertor.Export(node, baseElement, @namespace);
    }
    internal static void ExportWithId(string id, dynamic node, BaseElement baseElement, string @namespace = null, bool exportId = true)
    {
        NamespaceExport(node, @namespace);
        //if(exportId)
        node.id = id;
        ExtensionDefinitionsExport(node, baseElement);
        //todo values
        DocumentationXmlConvertor.Export(node, baseElement, @namespace);
    }


    internal static void Import(ElasticObject element, BaseElement baseElement)
    {
        baseElement.Id = element.GetString("id");
        ImportExtensionDefinitions(element, baseElement);
        //ImportExtensionValues(element, baseElement );
        DocumentationXmlConvertor.Import(element, baseElement);
    }

    internal static void ExportExtensionItem(dynamic node, ElasticObject extension)
    {
        var extensionElements = ExportExtensionElements(node);
        extensionElements.AddElement(extension);
    }

    internal static dynamic ExportExtensionElements(dynamic node)
    {
        var extensionElements = node.GetElement("extensionElements");
        if (extensionElements == null)
        {
            extensionElements = node.extensionElements();
            NamespaceExport(extensionElements);
        }
        return extensionElements;
    }

    internal static List<ElasticObject> ImportExtensionItem(ElasticObject element, string extensionName)
    {
        return ImportExtensionElementsNode(element)?.GetElements(extensionName);
    }

    internal static ElasticObject ImportExtensionElementsNode(ElasticObject element)
    {
        return element?.GetElement("extensionElements");
    }

    private static void ImportExtensionDefinitions(ElasticObject element, BaseElement baseElement)
    {
        foreach (var extensionDefinitionNode in element.GetElements("extensionDefinition") ??
                                                             Enumerable.Empty<ElasticObject>())
        {
            //todo extensionDefinition defined in bpmn definitions
            baseElement.extensionDefinitions ??= [];
            var name = extensionDefinitionNode.GetString("name");
            var extensionDefinition = baseElement.extensionDefinitions.FirstOrDefault(e => e.name == name);
            if (extensionDefinition == null)
            {
                extensionDefinition = new ExtensionDefinition(name);
                baseElement.extensionDefinitions.Add(extensionDefinition);
            }
            foreach (var attributeDefinitionElement in extensionDefinitionNode.GetElements("attributeDefinition") ??
                                                                     Enumerable.Empty<ElasticObject>())
            {
                extensionDefinition.extensionAttributeDefinitions ??= [];
                var attributeName = attributeDefinitionElement.GetString("name");
                var extensionAttributeDefinition =
                    extensionDefinition.extensionAttributeDefinitions.FirstOrDefault(a => a.name == attributeName);
                if (extensionAttributeDefinition == null)
                {
                    extensionAttributeDefinition = new ExtensionAttributeDefinition(attributeName, string.Empty);
                    extensionDefinition.extensionAttributeDefinitions.Add(extensionAttributeDefinition);
                }

                extensionAttributeDefinition.type = attributeDefinitionElement.GetString("type");
                extensionAttributeDefinition.isReference = attributeDefinitionElement.GetBool("isReference", false);
            }
        }
    }

    private static void ExtensionDefinitionsExport(dynamic node, BaseElement baseElement)
    {
        if (baseElement.extensionDefinitions == null) return;
        var extensionDefinitions = node.extensionDefinitions();
        foreach (var extensionDefinition in baseElement.extensionDefinitions)
        {
            var extensionDefinitionElement = extensionDefinitions.extensionDefinition();
            NamespaceExport(extensionDefinitionElement);
            extensionDefinitionElement.name = extensionDefinition.name;
            foreach (var attributeDefinition in extensionDefinition.extensionAttributeDefinitions ??
                                                            Enumerable.Empty<ExtensionAttributeDefinition>())
            {
                var attribute = extensionDefinitionElement.attributeDefinition();
                NamespaceExport(attribute);
                attribute.name = attributeDefinition.name;
                attribute.type = attributeDefinition.type;
                attribute.isReference = attributeDefinition.isReference;
            }
        }
    }

    /*
	private static void ImportExtensionValues(ElasticObject element, BaseElement baseElement)
	{
		var extensionValueNode = element.GetElement("extensionElements");
		if (extensionValueNode == null) return;
		foreach (var elements in extensionValueNode.ElementCollections ?? Enumerable.Empty<KeyValuePair<string, List<ElasticObject>>>())
		{
			if( baseElement.extensionValues==null )
				baseElement.extensionValues = new List<ExtensionAttributeValue>();
			var extensionAttributeDefinition = elements.Key;
			var extensionValue = new ExtensionAttributeValue();
			baseElement.extensionValues.Add(extensionValue);
			foreach (var elementNode in elements.Value)
			{
			}
	}
		}*/
}
