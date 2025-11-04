using Task = Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.Task;
using Neo.Bpms.Domain.Entities.Bpmn.Collaborations;
using Neo.Bpms.Domain.Entities.Bpmn.Collaborations.MessageFlows;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

public class MessageFlowXmlConvertor
{
    internal static void Export(dynamic collaborationElement, Collaboration collaboration)
    {
        foreach (var messageFlow in collaboration.messageFlow ?? Enumerable.Empty<MessageFlow>())
        {
            var element = collaborationElement.messageFlow();
            BaseElementXmlConvertor.Export(element, messageFlow);
            element.name = messageFlow.Name;
            element.messageRef = messageFlow.messageRef?.Id;
            element.sourceRef = messageFlow.sourceRef?.id;
            element.targetRef = messageFlow.targetRef?.id;
        }
    }

    internal static void Import(ElasticObject collaborationElement, Collaboration collaboration)
    {
        collaboration.messageFlow = null;
        foreach (var element in collaborationElement.GetElements("messageFlow") ??
                                Enumerable.Empty<ElasticObject>())
        {
            var messageFlow = new MessageFlow(collaboration, "", element.GetString("name"), null, null);
            BaseElementXmlConvertor.Import(element, messageFlow);
            messageFlow.messageRef = ProjectDefinition.Project.GetMessage(element.GetString("messageRef"));
            messageFlow.sourceRef = FindInteractionNode(element.GetString("sourceRef"), collaboration);
            messageFlow.targetRef = FindInteractionNode(element.GetString("targetRef"), collaboration);

            collaboration.messageFlow ??= [];
            collaboration.messageFlow.Add(messageFlow);
        }
    }

    private static InteractionNode FindInteractionNode(string id, Collaboration collaboration)
    {
        foreach (var participant in collaboration.participants)
        {
            if (participant.Id == id)
                return new InteractionNode(participant);
            if (participant.processRef?.flowElements == null)
                continue;
            FlowElement flowElement;
            if (!participant.processRef.flowElements.TryGetValue(id, out flowElement))
                continue;
            if (flowElement is Event)
                return new InteractionNode(flowElement as Event);
            if (flowElement is Task)
                return new InteractionNode(flowElement as Task);
        }
        return null;
    }
}
