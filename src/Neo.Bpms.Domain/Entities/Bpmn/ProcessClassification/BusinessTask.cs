namespace Neo.Bpms.Domain.Entities.Bpmn.ProcessClassification;

public class BusinessTask(BusinessProcess businessProcess, int id, string name) : BaseModelClass(businessProcess, id, name)
{
    public readonly BusinessProcess businessProcess = businessProcess;
}
