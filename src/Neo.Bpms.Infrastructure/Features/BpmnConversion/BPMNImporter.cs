using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.XPDL;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

public static class BpmnImporter
{
    public static BpmnDefinitions ImportFromBpmnFile(
        BusinessProcess businessProcess, BusinessProcessVersion businessProcessVersion,
        string path, ImportType type, bool IsMainBpmn)
    {
        return Import(businessProcess, businessProcessVersion, ReadProcessFileText(path), type, IsMainBpmn);
    }

    public static BpmnDefinitions Import(
        BusinessProcess businessProcess, BusinessProcessVersion businessProcessVersion,
        string strXml, ImportType importType, bool IsMainBpmn)
    {
        dynamic bpmn = XElement.Parse(strXml).ToElastic();
        if (businessProcessVersion == null)
        {
            return null;
        }

        return importType switch
        {
            ImportType.Bpmn => BpmnDefinitionsXmlConvertor.Import(businessProcess, businessProcessVersion, bpmn, IsMainBpmn),
            ImportType.Xpdl => (BpmnDefinitions)XPDLImporter.LoadFrom(bpmn),
            _ => null,
        };
    }

    public static ElasticObject ReadProcessFile(string path)
    {
        return XElement.Parse(ReadProcessFileText(path)).ToElastic();
    }

    private static string ReadProcessFileText(string path)
    {
        using StreamReader sr = new(path);
        string data = sr.ReadToEnd();
        return data;
    }

    public enum ImportType
    {
        Bpmn = 1,
        Xpdl = 2,
    }
}
