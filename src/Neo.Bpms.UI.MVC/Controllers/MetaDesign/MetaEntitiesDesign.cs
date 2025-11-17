using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;
using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityModels;


namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

public partial class MetaDesignController
{
    [HttpGet]
    public JsonResult Entities(string namespaceId, string entityId)
    {
        if (string.IsNullOrEmpty(namespaceId) || string.IsNullOrEmpty(entityId))
        {
            return Json(new { error = "Invalid entity" });
        }

        CheckEntityDesignAccess(false);

        IEnumerable<Entity> entities = ProjectDefinition.Project.Namespaces?.Values.SelectMany(n => n.GetEntities()?.Values);
        if (namespaceId != null && entityId != null)
        {
            entities = entities?.Where(e => e.NamespaceId == namespaceId && e.Id == entityId).ToList();
        }

        return Json(entities?.Select(e => new EntityRecognizer(e)));
    }

    [HttpGet]
    public JsonResult Entity(string namespaceId, string entityId)
    {
        CheckEntityDesignAccess(false);

        Entity entity = ProjectDefinition.Project.GetEntity(namespaceId, entityId);
        return Json(new EntityViewModel(entity));
    }

    [HttpGet]
    public JsonResult EntityFields(string namespaceId, string entityId, bool onlyAssociations = false)
    {
        CheckEntityDesignAccess(false);
        Entity entity = ProjectDefinition.Project.Namespaces?[namespaceId].GetEntity(entityId);
        ICollection<EntityField> fields = entity?.entityFields?.Values;
        if (onlyAssociations)
        {
            fields = fields?.Where(f => f.FieldType == TVariableTypes.Association).ToList();
        }
        return Json(new
        {
            fields = fields?.Select(f => new FieldRecognizer(f))
        });
    }

    [HttpGet]
    public JsonResult EntityStates(string namespaceId, string entityId)
    {
        CheckEntityDesignAccess(false);
        Entity entity = ProjectDefinition.Project.Namespaces?[namespaceId].GetEntity(entityId);
        return Json(entity?.GetStateCollection()?.States?.Values.Select(s => new { name = s.Name, id = s.Id }));
    }

    [HttpPost]
    public JsonResult NewEntity(string namespaceId)
    {
        CheckEntityDesignAccess(true);
        ModelNamespace @namespace = ProjectDefinition.Project.GetModel(namespaceId);
        string id = GenerateNewId(namespaceId, "Entity");
        UiEntity entity = new(@namespace, null, "موجودیت جدید در " + @namespace.Name, id,
                $"New Entity In {@namespace.Id}");
        @namespace.AddEntity(entity);
        ProjectEntity.Save(entity);

        return Json(new EntityRecognizer(entity));
    }

    [HttpDelete]
    public JsonResult Entity(string namespaceId, string entityId, int? justToMakeADifference)
    {
        CheckEntityDesignAccess(true);
        justToMakeADifference ??= justToMakeADifference ?? 0 + 1;
        ProjectEntity.Remove(namespaceId, entityId);

        //ProjectDefinition.Project.GetModel(namespaceId)?.GetEntities()?.Remove(entityId);

        return Json(new { Success = true });
    }

    [HttpPut]
    public JsonResult Entity([FromBody] EntityViewModel entityViewModel, string prevEntityId)
    {
        CheckEntityDesignAccess(true);

        Entity entity = ProjectDefinition.Project.GetEntity(entityViewModel.namespaceId, prevEntityId);
        entityViewModel.ModifyEntity(entity);
        if (!Equals(entityViewModel.id, prevEntityId))
        {
            ProjectEntity.Remove(entityViewModel.namespaceId, prevEntityId);
            //entity.model.GetEntities().Remove(prevEntityId);
            entity.model.GetEntities().Add(entityViewModel.id, entity);
        }

        ProjectEntity.ReConfig(entity, entity.model);
        ProjectEntity.Save(entity);
        return Json(new EntityViewModel(entity));
    }
}
