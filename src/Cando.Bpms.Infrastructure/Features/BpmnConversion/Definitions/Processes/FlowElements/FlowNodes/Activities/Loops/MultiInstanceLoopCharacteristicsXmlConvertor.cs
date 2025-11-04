using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.LoopCharacteristic;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal class MultiInstanceLoopCharacteristicsXmlConvertor
{
    internal static void Export(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, dynamic element,
        MultiInstanceLoopCharacteristics multiInstance)
    {
        if (multiInstance == null) return;
        BaseElementXmlConvertor.Export(element, multiInstance);
        element.isSequential = multiInstance.isSequential;
        element.behavior = multiInstance.behavior;
        if (multiInstance.oneBehaviorEventRef != null)
        {
            element.oneBehaviorEventRef = multiInstance.oneBehaviorEventRef.Id;
            EventDefinitionXmlConvertor.Export(null, bpmnElement, multiInstance.oneBehaviorEventRef);
        }
        if (multiInstance.noneBehaviorEventRef != null)
        {
            element.noneBehaviorEventRef = multiInstance.noneBehaviorEventRef.Id;
            EventDefinitionXmlConvertor.Export(null, bpmnElement, multiInstance.noneBehaviorEventRef);
        }

        if (multiInstance.loopCardinality != null)
            FormalExpressionXmlConvertor.Export(element.loopCardinality(), multiInstance.loopCardinality);
        if (multiInstance.completionCondition != null)
            FormalExpressionXmlConvertor.Export(element.completionCondition(), multiInstance.completionCondition);

        if (multiInstance.loopDataInputRef != null)
            ItemAwareElementXmlConvertor.ExportElementRef(element.loopDataInputRef(), multiInstance.loopDataInputRef);
        if (multiInstance.loopDataOutputRef != null)
            ItemAwareElementXmlConvertor.ExportElementRef(element.loopDataOutputRef(), multiInstance.loopDataOutputRef);
        if (multiInstance.inputDataItem != null)
            DataInputXmlConvertor.ExportDataInput(bpmnElement, multiInstance.inputDataItem, element.inputDataItem());
        if (multiInstance.outputDataItem != null)
            DataOutputXmlConvertor.ExportDataOutput(bpmnElement, multiInstance.outputDataItem, element.outputDataItem());
        ComplexBehaviorDefinitionXmlConvertor.Export(bpmnDefinitions, bpmnElement, multiInstance, element);
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        Activity activity, ElasticObject element)
    {
        var multiInstance = new MultiInstanceLoopCharacteristics(activity, element.GetString("id"));
        BaseElementXmlConvertor.Import(element, multiInstance);
        multiInstance.isSequential = element.GetBool("isSequential");
        multiInstance.behavior = element.GetEnumText("behavior", MultiInstanceLoopCharacteristics.MultiInstanceBehavior.All);
        multiInstance.oneBehaviorEventRef =
            bpmnDefinitions.GetRootElement(element.GetString("oneBehaviorEventRef")) as EventDefinition;
        multiInstance.noneBehaviorEventRef =
            bpmnDefinitions.GetRootElement(element.GetString("noneBehaviorEventRef")) as EventDefinition;

        multiInstance.loopCardinality = FormalExpressionXmlConvertor.Import(element.GetElement("loopCardinality"));
        multiInstance.completionCondition = FormalExpressionXmlConvertor.Import(element.GetElement("completionCondition"));

        multiInstance.loopDataInputRef =
            ItemAwareElementXmlConvertor.ImportElementRef(bpmnDefinitions, multiInstance, element.GetElement("loopDataInputRef"), true);
        multiInstance.loopDataOutputRef =
            ItemAwareElementXmlConvertor.ImportElementRef(bpmnDefinitions, multiInstance, element.GetElement("loopDataOutputRef"), false);
        multiInstance.inputDataItem = DataInputXmlConvertor.ImportDataInput(bpmnDefinitions, bpmnElement, multiInstance,
            element.GetElement("inputDataItem"));
        multiInstance.outputDataItem = DataOutputXmlConvertor.ImportDataOutput(bpmnDefinitions, bpmnElement, multiInstance,
            element.GetElement("outputDataItem"));

        ComplexBehaviorDefinitionXmlConvertor.Import(bpmnDefinitions, bpmnElement, activity, multiInstance, element);
        activity.loopCharacteristics = multiInstance;
    }
}
