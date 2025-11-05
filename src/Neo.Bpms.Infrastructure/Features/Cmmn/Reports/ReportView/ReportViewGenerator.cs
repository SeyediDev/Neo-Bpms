using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports.ReportView;

public abstract class ReportViewGenerator
{
    public CancellationToken CancellationToken { get; set; }
    public CommonFormStructure Structure { get; set; }
    public List<ColumnFieldDefinition> ColumnList { get; set; }
    public string Culture { get; set; }
    public string Calendar { get; set; }
    protected int RowIndex { get; set; }

    protected ReportViewGenerator(CancellationToken cancellationToken,
        CommonFormStructure structure, List<ColumnFieldDefinition> columnList,
        string culture, string calendar)
    {
        CancellationToken = cancellationToken;
        Structure = structure;
        ColumnList = columnList;
        Culture = culture;
        Calendar = calendar;
        Content = null;
    }

    public void Export(IList<ReportRowInfo> rowList, ElasticObject totalRecord)
    {
        Init();
        GenerateHeader();
        GenerateTotalRow(totalRecord);
        GenerateRows(rowList);
        GenerateFooter();
        Release();
    }

    public virtual object Content { get; }

    public virtual void Init()
    {
        RowIndex = 0;
    }

    public virtual void Release()
    {
    }

    public virtual void GenerateTotalRow(ElasticObject totalRecord)
    {
    }

    public virtual void GenerateHeader()
    {
    }

    public virtual void GenerateRows(IList<ReportRowInfo> rowList)
    {
        GenerateRowsHeader(rowList);
        foreach (ReportRowInfo row in rowList)
        {
            if (CancellationToken.IsCancellationRequested)
                break;
            GenerateRow(rowList, row);
        }

        GenerateRowsFooter(rowList);
    }

    protected virtual void GenerateRowsHeader(IList<ReportRowInfo> rowList)
    {
    }

    protected virtual void GenerateRowsFooter(IList<ReportRowInfo> rowList)
    {
    }

    public virtual void GenerateFooter()
    {
    }

    protected virtual void GenerateRow(IList<ReportRowInfo> rowList, ReportRowInfo row)
    {
        RowIndex++;
        GenerateRowHeader(row);
        GenerateRowIndex(rowList);
        foreach (ColumnFieldDefinition column in ColumnList)
        {
            GenerateRowColumn(column, row);
        }

        GenerateRowFooter(row);
    }

    protected virtual void GenerateRowHeader(ReportRowInfo row)
    {
    }

    protected abstract void GenerateRowIndex(IList<ReportRowInfo> rowList);

    protected virtual void GenerateRowFooter(ReportRowInfo row)
    {
    }

    protected abstract void GenerateRowColumn(ColumnFieldDefinition column, ReportRowInfo row);

    public string FetchColumnValue(ColumnFieldDefinition column, ReportRowInfo row)
    {
        object value = ReportRenderer.GetCelValue(column, row);
        string val = CreateCellElem(value, column.FieldType, column);
        val = NormalizeCellValue(val);
        return val;
    }

    protected virtual string NormalizeCellValue(string val)
    {
        return val;
    }

    private string CreateCellElem(object value, TVariableTypes cellType,
        InputFieldDefinition cellInfo)
    {
        object str = value ?? "";
        switch (cellType)
        {
            case TVariableTypes.Double:
                try
                {
                    if (value is DateTime)
                        return CreateCellElem(value, TVariableTypes.DateTime, cellInfo);
                    str = double.TryParse(value?.ToString(), out double v)
                        ? DoubleElementValue(v)
                        : value?.ToString();
                }
                catch
                {
                    str = value?.ToString();
                }

                break;
            case TVariableTypes.BOOL:
                str = CreateBoolElement(cellInfo, value);
                break;
            case TVariableTypes.DayHourMinute:
            case TVariableTypes.DurHourMinute:
                if (!string.IsNullOrEmpty(str.ToString().Trim()))
                {
                    str = FormDataRoutines.GetTimeSpanDisplayValue(str);
                }

                break;
            case TVariableTypes.Date:
            case TVariableTypes.DateStr:
            case TVariableTypes.DateTime:
                UIComponentProperty calProp = cellInfo.GetProperty(eControlPropertyId.Calender);
                if (calProp != null)
                    Calendar = calProp.Value?.ToString();
                str = CreateDateTimeElement(str, cellInfo);
                break;
        }

        return str?.ToString();
    }

    private object CreateDateTimeElement(object str, FormFieldDefinition cellInfo)
    {
        if (!string.IsNullOrEmpty(str.ToString().Trim()))
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                dt = Convert.ToDateTime(str);
            }
            catch
            {
                // ignored
            }

            if (dt.Year > 1900)
            {
                string dStr;
                if (Calendar == "shamsi")
                {
                    PersianCalendar pc = new();
                    dStr = pc.GetYear(dt).ToString() + '/' + Get2Digit(pc.GetMonth(dt)) + '/' +
                           Get2Digit(pc.GetDayOfMonth(dt));
                }
                else
                {
                    dStr = dt.Year.ToString() + '/' + dt.Month.ToString("d2") + '/' + dt.Day.ToString("d2");
                }

                if (dt.Hour != 0 || dt.Minute != 0 || dt.Second != 0)
                {
                    dStr += " " + dt.Hour + ":" + dt.Minute;
                    if (cellInfo?.PropertyValue(eControlPropertyId.TimeDisplayResolution)?.Equals("s") ?? false
                    ) //todo s is :/
                        dStr += $":{dt.Second}";
                }

                str = dStr;
            }
            else
            {
                str = "";
            }
        }

        return str;
    }

    private object CreateBoolElement(InputFieldDefinition col, object value)
    {
        if (col.ControlType == eControlTypeId.BooleanCombo)
        {
            BooleanItem valueItem = BooleanEntityField.GetValueItem(false, value);
            return valueItem switch
            {
                BooleanItem.True => col.PropertyValue(eControlPropertyId.TrueTitle) ?? col.Alias,
                BooleanItem.False => col.PropertyValue(eControlPropertyId.FalseTitle) ?? "",
                BooleanItem.Null => col.PropertyValue(eControlPropertyId.NullTitle) ?? "-",
                _ => "",
            };
        }

        string str = value?.ToString() ?? "";
        if (!string.IsNullOrEmpty(str.Trim()))
            str = BoolElementValue(ConvUtill.ToBoolean(value));

        return str;
    }

    protected virtual string BoolElementValue(bool boolValue)
    {
        return boolValue ? "بلی" : "خیر";
    }

    protected virtual string DoubleElementValue(double v)
    {
        return Math.Abs(v - Math.Floor(v)) < .0001 ? $"{v:n0}" : $"{v:n2}";
    }

    private static string Get2Digit(int d)
    {
        return d < 10 ? "0" + d : d.ToString();
    }
}
