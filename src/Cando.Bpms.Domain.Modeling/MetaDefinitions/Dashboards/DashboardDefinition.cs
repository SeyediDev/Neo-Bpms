using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Dashboards;

public abstract class DashboardDefinition : FormDefinition
{
    #region dashboard

    /// <summary>
    /// Define Dashboard
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="enName">enName</param>
    /// <returns></returns>
    protected Dashboard DefineDashboard(string name, string enName = null)
    {
        dashboard = new Dashboard(entity, GetType().Name, name, enName ?? GetType().Name);
        form = dashboard;
        return dashboard;
    }

    /// <summary>
    /// Add Report to the dashboard
    /// </summary>
    /// <typeparam name="T">Report</typeparam>
    /// <returns></returns>
    public bool AddReport<TEntity, TReport>()
    {
        string reportNamespaceId = EntityDefinition.GetNamespaceName<TEntity>();
        string reportEntityId = EntityDefinition.GetEntityName<TEntity>();
        string reportId = typeof(TReport).Name;
        return dashboard.AddReport(reportNamespaceId, reportEntityId, reportId);
    }
    
    public bool AddReport<TEntity>()
    {
        string reportNamespaceId = EntityDefinition.GetNamespaceName<TEntity>();
        string reportEntityId = EntityDefinition.GetEntityName<TEntity>();
        return dashboard.AddReport(reportNamespaceId, reportEntityId, nameof(CRUDDefinition.PublicReport));
    }

    /// <summary>
    /// Add Report to the dashboard
    /// </summary>
    /// <param name="reportId">field</param>
    /// <returns></returns>
    public bool AddReport(string reportId)
    {
        return dashboard.AddReport(entity.NamespaceId, entity.Id, reportId);
    }

    /// <summary>
    /// Add Report to the dashboard
    /// </summary>
    /// <param name="entityId">entityId</param>
    /// <param name="reportId">field</param>
    /// <param name="namespaceId">namespaceId</param>
    /// <returns></returns>
    public bool AddReport(string namespaceId, string entityId, string reportId)
    {
        return dashboard.AddReport(namespaceId, entityId, reportId);
    }
    #endregion dashboard

    #region configuration
    protected Dashboard dashboard;
    protected ConfiguredDashboard dashboardConfig;
    #endregion configuration


    /// <summary>
    /// Define All
    /// </summary>
    /// <param name="uiEntity">entity</param>
    /// <returns></returns>
    public Dashboard DefineDashboard(UiEntity uiEntity, EntityDefinition definition)
    {
        entityDefinition = definition;
        entity = uiEntity;
        dashboard = (Dashboard)Identify();
        if (dashboard == null) return null;
        entity.AddDashboard(dashboard);

        Filters();
        if (!DefineUIRules()) return null;
        UIRules();
        DataSources();
        DefineConfigs();
        return dashboard;
    }

    /// <summary>
    /// Define Dashboard Data Sources
    /// </summary>
    /// <returns></returns>
    protected virtual void DataSources()
    {
    }

    /// <summary>
    /// Define Configs
    /// </summary>
    /// <returns></returns>
    private void DefineConfigs()
    {
        foreach (var configDefinition in ExtractSubsInstances<DashboardConfigDefinition>())
        {
            configDefinition?.Define(dashboard);
        }
    }
}
