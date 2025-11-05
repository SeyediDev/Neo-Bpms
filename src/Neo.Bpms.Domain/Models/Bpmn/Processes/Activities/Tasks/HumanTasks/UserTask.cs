using Neo.Bpms.Domain.Models.WorkManagement;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.HumanTasks;

/// <summary>
/// A User Task is a typical "workflow" Task where a human performer performs the Task with the assistance of a
/// software application and is scheduled through a task list manager of some sort.
/// A User Task is executed by and managed by a business process runtime. 
/// Attributes concerning the human involvement, like people assignments and UI rendering can be specified in great detail
/// 
/// A User Task is a typical "workflow" Task where a human performer performs the Task with the assistance of a
/// software application. The lifecycle of the Task is managed by a software component (called task manager) and is
/// typically executed in the context of a Process.	
/// </summary>

public class UserTask : Task
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

    public UserTask(IFlowElementsContainer flowElementsContainer,
        string id, string name,
        string formId, string indexFormId, string associationFieldId)
        : base(flowElementsContainer, id, name, eActivityType.UserTask)
    {
        rendering =
        [
            new RenderingForm(this, DefaultRenderingId, formId, associationFieldId, indexFormId)
        ];
    }

    private string DefaultRenderingId => $"{Id}.Rendering";

    public string FormId
    {
        get => rendering.OfType<RenderingForm>().FirstOrDefault()?.formId;
        set
        {
            RenderingForm r = rendering.OfType<RenderingForm>().FirstOrDefault();
            if (r == null)
            {
                rendering = [new RenderingForm(this, DefaultRenderingId, value, null, null)];
            }
            else
            {
                r.formId = value;
            }
        }
    }

    public string AssociationFieldId
    {
        get => rendering.OfType<RenderingForm>().FirstOrDefault()?.associationFieldId;
        set
        {
            RenderingForm r = rendering.OfType<RenderingForm>().FirstOrDefault();
            if (r == null)
            {
                rendering = [new RenderingForm(this, DefaultRenderingId, null, value, null)];
            }
            else
            {
                r.associationFieldId = value;
            }
        }
    }

    public string IndexFormId
    {
        get => rendering.OfType<RenderingForm>().FirstOrDefault()?.indexFormId;
        set
        {
            RenderingForm r = rendering.OfType<RenderingForm>().FirstOrDefault();
            if (r == null)
            {
                rendering = [new RenderingForm(this, DefaultRenderingId, null, null, value)];
            }
            else
            {
                r.indexFormId = value;
            }
        }
    }

    public WorkDistributionPolicy WorkDistributionPolicy { get; set; }
}
