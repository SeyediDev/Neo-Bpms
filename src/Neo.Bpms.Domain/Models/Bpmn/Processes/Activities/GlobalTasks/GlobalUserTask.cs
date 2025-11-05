using Neo.Bpms.Domain.Models.WorkManagement;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.GlobalTasks;

public class GlobalUserTask : GlobalTask
{
    /// <summary>
    /// This attribute specifies the technology that will be used to implement the User Task. 
    /// Valid values are "##unspecified" for leaving the implementation technology open, 
    /// "##WebService" for the Web service technology or a URI identifying any other technology or coordination protocol. 
    /// The default technology for this task is unspecified.
    /// </summary>
    public string implementation = "##unspecified";

    /// <summary>
    /// This attributes acts as a hook which allows BPMN adopters to specify task
    /// rendering attributes by using the BPMN Extension mechanism
    /// </summary>
    public List<Rendering> rendering;

    public GlobalUserTask(BpmnDefinitions parent, string id, string name, string formId = null) : base(parent, id, name,
        eGlobalTaskType.User)
    {
        rendering =
        [
            new RenderingForm(this, $"{id}.Rendering", formId)
        ];
    }
    public WorkDistributionPolicy WorkDistributionPolicy { get; set; }
}
