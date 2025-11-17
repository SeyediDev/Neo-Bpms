using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Reports;

public abstract class ChartConfigDefinition(ChartType chartType) : GroupByConfigDefinition
{
    protected override ReportViewType ViewType => ReportViewType.Chart;
    protected virtual string ChartAdvancedOptions { get; }
    protected override void DefineExtra()
    {
        base.DefineExtra();
        reportConfig.ChartType = chartType;
        if (!string.IsNullOrEmpty(ChartAdvancedOptions))
        {
            reportConfig.AddProperty(ReportConfigProperty.ChartAdvancedOptions, ChartAdvancedOptions);
        }
    }
}
public abstract class BpmnDiagramDefinition() : ChartConfigDefinition(ChartType.BpmnDiagram)
{
}
