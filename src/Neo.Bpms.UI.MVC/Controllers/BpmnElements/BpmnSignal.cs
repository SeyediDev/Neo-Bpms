using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.UI.MVC.Controllers.BpmnElements;

public class BpmnSignalController(IProjectBpmn projectBpmn) : BpmsController
{
    [HttpGet]
    public JsonResult List()
    {
        IEnumerable<SignalViewModel> signals = ProjectDefinition.Project.Signals?.Select(e => new SignalViewModel(e));
        return Json(signals);
    }

    [HttpPost]
    public JsonResult Save(string id, SignalViewModel viewModel)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        bool isNew = string.IsNullOrEmpty(id);
        if (isNew)
            viewModel.id = "Signal" + Guid.NewGuid().ToString("N");
        if (string.IsNullOrEmpty(viewModel.namespaceId))
            viewModel.namespaceId = "ProcessEntities";
        Signal newItem = viewModel.ToSignal();

        Signal item = isNew ? newItem : ProjectDefinition.Project.GetSignal(id);
        if (item == null)
        {
            isNew = true;
            item = newItem;
        }
        if (!isNew)
            item.Copy(newItem);
        if (isNew)
            ProjectDefinition.Project.AddSignal(item);
        projectBpmn.Save();
        return Json(new SignalViewModel(item));
    }

    [HttpPost]
    public JsonResult Delete(string id)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        ProjectDefinition.Project.DeleteSignal(id);
        projectBpmn.Save();
        return Json(id);
    }
}
