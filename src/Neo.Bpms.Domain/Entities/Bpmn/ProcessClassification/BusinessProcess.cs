namespace Neo.Bpms.Domain.Entities.Bpmn.ProcessClassification;

public class BusinessProcess(int id, string name) : BaseModelClass(null, id, name)
{
    public ProcessGroup group;
    public List<BusinessTask> businessTasks = null;
    public BusinessTask AddBusinessTask(BusinessTask task)
    {
        businessTasks ??= [];
        businessTasks.Add(task);
        return task;
    }
    public List<KPI> KPIs = [];
    public KPI AddKPI(KPI kpi)
    {
        KPIs.Add(kpi);
        return kpi;
    }
    public List<ProcessRole> ProcessRoles = [];
    public ProcessRole AddProcessRole(ProcessRole processRole)
    {
        ProcessRoles.Add(processRole);
        return processRole;
    }
    public List<BaseModelClass> ProcessDefinitions = [];
    public BaseModelClass AddProcessDefinition(BaseModelClass process)
    {
        ProcessDefinitions.Add(process);
        return process;
    }
}
