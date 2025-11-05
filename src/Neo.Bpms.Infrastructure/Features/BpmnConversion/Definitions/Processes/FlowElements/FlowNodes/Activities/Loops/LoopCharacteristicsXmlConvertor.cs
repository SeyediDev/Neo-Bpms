using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.LoopCharacteristic;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes.Activities.Loops;

internal static class LoopCharacteristicsXmlConvertor
{
    internal static void Export(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, dynamic activityElement, Activity activity)
    {
        switch (activity?.loopCharacteristics?.loopType)
        {
            case LoopCharacteristics.eLoopType.Standard:
                StandardLoopCharacteristicsXmlConvertor.Export(
                    activityElement.standardLoopCharacteristics(),
                    activity.loopCharacteristics as StandardLoopCharacteristics);
                break;
            case LoopCharacteristics.eLoopType.MultiInstance:
                MultiInstanceLoopCharacteristicsXmlConvertor.Export(bpmnDefinitions, bpmnElement,
                    activityElement.multiInstanceLoopCharacteristics(),
                    activity.loopCharacteristics as MultiInstanceLoopCharacteristics);
                break;
        }
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        Activity activity, ElasticObject activityElement)
    {
        var element = activityElement.GetElement("standardLoopCharacteristics");
        if (element != null)
        {
            StandardLoopCharacteristicsXmlConvertor.Import(activity, element);
            return;
        }
        element = activityElement.GetElement("multiInstanceLoopCharacteristics");
        if (element != null)
            MultiInstanceLoopCharacteristicsXmlConvertor.Import(bpmnDefinitions, bpmnElement,
                activity, element);
    }
}
