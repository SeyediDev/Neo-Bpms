using Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;
using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels;


namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

public partial class MetaDesignController
{
    [HttpGet]
    public JsonResult PartitionSchemes(/*string namespaceId*/)
    {
        CheckEntityDesignAccess(false);

        List<Domain.Entities.Cmmn.Partitions.PartitionScheme> partitionSchemes = ProjectDefinition.Project.Namespaces?.Values.Where(n => n.PartitionSchemes?.Values != null).SelectMany(n => n.PartitionSchemes?.Values).ToList();
        return Json(partitionSchemes?.Select(e => new PartitionSchemeViewModel(e)));
    }

    [HttpGet]
    public JsonResult PartitionScheme(string namespaceId, string partitionSchemeId)
    {
        CheckEntityDesignAccess(false);

        Domain.Entities.Cmmn.Partitions.PartitionScheme partitionScheme = ProjectDefinition.Project.GetModel(namespaceId)?.PartitionSchemes[partitionSchemeId];
        return Json(new PartitionSchemeViewModel(partitionScheme));
    }

    [HttpPost]
    public JsonResult NewPartitionScheme(string namespaceId)
    {
        CheckEntityDesignAccess(true);

        Domain.Entities.Cmmn.ModelNamespace modelNamespace = ProjectDefinition.Project.GetModel(namespaceId);
        if (modelNamespace == null)
            return Json(new { Success = false });
        _ = GenerateNewId(namespaceId, "Form");
        //var enumeration = new Enumeration(modelNamespace, "رشته جدید در " + modelNamespace.Name, partitionSchemeId);

        //modelNamespace.AddEnumeration(enumeration);
        //ProjectEnum.Save(report);
        return Json(new { Success = true });
        //return Json(new PartitionSchemeViewModel(enumeration), JsonRequestBehavior.AllowGet);
    }

    [HttpPut]
    public JsonResult PartitionScheme([FromBody] PartitionSchemeViewModel partitionSchemeViewModel, string prevPartitionSchemeId)
    {
        CheckEntityDesignAccess(true);

        Domain.Entities.Cmmn.ModelNamespace modelNamespace = ProjectDefinition.Project.GetModel(partitionSchemeViewModel.namespaceId);
        if (modelNamespace == null)
            return Json(new { Success = true });
        Domain.Entities.Cmmn.Partitions.PartitionScheme partitionScheme = modelNamespace.GetPartitionScheme(prevPartitionSchemeId);
        partitionSchemeViewModel.Modify(partitionScheme);
        if (partitionScheme.Id != prevPartitionSchemeId)
        {
            modelNamespace.DeletePartitionScheme(prevPartitionSchemeId);
            //todo modelNamespace.AddPartitionScheme(partitionScheme);
        }
        ProjectNamespace.Save(modelNamespace);
        return Json(new { Success = true });
    }

    [HttpDelete]
    public JsonResult PartitionScheme(string namespaceId, string partitionSchemeId, string justToMakeADifference)
    {
        CheckEntityDesignAccess(true);

        ProjectEnum.Remove(namespaceId, partitionSchemeId);

        return Json(new { Success = true });
    }
}