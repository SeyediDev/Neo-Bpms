namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports.ReportView;

public class ReportToHtmlGenerator : ReportViewGenerator
{
    public ReportToHtmlGenerator(CancellationToken cancellationToken, CommonFormStructure structure,
        List<ColumnFieldDefinition> columnList, string culture, string calendar) :
        base(cancellationToken, structure, columnList, culture, calendar)
    {
    }

    public override object Content => _text.ToString();
    private NeoStringBuilder _text;

    public override void Init()
    {
        base.Init();
        _text = new NeoStringBuilder();
    }

    public override void Release()
    {
        base.Release();
        _text = null;
    }

    public override void GenerateHeader()
    {
        _text += "<meta charset=\"utf-8\"/>";
        _text +=
            @"<style>body,div,p,table{direction: rtl;font-family: Calibri, Tahoma;}table {border-spacing: 0;border-collapse: collapse;width: 100%;max-width: 100 %;margin-bottom: 20px;}td,th {padding: 8px;line-height: 1.42857143;vertical-align: top;border-top: 1px solid #ddd;}th {vertical-align: bottom;border-bottom: 2px solid #ddd;border-top: 0;}header, th, td {text-align: center;}div {padding-right: 15px;padding-left: 15px;margin-right: auto;margin-left: auto;}
    .reportRow {
    margin-left: 0px;
    margin-right: 0px;
}
.reportHeader {
    text-align: center;
    vertical-align: middle;
    display: table;
    padding: 5px 0px 5px 0px;
    height: 78px;
}
.headerIcon {
    width: 140px;
    height: 39px;
}
.tblCell {
    vertical-align: middle;
    display: table-cell;
}
.valueSpan {
    color: #063E6D;
    vertical-align: middle;
}
.titleSpan {
    color: #387CB4;
}
.reportBody {
    padding: 10px;
}
.customDiv {
    box-shadow: 0px 0px 10px #888888;
    border-bottom-left-radius: 5px;
    border-bottom-right-radius: 5px;
    padding: 8px !important;
    overflow: hidden;
}
.table-responsive {
    min-height: .01%;
    overflow-x: auto;
}
.pull-right {
    float: right!important;
}
.table {
    width: 100%;
    max-width: 100%;
    margin-bottom: 20px;
}

.reportTbl th {
	border-left: 2px solid #cac2c2 !important;
	border-right: 2px solid #cac2c2 !important;
	transition: all 200ms;
	color: #0e71b9;
}

.reportTbl th:hover { border-bottom: 2px solid #e12323; }

.reportTbl tr {
	border: none;
	border-top: 2px solid black;
}

.reportTbl td {
	border-left: 2px solid #cac2c2;
	border-right: 2px solid #cac2c2;
}

.reportTbl tr:nth-child(even) {
	background-color: #F6F8FE;
	transition: all 200ms;
	border-bottom: 2px solid transparent;
}

.reportTbl tr:nth-child(odd) {
	background-color: #fff;
	transition: all 200ms;
	border-bottom: 2px solid transparent;
}

.reportTbl tr:last-child { border-bottom: 2px solid black; }

.reportTbl tr:nth-child(n + 1):hover {
	background: #E5EAFB;
}

.reportTbl tr:nth-child(n + 1):hover table tr th button span { color: black !important; }

.reportTbl tr:nth-child(n + 1):hover table tr th {
	background-color: #c3c7c7 !important;
	border-left: 2px solid #a19c9c !important;
	border-right: 2px solid #a19c9c !important;
}
.reportTbl tr:nth-child(n + 1):hover button span {
	color: white !important;
	opacity: 1;
}
.cColumn {
    text-align: center;
    direction: rtl;
    opacity: 1;
    transition: all 200ms;
    font-size: 13px;
    cursor: default;
}
.table-col-value {
    font-size: 12px;
}
</style>
</head>
<div class=' reportRow' id='ReportContainer'>
    <div class='col-md-12 col-sm-12 col-xs-12 noPadding'>
        <div class='customDiv'>
            <div class='row reportRow reportHeaderBorder' id='ReportHeader'>
                <div class='col-md-12 col-sm-12 col-xs-12'>
                    <div class='row' style='margin: auto 0px;'>
                        <div class='col-md-6 col-sm-6 col-xs-12 reportHeader'>";
        DateTime dt = DateTime.UtcNow; //todo
        string dStr = "";
        string tStr = "";
        if (dt.Year > 1900)
        {
            if (Calendar == "shamsi")
            {
                PersianCalendar pc = new();
                dStr = pc.GetYear(dt).ToString() + '/' + pc.GetMonth(dt) + '/' + pc.GetDayOfMonth(dt);
            }
            else
            {
                dStr = dt.Year.ToString() + '/' + dt.Month + '/' + dt.Day;
            }

            tStr = dt.Hour.ToString() + ':' + dt.Minute;
        }

        _text += " <div class='tblCell' style='text-align: right;'>" +
                $"<span class='valueSpan'>{Structure.Name}</span>" +
                @"</div>
                </div>
                <div class='col-md-2 col-sm-2 col-xs-12 col-md-push-1 reportHeader'>
                <div class='tblCell' style='text-align: right;'>" +
                $"<span class='titleSpan'>تاریخ گزارش: </span><span class='valueSpan'>{dStr}</span>" +
                $"<br><span class='titleSpan'>ساعت گزارش: </span><span class='valueSpan'>{tStr}</span>" +
                //"<br><span class='titleSpan'>زمان گزارش‌گیری: </span><span class='valueSpan'></span>" +
                "</div></div></div></div></div>";


        _text += @"<div class='col-md-12 col-sm-12 col-xs-12 reportBody'>
                <div>
                <div class='col-md-12'>
                <div class='table-responsive'>" +
                $"<table key='{Structure.ConfigId}' class='table pull-right nowrap reportTbl' cellspacing='0' style='width: 100%; font-size: xx-small; border-radius: 5px;'>";
        _text += "<thead><tr>";
        _text += "<th style='text-align: center; min-width: 40px; direction: rtl; width: 20px;' class='cColumn'>" +
                "ردیف" + "</th>";
        foreach (ColumnFieldDefinition column in ColumnList)
        {
            _text +=
                "<th data-colname='Title' style='text-align: center; padding: 5px 0px 0px 0px; min-width: 40px; direction: rtl;' class='cColumn'>"
                + column.Label + "</th>";
        }

        _text += "</tr></thead><tbody>";
    }

    public override void GenerateTotalRow(ElasticObject totalRecord)
    {
        _text += "<tr>";
        _text += "<th style='text-align: center; min-width: 40px; direction: rtl; width: 20px;' class='cColumn'>" +
                 CulturalTexts.Total + "</th>";
        ReportRowInfo totalRow = new() { Data = totalRecord };
        foreach (ColumnFieldDefinition column in ColumnList)
        {
            string value = FetchColumnValue(column, totalRow);
            _text +=
                "<th data-colname='Title' style='text-align: center; padding: 5px 0px 0px 0px; min-width: 40px; direction: rtl;' class='cColumn'>"
                + value + "</th>";
        }

        _text += "</tr>";
    }

    protected override void GenerateRowHeader(ReportRowInfo row)
    {
        _text += "<tr sreportcount='2' >";
    }

    protected override void GenerateRowIndex(IList<ReportRowInfo> rowList)
    {
        _text += "<td style='text-align: center; direction: rtl;'>" + RowIndex + "</td>";
    }

    protected override void GenerateRowColumn(ColumnFieldDefinition column, ReportRowInfo row)
    {
        string val = FetchColumnValue(column, row);
        _text += $"<td><span class='table-col-value' title=''>{val}</span></td>";
    }

    protected override void GenerateRowFooter(ReportRowInfo row)
    {
        _text += "</tr>";
    }

    public override void GenerateFooter()
    {
        _text += "</tbody></table>";
        _text += "</body></html></div></div></div></div></div></div></div></div></div>";
    }

    protected override string BoolElementValue(bool boolValue)
    {
        return "<input disabled=\"disabled\" type=\"checkbox\" " +
              (boolValue ? "checked" : "") + "/>";
    }
}
