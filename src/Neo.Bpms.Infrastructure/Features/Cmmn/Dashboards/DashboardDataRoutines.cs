using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;

public class DashboardDataRoutines(ReportDataRoutines reportDataRoutines)
{
    internal async Task GetDashboardData(
        DashboardData dashboardData, ConfiguredDashboard config,
        ConfiguredReport parentReportConfig, ConfiguredReport.ConfiguredSubReport subReport,
        string parentReportIds, ElasticObject parentFilters,
        string culture, bool forPrint, IdentityUser user, ReportConfigBackupRestore reportConfigBackupRestore)
    {
        Dashboard dashboard = config.Dashboard;
        IEnumerable<ConfiguredDashboard.ConfigWidget> widgets = config.Widgets.Where(w => !string.IsNullOrEmpty(w.ReportConfigId));
        SetFilterValuesWithParentDataAndGetIds(parentFilters, dashboardData);
        List<object> parentReportIdList = parentReportIds != null ? [.. parentReportIds.Split(',')] : null;
        foreach (ConfiguredDashboard.ConfigWidget widget in widgets)
        {
            ConfiguredDashboard.ConfigDiv div = GetWidgetDiv(config.Divs, widget);
            if (div == null)
                continue;

            UiEntity entity = ProjectDefinition.Project.GetEntity(widget.ReportNamespaceId, widget.ReportEntityId) as UiEntity;
            Report report = entity?.GetReport(widget.ReportId);
            if (report == null) continue;
            ConfiguredReport reportConfig = await reportConfigBackupRestore.GetConfig(report, widget.ReportConfigId);
            if (reportConfig == null) continue;
            int maxRecord = GetWidgetMaxRecord(widget, reportConfig.ViewType);
            ReportData reportData = dashboardData.ReportsData.FirstOrDefault(rd => rd.Structure.ConfigId == reportConfig.ConfigId);
            if (reportData != null)
                continue;
            reportData = await reportDataRoutines.GetReportData(
                reportConfig, true, dashboardData.Structure.FilterValues, 1, null,
                parentReportConfig, subReport, parentReportIdList, culture, forPrint, user, maxRecord);
            reportData.Structure.Name = string.IsNullOrEmpty(reportConfig.Name) ? report.Name : reportConfig.Name;
            dashboardData.ReportsData.Add(reportData);
        }

        dashboardData.Structure.Name = string.IsNullOrEmpty(config.Name) ? dashboard.Name : config.Name;
    }

    private static void SetFilterValuesWithParentDataAndGetIds(ElasticObject parentFilters, DashboardData dashboardData)
    {
        if (parentFilters != null)
        {
            if (dashboardData.Structure.FilterValues != null)
            {
                foreach (KeyValuePair<string, ElasticObject> att in parentFilters.Attributes)
                {
                    if (!dashboardData.Structure.FilterValues.GetField(att.Key, out object v) || v == null)
                        dashboardData.Structure.FilterValues.SetField(att.Key, att.Value);
                }
            }
            else dashboardData.Structure.FilterValues = parentFilters;
        }
    }

    private static ConfiguredDashboard.ConfigDiv GetWidgetDiv(List<ConfiguredDashboard.ConfigDiv> divs,
        ConfiguredDashboard.ConfigWidget widget)
    {
        ConfiguredDashboard.ConfigDiv div = divs.FirstOrDefault(d => d.WidgetId == widget.Id);
        if (div == null)
        {
            foreach (ConfiguredDashboard.ConfigDiv parent in divs)
                if (parent.Children != null)
                {
                    ConfiguredDashboard.ConfigDiv child = GetWidgetDiv(parent.Children, widget);
                    if (child != null)
                        return child;
                }
        }

        return div;
    }


    public static int GetWidgetMaxRecord(ConfiguredDashboard.ConfigWidget widget, ReportViewType viewType)
    {
        int maxRecord = widget.GetPropertyValueInt(eControlPropertyId.MaxRecordCount);
        if (maxRecord <= 0)
            maxRecord = viewType == ReportViewType.Chart ? 15 : 5;
        return maxRecord;
    }
}
