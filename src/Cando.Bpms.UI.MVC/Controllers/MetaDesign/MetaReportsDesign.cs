using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;
using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;


namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

public partial class MetaDesignController
{
    [HttpGet]
    public JsonResult Reports(string namespaceId, string entityId)
    {
        CheckEntityDesignAccess(false);

        IEnumerable<Report> reports = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?.GetReports();
        return Json(reports?.Select(r => new ReportRecognizer(r)));
    }

    [HttpGet]
    public JsonResult Report(string namespaceId, string entityId, string reportId)
    {
        CheckEntityDesignAccess(false);

        Report report = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?.GetReport(reportId);
        return Json(new ReportViewModel(report));
    }

    [HttpDelete]
    public async Task<JsonResult> Report(string namespaceId, string entityId, string reportId, int? justToMakeADifference, CancellationToken cancellationToken)
    {
        CheckEntityDesignAccess(false);

        await reportConfigManager.DeleteReportConfigs(namespaceId, entityId, reportId, cancellationToken);

        ProjectEntityReport.Remove(namespaceId, entityId, reportId);

        return Json(new { Success = true });
    }

    [HttpPut]
    public JsonResult Report([FromBody] ReportViewModel reportViewModel, string prevReportId)
    {
        CheckEntityDesignAccess(true);

        UiEntity entity = ProjectDefinition.Project.GetUiEntity(reportViewModel.namespaceId, reportViewModel.entityId);
        if (entity == null)
            return Json(new { Success = true });
        Report report = entity.GetReport(prevReportId);
        reportViewModel.ModifyReport(report);
        if (report.Id != prevReportId)
        {
            ProjectEntityReport.Remove(reportViewModel.namespaceId, reportViewModel.entityId, prevReportId);
            //entity.DeleteReport(prevReportId);
            entity.AddReport(report);
        }
        ProjectEntityReport.Save(report);
        return Json(new { Success = true });
    }

    [HttpPost]
    public ActionResult NewReport(string namespaceId, string entityId)
    {
        CheckEntityDesignAccess(true);

        UiEntity uiEntity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        if (uiEntity == null)
            return BadRequest();
        string reportId = GenerateNewId(namespaceId, "ReportId");
        Report report = new(uiEntity, reportId, "گزارش موجودیت " + uiEntity.Name, uiEntity.EnName + " Report");
        uiEntity.AddReport(report);
        ProjectEntityReport.Save(report);
        return Json(new ReportRecognizer(report));
    }

    [HttpGet]
    public JsonResult ReportPossibleColumns(string namespaceId, string entityId, string reportNamespaceId, string reportEntityId, string reportId)
    {
        CheckEntityDesignAccess(false);
        UiEntity uiEntity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        Report report = ProjectDefinition.Project.GetUiEntity(reportNamespaceId, reportEntityId)?.GetReport(reportId);
        if (uiEntity == null || report == null)
            return Json(new { Success = false });
        Domain.Entities.Cmmn.Fields.EntityFields entityFields = uiEntity.entityFields;
        PossibleColumunsViewModel ret = new()
        {
            fields = entityFields?.Values.Where(f => !report.reportFields.ContainsKey(f.Id)).Select(f => new PossibleFieldViewModel
            {
                id = f.Id,
                name = f.Name
            }).ToList(),
            associations = entityFields?.Values.Where(a => a.AssociationEntity != null)
                  .Select(f => new PossibleAssociationsViewModel
                  {
                      associationId = f.Id,
                      namespaceId = f.AssociationEntity.DestNamespaceId,
                      entityId = f.AssociationEntity.DestEntityId
                  }).ToList(),
        };

        return Json(ret);
    }

    [HttpGet]
    public JsonResult PossibleSubReports(string namespaceId, string entityId)
    {
        CheckEntityDesignAccess(false);

        if (string.IsNullOrEmpty(namespaceId) || string.IsNullOrEmpty(entityId))
            throw new Exception("namespaceId and entityId can't be null");
        List<PossibleSubReportsViewModel> possibleSubReports = [];
        List<Entity> entities = ProjectDefinition.Project.Namespaces?.Values.SelectMany(n => n.GetEntities()?.Values)
             .ToList();
        foreach (Entity e in entities ?? Enumerable.Empty<Entity>())
        {
            Domain.Entities.Cmmn.Fields.EntityFields entityFields = e?.entityFields;
            if (entityFields == null) continue;
            foreach (Domain.Entities.Cmmn.Fields.EntityField rf in entityFields.Values.Where(f =>
                 (!f?.NotMapped ?? false) &&
                 f.AssociationEntity?.DestEntityId == entityId &&
                 f.AssociationEntity?.DestNamespaceId == namespaceId))
            {
                List<Report> reports = (rf.Entity as UiEntity)?.GetReports()?.ToList();
                if (!reports?.Any() ?? true) continue;
                //                    possibleSubReports.AddRange(
                //                        reports.Select(r =>
                //                            new SubReportViewModel(r, rf.Association.enName))); //todo correct association name?!
                possibleSubReports.Add(new PossibleSubReportsViewModel
                {
                    entityId = rf.Entity.Id,
                    entityName = rf.Entity.Name,
                    linkedField = rf.Id,
                    subReports = reports.Select(r => new PossibleSubReportViewModel
                    {
                        id = r.Id,
                        name = r.Name,
                        entityName = rf.Entity.Name,
                        entityId = rf.Entity.Id
                    })
                });
            }
        }
        return Json(possibleSubReports);
    }
}
