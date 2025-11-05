using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.UI.MVC.Controllers.BpmnElements;

public class BpmnMessageController(IProjectBpmn projectBpmn) : BpmsController
{
    [HttpGet]
    public JsonResult List()
    {
        IEnumerable<MessageViewModel> messages = ProjectDefinition.Project.Messages?.Select(e => new MessageViewModel(e));
        return Json(messages);
    }

    [HttpPost]
    public JsonResult Save(string id, MessageViewModel viewModel)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        bool isNew = string.IsNullOrEmpty(id);
        if (isNew)
        {
            viewModel.id = "Message" + Guid.NewGuid().ToString("N");
        }

        if (string.IsNullOrEmpty(viewModel.namespaceId))
        {
            viewModel.namespaceId = "ProcessEntities";
        }

        Message newItem = viewModel.ToMessage();

        Message item = isNew ? newItem : ProjectDefinition.Project.GetMessage(id);
        if (item == null)
        {
            isNew = true;
            item = newItem;
        }
        if (!isNew)
        {
            ProjectDefinition.Project.RemoveCorrelationProperties(item);
            item.Copy(newItem);
            ProjectDefinition.Project.AddCorrelationProperties(item);
        }
        if (isNew)
        {
            ProjectDefinition.Project.AddMessage(item);
        }

        projectBpmn.Save();
        return Json(new MessageViewModel(item));
    }

    [HttpPost]
    public JsonResult Delete(string id)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        ProjectDefinition.Project.DeleteMessage(id);
        projectBpmn.Save();
        return Json(id);
    }
}
