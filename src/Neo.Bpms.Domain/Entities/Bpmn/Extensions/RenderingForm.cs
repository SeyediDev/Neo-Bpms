using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.GlobalTasks;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;

namespace Neo.Bpms.Domain.Entities.Bpmn.Extensions;
public class RenderingForm : Rendering
{
    public static string WorkDescription { get; } = "__WorkDescription";

    public RenderingForm(UserTask parent, string id,
        string formId, string associationFieldId, string indexFormId)
        : base(parent, id)
    {
        this.formId = formId;
        this.associationFieldId = associationFieldId;
        this.indexFormId = indexFormId;
    }

    public RenderingForm(GlobalUserTask parent, string id, string formId)
        : base(parent, id)
    {
        this.formId = formId;
    }
    public string formId { get; set; }
    public string associationFieldId { get; set; }
    public string indexFormId { get; set; }
}