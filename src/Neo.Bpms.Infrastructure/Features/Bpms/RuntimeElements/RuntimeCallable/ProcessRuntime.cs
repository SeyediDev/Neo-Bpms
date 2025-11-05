using Neo.Bpms.Domain.Entities.Bpmn.Extensions.BusinessProcesses;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

public class ProcessRunTime
{
    public readonly ConcurrentDictionary<string, ProcessVersionRuntime> Versions =
        new();

    public BusinessProcess BusinessProcess;
    public long DbId { get; set; }
    public ProcessVersionRuntime ActiveVersion => Versions.Values.FirstOrDefault(v => v.BusinessProcessVersion?.IsActive ?? false);
    public string Id => BusinessProcess.Id;
    public string Name => BusinessProcess.Name;

    public ProcessVersionRuntime GetVersion(string versionId)
    {
        if (string.IsNullOrEmpty(versionId))
            return ActiveVersion;
        Versions.TryGetValue(versionId, out var v);
        return v;
    }
}