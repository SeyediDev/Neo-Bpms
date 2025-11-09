namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports.ReportView;

public class ReportToCSVGenerator : ReportViewGenerator
{
    public ReportToCSVGenerator(CancellationToken cancellationToken, CommonFormStructure structure,
        List<ColumnFieldDefinition> columnList, string culture, string calendar) :
        base(cancellationToken, structure, columnList, culture, calendar)
    {
    }

    public override object Content => _text.ToString();
    private NeoStringBuilder _text;
    private const char Discriminator = ',';
    private const string NewLine = "\n";

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
        _text += "Index" + Discriminator;
        foreach (ColumnFieldDefinition column in ColumnList)
        {
            _text += column.Label + Discriminator;
        }

        _text += NewLine;
    }

    public override void GenerateTotalRow(ElasticObject totalRecord)
    {
        ReportRowInfo totalRow = new() { Data = totalRecord };
        _text += "Total" + Discriminator;
        foreach (ColumnFieldDefinition column in ColumnList)
        {
            string value = FetchColumnValue(column, totalRow);
            _text += value + Discriminator;
        }

        _text += NewLine;
    }

    protected override void GenerateRowIndex(IList<ReportRowInfo> rowList)
    {
        _text += RowIndex.ToString() + Discriminator;
    }

    protected override void GenerateRowColumn(ColumnFieldDefinition column, ReportRowInfo row)
    {
        _text += FetchColumnValue(column, row) + Discriminator;
    }

    protected override void GenerateRowFooter(ReportRowInfo row)
    {
        _text += NewLine;
    }

    protected override string BoolElementValue(bool boolValue)
    {
        return boolValue ? "Yes" : "No";
    }

    protected override string DoubleElementValue(double v)
    {
        return Math.Abs(v - Math.Floor(v)) < .0001 ? $"{v:f0}" : $"{v:f2}";
    }

    protected override string NormalizeCellValue(string val)
    {
        return val?.Replace(",", "");
    }
}