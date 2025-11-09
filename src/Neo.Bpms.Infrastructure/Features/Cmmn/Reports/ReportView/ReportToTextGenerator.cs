namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports.ReportView;

public class ReportToTextGenerator : ReportViewGenerator
{
    public ReportToTextGenerator(CancellationToken cancellationToken, CommonFormStructure structure,
        List<ColumnFieldDefinition> columnList, string culture, string calendar)
        : base(cancellationToken, structure, columnList, culture, calendar)
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
    protected override void GenerateRowIndex(IList<ReportRowInfo> rowList)
    {
        if (rowList.Count > 1)
            _text += $"سطر {RowIndex} : \n";
    }

    protected override void GenerateRowColumn(ColumnFieldDefinition column, ReportRowInfo row)
    {
        string val = FetchColumnValue(column, row);
        _text += $"{column.Label} : {val}\n";
    }
}
