using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.CallActivity;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class CallableElementXmlConvertor
{
    internal static void Export(ElasticObject bpmnElement, dynamic node, CallableElement callableElement)
    {
        RootElementXmlConvertor.Export(node, callableElement);
        node.name = callableElement.Name;
        IoSpecificationXmlConvertor.Export(bpmnElement, node, callableElement);
        // todo 
        // supportedInterfaceRefs
        // ioBinding
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        ElasticObject element, CallableElement callableElement)
    {
        RootElementXmlConvertor.Import(element, callableElement);
        callableElement.Name = element.GetString("name");
        IoSpecificationXmlConvertor.Import(bpmnDefinitions, bpmnElement, callableElement, element);
        // todo 
        // supportedInterfaceRefs
        // ioBinding
    }
}
