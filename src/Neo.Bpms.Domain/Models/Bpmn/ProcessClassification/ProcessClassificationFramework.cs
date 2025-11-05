namespace Neo.Bpms.Domain.Models.Bpmn.ProcessClassification;

public class ProcessClassificationFramework(string id, string name, string source, string destination) : BaseModelClass(null, id, name)
{
    public string source = source;
    public string destination = destination;
    public List<ProcessCategory> processCategories = [];
    public ProcessCategory addProcessCategory(ProcessCategory processCategory)
    {
        processCategories.Add(processCategory);
        return processCategory;
    }

}
