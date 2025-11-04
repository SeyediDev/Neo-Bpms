using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Dashboards;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Reports;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Projects;

/// <summary>
/// Base class to define menus and their details. Menu definitions in the business are sub classes of this object.
/// </summary>
public abstract class MenuDefinition : BaseModelingDefinition
{
    public MenuItem currentMenuItem;
    public MenuItem currentParentMenuItem;

    /// <summary>
    /// Adds the menu.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="iconName">Name of the icon.</param>
    /// <param name="area">The area.</param>
    /// <param name="targetAddress">The target address.</param>
    /// <param name="targetPlace">The target place.</param>
    /// <param name="paramValues"></param>
    /// <returns></returns>
    protected MenuItem AddMenu(string name, string iconName, string area, string targetAddress, string targetPlace = null, params string[] paramValues)
    {
        return AddMenuWithEnName(null, name, iconName, area, targetAddress, targetPlace, paramValues);
    }
    /// <summary>
    /// Adds the menu.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="iconName">Name of the icon.</param>
    /// <param name="area">The area.</param>
    /// <param name="targetAddress">The target address.</param>
    /// <param name="targetPlace">The target place.</param>
    /// <param name="paramValues"></param>
    /// <returns></returns>
    protected MenuItem AddPublicMenu(string name, string iconName, string area, string targetAddress, string targetPlace = null, params string[] paramValues)
    {
        var item = AddMenuWithEnName(null, name, iconName, area, targetAddress, targetPlace, paramValues);
        item.IsPublic = true;
        return item;
    }
    protected MenuItem AddMenuWithEnName(string enName, string name, string iconName = null, string area = null, string targetAddress = null, string targetPlace = null, params string[] paramValues)
    {
        var id = currentParentMenuItem == null ? "root" : currentParentMenuItem.Id + "." + (currentParentMenuItem.subMenues?.Count ?? 0) + 1;
        currentMenuItem = new MenuItem(id, enName, name, iconName, area, targetAddress, targetPlace);
        currentBaseElement = currentMenuItem;
        currentParentMenuItem?.addSubMenu(currentMenuItem);
        if (paramValues != null)
        {
            foreach (var paramValue in paramValues)
            {
                AddMenuParameter(eMenuItemParameter.ParamValue, paramValue);
            }
        }
        return currentMenuItem;
    }

    /// <summary>
    /// Adds the menu for form.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="iconName">Name of the icon.</param>
    /// <param name="subjectId"></param>
    /// <param name="targetPlace">The target place.</param>
    /// <param name="paramValues"></param>
    /// <returns></returns>
    protected MenuItem AddMenu<TEntity, TEntityItem>(string name, string iconName = null,
         string subjectId = null, string targetPlace = null, params string[] paramValues)
    {
        return AddMenuEntityItem<TEntity, TEntityItem>(null, name, iconName,
            subjectId, targetPlace, paramValues);
    }
    protected MenuItem AddReport<TEntity>(string name=null, string iconName = null,
         string subjectId = null, string targetPlace = null, params string[] paramValues)
    {
        if(string.IsNullOrEmpty(name))
        {
            var entity = ProjectDefinition.Project.GetEntity<TEntity>();
            name = entity.Name;
            iconName = "madkha-menu";
        }
        return AddMenuEntityItem<TEntity, CRUDDefinition.PublicReport>(null, name, iconName,
            subjectId, targetPlace, paramValues);
    }
    protected MenuItem AddForm<TEntity>(string name = null, string iconName = null,
         string subjectId = null, string targetPlace = null, params string[] paramValues)
    {
        var entity = ProjectDefinition.Project.GetEntity<TEntity>();
        if (string.IsNullOrEmpty(name))
        {
            name = entity.Name;
            iconName = "madkha-menu";
        }
        return entity != null
             ? AddMenuForFormWithEnName(entity.EnName, name, iconName, entity.NamespaceId, entity.Id, null, subjectId, targetPlace, paramValues) : null;
    }
    protected MenuItem AddDashboard<TEntity>(string name, string iconName = null,
         string subjectId = null, string targetPlace = null, params string[] paramValues)
    {
        return AddMenuEntityItem<TEntity, CRUDDefinition.PublicDashboard>(null, name, iconName,
            subjectId, targetPlace, paramValues);
    }

    /// <summary>
    /// Adds the menu for form.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="iconName">Name of the icon.</param>
    /// <param name="subjectId"></param>
    /// <param name="targetPlace">The target place.</param>
    /// <param name="paramValues"></param>
    /// <returns></returns>
    protected MenuItem AddMenu<TEntity>(string name, string iconName = null,
         string subjectId = null, string targetPlace = null, params string[] paramValues)
    {
        var entity = ProjectDefinition.Project.GetEntity<TEntity>();
        return entity != null
             ? AddMenuForFormWithEnName(entity.EnName, name, iconName, entity.NamespaceId, entity.Id, null, subjectId, targetPlace, paramValues) : null;
    }

    protected MenuItem AddMenuEntityItem<TEntity, TEntityItem>(string enName, string name, string iconName = null,
         string subjectId_configId = null, string targetPlace = null, params string[] paramValues)
    {
        if (FindOutParametersByType<TEntity, TEntityItem>(out var namespaceId, out var entityId, out var id, out var type))
        {
            if (type == typeof(FormDefinition))
            {
                return AddMenuForFormWithEnName(enName, name, iconName, namespaceId,
                     entityId, id, subjectId_configId, targetPlace, paramValues);
            }
            if (type == typeof(ReportDefinition))
            {
                return AddMenuForReport(enName, name, iconName, namespaceId,
                     entityId, id, subjectId_configId, targetPlace, paramValues);
            }
            if (type == typeof(DashboardDefinition))
            {
                return AddMenuForDashboard(enName, name, iconName, namespaceId,
                     entityId, id, subjectId_configId, targetPlace, paramValues);
            }
        }
        var entity = ProjectDefinition.Project.GetEntity<TEntity>();
        return entity != null
             ? AddMenuForFormWithEnName(enName, name, iconName, entity.NamespaceId, entity.Id, id, subjectId_configId, targetPlace, paramValues) : null;
    }
    protected MenuItem AddReportConfig<TEntity, TConfig>(string name, string iconName = null,
        string targetPlace = null, params string[] paramValues)
    where TConfig : ReportConfigDefinition
    {
        return AddReportConfig<TEntity, TConfig>(null, name, iconName, targetPlace, paramValues);
    }
    protected MenuItem AddReportConfig<TEntity, TConfig>(string enName, string name, string iconName = null,
        string targetPlace = null, params string[] paramValues)
        where TConfig : ReportConfigDefinition
    {
        var configType = typeof(TConfig);
        var reportType = configType.DeclaringType;
        var entity = ProjectDefinition.Project.GetEntity<TEntity>();
        return AddMenuForReport(enName, name, iconName, entity.NamespaceId,
             entity.Id, reportType.Name, configType.Name, targetPlace, paramValues);
    }

    protected MenuItem AddMenuForPage<TEntity, TEntityItem>(string name, string iconName)
    {
        return AddMenuForPage<TEntity, TEntityItem>(null, name, iconName);
    }

    protected MenuItem AddMenuForPage<TEntity, TEntityItem>(string enName, string name, string iconName)
    {
        if (FindOutParametersByType<TEntity, TEntityItem>(out var namespaceId, out var entityId, out var id, out var type)
             && type == typeof(FormDefinition))
        {
            var menu = AddMenuForSomeForm(enName, name, iconName, namespaceId, entityId, id, "CustomPage");
            menu.addParameter(eMenuItemParameter.RouteName, "CustomPage");
            return menu;
        }
        throw new Exception("Invalid type for page");
    }

    /// <summary>
    /// Adds the menu for form.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="iconName">Name of the icon.</param>
    /// <param name="namespaceId">The namespace identifier.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="formId">The form identifier.</param>
    /// <param name="formSubjectId">The subject identifier.</param>
    /// <param name="targetPlace">The target place.</param>
    /// <param name="paramValues"></param>
    /// <returns></returns>
    protected MenuItem AddMenuForForm(string name, string iconName, string namespaceId,
         string entityId, string formId = null, string formSubjectId = null,
         string targetPlace = null, params string[] paramValues)
    {
        return AddMenuForSomeForm(null, name, iconName, namespaceId, entityId, formId, "Index", formSubjectId, targetPlace, paramValues);
    }
    protected MenuItem AddMenuForFormWithEnName(string enName, string name, string iconName, string namespaceId,
         string entityId, string formId = null, string formSubjectId = null,
         string targetPlace = null, params string[] paramValues)
    {
        return AddMenuForSomeForm(enName, name, iconName, namespaceId, entityId, formId, "Index", formSubjectId, targetPlace, paramValues);
    }

    private MenuItem AddMenuForSomeForm(string enName, string name, string iconName,
         string namespaceId, string entityId, string formId, string targetAddress,
         string formSubjectId = null, string targetPlace = null, string[] paramValues = null)
    {
        var menu = AddMenuWithEnName(enName, name, iconName, "Form", targetAddress, targetPlace, paramValues);
        menu.addParameter(eMenuItemParameter.EntityId, entityId);
        if (formId != null)
        {
            menu.addParameter(eMenuItemParameter.FormId, formId);
        }

        if (namespaceId != null)
        {
            menu.addParameter(eMenuItemParameter.NamespaceId, namespaceId);
        }

        if (formSubjectId != null)
        {
            menu.addParameter(eMenuItemParameter.FormSubjectId, formSubjectId);
        }

        return menu;
    }

    /// <summary>
    /// Adds the menu for edit form
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="iconName">Name of the icon.</param>
    /// <param name="namespaceId">The namespace identifier.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="formId">The form identifier.</param>
    /// <param name="formSubjectId">The subject identifier.</param>
    /// <param name="targetPlace">The target place.</param>
    /// <param name="paramValues"></param>
    /// <returns></returns>
    protected MenuItem AddMenuForEditForm(string name, string iconName, string namespaceId, string entityId, string formId = null, string formSubjectId = null, string targetPlace = null, params string[] paramValues)
    {
        return AddMenuForEditFormWithEnName(null, name, iconName, namespaceId, entityId, formId, formSubjectId, targetPlace, paramValues);
    }
    protected MenuItem AddMenuForEditFormWithEnName(string enName, string name, string iconName, string namespaceId, string entityId, string formId = null, string formSubjectId = null, string targetPlace = null, params string[] paramValues)
    {
        var menu = AddMenuWithEnName(enName, name, iconName, "Form", "Edit", targetPlace, paramValues);
        menu.addParameter(eMenuItemParameter.EntityId, entityId);
        if (formId != null)
        {
            menu.addParameter(eMenuItemParameter.FormId, formId);
        }
        if (namespaceId != null)
        {
            menu.addParameter(eMenuItemParameter.NamespaceId, namespaceId);
        }
        if (formSubjectId != null)
        {
            menu.addParameter(eMenuItemParameter.FormSubjectId, formSubjectId);
        }
        return menu;
    }
    protected MenuItem AddMenuForServieOperationForm(string enName, string name, string iconName, string namespaceId, string entityId, string formId = null, string formSubjectId = null, string targetPlace = null, params string[] paramValues)
    {
        var menu = AddMenuWithEnName(enName, name, iconName, "Form", "ServiceOperation", targetPlace, paramValues);
        menu.addParameter(eMenuItemParameter.EntityId, entityId);
        if (formId != null)
        {
            menu.addParameter(eMenuItemParameter.FormId, formId);
        }
        if (namespaceId != null)
        {
            menu.addParameter(eMenuItemParameter.NamespaceId, namespaceId);
        }
        if (formSubjectId != null)
        {
            menu.addParameter(eMenuItemParameter.FormSubjectId, formSubjectId);
        }
        return menu;
    }
    protected MenuItem AddMenuForServieOperationForm<TEntity, TEntityItem>(string name, string iconName)
    {
        if (FindOutParametersByType<TEntity, TEntityItem>(out var namespaceId, out var entityId, out var id, out var type))
        {
            if (type == typeof(FormDefinition))
            {
                return AddMenuForServieOperationForm(null, name, iconName, namespaceId, entityId, id);
            }
        }
        return null;
    }
    protected MenuItem AddMenuForServieOperationForm<TEntity, TEntityItem>(string enName, string name, string iconName)
    {
        if (FindOutParametersByType<TEntity, TEntityItem>(out var namespaceId, out var entityId, out var id, out var type))
        {
            if (type == typeof(FormDefinition))
            {
                return AddMenuForServieOperationForm(enName, name, iconName, namespaceId, entityId, id);
            }
        }
        return null;
    }

    /// <summary>
    /// Adds menu for process
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="iconName">icon name</param>
    /// <param name="processId">process id</param>
    /// <param name="targetPlace">target place</param>
    /// <param name="paramValues">param value</param>
    /// <returns></returns>
    protected MenuItem AddMenuForProcess(string name, string iconName, string processId, string targetPlace = null, params string[] paramValues)
    {
        return AddMenuForProcess(null, name, iconName, processId, targetPlace, paramValues);
    }
    protected MenuItem AddMenuForProcess(string enName, string name, string iconName, string processId, string targetPlace = null, params string[] paramValues)
    {
        var menu = AddMenuWithEnName(enName, name, iconName, "Process", "IndexWorkItems", targetPlace, paramValues);
        menu.addParameter(eMenuItemParameter.ProcessId, processId);
        menu.addParameter(eMenuItemParameter.PageType, "WorkItems");
        return menu;
    }
    /// <summary>
    /// Adds menu for process create task form
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="iconName">icon name</param>
    /// <param name="processId">process id</param>
    /// <param name="taskId">task id</param>
    /// <param name="targetPlace">target place</param>
    /// <param name="paramValues">param values</param>
    /// <returns></returns>
    protected MenuItem AddMenuForProcessTask(string name, string iconName, string processId, string taskId, string targetPlace = null, params string[] paramValues)
    {
        return AddMenuForProcessTask(null, name, iconName, processId, taskId, targetPlace, paramValues);
    }
    protected MenuItem AddMenuForProcessTask(string enName, string name, string iconName, string processId, string taskId, string targetPlace = null, params string[] paramValues)
    {
        var menu = AddMenuWithEnName(enName, name, iconName, "Process", "Form", targetPlace, paramValues);
        menu.addParameter(eMenuItemParameter.ProcessId, processId);
        menu.addParameter(eMenuItemParameter.TaskId, taskId);
        return menu;
    }

    /// <summary>
    /// Adds the menu for report.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="iconName">Name of the icon.</param>
    /// <param name="namespaceId">The namespace identifier.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="reportId">The report identifier.</param>
    /// <param name="configId">The configuration identifier.</param>
    /// <param name="targetPlace">The target place.</param>
    /// <param name="paramValues"></param>
    /// <returns></returns>
    protected MenuItem AddMenuForReport(string name, string iconName, string namespaceId, string entityId, string reportId, string configId = null, string targetPlace = null, params string[] paramValues)
    {
        return AddMenuForReport(null, name, iconName, namespaceId, entityId, reportId, configId, targetPlace, paramValues);
    }
    protected MenuItem AddMenuForReport(string enName, string name, string iconName, string namespaceId, string entityId, string reportId, string configId = null, string targetPlace = null, params string[] paramValues)
    {
        var menu = AddMenuWithEnName(enName, name, iconName, "Report", "Index", targetPlace, paramValues);
        menu.addParameter(eMenuItemParameter.EntityId, entityId);
        menu.addParameter(eMenuItemParameter.ReportId, reportId);
        if (namespaceId != null)
        {
            menu.addParameter(eMenuItemParameter.NamespaceId, namespaceId);
        }
        if (configId != null)
        {
            menu.addParameter(eMenuItemParameter.ConfigId, configId);
        }
        return menu;
    }

    /// <summary>
    /// Adds the menu for report.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="iconName">Name of the icon.</param>
    /// <param name="namespaceId">The namespace identifier.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="dashboardId">The report identifier.</param>
    /// <param name="configId">The configuration identifier.</param>
    /// <param name="targetPlace">The target place.</param>
    /// <param name="paramValues"></param>
    /// <returns></returns>
    protected MenuItem AddMenuForDashboard(string name, string iconName, string namespaceId, string entityId, string dashboardId, string configId = null, string targetPlace = null, params string[] paramValues)
    {
        return AddMenuForDashboard(null, name, iconName, namespaceId, entityId, dashboardId, configId, targetPlace, paramValues);
    }
    protected MenuItem AddMenuForDashboard(string enName, string name, string iconName, string namespaceId, string entityId, string dashboardId, string configId = null, string targetPlace = null, params string[] paramValues)
    {
        var menu = AddMenuWithEnName(enName, name, iconName, "Dashboard", "Index", targetPlace, paramValues);
        menu.addParameter(eMenuItemParameter.EntityId, entityId);
        menu.addParameter(eMenuItemParameter.DashboardId, dashboardId);
        if (namespaceId != null)
        {
            menu.addParameter(eMenuItemParameter.NamespaceId, namespaceId);
        }
        if (configId != null)
        {
            menu.addParameter(eMenuItemParameter.ConfigId, configId);
        }
        return menu;
    }
    /// <summary>
    /// Adds the menu divider.
    /// </summary>
    /// <returns></returns>
    protected MenuItem AddMenuDivider(string enName = null, string name = null)
    {
        var menu = AddMenuWithEnName(enName, name);
        menu.IsDivider = true;
        return menu;
    }
    protected MenuItem AddMenuWithHTML(string HTMLString)
    {
        var menu = AddMenu(null, null, null, null);
        menu.HTMLString = HTMLString;
        return menu;
    }
    /// <summary>
    /// Adds the menu parameter.
    /// </summary>
    /// <param name="paramId">The parameter identifier.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public bool AddMenuParameter(eMenuItemParameter paramId, object value)
    {
        if (currentMenuItem == null) return false;
        currentMenuItem.addParameter(paramId, value);
        return true;
    }
    /// <summary>
    /// Starts the sub menus.
    /// </summary>
    protected void StartSubMenus()
    {
        currentParentMenuItem = currentMenuItem;
    }
    /// <summary>
    /// Ends the sub mneues.
    /// </summary>
    protected void EndSubMenus()
    {
        if (currentParentMenuItem != null)
            currentParentMenuItem = currentParentMenuItem.parentMenuItem;
    }
    /// <summary>
    /// Defines all.
    /// </summary>
    /// <returns></returns>
    public MenuItem DefineAll()
    {
        var currentParentMenuItem0 = Identify();
        currentParentMenuItem = currentParentMenuItem0;
        if (currentParentMenuItem0 == null) return null;
        if (!DefineMenuItems()) return null;
        return currentParentMenuItem0;
    }

    /// <summary>
    ///     Determines whether the current type is or implements the specified generic interface, and determines that
    ///     interface's generic type parameters.</summary>
    /// <returns>
    ///     True if the current type is or implements the specified generic interface.</returns>
    public abstract MenuItem Identify();
    /// <summary>
    /// Define Menu Items
    /// </summary>
    /// <returns></returns>
    public abstract bool DefineMenuItems();

    private bool FindOutParametersByType<TEntity, TEntityItem>(out string namespaceId, out string entityId, out string id, out Type type)
    {
        var entity = ProjectDefinition.Project.GetEntity<TEntity>();
        namespaceId = entity?.NamespaceId;
        entityId = entity?.Id;
        id = typeof(TEntityItem).Name;
        type = ReflectionTools.FetchBaseType<TEntityItem>(typeof(FormDefinition),
             typeof(ReportDefinition), typeof(DashboardDefinition));
        return type != null;
    }
}
