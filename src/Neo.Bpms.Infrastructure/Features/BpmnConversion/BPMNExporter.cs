global using System.Xml.Linq;
global using Neo.Bpms.Infrastructure.Features.BpmnConversion;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.XPDL;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

public class BpmnExporter
{
    public static string Export(BpmnDefinitions bpmndef, ElasticObject diagramBpmnDefinition, ExportType optype)
    {
        StringWriter sw = new();
        XElement xml;
        switch (optype)
        {
            case ExportType.Bpmn:
                ElasticObject bpmn = BpmnDefinitionsXmlConvertor.Export(bpmndef, diagramBpmnDefinition);
                xml = bpmn.ToXElement();
                //sw.Write( "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>\r\n");
                sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n");
                sw.Write(xml.ToString(SaveOptions.None));
                break;
            case ExportType.Xpdl:
                string sHeader, footer;
                dynamic xpdl = XPDLExporter.ExportTo(bpmndef, out sHeader, out footer);
                xml = xpdl > FormatType.Xml;
                sw.Write(sHeader);
                sw.Write(xml);
                sw.Write(footer);
                break;
            //case ExportType.JPEG:
            //	break;
            //case ExportType.HTML:
            //	break;
            //case ExportType.WORD:
            //	break;
            //case ExportType.BMP:
            //	break;
            //case ExportType.PDF:
            //	break;
            default:
                return sw.ToString();
        }
        return sw.ToString();
    }

    public enum ExportType
    {
        Bpmn = 1,
        Xpdl = 2,
        //JPEG = 3,
        //HTML = 4,
        //WORD = 5,
        //BMP = 6,
        //PDF = 7
    }
}
