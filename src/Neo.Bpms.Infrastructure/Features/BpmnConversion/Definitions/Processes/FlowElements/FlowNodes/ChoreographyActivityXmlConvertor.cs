using Neo.Bpms.Domain.Models.Bpmn.Choreographies;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Collaborations;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes;

internal static class ChoreographyActivityXmlConvertor
{
    internal static void Export(dynamic processElement, FlowElement flowElement)
    {
        if (flowElement is not ChoreographyActivity choreographyActivity) return;
        var element = processElement.choreographyActivity();
        FlowNodeXmlConvertor.Export(element, choreographyActivity);
        element.loopType = choreographyActivity.loopType;
        //todo participantRefs
        //todo initiatingParticipantRef
        CorrelationKeyXmlConvertor.Export(element, choreographyActivity);
    }

    internal static ChoreographyActivity Import(IFlowElementsContainer flowElementsContainer, string elementName,
        ElasticObject elementNode, ref FlowElement flowElement, string id)
    {
        ChoreographyActivity choreographyActivityTypeCheckElement = null;
        var choreographyActivity = flowElement as ChoreographyActivity;

        var choreographyActivityDef = ImportChoreographyActivity(flowElementsContainer, elementName, elementNode,
            choreographyActivity, ref choreographyActivityTypeCheckElement);

        if (choreographyActivityTypeCheckElement == null && choreographyActivity != null)
        {
            flowElementsContainer.flowElements.Remove(id);
            flowElement = null;
        }

        return choreographyActivityDef;
    }

    private static ChoreographyActivity ImportChoreographyActivity(IFlowElementsContainer flowElementsContainer,
        string elementsKey, ElasticObject elementNode,
        FlowElement flowElement, ref ChoreographyActivity choreographyActivityTypeCheckElement)
    {
        ChoreographyActivity choreographyActivityDef = null;
        switch (elementsKey)
        {
            case "choreographyActivity":
                choreographyActivityTypeCheckElement = flowElement as ChoreographyActivity;
                choreographyActivityDef = ImportChoreographyActivity(flowElementsContainer, elementNode,
                    flowElement as ChoreographyActivity);
                break;
        }
        return choreographyActivityDef;
    }

    private static ChoreographyActivity ImportChoreographyActivity(IFlowElementsContainer flowElementsContainer,
        ElasticObject element, ChoreographyActivity choreographyActivity)
    {
        var id = element.GetString("id");
        var name = element.GetString("name");
        if (choreographyActivity == null)
            choreographyActivity = new ChoreographyActivity(flowElementsContainer as Choreography, id, name);
        else
            choreographyActivity.Name = name;
        FlowNodeXmlConvertor.Import(element, choreographyActivity);
        choreographyActivity.loopType =
            element.GetEnumText("loopType", ChoreographyActivity.ChoreographyLoopType.None);
        //todo participantRefs
        //todo initiatingParticipantRef
        CorrelationKeyXmlConvertor.Import(element, choreographyActivity);
        return choreographyActivity;
    }
}
