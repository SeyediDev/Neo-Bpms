using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Domain.Entities.Cmmn.UI;

public class Dashboard : Form
{
    public Dashboard()
    {
    }
    /// <summary>
    /// Dashboard
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="id">id</param>
    /// <param name="name">name</param>
    /// <param name="enName"></param>
    /// <returns></returns>
    public Dashboard(UiEntity entity, string id, string name, string enName)
        : base(entity, id, name, enName, eFormType.Dashboard)
    {
    }

    public List<DashboardReport> Reports { get; set; } = [];
    [XmlIgnore]
    public Dictionary<string, ConfiguredDashboard> MetaConfigures { get; set; } = [];

    public class DashboardReport
    {
        public string NamespaceId { get; set; }
        public string EntityId { get; set; }
        public string ReportId { get; set; }
    }

    public bool AddReport(string namespaceId, string entityId, string reportId)
    {
        Reports.Add(new DashboardReport
        {
            NamespaceId = namespaceId,
            EntityId = entityId,
            ReportId = reportId,
        });
        return true;
    }
}
