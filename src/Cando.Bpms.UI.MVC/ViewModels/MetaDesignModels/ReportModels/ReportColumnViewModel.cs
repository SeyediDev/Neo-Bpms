namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;

public class ReportColumnViewModel
{
    public ReportColumnViewModel()
    {

    }
    public ReportColumnViewModel(Report.Field field)
    {
        id = field.fieldId;
        name = field.alias;
        formula = field.formula;
        asColumn = field.asColumn;
        asAggregation = field.asAggregation;
        asGroupBy = field.asGroupBy;
        properties = field.properties?.Select(p => new ReportColumnPropertyViewModel(p)).ToList();
    }

    public string id { get; set; }
    public string name { get; set; }
    public string formula { get; set; }
    public bool asColumn { get; set; }
    public bool asAggregation { get; set; }
    public bool asGroupBy { get; set; }
    public List<ReportColumnPropertyViewModel> properties { get; set; }

    public Report.Field ToReportField(Report report)
    {
        return new Report.Field
        {
            fieldId = id,
            alias = name,
            formula = formula,
            asColumn = asColumn,
            asAggregation = asAggregation,
            asGroupBy = asGroupBy,
            properties = properties?.Select(p => p.ToFieldPropery()).ToList()
        };
    }
}
