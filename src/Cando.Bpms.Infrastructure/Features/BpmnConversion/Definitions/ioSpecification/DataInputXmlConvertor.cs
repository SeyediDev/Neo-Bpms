using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class DataInputXmlConvertor
{
    internal static void Export(ElasticObject bpmnElement, dynamic containerElement, IDataInputContainer container)
    {
        foreach (var input in container.dataInputs ?? Enumerable.Empty<DataInput>())
        {
            var element = containerElement.dataInput();
            ExportDataInput(bpmnElement, input, element);
        }
    }

    internal static void ExportDataInput(ElasticObject bpmnElement, DataInput dataInput, dynamic dataInputElement)
    {
        DataElementXmlConvertor.Export(bpmnElement, dataInputElement, dataInput);
        dataInputElement.isCollection = dataInput.isCollection;
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        IDataInputContainer container, ElasticObject containerElement)
    {
        container.dataInputs = null;
        foreach (var element in containerElement.GetElements("dataInput") ?? Enumerable.Empty<ElasticObject>())
        {
            var dataInput = ImportDataInput(bpmnDefinitions, bpmnElement, container, element);
            container.dataInputs ??= [];
            container.dataInputs.Add(dataInput);
        }
    }

    internal static DataInput ImportDataInput(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        IBaseDataInputContainer container, ElasticObject element)
    {
        if (element == null) return null;

        var dataInput = new DataInput(container, element.GetString("id"), element.GetString("name"), null, element.GetBool("isCollection"));
        DataElementXmlConvertor.Import(bpmnDefinitions, bpmnElement, element, dataInput);
        return dataInput;
    }
}
