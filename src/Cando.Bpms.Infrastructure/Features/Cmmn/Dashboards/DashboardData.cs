namespace Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;

public class DashboardData
{
    public DashboardConfigViewModel Structure { get; set; }
    public IList<ReportData> ReportsData { get; set; } = [];
    public long QueryTime => ReportsData.Sum(r => r.QueryInfo.QueryTime);
}
