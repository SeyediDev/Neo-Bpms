using Neo.Bpms.Domain.Entities.Bpmn.Collaborations;
using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.PoolAndParticipant;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Services;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class ParticipantXmlConvertor
{
    internal static void Export(Collaboration collaboration, dynamic collaborationElement)
    {
        foreach (var participant in collaboration.participants ?? Enumerable.Empty<Participant>())
        {
            var element = collaborationElement.participant();
            BaseElementXmlConvertor.Export(element, participant);
            element.name = participant.Name;
            element.processRef = participant.processRef?.Id;
            foreach (var interfaceRef in participant.interfaceRef ?? Enumerable.Empty<Interface>())
            {
                var interfaceElement = element.interfaceRef();
                interfaceElement.InternalValue = interfaceRef.Id;
            }
            //TODO
            /*<xsd:element name="endPointRef" type="xsd:QName" minOccurs="0" maxOccurs="unbounded"/>
				<xsd:element ref="participantMultiplicity" minOccurs="0" maxOccurs="1"/>*/
        }
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject collaborationElement, Collaboration collaboration)
    {
        var oldParticipant = collaboration.participants;
        collaboration.participants = null;
        foreach (var element in collaborationElement.GetElements("participant") ?? Enumerable.Empty<ElasticObject>())
        {
            var participantId = element.GetString("id");
            var participant = oldParticipant?.FirstOrDefault(p => p.Id == participantId) ??
                              new Participant(collaboration, "", element.GetString("name"));
            BaseElementXmlConvertor.Import(element, participant);
            var processId = element.GetString("processRef");
            if (participant.processRef?.Id != processId)
                participant.processRef = bpmnDefinitions.GetRootElement(processId) as Process /*?? trow exception*/;
            //todo			else throw exception
            foreach (var interfaceRefElement in element.GetElements("interfaceRef") ?? Enumerable.Empty<ElasticObject>())
            {
                var interfaceId = interfaceRefElement.InternalValue.ToString();
                if (participant.interfaceRef.FirstOrDefault(i => i.Id == interfaceId) == null)
                    participant.interfaceRef.Add(ProjectDefinition.Project.GetInterface(interfaceId));
            }

            collaboration.participants ??= [];
            collaboration.participants.Add(participant);
        }
    }
}