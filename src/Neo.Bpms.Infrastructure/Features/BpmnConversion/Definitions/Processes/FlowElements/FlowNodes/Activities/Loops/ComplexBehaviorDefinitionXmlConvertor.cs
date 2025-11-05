using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.LoopCharacteristic;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.ThrowEvent;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal class ComplexBehaviorDefinitionXmlConvertor
{
    public static void Export(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        MultiInstanceLoopCharacteristics multiInstance, dynamic multiInstanceElement)
    {
        foreach (var complexBehaviorDefinition in multiInstance.complexBehaviorDefinition ??
                                                  Enumerable.Empty<ComplexBehaviorDefinition>())
        {
            var element = multiInstanceElement.complexBehaviorDefinition();
            BaseElementXmlConvertor.Export(element, complexBehaviorDefinition);
            if (complexBehaviorDefinition.condition != null)
                FormalExpressionXmlConvertor.Export(element.condition(), complexBehaviorDefinition.condition);
            if (complexBehaviorDefinition.implicitEvent != null)
                ThrowEventXmlConvertor.Export(bpmnDefinitions, bpmnElement, element, complexBehaviorDefinition.implicitEvent, "event");
        }
    }

    public static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        Activity activity, MultiInstanceLoopCharacteristics multiInstance, ElasticObject multiInstanceElement)
    {
        multiInstance.complexBehaviorDefinition = null;
        foreach (var element in multiInstanceElement.GetElements("complexBehaviorDefinition") ?? Enumerable.Empty<ElasticObject>())
        {
            FlowElement dummy = null;
            var implicitEventElement = element.GetElement("event");
            var implicitEvent = EventXmlConvertor.Import(bpmnDefinitions, bpmnElement, activity.FlowElementsContainer,
                "implicitThrowEvent", implicitEventElement, ref dummy, implicitEventElement.GetString("id")) as ImplicitThrowEvent;

            var complexBehaviorDefinition = new ComplexBehaviorDefinition(multiInstance, element.GetString("id"),
                FormalExpressionXmlConvertor.Import(element.GetElement("condition")),
                implicitEvent);

            multiInstance.complexBehaviorDefinition ??= [];
            multiInstance.complexBehaviorDefinition.Add(complexBehaviorDefinition);
        }
    }
}
