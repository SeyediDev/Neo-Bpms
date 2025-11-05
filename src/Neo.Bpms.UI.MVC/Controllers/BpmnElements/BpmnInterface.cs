using Neo.Bpms.Domain.Models.Bpmn.Core.Services;

namespace Neo.Bpms.UI.MVC.Controllers.BpmnElements;

public class BpmnInterfaceController(IProjectBpmn projectBpmn) : BpmsController
{
    [HttpGet]
    public JsonResult List()
    {
        List<InterfaceViewModel> interfaces = ProjectDefinition.Project.Interfaces?.Select(e => new InterfaceViewModel(e)).ToList();
        return Json(interfaces);
    }

    [HttpPost]
    public JsonResult Save(string id, InterfaceViewModel viewModel)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        bool isNew = string.IsNullOrEmpty(id);
        if (isNew)
            viewModel.id = "Interface" + Guid.NewGuid().ToString("N");
        Interface newItem = viewModel.ToInterface();

        Interface item = isNew ? newItem : ProjectDefinition.Project.GetInterface(id);
        if (item == null)
        {
            isNew = true;
            item = newItem;
        }
        if (!isNew)
            item.Copy(newItem);
        if (isNew)
            ProjectDefinition.Project.AddInterface(item);
        projectBpmn.Save();
        return Json(new InterfaceViewModel(item));
    }

    [HttpPost]
    public JsonResult Delete(string id)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        ProjectDefinition.Project.DeleteInterface(id);
        projectBpmn.Save();
        return Json(id);

    }
}
