using Neo.Bpms.Domain.Entities.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Model.BPMN.Processes;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.MenuModels;

public class MenuViewModel
{
    public MenuViewModel()
    {

    }

    public MenuViewModel(MenuItem menuItem)
    {
        enName = menuItem.EnName;
        name = menuItem.Name;
        isPublic = menuItem.IsPublic;
        iconName = menuItem.iconName;
        area = menuItem.Area;
        action = menuItem.action;
        targetPlace = menuItem.targetPlace;
        HTMLString = menuItem.HTMLString;
        type = GetMenuType(menuItem);
        namespaceId = menuItem.GetParameterValue(eMenuItemParameter.NamespaceId);
        entityId = menuItem.GetParameterValue(eMenuItemParameter.EntityId);
        formId = menuItem.GetParameterValue(eMenuItemParameter.FormId);
        processId = menuItem.GetParameterValue(eMenuItemParameter.ProcessId);
        taskId = menuItem.GetParameterValue(eMenuItemParameter.TaskId);
        pageType = menuItem.GetParameterValue(eMenuItemParameter.PageType);
        reportId = menuItem.GetParameterValue(eMenuItemParameter.ReportId);
        dashboardId = menuItem.GetParameterValue(eMenuItemParameter.DashboardId);
        configId = menuItem.GetParameterValue(eMenuItemParameter.ConfigId);
        formSubjectId = menuItem.GetParameterValue(eMenuItemParameter.FormSubjectId);
        //            parameters = menuItem.parameters?.Where(p =>
        //                p.Parameter != eMenuItemParameter.NamespaceId && p.Parameter != eMenuItemParameter.EntityId).Select(p => new MenuParamViewModel(p));
        subMenues = menuItem.subMenues?.Select(m => new MenuViewModel(m));
    }

    public MenuViewModel(Dashboard item)
    {
        enName = item.EnName;
        name = item.Name;
        namespaceId = item.NamespaceId;
        entityId = item.EntityId;
        area = "Dashboard";
        action = "Index";
        dashboardId = item.Id;
        //            parameters = new[] { new MenuParamViewModel(eMenuItemParameter.DashboardId, item.Id) };
    }
    public MenuViewModel(Form item)
    {
        enName = item.EnName;
        name = item.Name;
        namespaceId = item.NamespaceId;
        entityId = item.EntityId;
        area = "Form";
        action = "Index"; // todo
        formId = item.Id;
        //            parameters = new[] { new MenuParamViewModel(eMenuItemParameter.FormId, item.Id) };
        // todo subject :(              
    }
    public MenuViewModel(Form item, Process process, string taskId)
    {
        enName = item.EnName + "-" + process.Id;
        name = item.Name + "-" + process.Name;
        namespaceId = item.NamespaceId;
        entityId = item.EntityId;
        area = "Process";
        action = "Form";
        processId = process.Id;
        this.taskId = taskId;
        formId = item.Id;
    }
    public MenuViewModel(Report item)
    {
        enName = item.EnName;
        name = item.Name;
        namespaceId = item.NamespaceId;
        area = "Report";
        action = "Index"; // todo
        entityId = item.EntityId;
        reportId = item.Id;
        //            parameters = new[] { new MenuParamViewModel(eMenuItemParameter.ReportId, item.Id) };
    }
    public MenuViewModel(BusinessProcess item)
    {
        name = item.Name;
        area = "Process";
        action = "IndexWorkItems"; // todo action="Form"
        processId = item.Id;
        pageType = "WorkItems";
        //            parameters = new[]
        //            {
        //                new MenuParamViewModel(eMenuItemParameter.ProcessId, item.Id),
        //                new MenuParamViewModel(eMenuItemParameter.PageType, )
        //            };
    }

    //public string id { get; set; }
    public string enName { get; set; }
    public string name { get; set; }
    public bool isPublic { get; set; }
    public string iconName { get; set; }
    public string area { get; set; }
    public string action { get; set; }
    public string targetPlace { get; set; }
    public string HTMLString { get; set; }
    public MenuItemTypes type { get; set; } // todo
    public string namespaceId { get; set; }
    public string entityId { get; set; }
    public string formId { get; set; }
    public string formSubjectId { get; set; } // todo
    public string processId { get; set; }
    public string taskId { get; set; }
    public string pageType { get; set; }
    public string reportId { get; set; }
    public string dashboardId { get; set; }
    public string configId { get; set; }
    //        public IEnumerable<MenuParamViewModel> parameters { get; set; }
    public IEnumerable<MenuViewModel> subMenues { get; set; }

    //TODO difference between cartable and process task and form !?
    private static MenuItemTypes GetMenuType(MenuItem menuItem)
    {
        if (menuItem.subMenues?.Any() ?? false)
            return MenuItemTypes.Folder;
        if (menuItem.parameters != null)
        {
            if (menuItem.parameters.Exists(p => p.Parameter == eMenuItemParameter.ReportId))
                return MenuItemTypes.Report;
            if (menuItem.parameters.Exists(p => p.Parameter == eMenuItemParameter.ProcessId))
                return MenuItemTypes.Process;
            if (menuItem.parameters.Exists(p => p.Parameter == eMenuItemParameter.FormId))
                return MenuItemTypes.Form;
            if (menuItem.parameters.Exists(p => p.Parameter == eMenuItemParameter.DashboardId))
                return MenuItemTypes.Dashboard;
        }
        if (menuItem.IsDivider)
            return MenuItemTypes.Divider;
        return !string.IsNullOrEmpty(menuItem.Area) ? MenuItemTypes.Link : MenuItemTypes.Default;
    }

    public MenuItem ToMenuItem(MenuItem parent)
    {
        MenuItem menuItem = new(null, name, iconName, area, action, targetPlace)
        {
            IsPublic = isPublic,
            HTMLString = HTMLString,
            parameters = /*parameters?.Select(p => p.ToMenuParam()).ToList() ??*/ [],
            parentMenuItem = parent,
            IsDivider = type == MenuItemTypes.Divider
        };
        if (!string.IsNullOrEmpty(namespaceId))
        {
            //                if (menuItem.parameters.Exists(p => p.Parameter == eMenuItemParameter.NamespaceId))
            //                    throw new Exception("NamespaceId in parameters");
            menuItem.parameters.Add(new MenuParam(eMenuItemParameter.NamespaceId, namespaceId));
        }
        if (!string.IsNullOrEmpty(entityId))
        {
            //                if (menuItem.parameters.Exists(p => p.Parameter == eMenuItemParameter.EntityId))
            //                    throw new Exception("EntityId in parameters");
            menuItem.parameters.Add(new MenuParam(eMenuItemParameter.EntityId, entityId));
        }
        if (!string.IsNullOrEmpty(formId))
            menuItem.parameters.Add(new MenuParam(eMenuItemParameter.FormId, formId));
        if (!string.IsNullOrEmpty(processId))
            menuItem.parameters.Add(new MenuParam(eMenuItemParameter.ProcessId, processId));
        if (!string.IsNullOrEmpty(taskId))
            menuItem.parameters.Add(new MenuParam(eMenuItemParameter.TaskId, taskId));
        if (!string.IsNullOrEmpty(pageType))
            menuItem.parameters.Add(new MenuParam(eMenuItemParameter.PageType, pageType));
        if (!string.IsNullOrEmpty(reportId))
            menuItem.parameters.Add(new MenuParam(eMenuItemParameter.ReportId, reportId));
        if (!string.IsNullOrEmpty(dashboardId))
            menuItem.parameters.Add(new MenuParam(eMenuItemParameter.DashboardId, dashboardId));
        if (!string.IsNullOrEmpty(formSubjectId))
            menuItem.parameters.Add(new MenuParam(eMenuItemParameter.FormSubjectId, formSubjectId));
        if (!string.IsNullOrEmpty(configId))
            menuItem.parameters.Add(new MenuParam(eMenuItemParameter.ConfigId, configId));

        menuItem.subMenues = subMenues?.Select(m => m.ToMenuItem(menuItem)).ToList();
        return menuItem;
    }
}
