using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.UI.MVC.Controllers.BpmnElements;

public class BpmnErrorController(IProjectBpmn projectBpmn) : BpmsController
{
    [HttpGet]
    public JsonResult List()
    {
        IEnumerable<ErrorViewModel> errors = ProjectDefinition.Project.Errors?.Select(e => new ErrorViewModel(e));
        return Json(errors);
    }

    [HttpPost]
    public JsonResult Save(string id, ErrorViewModel viewModel)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        bool isNew = string.IsNullOrEmpty(id);
        if (isNew)
            viewModel.id = $"Error.{viewModel.errorCode}.{viewModel.name}.{Guid.NewGuid():N}";

        if (string.IsNullOrEmpty(viewModel.namespaceId))
            viewModel.namespaceId = "ProcessEntities";
        Error newItem = viewModel.ToError();

        Error item = isNew ? newItem : ProjectDefinition.Project.GetError(id);
        if (item == null)
        {
            isNew = true;
            item = newItem;
        }
        if (!isNew)
            item.Copy(newItem);
        if (isNew)
            ProjectDefinition.Project.AddError(item);
        projectBpmn.Save();
        return Json(new ErrorViewModel(item));
    }

    [HttpPost]
    public JsonResult Delete(string id)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        ProjectDefinition.Project.DeleteError(id);
        projectBpmn.Save();
        return Json(id);
    }

}
