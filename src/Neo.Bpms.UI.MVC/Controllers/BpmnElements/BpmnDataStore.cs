using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.UI.MVC.Controllers.BpmnElements;

public class BpmnDataStoreController(IProjectBpmn projectBpmn) : BpmsController
{
    [HttpGet]
    public JsonResult List()
    {
        IEnumerable<DataStoreViewModel> datastores = ProjectDefinition.Project.DataStores?.Select(ds => new DataStoreViewModel(ds));
        return Json(datastores);
    }

    [HttpPost]
    public JsonResult Save(string id, DataStoreViewModel viewModel)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        bool isNew = string.IsNullOrEmpty(id);
        if (isNew)
            viewModel.id = $"DataStore.{viewModel.namespaceId}.{viewModel.entityId}";
        DataStore newItem = viewModel.ToDataStore();

        DataStore item = isNew ? newItem : ProjectDefinition.Project.GetDataStore(id);
        if (item == null)
        {
            isNew = true;
            item = newItem;
        }

        if (!isNew)
            item.Copy(newItem);
        if (isNew)
            ProjectDefinition.Project.AddDataStore(item);
        projectBpmn.Save();
        return Json(new DataStoreViewModel(item));
    }

    [HttpPost]
    public JsonResult Delete(string id)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        ProjectDefinition.Project.DeleteDataStore(id);
        projectBpmn.Save();
        return Json(id);
    }
}
