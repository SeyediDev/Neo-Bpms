using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes.Activities;

internal static class SubProcessXmlConvertor
{
    internal static dynamic Export(BpmnDefinitions bpmnDefinitions, dynamic bpmnElement, dynamic containerElement,
        SubProcess subProcess)
    {
        dynamic element = null;
        switch (subProcess.ActivityType)
        {
            case Activity.eActivityType.EventSubProcess:
            case Activity.eActivityType.EmbeddedSubProcess:
                element = ExportSubProcess(containerElement, subProcess);
                break;
            case Activity.eActivityType.TransactionSubProcess:
                element = ExportTransactionSubProcess(containerElement, subProcess as TransactionSubProcess);
                break;
            case Activity.eActivityType.AdhocSubProcess:
                element = ExportAdHocSubProcess(containerElement, subProcess as AdHocSubProcess);
                break;
        }

        if (element != null)
            BasicExport(bpmnDefinitions, bpmnElement, subProcess, element);
        return element;
    }

    internal static Activity Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        IFlowElementsContainer flowElementsContainer, string elementName, ElasticObject elementNode,
        Activity activity, ref Activity activityTypeCheckElement)
    {
        SubProcess subProcess = null;
        switch (elementName)
        {
            case "subProcess":
                activityTypeCheckElement = activity as SubProcess;
                subProcess = ImportSubProcess(flowElementsContainer, elementNode,
                    activity as SubProcess);
                break;
            case "transaction":
                activityTypeCheckElement = activity as TransactionSubProcess;
                subProcess = ImportTransactionSubProcess(flowElementsContainer, elementNode,
                    activity as TransactionSubProcess);
                break;
            case "adHocSubProcess":
                activityTypeCheckElement = activity as AdHocSubProcess;
                subProcess = ImportAdHocSubProcess(flowElementsContainer, elementNode,
                    activity as AdHocSubProcess);
                break;
        }

        if (subProcess != null)
            BasicImport(bpmnDefinitions, bpmnElement, subProcess, elementNode);
        return subProcess;
    }

    private static void BasicExport(BpmnDefinitions bpmnDefinitions, dynamic bpmnElement, SubProcess subProcess,
        dynamic element)
    {
        FlowElementsContainerXmlConvertor.Export(bpmnDefinitions, bpmnElement, element, subProcess);
        ArtifactXmlConvertor.Export(element, subProcess);
    }

    private static void BasicImport(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, SubProcess subProcess,
        ElasticObject element)
    {
        FlowElementsContainerXmlConvertor.Import(bpmnDefinitions, bpmnElement, subProcess, element);
        ArtifactXmlConvertor.Import(element, subProcess);
    }

    private static dynamic ExportSubProcess(dynamic containerElement, SubProcess subProcess)
    {
        var element = containerElement.subProcess();
        element.triggeredByEvent = subProcess.triggeredByEvent;
        return element;
    }

    private static SubProcess ImportSubProcess(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        SubProcess subProcess)
    {
        var triggeredByEvent = element.GetBool("triggeredByEvent");
        if (subProcess == null)
            subProcess = new SubProcess(flowElementsContainer, "", "", triggeredByEvent);
        else
            subProcess.triggeredByEvent = triggeredByEvent;
        return subProcess;
    }

    private static dynamic ExportTransactionSubProcess(dynamic containerElement,
        TransactionSubProcess transactionSubProcess)
    {
        var element = containerElement.transaction();
        element.method = transactionSubProcess.method;
        return element;
    }

    private static SubProcess ImportTransactionSubProcess(IFlowElementsContainer flowElementsContainer,
        ElasticObject element,
        TransactionSubProcess transactionSubProcess)
    {
        transactionSubProcess ??= new TransactionSubProcess(flowElementsContainer, "", "");
        transactionSubProcess.method = element.GetString("method");
        return transactionSubProcess;
    }

    private static dynamic ExportAdHocSubProcess(dynamic containerElement, AdHocSubProcess adHocSubProcess)
    {
        var element = containerElement.adHocSubProcess();
        element.ordering = adHocSubProcess.ordering;
        if (adHocSubProcess.completionCondition != null)
            FormalExpressionXmlConvertor.Export(element.completionCondition(), adHocSubProcess.completionCondition);
        element.cancelRemainingInstances = adHocSubProcess.cancelRemainingInstances;
        return element;
    }

    private static AdHocSubProcess ImportAdHocSubProcess(IFlowElementsContainer flowElementsContainer,
        ElasticObject element,
        AdHocSubProcess adHocSubProcess)
    {
        adHocSubProcess ??= new AdHocSubProcess(flowElementsContainer, "", "");
        adHocSubProcess.completionCondition =
            FormalExpressionXmlConvertor.Import(element.GetElement("completionCondition"));
        adHocSubProcess.ordering = element.GetEnumText("ordering", AdHocSubProcess.AdHocOrdering.Parallel);
        adHocSubProcess.cancelRemainingInstances = element.GetBool("cancelRemainingInstances", true);
        return adHocSubProcess;
    }
}
