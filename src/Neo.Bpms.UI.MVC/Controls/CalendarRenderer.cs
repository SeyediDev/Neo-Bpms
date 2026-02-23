using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using System.Globalization;

namespace Neo.Bpms.UI.MVC.Controls;

public class CalendarRenderer(ReportData reportInfo, string calendar)
{
    /// <summary>
    /// Normalized calendar: "shamsi" or "gregorian" (miladi is mapped to gregorian).
    /// </summary>
    private readonly string _calendar = NormalizeCalendar(calendar ?? "shamsi");

    private static string NormalizeCalendar(string calendar)
    {
        if (string.IsNullOrEmpty(calendar)) return "shamsi";
        return calendar.Equals("miladi", StringComparison.OrdinalIgnoreCase) ? "gregorian" : calendar.ToLowerInvariant();
    }
    
    public class CalendarDayData
    {
        public DateTime Date { get; set; }
        public string PersianDate { get; set; }
        public string GregorianDate { get; set; }
        public int DayOfMonth { get; set; }
        public string DayOfWeek { get; set; }
        public List<CalendarCardItem> Items { get; set; } = [];
        public bool HasData => Items.Count > 0;
        public double? PrimaryValue { get; set; }
        public string PrimaryValueFormatted { get; set; }
    }

    public class CalendarCardItem
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string Type { get; set; } // "aggregation", "dimension", "column"
        public int Order { get; set; }
    }

    public class MonthSummary
    {
        public string MonthName { get; set; }
        public int Year { get; set; }
        public int TotalDaysWithData { get; set; }
        public double? TotalPrimaryValue { get; set; }
        public string TotalPrimaryValueFormatted { get; set; }
        public double? AverageValue { get; set; }
        public string AverageValueFormatted { get; set; }
        public double? MaxValue { get; set; }
        public string MaxValueFormatted { get; set; }
        public double? MinValue { get; set; }
        public string MinValueFormatted { get; set; }
    }

    public List<CalendarDayData> GetCalendarData(int year, int month)
    {
        var result = new List<CalendarDayData>();
        var pc = new PersianCalendar();
        
        DateTime startDate, endDate;
        
        if (_calendar == "shamsi")
        {
            startDate = pc.ToDateTime(year, month, 1, 0, 0, 0, 0, pc.PersianEra);
            int daysInMonth = pc.GetDaysInMonth(year, month);
            endDate = pc.ToDateTime(year, month, daysInMonth, 23, 59, 59, 0, pc.PersianEra);
        }
        else
        {
            startDate = new DateTime(year, month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
        }

        // Get date column (first GroupBy column)
        var dateColumn = reportInfo.Structure.SelectedColumns
            .FirstOrDefault(c => c.aggrType == eAggregationFunctions.GroupByItem && 
                                (c.FieldType == TVariableTypes.Date || c.FieldType == TVariableTypes.DateTime));

        if (dateColumn == null)
        {
            // Try to find any date field
            dateColumn = reportInfo.Structure.SelectedColumns
                .FirstOrDefault(c => c.FieldType == TVariableTypes.Date || c.FieldType == TVariableTypes.DateTime);
        }

        if (dateColumn == null)
            return result;

        // Get aggregation columns
        var aggregationColumns = reportInfo.Structure.SelectedColumns
            .Where(c => c.aggrType != eAggregationFunctions.GroupByItem && 
                       c.aggrType != eAggregationFunctions.InColumn &&
                       !c.IsTooltip)
            .ToList();

        var primaryAggregation = aggregationColumns.FirstOrDefault();

        // Get dimension columns (InColumn)
        var dimensionColumns = reportInfo.Structure.SelectedColumns
            .Where(c => c.aggrType == eAggregationFunctions.InColumn)
            .ToList();

        // Group data by date
        var groupedByDate = new Dictionary<DateTime, List<ReportRowInfo>>();
        
        foreach (var row in reportInfo.Rows)
        {
            var dateValue = row.Data.GetNullableDateTime(dateColumn.ColumnTypeName);
            if (dateValue.HasValue)
            {
                var dateKey = dateValue.Value.Date;
                if (!groupedByDate.ContainsKey(dateKey))
                    groupedByDate[dateKey] = [];
                groupedByDate[dateKey].Add(row);
            }
        }

        // Generate all days in month
        var currentDate = startDate;
        while (currentDate <= endDate)
        {
            var dayData = new CalendarDayData
            {
                Date = currentDate,
                GregorianDate = currentDate.ToString("yyyy-MM-dd"),
                PersianDate = _calendar == "shamsi" 
                    ? $"{pc.GetYear(currentDate)}/{pc.GetMonth(currentDate):D2}/{pc.GetDayOfMonth(currentDate):D2}"
                    : currentDate.ToString("yyyy/MM/dd"),
                DayOfMonth = _calendar == "shamsi" ? pc.GetDayOfMonth(currentDate) : currentDate.Day,
                DayOfWeek = GetDayOfWeekName(currentDate)
            };

            if (groupedByDate.TryGetValue(currentDate, out var rows))
            {
                int order = 0;
                
                // Add dimension values
                foreach (var dimCol in dimensionColumns)
                {
                    if (dimCol.ColumnName == dateColumn.ColumnName) continue;
                    foreach (var row in rows)
                    {
                        var value = ReportRenderer.GetCelValue(dimCol, row);
                        if (value != null)
                        {
                            dayData.Items.Add(new CalendarCardItem
                            {
                                Label = dimCol.Alias,
                                Value = value.ToString(),
                                Type = "dimension",
                                Order = order++
                            });
                        }
                    }
                }

                // Add aggregation values
                foreach (var aggrCol in aggregationColumns)
                {
                    double totalValue = 0;
                    int count = 0;
                    
                    foreach (var row in rows)
                    {
                        var value = ReportRenderer.GetCelValue(aggrCol, row);
                        if (value != null && double.TryParse(value.ToString(), out double dVal))
                        {
                            totalValue += dVal;
                            count++;
                        }
                    }

                    if (count > 0)
                    {
                        var avgValue = aggrCol.aggrType == eAggregationFunctions.Avg 
                            ? totalValue / count 
                            : totalValue;
                            
                        dayData.Items.Add(new CalendarCardItem
                        {
                            Label = aggrCol.Alias,
                            Value = FormatNumber(avgValue),
                            Type = "aggregation",
                            Order = order++
                        });

                        if (aggrCol == primaryAggregation)
                        {
                            dayData.PrimaryValue = avgValue;
                            dayData.PrimaryValueFormatted = FormatNumber(avgValue);
                        }
                    }
                }
            }

            result.Add(dayData);
            currentDate = currentDate.AddDays(1);
        }

        return result;
    }

    public MonthSummary GetMonthSummary(int year, int month, List<CalendarDayData> data)
    {
        var daysWithData = data.Where(d => d.HasData).ToList();
        
        var summary = new MonthSummary
        {
            MonthName = GetMonthName(month),
            Year = year,
            TotalDaysWithData = daysWithData.Count
        };

        if (daysWithData.Any(d => d.PrimaryValue.HasValue))
        {
            var values = daysWithData.Where(d => d.PrimaryValue.HasValue).Select(d => d.PrimaryValue.Value).ToList();
            
            summary.TotalPrimaryValue = values.Sum();
            summary.TotalPrimaryValueFormatted = FormatNumber(values.Sum());
            
            summary.AverageValue = values.Average();
            summary.AverageValueFormatted = FormatNumber(values.Average());
            
            summary.MaxValue = values.Max();
            summary.MaxValueFormatted = FormatNumber(values.Max());
            
            summary.MinValue = values.Min();
            summary.MinValueFormatted = FormatNumber(values.Min());
        }

        return summary;
    }

    /// <summary>
    /// Returns current month (year, month) in the active calendar type.
    /// Used as default when no date is specified.
    /// </summary>
    public (int Year, int Month) GetCurrentMonth()
    {
        var pc = new PersianCalendar();
        var now = DateTime.Now;
        if (_calendar == "shamsi" || _calendar == "gregorian" || _calendar == "miladi")
        {
            return _calendar == "shamsi"
                ? (pc.GetYear(now), pc.GetMonth(now))
                : (now.Year, now.Month);
        }
        return (pc.GetYear(now), pc.GetMonth(now));
    }

    private string GetDayOfWeekName(DateTime date)
    {
        var dayNames = _calendar == "shamsi"
            ? new[] { "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه" }
            : new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        
        return dayNames[(int)date.DayOfWeek];
    }

    private string GetMonthName(int month)
    {
        if (_calendar == "shamsi")
        {
            var persianMonths = new[] { "", "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
            return month >= 1 && month <= 12 ? persianMonths[month] : "";
        }
        var gregorianMonths = new[] { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        return month >= 1 && month <= 12 ? gregorianMonths[month] : "";
    }

    private string FormatNumber(double value)
    {
        var hasFraction = Math.Abs(value % 1) > double.Epsilon;
        var format = hasFraction ? "#,0.00" : "#,0";
        return value.ToString(format, CultureInfo.InvariantCulture);
    }
}

public static class CalendarHelpers
{
    public static HtmlString ToCalendarJson(this List<CalendarRenderer.CalendarDayData> data)
    {
        return new HtmlString(JsonConvert.SerializeObject(data));
    }

    public static HtmlString ToMonthSummaryJson(this CalendarRenderer.MonthSummary summary)
    {
        return new HtmlString(JsonConvert.SerializeObject(summary));
    }
}
