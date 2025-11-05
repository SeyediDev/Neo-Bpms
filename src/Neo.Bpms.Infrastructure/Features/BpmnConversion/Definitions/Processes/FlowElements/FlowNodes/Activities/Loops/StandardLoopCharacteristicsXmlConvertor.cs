using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.LoopCharacteristic;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes.Activities.Loops;

internal class StandardLoopCharacteristicsXmlConvertor
{
    internal static void Export(dynamic element, StandardLoopCharacteristics standardLoopCharacteristics)
    {
        if (standardLoopCharacteristics == null) return;
        BaseElementXmlConvertor.Export(element, standardLoopCharacteristics);
        if (standardLoopCharacteristics.loopCondition != null)
            FormalExpressionXmlConvertor.Export(element.loopCondition(), standardLoopCharacteristics.loopCondition);
        element.testBefore = standardLoopCharacteristics.testBefore;
        element.loopMaximum = standardLoopCharacteristics.loopMaximum;
    }

    internal static void Import(Activity activity, ElasticObject element)
    {
        var standardLoopCharacteristics = new StandardLoopCharacteristics(activity,
            element.GetString("id"),
            FormalExpressionXmlConvertor.Import(element.GetElement("loopCondition")));
        BaseElementXmlConvertor.Import(element, standardLoopCharacteristics);
        standardLoopCharacteristics.testBefore = element.GetBool("testBefore");
        standardLoopCharacteristics.loopMaximum = element.GetInteger("loopMaximum", 1);
        activity.loopCharacteristics = standardLoopCharacteristics;
    }
}
