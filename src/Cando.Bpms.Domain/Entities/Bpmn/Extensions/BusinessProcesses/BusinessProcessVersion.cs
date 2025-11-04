using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Model.BPMN.Processes;

namespace Neo.Bpms.Domain.Entities.Bpmn.Extensions.BusinessProcesses;
public class BusinessProcessVersion : BaseModelClass
{
    /// <summary>
    /// this constructor is for XmlSerializer
    /// </summary>
    public BusinessProcessVersion() { }
    public BusinessProcessVersion(BusinessProcess process, string id, BpmnDefinitions definitions) :
        base(process, id, definitions?.Name + "-" + id)
    {
        BpmnDefinitions = definitions;
    }

    [XmlIgnore]
    public BusinessProcess BusinessProcess => (BusinessProcess)Parent;
    [XmlIgnore]
    public BpmnDefinitions BpmnDefinitions { get; set; }

    public bool IsActive => BpmnDefinitions?.Process?.Status == Process.ProcessStatus.Active && (BpmnDefinitions?.Process?.isExecutable ?? false);
    public bool AdministratorLock { get; set; }
    public string CheckInUserId { get; set; }
    public string Version => Id;
    public string ProcessIdVersionId => Version == "1.0" ? BusinessProcess.Id : $"{BusinessProcess.Id}_V{Version}";
    public long DbId { get; set; }

    public static void FetchVersionId(string processIdVersionId, out string id, out string versionId)
    {
        id = processIdVersionId;
        versionId = "1.0";
        // ReSharper disable once InconsistentNaming
        int indexOf_V = processIdVersionId.IndexOf("_V", StringComparison.Ordinal);
        if (indexOf_V > 0)
        {
            id = processIdVersionId[..indexOf_V];
            if (indexOf_V + 2 < processIdVersionId.Length)
            {
                versionId = processIdVersionId[(indexOf_V + 2)..].Replace("_", ".");
            }
        }
    }
}
