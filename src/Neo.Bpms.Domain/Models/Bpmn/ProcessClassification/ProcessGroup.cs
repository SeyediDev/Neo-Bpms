namespace Neo.Bpms.Domain.Models.Bpmn.ProcessClassification;

public class ProcessGroup(ProcessCategory category, string id, string name) : BaseModelClass(category, id, name)
{
    public List<BusinessProcess> businessProcesses = [];
    public ProcessCategory category = category;
    public BusinessProcess addBusinessProcess(BusinessProcess businessProcess)
    {
        businessProcesses.Add(businessProcess);
        return businessProcess;
    }
}
