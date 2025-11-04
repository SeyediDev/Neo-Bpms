namespace Neo.Bpms.Domain.Entities.Cmmn.UI;

public partial class Report
{
    [Flags]
    public enum SubReportType
    {
        DrillDown = 0x1,
        SubReport = 0x2
    }
}
