namespace Neo.Bpms.UI.MVC.Controllers.BpmnElements;

public class BpmnInterfaceOperationsController(IProjectBpmn projectBpmn) : BpmsController
{
    [HttpPost]
    public JsonResult Save(string id, OperationViewModel viewModel)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        bool isNew = string.IsNullOrEmpty(id);
        if (isNew)
            viewModel.id = "Operation" + Guid.NewGuid().ToString("N");

        Domain.Entities.Bpmn.Core.Services.Interface @interface = ProjectDefinition.Project.GetInterface(viewModel.interfaceId) ?? throw new ValidationException("inteface id must be determind.");
        Domain.Entities.Bpmn.Core.Services.Operation newItem = viewModel.ToOperation(@interface);
        Domain.Entities.Bpmn.Core.Services.Operation item = isNew ? newItem : ProjectDefinition.Project.GetOperation(id);
        if (item == null)
        {
            isNew = true;
            item = newItem;
        }
        if (!isNew)
            item.Copy(newItem);
        if (isNew)
        {
            @interface.operations ??= [];
            @interface.operations.Add(viewModel.name, item);
        }
        projectBpmn.Save();
        return Json(new OperationViewModel(item));
    }

    [HttpPost]
    public JsonResult Delete(string id)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        ProjectDefinition.Project.DeleteOperation(id);
        projectBpmn.Save();
        return Json(id);

    }
}
