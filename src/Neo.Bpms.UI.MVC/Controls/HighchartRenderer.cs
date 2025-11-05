using Microsoft.AspNetCore.Html;
using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.Fields;

namespace Neo.Bpms.UI.MVC.Controls;

public class HighchartRenderer(ReportData reportInfo)
{
    private Dictionary<string, List<ReportRowInfo>> _groupByRecords;
    private IEnumerable<ColumnFieldDefinition> _aggrColumns;

    public bool YAxisIsTimespan => _aggrColumns?.Any(c =>
        c.FieldType == TVariableTypes.DurHourMinute || c.FieldType == TVariableTypes.DayHourMinute) ?? false;

    /// <summary>
    /// get chart Categories.
    /// </summary>
    /// <returns></returns>
    public HtmlString GetCategories()
    {
        _aggrColumns = [.. reportInfo.AggregationColumns];
        List<ColumnFieldDefinition> categoryFields = [.. reportInfo.InColumns.Where(col => !col.IsTooltip)];
        if (categoryFields.Count == 0)
            return new HtmlString("[]");
        _groupByRecords = [];
        int rowIndex = 0;
        foreach (ReportRowInfo row in reportInfo.Rows)
        {
            string groupByKey = CreateGroupByKey(categoryFields, row, "#", _aggrColumns.Count() <= 1);
            AddRowToGroupByRecord(groupByKey, row);
            row.Data.SetField("__rowIndex", rowIndex++);
        }

        return HtmlStringFromObject(_groupByRecords.Select(k =>
        {
            string[] keyItems = k.Key.Split('#');
            List<string> keyItemsDescriptions = [];
            for (int i = 0; i < categoryFields.Count; i++)
            {
                string description = i < keyItems.Length ? keyItems[i] : null;
                if (categoryFields[i].FieldType.In(TVariableTypes.Date, TVariableTypes.DateTime))
                {
                    DateTime? keyItemValue = k.Value[0].Data.GetNullableDateTime(categoryFields[i].ColumnTypeName);
                    description = keyItemValue?.ToHtmlInputValue(ProjectDefinition.Project.DefaultCalendar) ?? "";
                }

                keyItemsDescriptions.Add(description);
            }

            return string.Join(" ", keyItemsDescriptions);
        }));
    }

    private void AddRowToGroupByRecord(string groupByKey, ReportRowInfo row)
    {
        if (_groupByRecords.TryGetValue(groupByKey, out List<ReportRowInfo> groupByRecord))
            groupByRecord?.Add(row);
        else
            _groupByRecords.Add(groupByKey, [row]);
    }

    private static string CreateGroupByKey(IEnumerable<ColumnFieldDefinition> categoryFields, ReportRowInfo row,
        string seperator, bool checkOneItem)
    {
        string groupByKey = "";
        foreach (ColumnFieldDefinition categoryField in categoryFields)
        {
            groupByKey += string.IsNullOrEmpty(groupByKey) ? "" : seperator;
            if (GetValue(row, categoryField, out object obj))
                groupByKey += obj;
            else
                groupByKey += "-";
            //if (checkOneItem)//todo
            //	break;
        }

        return groupByKey;
    }

    private static bool GetValue(ReportRowInfo row, ColumnFieldDefinition columnField, out object obj)
    {
        obj = ReportRenderer.GetCelValue(columnField, row);
        return obj != null;
    }


    /// <summary>
    /// get YAxis names for Chart View report
    /// </summary>
    /// <returns></returns>
    public HtmlString GetYAxisName()
    {
        return _aggrColumns == null || !_aggrColumns.Any()
            ? new HtmlString("\"\"")
            : new HtmlString(JsonConvert.SerializeObject(string.Join(" - ",
            _aggrColumns.Where(ac => !ac.IsTooltip).Select(col => col.Alias))));
    }

    /// <summary>
    /// get chart series
    /// </summary>
    /// <returns></returns>
    public HtmlString GetSeries()
    {
        if (_aggrColumns == null || !_aggrColumns.Any())
            return new HtmlString("[]");
        if (_groupByRecords == null)
            return new HtmlString("[]");
        if (_aggrColumns.Count() > 1)
        {
            return HtmlStringFromObject(
                _aggrColumns.Where(a => !a.IsTooltip)
                    .Select(a => SeryAccordingToAggregationColumn(a.Alias, a))
            );
        }

        List<ColumnFieldDefinition> inColumns = reportInfo.InColumns.ToList();
        ColumnFieldDefinition aggrColumn = _aggrColumns.FirstOrDefault();
        if (inColumns.Count == 0 || aggrColumn == null)
            return new HtmlString("[]");
        if (inColumns.Count == 1)
        {
            return HtmlStringFromObject(new[]
            {
                SeryAccordingToAggregationColumn(aggrColumn.Alias,
                    aggrColumn)
            });
        }

        List<ColumnFieldDefinition> secondInColumns = inColumns.ToList();
        secondInColumns.RemoveAt(0);
        Dictionary<string, object> groupByRecordInColumnsRecords = [];
        foreach (ReportRowInfo row in reportInfo.Rows)
        {
            string groupByKey = CreateGroupByKey(secondInColumns, row, ",", false);
            if (!groupByRecordInColumnsRecords.ContainsKey(groupByKey))
                groupByRecordInColumnsRecords.Add(groupByKey, null);
        }

        return HtmlStringFromObject(
            groupByRecordInColumnsRecords.Keys.Select(gbrIcr => new
            {
                name = gbrIcr,
                fieldName = string.Join(" ", secondInColumns.Select(col => col.Alias)),
                data = _groupByRecords.Select(gbr =>
                {
                    ReportRowInfo rowInfo =
                        (from row in gbr.Value
                         let groupByKey = CreateGroupByKey(secondInColumns, row, ",", false)
                         where groupByKey == gbrIcr
                         select row).FirstOrDefault();
                    return rowInfo != null ? GetDoubleValue(aggrColumn, rowInfo) : 0;
                }),
                rowIdsIndex = _groupByRecords.Select(gbr =>
                {
                    ReportRowInfo rowInfo =
                        (from row in gbr.Value
                         let groupByKey = CreateGroupByKey(secondInColumns, row, ",", false)
                         where groupByKey == gbrIcr
                         select row).FirstOrDefault();
                    return rowInfo?.Data.GetLong("__rowIndex") ?? -1;
                })
            })
        );
    }

    private object SeryAccordingToAggregationColumn(string seryName, ColumnFieldDefinition aggrColumn)
    {
        return new
        {
            name = seryName,
            data = _groupByRecords.Values
                .Select(gbr =>
                    GetDoubleValue(aggrColumn, gbr.FirstOrDefault())
                ) //why first?
        };
    }

    private static double GetDoubleValue(ColumnFieldDefinition aggr, ReportRowInfo data)
    {
        double d = 0;
        if (data != null && GetValue(data, aggr, out object obj))
            double.TryParse(obj.ToString().Replace("/", "."), out d);
        return d;
    }

    private HtmlString HtmlStringFromObject(object o) => new(JsonConvert.SerializeObject(o));
}
