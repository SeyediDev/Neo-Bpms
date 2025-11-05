using Neo.Bpms.Domain.Entities.Cmmn;
using Neo.Bpms.Domain.Entities.Security.Authorization;
using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityModels;
using Microsoft.AspNetCore.Antiforgery;
namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

public partial class MetaDesignController(
    FilterConfigBackupRestore filterConfigBackupRestore,
    FolderConfigBackupRestore folderConfigBackupRestore,
    DashboardConfigManager dashboardConfigManager,
    FormStructRoutines formStructRoutines,
    ReportConfigManager reportConfigManager, IProjectBpmn projectBpmn,
    IAntiforgery antiForgery, ILogger<MetaDesignController> logger) : BpmsController
{
    public ActionResult Index()
    {
        CheckEntityDesignAccess(false, out IdentityUser user);
        AddXsrfToken();
        ViewBag.user = user;
        ViewBag.ContainerClass = "container-fluid";
        SetPagePackId("/MetaDesign/App/entity");
        return View();
    }

    private void AddXsrfToken()
    {
        Response.Cookies.Append("X-CSRF-TOKEN-COOKIE", antiForgery.GetAndStoreTokens(HttpContext).RequestToken,
            new CookieOptions()
            {
                HttpOnly = false
            });
    }

    // todo move them
    public JsonResult Namespaces()
    {
        CheckEntityDesignAccess(false);
        Dictionary<string, ModelNamespace>.ValueCollection namespaces = ProjectDefinition.Project.Namespaces.Values;
        return Json(namespaces.Select(n => new { id = n.Id, name = n.Name }));
    }

    public JsonResult MetaDesignerNamespaces()
    {
        CheckEntityDesignAccess(false);
        Dictionary<string, ModelNamespace>.ValueCollection namespaces = ProjectDefinition.Project.Namespaces.Values;
        var namespacesList = namespaces.Where(CanBeManipulatedByUser)
            .Select(n => new { value = n.Id, label = n.Name });

        return Json(new
        {
            defaultNamespaceId = "ContentPool",
            namespaceList = namespacesList
        });
    }

    public JsonResult NamespacesForMetaResourceDesigner()
    {
        CheckEntityDesignAccess(false);
        List<ModelNamespace> namespaces = ProjectDefinition.Project.Namespaces.Values
            .Where(CanBeUsedForResources).ToList();
        return Json(namespaces.Select(n => new { id = n.Id, name = n.Name }));
    }

    private static bool CanBeUsedForResources(ModelNamespace ns)
    {
        return ns.Id != "Shared"
               && ns.Id != "ProcessEntities"
               && ns.Id != "SystemConfigs"
               && ns.Id != "Audit"
               && ns.Id != "Message"
               && ns.Id != "Notification"
               && ns.Id != "ProcessData"
               && ns.Id != "BPMNTemplate"
               && ns.Id != "DatabaseManagementViews";
    }

    private static bool CanBeManipulatedByUser(ModelNamespace ns)
    {
        return ns.Id != "Shared"
               && ns.Id != "SystemConfigs"
               && ns.Id != "Audit"
               && ns.Id != "Message"
               && ns.Id != "Notification"
               && ns.Id != "ProcessData"
               && ns.Id != "BPMNTemplate"
               && ns.Id != "DatabaseManagementViews";
    }

    public JsonResult MessageStructures()
    {
        CheckEntityDesignAccess(false);
        ProjectDefinition.Project.Namespaces.TryGetValue("ProcessEntities", out ModelNamespace @namespace);
        List<Domain.Entities.Cmmn.Entities.Entity> structures = @namespace?.GetEntities()?.Values.ToList();
        return Json(structures?.Select(e => new EntityRecognizer(e)));
    }

    public JsonResult MessageStructure(string entityId)
    {
        CheckEntityDesignAccess(false);
        Domain.Entities.Cmmn.Entities.Entity structure = ProjectDefinition.Project.Namespaces?["ProcessEntities"]?.GetEntity(entityId);
        return Json(new { fields = structure?.entityFields.Select(f => f.Key) });
    }

    public JsonResult InterfaceOperations(string interfaceId)
    {
        CheckEntityDesignAccess(false);

        return Json(ProjectDefinition.Project.GetInterface(interfaceId)?.operations
            ?.Select(o => new { o.Key, id = o.Value.Id }));
    }

    private void CheckEntityDesignAccess(bool toApplyChanges) // todo toApplyChanges
    {
        CheckEntityDesignAccess(toApplyChanges, out _);
    }

    private void CheckEntityDesignAccess(bool toApplyChanges, out IdentityUser user)
    {
        CheckDesignFeature(toApplyChanges);
        CheckSystemFeatureAccess(SystemFeatureId.EntityDesign, out user);
    }

    private static string GenerateNewId(string namespaceId, string subject)
    {
        return $"{namespaceId}{subject}_{Guid.NewGuid().ToString().Replace('.', '_')}";
    }
}
