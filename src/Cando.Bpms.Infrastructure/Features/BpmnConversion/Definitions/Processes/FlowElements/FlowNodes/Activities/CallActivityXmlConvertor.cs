using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.CallActivity;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class CallActivityXmlConvertor
{
    internal static dynamic Export(dynamic process, Activity activity)
    {
        var node = process.callActivity();
        if (activity is not CallActivity callActivity) return node;
        node.calledElement = callActivity.CalledElementId;
        node.processRefId = callActivity.CalledElementId;
        return node;
    }

    internal static Activity Import(IFlowElementsContainer flowElementsContainer, string elementsKey,
        ElasticObject elementNode, FlowElement flowElement, ref Activity activityTypeCheckElement)
    {
        Activity activity = null;
        switch (elementsKey)
        {
            case "callActivity":
                activity = ImportCallActivity(flowElementsContainer, elementNode, flowElement, out activityTypeCheckElement);
                break;
            case "globalTask":
                activity = ImportGlobalTask(flowElementsContainer, elementNode, flowElement, out activityTypeCheckElement);
                break;
        }
        return activity;
    }

    private static CallActivity ImportCallActivity(IFlowElementsContainer flowElementsContainer, ElasticObject elementNode, FlowElement flowElement,
        out Activity activityTypeCheckElement)
    {
        activityTypeCheckElement = flowElement as CallActivity;
        var callActivity = flowElement as CallActivity;
        var calledElement = elementNode.GetString("processRefId"); //todo
        if (string.IsNullOrEmpty(calledElement))
            calledElement = elementNode.GetString("calledElement");
        if (callActivity == null)
        {
            callActivity = new CallActivity(flowElementsContainer, "", "", calledElement,
                Activity.CallActivityTypeId.SubProcess); //todo type of callActivity Global
        }
        else
            callActivity.CalledElementId = calledElement;
        return callActivity;
    }

    private static CallActivity ImportGlobalTask(IFlowElementsContainer flowElementsContainer, ElasticObject elementNode, FlowElement flowElement,
        out Activity activityTypeCheckElement)
    {
        //todo not implementation
        activityTypeCheckElement = flowElement as CallActivity;
        var callActivity = flowElement as CallActivity;
        var calledElement = elementNode.GetString("processRefId"); //todo
        if (string.IsNullOrEmpty(calledElement))
            calledElement = elementNode.GetString("calledElement");
        if (callActivity == null)
        {
            callActivity = new CallActivity(flowElementsContainer, "", "", calledElement,
                Activity.CallActivityTypeId.SubProcess); //todo type of callActivity Global
        }
        else
            callActivity.CalledElementId = calledElement;
        return callActivity;
    }
}
