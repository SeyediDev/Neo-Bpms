namespace Neo.Bpms.Domain.Entities.Bpmn.ProcessClassification;

public class ProcessCategory(ProcessClassificationFramework framework, string id, string name, ProcessCategory.eCategoryType type) : BaseModelClass(framework, id, name)
{
    public enum eCategoryType
    {
        Operational = 1,
        Management = 2,
        Supportive = 3,
    }

    public eCategoryType type = type;
    public List<ProcessGroup> processGroups = [];
    public ProcessClassificationFramework framework = framework;
    public ProcessGroup addProcessGroup(ProcessGroup processGroup)
    {
        processGroups.Add(processGroup);
        return processGroup;
    }

}
