using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class IoSpecificationXmlConvertor
{
    internal static void Export(ElasticObject bpmnElement, dynamic containerElement,
        IIoSpecificationContainer ioSpecificationContainer)
    {
        if (ioSpecificationContainer.ioSpecification == null) return;
        if (string.IsNullOrEmpty(ioSpecificationContainer.ioSpecification.Id)) //todo kar bad 
            ioSpecificationContainer.ioSpecification.Id =
                (ioSpecificationContainer as BaseElement)?.Id + ".ioSpecification";
        var element = containerElement.ioSpecification();
        BaseElementXmlConvertor.Export(element, ioSpecificationContainer.ioSpecification);
        DataInputXmlConvertor.Export(bpmnElement, element, ioSpecificationContainer.ioSpecification);
        DataOutputXmlConvertor.Export(bpmnElement, element, ioSpecificationContainer.ioSpecification);
        InputSetXmlConvertor.Export(element, ioSpecificationContainer.ioSpecification);
        OutputSetXmlConvertor.Export(element, ioSpecificationContainer.ioSpecification);
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions,
        ElasticObject bpmnElement, IIoSpecificationContainer ioSpecificationContainer,
        ElasticObject containerElement)
    {
        ioSpecificationContainer.ioSpecification = null;
        var element = containerElement.GetElement("ioSpecification");
        if (element == null) return;
        var elementId = element.GetString("id");
        var baseElement = ioSpecificationContainer as BaseElement;
        if (string.IsNullOrEmpty(elementId))
            elementId = baseElement?.Id + ".ioSpecification";
        var ioSpecification = new InputOutputSpecification(ioSpecificationContainer, elementId);
        BaseElementXmlConvertor.Import(element, ioSpecification);
        DataInputXmlConvertor.Import(bpmnDefinitions, bpmnElement, ioSpecification, element);
        DataOutputXmlConvertor.Import(bpmnDefinitions, bpmnElement, ioSpecification, element);
        InputSetXmlConvertor.Import(ioSpecification, element);
        OutputSetXmlConvertor.Import(ioSpecification, element);
        ioSpecificationContainer.ioSpecification = ioSpecification;
    }
}