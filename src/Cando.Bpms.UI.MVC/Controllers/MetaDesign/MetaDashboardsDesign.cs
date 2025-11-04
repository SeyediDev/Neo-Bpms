using Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;

namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

public partial class MetaDesignController
{
    [HttpGet]
    public JsonResult Dashboards(string namespaceId, string entityId)
    {
        CheckEntityDesignAccess(false);

        ICollection<Dashboard> dashboards = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?.GetDashboards()?.Values;
        return Json(dashboards?.Select(d => new DashboardViewModel(d)));
    }

    //isn't needed
    [HttpGet]
    public JsonResult Dashboard(string namespaceId, string entityId, string dashboardId)
    {
        CheckEntityDesignAccess(false);

        Dashboard dashboard = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?.GetDashboard(dashboardId);
        return Json(new DashboardViewModel(dashboard));
    }

    [HttpDelete]
    public async Task<JsonResult> Dashboard(string namespaceId, string entityId, string dashboardId, int? justToMakeADifference, 
        CancellationToken cancellationToken)
    {
        CheckEntityDesignAccess(true);

        await dashboardConfigManager.DeleteDashboardConfigs(namespaceId, entityId, dashboardId, cancellationToken);

        ProjectEntityDashboard.Remove(namespaceId, entityId, dashboardId);

        return Json(new { Success = true });
    }

    [HttpPut]
    public JsonResult Dashboard([FromBody] DashboardViewModel dashboardViewModel, string prevDashboardId)
    {
        CheckEntityDesignAccess(true);

        UiEntity entity =
             ProjectDefinition.Project.GetUiEntity(dashboardViewModel.namespaceId, dashboardViewModel.entityId);
        if (entity == null)
            return Json(new { Success = true });
        Dashboard dashboard = entity.GetDashboard(prevDashboardId);
        dashboardViewModel.Modify(dashboard);
        if (dashboard.Id != prevDashboardId)
        {
            ProjectEntityDashboard.Remove(dashboardViewModel.namespaceId, dashboardViewModel.entityId, prevDashboardId);
            //entity.DeleteDashboard(prevDashboardId);
            entity.AddDashboard(dashboard);
        }
        ProjectEntityDashboard.Save(dashboard);
        return Json(new { Success = true });
    }

    [HttpPost]
    public JsonResult NewDashboard(string namespaceId, string entityId)
    {
        CheckEntityDesignAccess(true);

        UiEntity uiEntity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        if (uiEntity == null)
            return Json(new { Success = false });
        string dashboardId = GenerateNewId(namespaceId, "Dashboard");
        Dashboard dashboard = new(uiEntity, dashboardId, "داشبورد " + uiEntity.Name, uiEntity.EnName + " Dashboard");
        uiEntity.AddDashboard(dashboard);
        ProjectEntityDashboard.Save(dashboard);
        return Json(new DashboardViewModel(dashboard));
    }

    [HttpGet]
    public JsonResult PossibleReports(string namespaceId, string entityId)
    {
        CheckEntityDesignAccess(false);

        IEnumerable<EntityReportsViewModel> possibleItems = ProjectDefinition.Project.Namespaces?.Values.SelectMany(ns =>
             ns.GetEntities()?.Values.Where(e => ((UiEntity)e).GetReports()?.Any() ?? false)
                  .Select(e => new EntityReportsViewModel((UiEntity)e)));

        return Json(possibleItems);
    }
}
