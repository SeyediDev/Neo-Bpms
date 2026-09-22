using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Helpers
{
    public static class DashboardRenderHelpers
    {
        public static ConfiguredDashboard.ConfigWidget ResolveWidget(DashboardData dashboardData, ConfiguredDashboard.ConfigDiv divConfig)
        {
            if (divConfig == null)
            {
                return null;
            }

            if (divConfig.Widget != null)
            {
                return divConfig.Widget;
            }

            return dashboardData?.Structure?.ActiveConfig?.Widgets?.FirstOrDefault(w => w.Id == divConfig.WidgetId);
        }

        public static int ResolveWidth(ConfiguredDashboard.ConfigDiv divConfig)
        {
            return divConfig?.Width is >= 1 and <= 12 ? (int)divConfig.Width : 12;
        }

        public static int GetWidgetHeight(ConfiguredDashboard.ConfigWidget widget)
        {
            var widgetHeight = widget?.GetPropertyValueInt(eControlPropertyId.HeightInPixels) ?? 0;
            return widgetHeight == 0 ? 300 : widgetHeight;
        }

        public static int GetMaxRecord(ConfiguredDashboard.ConfigWidget widget)
        {
            return widget != null ? DashboardDataRoutines.GetWidgetMaxRecord(widget, ReportViewType.Chart) : 15;
        }

        public static ReportData GetReportData(DashboardData dashboardData, ConfiguredDashboard.ConfigWidget widget, int maxRecord)
        {
            if (widget == null || dashboardData?.ReportsData == null)
            {
                return null;
            }

            var reportsData = dashboardData.ReportsData;

            // Older saved dashboard configurations can omit ReportNamespaceId. In that
            // case the entity id/report/config tuple is still sufficient to select data.
            if (string.IsNullOrWhiteSpace(widget.ReportNamespaceId))
            {
                return reportsData.FirstOrDefault(rd =>
                    rd.Structure?.EntityId == widget.ReportEntityId &&
                    rd.Structure?.Form_ReportId == widget.ReportId &&
                    rd.Structure?.ConfigId == widget.ReportConfigId);
            }

            var reportData = reportsData.FirstOrDefault(rd =>
                    rd.Structure?.NamespaceId == widget.ReportNamespaceId &&
                    rd.Structure?.EntityId == widget.ReportEntityId &&
                    rd.Structure?.Form_ReportId == widget.ReportId &&
                    rd.Structure?.ConfigId == widget.ReportConfigId &&
                    rd.RecordCount == maxRecord) ??
                reportsData.FirstOrDefault(rd =>
                    rd.Structure?.NamespaceId == widget.ReportNamespaceId &&
                    rd.Structure?.EntityId == widget.ReportEntityId &&
                    rd.Structure?.Form_ReportId == widget.ReportId &&
                    rd.Structure?.ConfigId == widget.ReportConfigId);

            return reportData;
        }
    }
}
