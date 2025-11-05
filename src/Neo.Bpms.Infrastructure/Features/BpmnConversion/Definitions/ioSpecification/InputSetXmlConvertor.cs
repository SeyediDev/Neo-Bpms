using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class InputSetXmlConvertor
{
    internal static void Export(dynamic containerElement, IDataInputContainer container)
    {
        foreach (var inputSet in container.inputSets ?? Enumerable.Empty<InputSet>())
        {
            var element = containerElement.inputSet();
            BaseElementXmlConvertor.Export(element, inputSet);
            element.name = inputSet.Name;
            // TODO
            // <xsd:element name="dataInputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            // <xsd:element name="optionalInputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            // <xsd:element name="whileExecutingInputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            // <xsd:element name="outputSetRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
        }
    }

    internal static void Import(IDataInputContainer container, ElasticObject containerElement)
    {
        container.inputSets = null;
        foreach (var element in containerElement.GetElements("inputSet") ?? Enumerable.Empty<ElasticObject>())
        {
            var inputSet = new InputSet(container, element.GetString("id"), element.GetString("name"));
            container.inputSets ??= [];
            container.inputSets.Add(inputSet);
            BaseElementXmlConvertor.Export(element, inputSet);

            // TODO
            // <xsd:element name="dataInputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            // <xsd:element name="optionalInputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            // <xsd:element name="whileExecutingInputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            // <xsd:element name="outputSetRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
        }
    }
}
