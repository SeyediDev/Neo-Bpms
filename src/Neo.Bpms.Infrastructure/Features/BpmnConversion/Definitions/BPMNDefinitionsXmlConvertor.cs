global using Neo.Bpms.Domain.Features.Dynamic;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Collaborations;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.DataStores;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Events;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Interfaces;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.Resources;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.RootElements.CorrelationProperties;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions;

/// <summary>
/// 14.4
/// </summary>
internal static class BpmnDefinitionsXmlConvertor
{
    internal static ElasticObject Export(BpmnDefinitions bpmnDefinitions, ElasticObject diagramBpmnDefinition)
    {
        dynamic bpmnElement = new ElasticObject("definitions");
        try
        {
            var process = bpmnDefinitions?.Process;
            CheckDiagrams(bpmnDefinitions, process);
            bpmnElement.xsi = GetXmlNamespace("http://www.w3.org/2001/XMLSchema-instance");
            bpmnElement.bpmn2 = GetXmlNamespace("http://www.omg.org/spec/BPMN/20100524/MODEL");
            bpmnElement.bpmndi = GetXmlNamespace("http://www.omg.org/spec/BPMN/20100524/DI");
            bpmnElement.dc = GetXmlNamespace("http://www.omg.org/spec/DD/20100524/DC");
            bpmnElement.di = GetXmlNamespace("http://www.omg.org/spec/DD/20100524/DI");
            bpmnElement.schemaLocation =
                GetSchemaLocation(bpmnElement.xsi, "http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd");
            if (bpmnDefinitions == null) return bpmnElement;
            bpmnDefinitions.ErrorInfos = [];
            bpmnElement.id = bpmnDefinitions.Id + ".Definitions";
            bpmnElement.name = bpmnDefinitions.Name;
            bpmnElement.targetNamespace = GetXmlNamespace(bpmnDefinitions.targetNamespace ?? "http://bpmn.io/schema/bpmn");
            bpmnElement.typeLanguage = GetXmlNamespace(bpmnDefinitions.typeLanguage ?? "http://www.w3.org/2001/XMLSchema");
            bpmnElement.expressionLanguage =
                GetXmlNamespace(bpmnDefinitions.expressionLanguage ?? "http://www.w3.org/1999/XPath");
            bpmnElement.teta = GetXmlNamespace("http://www.bmi.ir/schemas/BPMN20Extensions/MODEL");
            bpmnElement.exporter = bpmnDefinitions.exporter;
            bpmnElement.exporterVersion = bpmnDefinitions.exporterVersion;
            BaseElementXmlConvertor.NamespaceExport(bpmnElement);

            ResourceXmlConvertor.Export(bpmnElement);
            DataStoreXmlConvertor.Export(bpmnElement);
            InterfaceXmlConvertor.Export(bpmnElement);
            MessageXmlConvertor.Export(bpmnElement);
            SignalXmlConvertor.Export(bpmnElement);
            ErrorXmlConvertor.Export(bpmnElement);
            EscalationXmlConvertor.Export(bpmnElement);
            //CorrelationPropertyXmlConvertor.Export(bpmnElement);

            ProcessXmlConvertor.Export(bpmnElement, bpmnDefinitions);
            CollaborationXmlConvertor.Export(bpmnElement, bpmnDefinitions);
            BpmnDiagramXmlConvertor.Export(bpmnElement, bpmnDefinitions, diagramBpmnDefinition);

            Validate(bpmnDefinitions);
        }
        catch (Exception e)
        {
            bpmnDefinitions?.ErrorInfos.AddFatal(e.ToString(), "BpmnDefinitions" + bpmnDefinitions.Id, "14.4.0", "Exception");
        }
        return bpmnElement;
    }

    internal static BpmnDefinitions Import(
        BusinessProcess businessProcess, BusinessProcessVersion businessProcessVersion,
        ElasticObject bpmnElement, bool IsMainBpmn)
    {
        var targetNamespace = "www.bmi.ir/xmlns/e-bpmn2.xsd"; //todo xml don't have namespace
        var exporter = bpmnElement.GetString("exporter") ?? "NeoBpms";
        var exporterVersion = bpmnElement.GetString("exporterVersion") ?? "1.0";
        // todo add exrea BpmnDefinitions fields

        var bpmnDefinitions = ProjectDefinition.Project.GetBpmnDefinition(businessProcess.Id, businessProcessVersion.Id);
        if (bpmnDefinitions == null)
        {
            bpmnDefinitions = new BpmnDefinitions(businessProcess.Id, businessProcess.Name,
                targetNamespace, exporter, exporterVersion);
        }
        else
        {
            bpmnDefinitions.Name = businessProcess.Name;
            bpmnDefinitions.targetNamespace = targetNamespace;
            bpmnDefinitions.exporter = exporter;
            bpmnDefinitions.exporterVersion = exporterVersion;
            bpmnDefinitions.ErrorInfos = [];
        }
        if (IsMainBpmn)
        {
            ResourceXmlConvertor.Import(bpmnElement);
            DataStoreXmlConvertor.Import(bpmnElement);
            InterfaceXmlConvertor.Import(bpmnElement);
            MessageXmlConvertor.Import(bpmnElement);
            SignalXmlConvertor.Import(bpmnElement);
            ErrorXmlConvertor.Import(bpmnElement);
            EscalationXmlConvertor.Import(bpmnElement);
            CorrelationPropertyXmlConvertor.Import(bpmnElement);//todo
        }

        ProcessXmlConvertor.Import(bpmnElement, bpmnDefinitions);
        CollaborationXmlConvertor.Import(bpmnElement, bpmnDefinitions);
        Validate(bpmnDefinitions);
        businessProcessVersion.BpmnDefinitions = bpmnDefinitions;
        return bpmnDefinitions;
    }

    internal static void Validate(BpmnDefinitions bpmnDefinitions)
    {
    }

    private static XNamespace GetXmlNamespace(string value)
    {
        XNamespace xNamespace = value;
        return xNamespace;
    }

    private static XAttribute GetSchemaLocation(XNamespace xNamespace, string value)
    {
        return new XAttribute(xNamespace + "schemaLocation", XNamespace.Get(value));
    }

    private static void CheckDiagrams(BpmnDefinitions bpmndef, Process processDef)
    {
        //todo ???چرااااا
        if (bpmndef == null || processDef == null) return;
        if (bpmndef.diagrams != null)
            return;
        bpmndef.diagrams = [];
        var diagram = new BPMNDiagram(new Bounds { width = 300, height = 200 }, processDef, processDef.Name);
        bpmndef.diagrams.Add(diagram);

        //diagram.addPlane( new BPMNPlane( new ));
    }
}
