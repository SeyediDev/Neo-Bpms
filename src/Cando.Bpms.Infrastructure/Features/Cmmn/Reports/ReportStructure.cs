using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Reports;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Common;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class ReportStructure : CommonFormStructure
{
    public ReportViewType ReportViewType { get; set; }
    public Report.ChartType ChartType { get; set; }

    public IList<ReportColumnFilter> SelectedFilters { get; set; }

    //		public IList<ReportSelectedGroupBy> ReportSelectedGroupBys{ get; set; }
    public IList<ReportOrderInfo> OrderInfos { get; set; } = [];

    public string HavingConstraint { get; set; }
    public string Constraint { get; set; }
    public ValueRange ValueRange { get; set; }
    public List<ValueRange> Levels { get; set; }

    public IList<ColumnFieldDefinition> SelectedColumns { get; set; } = [];
    public IEnumerable<ColumnFieldDefinition> Columns => SelectedColumns.Where(c => c.aggrType != eAggregationFunctions.GroupByItem);
    public IList<PostedProperty> Properties { get; set; }

    public bool HasAnyDrillDown =>
        SubReports != null && SubReports.Any(sr => sr.Type == Report.SubReportType.DrillDown);

    public List<ConfiguredReport.ConfiguredSubReport> SubReports { get; set; }
        = [];

    public List<ConfiguredReport> Configs { get; set; } = [];

    public List<ConfigTreeItem> ConfigsTree
    {
        get
        {
            List<ConfigTreeItem> result = ConfiguredFolders?.Where(f => f.IsForConfig)
                .OrderBy(f => f.IsPublic.ToString() + string.Join(",", f.Roles ?? []) + f.Name)
                .Select(f =>
                {
                    ConfiguredFolder folder = ConfiguredFolders?.FirstOrDefault(ff => ff.Id == f.FolderId);
                    return new ConfigTreeItem
                    {
                        id = f.Id.ToString(),
                        data = new
                        {
                            isPublic = f.IsPublic,
                            userGroupId = string.Join(",", f.Roles ?? []),
                            isForConfig = true,
                            parentFolderId = folder?.Id
                        },
                        parent = folder?.Id.ToString() ?? "#",
                        text = f.Name,
                        type = "default"
                    };
                }).ToList();

            result?.AddRange(Configs?.Where(cf => cf.Parent == null /*&& cf.viewType!= eReportViewType.Dashboard*/)
                                 .OrderBy(f => f.ViewType + f.IsPublic.ToString() + string.Join(",", f.Roles ?? []) + f.Name)
                                 .Select(c =>
                                 {
                                     ConfiguredFolder folder = ConfiguredFolders?.FirstOrDefault(ff => ff.Id == c.FolderId);
                                     return new ConfigTreeItem
                                     {
                                         id = c.ConfigId,
                                         parent = folder?.Id.ToString() ?? "#",
                                         text = c.Name,
                                         type = c.ViewType.ToString(),
                                         data = new
                                         {
                                             hasSchedules = c.ScheduledReports?.Any() ?? false,
                                             configId = c.ConfigId,
                                             isPublic = c.IsPublic,
                                             isMeta = c.IsMeta
                                         }
                                     };
                                 }) ?? []);

            return result;
        }
    }


    public List<ConfigTreeItem> ScheduledReportsTree
    {
        get
        {
            List<ConfigTreeItem> result =
            [
                new() 
                {
                    id = "thisConfig",
                    parent = "#",
                    text = CulturalTexts.ThisConfigSchedules,
                    type = "default"
                },
                new() 
                {
                    id = "otherConfig",
                    parent = "#",
                    text = CulturalTexts.OtherConfigsSchedules,
                    type = "default"
                },
                .. Configs.SelectMany(c => c.ScheduledReports)
                    .OrderBy(f => f.ReportConfigId.ToString() + f.Name)
                    .Select(c => new ConfigTreeItem
                    {
                        id = c.Id.ToString(),
                        data = new
                        {
                            configId = c.ReportConfigId?.ToString() ?? ""
                        },
                        parent = c.ReportConfigId == ConfigId ? "thisConfig" : "otherConfig",
                        text = c.Name,
                        type = "ScheduledReport"
                    }),
            ];
            return result;
        }
    }

    public string GetReportProperty(long id)
    {
        PostedProperty property = Properties?.FirstOrDefault(p => p.PropertyId == id);
        return property?.Value ?? GetDefaultPropertyValue(id);
    }

    private string GetDefaultPropertyValue(long id)
    {
        return id switch
        {
            (long)ReportConfigProperty.BackgroundColor => "#17a2b8",//If it got changed, tetametric.js default value should also change
            (long)ReportConfigProperty.TextColor => "#ffffff",//If it got changed, tetametric.js default value should also change
            _ => "",
        };
    }

    public string[] GetReportPropertyList(long id)
    {
        string[] property = Properties?.Where(p => p.PropertyId == id).Select(p2 => p2.Value).ToArray();
        return property;
    }
}
