using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.UI.MVC.Controllers.BpmnElements;

public class BpmnEscalateController(IProjectBpmn projectBpmn) : BpmsController
{
    [HttpGet]
    public JsonResult List()
    {
        IEnumerable<EscalationViewModel> escalations = ProjectDefinition.Project.Escalations?.Select(e => new EscalationViewModel(e));
        return Json(escalations);
    }

    [HttpPost]
    public JsonResult Save(string id, EscalationViewModel viewModel)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        bool isNew = string.IsNullOrEmpty(id);
        if (isNew)
            viewModel.id = "Escalate" + Guid.NewGuid().ToString("N");
        if (string.IsNullOrEmpty(viewModel.namespaceId))
            viewModel.namespaceId = "ProcessEntities";
        Escalation newItem = viewModel.ToEscalation();

        Escalation item = isNew ? newItem : ProjectDefinition.Project.GetEscalation(id);
        if (item == null)
        {
            isNew = true;
            item = newItem;
        }
        if (!isNew)
            item.Copy(newItem);
        if (isNew)
            ProjectDefinition.Project.AddEscalation(item);
        projectBpmn.Save();
        return Json(new EscalationViewModel(item));
    }


    [HttpPost]
    public JsonResult Delete(string id)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        ProjectDefinition.Project.DeleteEscalation(id);
        projectBpmn.Save();
        return Json(id);
    }

}
