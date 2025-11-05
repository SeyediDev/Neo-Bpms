namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

public partial class MetaDesignController
{
    [HttpGet]
    public JsonResult Enums()
    {
        CheckEntityDesignAccess(false);

        IEnumerable<Enumeration> enums = ProjectDefinition.Project.Namespaces?.Values.SelectMany(n => n.GetEnums()?.Values);
        return Json(enums?.Select(e => new EnumRecognizer(e)));
    }

    [HttpGet]
    public JsonResult Enum(string namespaceId, string enumId)
    {
        CheckEntityDesignAccess(false);

        Enumeration @enum = ProjectDefinition.Project.GetModel(namespaceId)?.GetEnum(enumId);
        return Json(new EnumViewModel(@enum));
    }
    [HttpGet]
    public JsonResult Claims()
    {
        EnumViewModel @enum = new()
        {
            id = "ClaimType",
            enName = "ClaimType",
            name = "شناسه کاربر",
            //namespaceId = "",
            values = ProjectDefinition.Project.Namespaces.Where(n => n.Value.GetEnum("ClaimType") != null).SelectMany(n =>
                             n.Value.GetEnum("ClaimType")?.items?.Values.Select(e => new EnumValueViewModel(e)))
        };
        return Json(@enum);
    }

    [HttpPost]
    public JsonResult NewEnum(string namespaceId)
    {
        CheckEntityDesignAccess(true);
        namespaceId ??= "SsoManagement";
        ModelNamespace namespaceModel = ProjectDefinition.Project.GetModel(namespaceId);
        string enumId = GenerateNewId(namespaceId, "Enum");
        Enumeration enumeration = new(namespaceModel, "رشته جدید در " + namespaceModel.Name, enumId);
        namespaceModel.AddEnumeration(enumeration);
        ProjectEnum.Save(enumeration);

        UiEntity entity = new(namespaceModel, null, "موجودیت جدید در " + namespaceModel.Name, enumId, $"New Entity In {namespaceModel.Id}");
        new CSharpObjectToEntityField(ProjectDefinition.Project, typeof(StringList), entity, logger)
            .DefineEntityFields();

        namespaceModel.AddEntity(entity);
        ProjectEntity.Save(entity);

        return Json(new EnumViewModel(enumeration));
    }

    [HttpPut]
    public JsonResult Enum([FromBody] EnumViewModel enumViewModel, string prevEnumId)
    {
        CheckEntityDesignAccess(true);

        ModelNamespace modelNamespace = ProjectDefinition.Project.GetModel(enumViewModel.namespaceId);
        if (modelNamespace == null)
            return Json(new { Success = true });
        Enumeration enumeration = modelNamespace.GetEnum(prevEnumId);
        enumViewModel.Modify(enumeration);
        if (enumeration.Id != prevEnumId)
        {
            ProjectEnum.Remove(enumViewModel.namespaceId, prevEnumId);
            //modelNamespace.DeleteEnum(prevEnumId);
            modelNamespace.AddEnumeration(enumeration);
        }
        ProjectEnum.Save(enumeration);

        Entity entity = ProjectDefinition.Project.GetEntity(enumViewModel.namespaceId, prevEnumId);
        entity.Id = enumViewModel.id;
        entity.Name = enumViewModel.name;
        if (!Equals(enumViewModel.id, prevEnumId))
        {
            ProjectEntity.Remove(enumViewModel.namespaceId, prevEnumId);
            entity.model.GetEntities().Add(enumViewModel.id, entity);
        }
        ProjectEntity.Save(entity);

        return Json(new { Success = true });
    }

    [HttpDelete]
    public JsonResult Enum(string namespaceId, string enumId, string justToMakeADifference)
    {
        CheckEntityDesignAccess(true);

        ProjectEnum.Remove(namespaceId, enumId);

        return Json(new { Success = true });
    }
}
