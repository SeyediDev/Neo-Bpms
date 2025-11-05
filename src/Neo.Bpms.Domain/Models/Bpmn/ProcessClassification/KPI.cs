namespace Neo.Bpms.Domain.Models.Bpmn.ProcessClassification;

public class KPI(ProcessGroup processGroup, int id, string name) : BaseModelClass(processGroup, id, name)
{
    public ProcessGroup processGroup = processGroup;
}
