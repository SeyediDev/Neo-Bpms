using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.ioSpecification;

internal static class DataOutputXmlConvertor
{
    internal static void Export(ElasticObject bpmnElement, dynamic containerElement, IDataOutputContainer container)
    {
        foreach (var output in container.dataOutputs ?? Enumerable.Empty<DataOutput>())
        {
            var element = containerElement.dataOutput();
            ExportDataOutput(bpmnElement, output, element);
        }
    }

    internal static void ExportDataOutput(ElasticObject bpmnElement, DataOutput dataOutput, dynamic dataOutputElement)
    {
        DataElementXmlConvertor.Export(bpmnElement, dataOutputElement, dataOutput);
        dataOutputElement.isCollection = dataOutput.isCollection;
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        IDataOutputContainer container, ElasticObject containerElement)
    {
        container.dataOutputs = null;
        foreach (var element in containerElement.GetElements("dataOutput") ?? Enumerable.Empty<ElasticObject>())
        {
            var dataOutput = ImportDataOutput(bpmnDefinitions, bpmnElement, container, element);
            container.dataOutputs ??= [];
            container.dataOutputs.Add(dataOutput);
        }
    }

    internal static DataOutput ImportDataOutput(BpmnDefinitions bpmnDefinitions,
        ElasticObject bpmnElement, IBaseDataOutputContainer container, ElasticObject element)
    {
        if (element == null) return null;

        var dataOutput = new DataOutput(container, element.GetString("id"), element.GetString("name"), null, element.GetBool("isCollection"));
        DataElementXmlConvertor.Import(bpmnDefinitions, bpmnElement, element, dataOutput);
        return dataOutput;
    }
}
