using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class OutputSetXmlConvertor
{
    internal static void Export(dynamic containerElement, IDataOutputContainer container)
    {
        foreach (var outputSet in container.outputSets ?? Enumerable.Empty<OutputSet>())
        {
            var element = containerElement.outputSet();
            BaseElementXmlConvertor.ExportRef(element, outputSet);
            element.name = outputSet.Name;
            // TODO
            //	<xsd:element name="dataOutputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            //	<xsd:element name="optionalOutputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            //	<xsd:element name="whileExecutingOutputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            //	<xsd:element name="inputSetRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
        }
    }

    internal static void Import(IDataOutputContainer container, ElasticObject containerElement)
    {
        container.outputSets = null;
        foreach (var element in containerElement.GetElements("outputSet") ?? Enumerable.Empty<ElasticObject>())
        {
            container.outputSets ??= [];
            var outputSet = new OutputSet(container, element.GetString("id"), element.GetString("name"));
            container.outputSets.Add(outputSet);
            BaseElementXmlConvertor.Export(element, outputSet);

            // TODO
            //	<xsd:element name="dataOutputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            //	<xsd:element name="optionalOutputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            //	<xsd:element name="whileExecutingOutputRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
            //	<xsd:element name="inputSetRefs" type="xsd:IDREF" minOccurs="0" maxOccurs="unbounded"/>
        }
    }
}
