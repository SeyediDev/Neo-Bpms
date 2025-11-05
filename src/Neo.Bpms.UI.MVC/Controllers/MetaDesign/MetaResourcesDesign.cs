using Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;

namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

public partial class MetaDesignController
{
    [HttpGet]
    public JsonResult MetaResources()
    {
        CheckEntityDesignAccess(false);

        return Json(ProjectDefinition.Project.Resources?.Select(f =>
            new ViewModels.MetaDesignModels.ResourceViewModel(f)));
    }

    [HttpGet]
    public JsonResult MetaResource(string id)
    {
        CheckEntityDesignAccess(false);

        Domain.Model.BPMN.Core.CommonElements.Resource resource = ProjectDefinition.Project.GetResource(id) ?? throw new Exception("چنین منبعی موجود نیست");
        return Json(new ViewModels.MetaDesignModels.ResourceViewModel(resource));
    }


    [HttpPut]
    public JsonResult MetaResource([FromBody] ViewModels.MetaDesignModels.ResourceViewModel resourceViewModel, string prevFormId)
    {
        CheckEntityDesignAccess(true);

        Domain.Model.BPMN.Core.CommonElements.Resource resource = ProjectDefinition.Project.GetResource(prevFormId) ?? throw new Exception("چنین منبعی موجود نیست");
        resourceViewModel.Modify(resource);
        if (resource.Id != prevFormId)
        {
            throw new Exception("Resource Id can't Change");

            //ProjectDefinition.Project.DeleteResource(prevFormId);
            //ProjectDefinition.Project.AddResource(resource);
        }
        projectBpmn.Save();
        return Json(new { Success = true });
    }

    [HttpDelete]
    public JsonResult MetaResource(string id, int? justToMakeADifference)
    {
        CheckEntityDesignAccess(true);
        foreach (BusinessProcess businessProcess in ProjectDefinition.Project.BusinessProcesses.Values)
        {
            foreach (BusinessProcessVersion businessProcessVersion in businessProcess.Versions.Values)
            {
                Domain.Model.BPMN.Processes.Process process = businessProcessVersion.BpmnDefinitions.Process;

                if (process.laneSets.SelectMany(laneSet => laneSet.lanes)
                    .Any(lane => lane.resources != null && lane.resources.Exists(r => r.resourceRef.Id == id)))
                    throw new Exception($"this resource has used in process : {process.Id}");
                if (process.resources != null && process.resources.Exists(r => r.resourceRef.Id == id))
                    throw new Exception($"this resource has used in process : {process.Id}");
                if (process.flowElements.Values.Select(f => f as Activity).Any(activity => activity?.resources != null && activity.resources.Exists(r => r.resourceRef.Id == id)))
                    throw new Exception($"this resource has used in process : {process.Id}");
            }
        }
        ProjectDefinition.Project.DeleteResource(id);
        projectBpmn.Save();

        return Json(new { Success = true });
    }

    [HttpPost]
    public JsonResult NewMetaResource(string id, string name, string namespaceId, string entityId)
    {
        CheckEntityDesignAccess(true);
        if (id.Contains(' '))
            throw new Exception("id can't contains space character.");
        ProjectDefinition.Project.AddResource(id, name, namespaceId, entityId);
        Domain.Model.BPMN.Core.CommonElements.Resource resource = ProjectDefinition.Project.GetResource(id);
        projectBpmn.Save();
        return Json(new ViewModels.MetaDesignModels.ResourceViewModel(resource));
    }
}
