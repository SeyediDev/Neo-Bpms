using Neo.Bpms.Domain.Models.Bpmn.Collaborations;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.RootElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Collaborations;

internal static class CollaborationXmlConvertor
{
    internal static void Export(dynamic bpmnElement, BpmnDefinitions bpmnDefinitions)
    {
        foreach (var collaboration in bpmnDefinitions?.GetRootElements()?.OfType<Collaboration>() ??
                                      [])
        {
            var element = bpmnElement.collaboration();
            RootElementXmlConvertor.Export(element, collaboration);
            element.name = collaboration.Name;
            element.isClosed = collaboration.isClosed;
            ParticipantXmlConvertor.Export(collaboration, element);
            ArtifactXmlConvertor.Export(element, collaboration);
            MessageFlowXmlConvertor.Export(element, collaboration);
            CorrelationKeyXmlConvertor.Export(element, collaboration);
            /*
						<xsd:element ref="conversationNode" minOccurs="0" maxOccurs="unbounded"/>
						<xsd:element ref="conversationAssociation" minOccurs="0" maxOccurs="unbounded"/>
						<xsd:element ref="participantAssociation" minOccurs="0" maxOccurs="unbounded"/>
						<xsd:element ref="messageFlowAssociation" minOccurs="0" maxOccurs="unbounded"/>
						<xsd:element name="choreographyRef" type="xsd:QName" minOccurs="0" maxOccurs="unbounded"/>
						<xsd:element ref="conversationLink" minOccurs="0" maxOccurs="unbounded"/>
				*/
        }
    }

    internal static void Import(ElasticObject bpmn, BpmnDefinitions bpmnDefinitions)
    {
        foreach (var element in bpmn.GetElements("collaboration") ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id");
            var name = element.GetString("name");
            var collaboration = bpmnDefinitions.GetRootElements()?.OfType<Collaboration>()
                .FirstOrDefault(c => c.Id == id);
            if (collaboration == null)
            {
                collaboration = new Collaboration(bpmnDefinitions, id, name);
                bpmnDefinitions.AddRootElement(collaboration);
            }
            RootElementXmlConvertor.Import(element, collaboration);
            collaboration.isClosed = element.GetBool("isClosed");

            ParticipantXmlConvertor.Import(bpmnDefinitions, element, collaboration);
            ArtifactXmlConvertor.Import(element, collaboration);
            MessageFlowXmlConvertor.Import(element, collaboration);
            CorrelationKeyXmlConvertor.Import(element, collaboration);
            /*
						<xsd:element ref="conversationNode" minOccurs="0" maxOccurs="unbounded"/>
						<xsd:element ref="conversationAssociation" minOccurs="0" maxOccurs="unbounded"/>
						<xsd:element ref="participantAssociation" minOccurs="0" maxOccurs="unbounded"/>
						<xsd:element ref="messageFlowAssociation" minOccurs="0" maxOccurs="unbounded"/>
						<xsd:element name="choreographyRef" type="xsd:QName" minOccurs="0" maxOccurs="unbounded"/>
						<xsd:element ref="conversationLink" minOccurs="0" maxOccurs="unbounded"/>
				*/
        }
    }
}
