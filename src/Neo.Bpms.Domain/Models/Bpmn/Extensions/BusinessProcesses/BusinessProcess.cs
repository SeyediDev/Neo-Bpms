namespace Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;

public class BusinessProcess : BaseModelClass
{
    //public string Id { get; set; }
    //public string Name { get; set; }
    public Dictionary<string, BusinessProcessVersion> Versions { get; }
        = [];

    public BusinessProcessVersion ActiveVersion => Versions.Values.FirstOrDefault(v => v.IsActive);
    public static bool DoSetTreeParams { get; set; }

    /// <summary>
    /// this constructor is for XmlSerializer
    /// </summary>
    public BusinessProcess()
    {
    }

    public BusinessProcess(string id, string name) : base(null, id, name)
    {
    }

    public BusinessProcessVersion AddVersion(BusinessProcess process, string versionId, BpmnDefinitions definitions)
    {
        BusinessProcessVersion version = new(process, versionId, definitions);
        return Versions.TryAdd(versionId, version) ? version : null;
    }

    public bool DeleteVersion(string versionId)
    {
        return Versions.Remove(versionId, out _);
    }

    public BusinessProcessVersion GetVersion(string versionId)
    {
        _ = Versions.TryGetValue(versionId ?? "1.0", out BusinessProcessVersion businessProcessVersion);
        if (businessProcessVersion == null && (versionId ?? "1.0") == "1.0")
        {
            businessProcessVersion = Versions.Values.FirstOrDefault();
        }

        return businessProcessVersion;
    }

    public BusinessProcessVersion TryAddVersion(BusinessProcess process, string versionId,
        BpmnDefinitions definitions)
    {
        BusinessProcessVersion businessProcessVersion = GetVersion(versionId);
        if (businessProcessVersion != null)
        {
            return businessProcessVersion;
        }

        businessProcessVersion = AddVersion(process, versionId, definitions);
        return businessProcessVersion;
    }
}
