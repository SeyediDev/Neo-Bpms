using System;
using System.Linq;
using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;

namespace Neo.Bpms.UI.MVC.Helpers
{
    public static class DashboardRenderHelpers
    {
        public static string ResolveDashboardIcon(ConfiguredDashboard config)
        {
            if (config == null)
            {
                return "home-dashboard";
            }

            if (!string.IsNullOrWhiteSpace(config.Icon))
            {
                return config.Icon;
            }

            return config.ConfigId switch
            {
                "ManagementOverviewDashboard" => "home-dashboard",
                "CustomerJourneyDashboard" => "users-analysis",
                "MarketingAndEngagementDashboard" => "gift-present",
                "OperationsHealthDashboard" => "cog-wheel",
                "LoyaltyAndValueDashboard" => "star-badge",
                _ => "home-dashboard"
            };
        }

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
            if (divConfig?.Width is >= 1 and <= 12)
            {
                return (int)divConfig.Width;
            }

            return 12;
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

            var reportData = reportsData.FirstOrDefault(rd =>
                rd.structure?.NamespaceId == widget.ReportNamespaceId &&
                rd.structure?.EntityId == widget.ReportEntityId &&
                rd.structure?.Form_ReportId == widget.ReportId &&
                rd.structure?.ConfigId == widget.ReportConfigId &&
                rd.RecordCount == maxRecord);

            if (reportData == null)
            {
                reportData = reportsData.FirstOrDefault(rd =>
                    rd.structure?.NamespaceId == widget.ReportNamespaceId &&
                    rd.structure?.EntityId == widget.ReportEntityId &&
                    rd.structure?.Form_ReportId == widget.ReportId &&
                    rd.structure?.ConfigId == widget.ReportConfigId);
            }

            if (reportData == null)
            {
                reportData = reportsData.FirstOrDefault(rd =>
                    rd.structure?.NamespaceId == widget.ReportNamespaceId &&
                    rd.structure?.EntityId == widget.ReportEntityId &&
                    rd.structure?.Form_ReportId == widget.ReportId);
            }

            if (reportData == null)
            {
                reportData = reportsData.FirstOrDefault(rd => rd.structure?.Form_ReportId == widget.ReportId);
            }

            return reportData;
        }

        public static string ResolveWidgetIcon(string widgetId)
        {
            if (string.IsNullOrWhiteSpace(widgetId))
            {
                return "chart-bar";
            }

            return widgetId switch
            {
                "TotalCustomersWidget" or "JourneyTotalCustomersWidget" => "users-group",
                "ActiveCustomersWidget" or "JourneyActiveCustomersWidget" => "users-analysis",
                "NewCustomersWidget" or "JourneyNewCustomersWidget" => "users-network",
                "JourneyAverageSatisfactionWidget" => "star-badge",
                "RfmSegmentOverviewWidget" or "AverageClvByRfmWidget" or "ClvDistributionWidget" => "users-analysis",
                "CustomerGrowthByMonthWidget" or "RetentionRateWidget" or "RevenueTrendWidget" or "CampaignRoiWidget" or "ConversionRateWidget" => "chart-bar",
                "TotalRevenueWidget" or "MarketingTotalRevenueWidget" => "coins-money",
                "RevenueOverallRoiWidget" or "MarketingOverallRoiWidget" => "chart-bar",
                "MarketingTotalMarketingCostsWidget" or "LoyaltyTotalPointsBalanceWidget" or "CustomerPointsBalanceWidget" => "wallet-money",
                "MarketingActiveCampaignsKpiWidget" => "rss-signal",
                "MessageFunnelWidget" => "list-checklist",
                "EngagementDistributionWidget" => "users-analysis",
                "OperationsPointsIssuedWidget" or "OperationsPointsRedeemedWidget" or "OperationsAverageTransactionWidget" or "LoyaltyAverageTransactionValueWidget" => "coins-money",
                "OperationsTotalEventsWidget" or "EventLogByEventTypeWidget" => "calendar-event",
                "TransactionsByMonthWidget" => "chart-bar",
                "TransactionsByTypeWidget" => "grid-layout",
                "DailyActiveUsersWidget" => "users-group",
                "LoyaltyPointsIssuedWidget" => "star-badge",
                "LoyaltyPointsRedeemedWidget" => "gift-present",
                "PointsByTypeWidget" => "star-badge",
                "PopularProductsWidget" => "box-package",
                "ProductRepeatPurchaseWidget" => "store-shop",
                _ => "chart-bar"
            };
        }
    }
}
