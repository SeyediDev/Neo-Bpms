using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Neo.Bpms.Domain.Features.Security;

namespace Neo.Bpms.UI.MVC.Controls;

public class MenuHelper(
    IAccessServices accessServices, ICustomIconProvider? customIconProvider = null) 
    : IMenuHelper
{
    private static IHttpContextAccessor? _httpContextAccessor;
    
    /// <summary>
    /// Sets the HttpContextAccessor for static method resolution (called during app startup)
    /// </summary>
    public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// get Navigation Menu Items.
    /// </summary>
    /// <param name="pagePackId"></param>
    /// <param name="user"></param>
    /// <param name="actionContext"></param>
    /// <param name="urlHelper"></param>
    /// <returns></returns>
    public HtmlString GetMenuItems(string pagePackId, IdentityUser user, IUrlHelper urlHelper)
    {
        return GetMenuItems(ProjectDefinition.Project?.MainMenuItem, pagePackId, user, urlHelper);
    }

    public static void GetSelectedItems(MenuItem menuItem, string pagePackId,
        ref MenuItem selectedItem, ref MenuItem indexSelectedItem)
    {
        if (string.IsNullOrEmpty(pagePackId))
            return;
        List<string> itemPackIds = GetPagePackIds(menuItem);
        string[] pagePackIds = pagePackId.Split(';');
        if (itemPackIds.Count > 0 && pagePackIds.Length > 0 && itemPackIds[0] == pagePackIds[0])
            selectedItem = menuItem;
        if (itemPackIds.Count > 1 && pagePackIds.Length > 1 && itemPackIds[1] == pagePackIds[1])
            indexSelectedItem = menuItem;
        if (selectedItem == null)
        {
            foreach (MenuItem sub in menuItem.subMenues ?? Enumerable.Empty<MenuItem>())
            {
                GetSelectedItems(sub, pagePackId,
                    ref selectedItem, ref indexSelectedItem);
                if (selectedItem != null) break;
            }
        }
    }

    /// <summary>
    /// get Navigation Menu Items.
    /// </summary>
    /// <param name="menuItem"></param>
    /// <param name="pagePackId"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    private HtmlString GetMenuItems(MenuItem menuItem, string pagePackId, IdentityUser user, IUrlHelper urlHelper)
    {
        StringBuilder sb = new();
        if (menuItem?.subMenues?.Any() ?? false)
        {
            MenuItem selectedItem = null, indexSelectedItem = null;
            GetSelectedItems(menuItem, pagePackId, ref selectedItem, ref indexSelectedItem);
            foreach (MenuItem sub in menuItem.subMenues)
            {
                TryWriteItems(ref sb, sub, selectedItem, indexSelectedItem, user, urlHelper);
            }
        }

        return new HtmlString(sb.ToString());
    }

    private bool TryWriteItems(ref StringBuilder sb, MenuItem item,
        MenuItem selectedItem, MenuItem indexSelectedItem, IdentityUser user, IUrlHelper urlHelper)
    {
        if (!CheckAccess(user, item) && !item.IsDivider)
        {
            return false;
        }

        GetMenuItemMvcString(ref sb, item, selectedItem, indexSelectedItem, urlHelper, user);

        return true;
    }

    private static bool ShouldIgnoreDivider(MenuItem item, bool lastItemWasHr, out bool isHr)
    {
        if (item.IsDivider)
        {
            isHr = true;
            if (lastItemWasHr)
            {
                return true;
            }
        }
        else
        {
            isHr = false;
        }

        return false;
    }

    private void GetMenuItemMvcString(ref StringBuilder sb, MenuItem item,
     MenuItem selectedItem, MenuItem indexSelectedItem, IUrlHelper urlHelper, IdentityUser user)
    {

        sb.Append(
            $"<li class=\"{(IsSelected(item, selectedItem, indexSelectedItem) ? "Selected" : "")} {(item.IsDivider ? "Divider" : "")}\">");

        if (!string.IsNullOrWhiteSpace(item.HTMLString))
        {
            sb.Append("<span>");
            sb.Append(item.HTMLString);
            sb.Append("</span>");
        }

        if (!string.IsNullOrWhiteSpace(item.Area) && !string.IsNullOrWhiteSpace(item.action))
        {
            sb.Append("<a href=\"");
            string routeName = item.parameters?.FirstOrDefault(p => p.Parameter == eMenuItemParameter.RouteName)?.value.ToString();
            if (routeName == null)
                AppendNormalUrl(sb, item, urlHelper);
            else
                AppendRouteUrl(sb, item, urlHelper, routeName);
            sb.Append("\">");
        }

        if (item.IsDivider)
        {
            sb.Append(GetLabel(item));
        }
        else
        {
            AddLabel(sb, item);
        }

        if (!string.IsNullOrWhiteSpace(item.Area) && !string.IsNullOrWhiteSpace(item.action))
            sb.Append("</a>");

        if (item.subMenues?.Any() ?? false)
        {
            sb.Append("<ul>");
            bool lastItemWasDivider = false;
            foreach (MenuItem sub in item.subMenues)
            {
                if (ShouldIgnoreDivider(sub, lastItemWasDivider, out bool isDivider))
                {
                    continue;
                }
                bool itemWrote = TryWriteItems(ref sb, sub, selectedItem, indexSelectedItem, user, urlHelper);
                if (itemWrote)
                {
                    lastItemWasDivider = isDivider;
                }
            }
            sb.Append("</ul>");
        }

        sb.Append("</li>");
    }

    private static bool IsSelected(MenuItem item, MenuItem selectedItem, MenuItem indexSelectedItem)
    {
        return (selectedItem != null && Equals(selectedItem, item))
               || (selectedItem == null && indexSelectedItem != null && Equals(indexSelectedItem, item));
    }

    private static string GetLabel(MenuItem item)
    {
        string culture = CultureHelper.GetNeutralCulture(CultureHelper.GetCurrentCulture());
        if (string.IsNullOrEmpty(culture))
            culture = CultureHelper.GetDefaultCulture();

        if (culture == "fa" && !string.IsNullOrWhiteSpace(item.Name))
        {
            return item.Name;
        }
        return culture == "en" && !string.IsNullOrWhiteSpace(item.EnName) ? item.EnName : item.Name;
    }

    private void AddLabel(StringBuilder sb, MenuItem item)
    {
        sb.Append("<span>");
        // Check for icon (menu icon or entity icon fallback)
        string iconName = GetMenuItemIcon(item);
        if (!string.IsNullOrEmpty(iconName))
        {
            AddIconName(sb, item);
        }

        sb.Append(GetLabel(item));
        sb.Append("</span>");
    }

    private void AddIconName(StringBuilder sb, MenuItem item)
    {
        string iconName = GetMenuItemIcon(item);
        if (string.IsNullOrEmpty(iconName)) return;
           
        sb.Append(IconSvg(iconName));
    }
    
    /// <summary>
    /// Gets the icon for a menu item.
    /// If the menu item has an icon, it is returned.
    /// Otherwise, the entity icon is returned (fallback).
    /// </summary>
    private static string GetMenuItemIcon(MenuItem item)
    {
        // Primary: Menu item's own icon
        if (!string.IsNullOrEmpty(item.iconName))
            return item.iconName;
        
        // Fallback: Entity's icon
        string namespaceId = item.GetParameterValue(eMenuItemParameter.NamespaceId);
        string entityId = item.GetParameterValue(eMenuItemParameter.EntityId);
        
        if (string.IsNullOrEmpty(entityId))
            return null;
        
        var entity = ProjectDefinition.Project?.GetEntity(namespaceId, entityId);
        return entity?.Icon;
    }
    
    private string IconSvg(string iconName)
    {
        return $"<svg class=\"sidemenu-icon\" data-icon=\"{iconName}\" viewBox=\"0 0 24 24\">" +
                    $"<use xlink:href=\"{IconHref(iconName)}\"></use>" +
               $"</svg>";
    }
    
    /// <summary>
    /// Static method for backward compatibility. Resolves ICustomIconProvider from DI if available.
    /// </summary>
    public static string IconSvg(string iconName, ICustomIconProvider? customIconProvider = null)
    {
        var iconHref = IconHref(iconName, customIconProvider);
        return $"<svg class=\"sidemenu-icon\" data-icon=\"{iconName}\" viewBox=\"0 0 24 24\">" +
                    $"<use xlink:href=\"{iconHref}\"></use>" +
               $"</svg>";
    }
    
    private string IconHref(string iconName)
    {
        return UseCustomIcon(iconName)
            ? $"/Content/custom-icons/custom-sprite.svg#{iconName}"
            : $"/Content/common-assets-includes/icons/svgSprite.svg#{iconName}";
    }
    
    /// <summary>
    /// Static method for backward compatibility. Resolves ICustomIconProvider from DI if available.
    /// </summary>
    private static string IconHref(string iconName, ICustomIconProvider? customIconProvider = null)
    {
        // Try to resolve from DI if not provided
        if (customIconProvider == null)
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext != null)
            {
                customIconProvider = httpContext.RequestServices.GetService<ICustomIconProvider>();
            }
        }
        
        bool isCustom = customIconProvider?.IsCustomIcon(iconName) ?? false;
        return isCustom
            ? $"/Content/custom-icons/custom-sprite.svg#{iconName}"
            : $"/Content/common-assets-includes/icons/svgSprite.svg#{iconName}";
    }
    
    private bool UseCustomIcon(string iconName)
    {
        return !string.IsNullOrEmpty(iconName) && (customIconProvider?.IsCustomIcon(iconName) ?? false);
    }

    private static void AppendRouteUrl(StringBuilder sb, MenuItem item, IUrlHelper urlHelper, string routeName)
    {
        if (routeName == "CustomPage")
        {
            sb.Append(urlHelper.RouteUrl(routeName, new
            {
                NamespaceId = item.GetParameterValue(eMenuItemParameter.NamespaceId),
                EntityId = item.GetParameterValue(eMenuItemParameter.EntityId),
                FormId = item.GetParameterValue(eMenuItemParameter.FormId)
            }));
        }
        else
        {
            throw new NotSupportedException($"RouteName {routeName}");
        }
    }

    private static void AppendNormalUrl(StringBuilder sb, MenuItem item, IUrlHelper urlHelper)
    {
        string action = item.action;
        string u = urlHelper.Action(action, item.Area).Replace("%2F", "/");
        sb.Append(u);
        string paramString = (item.parameters == null)
             ? ""
             : string.Join("&", item.parameters.Select(ParameterToUrlComponent));
        if (!string.IsNullOrEmpty(paramString))
        {
            sb.Append("?");
            sb.Append(paramString);
        }
    }

    private static string ParameterToUrlComponent(MenuParam p)
    {
        string key;
        switch (p.Parameter)
        {
            case eMenuItemParameter.ProcessId:
                key = "__ProcessId";
                break;
            case eMenuItemParameter.PageType:
                key = "__PageType";
                break;
            case eMenuItemParameter.ParamValue:
                return p.value.ToString();
            default:
                key = p.Parameter.ToString();
                break;
        }

        return key + "=" + p.value;
    }

    /// <summary>
    /// check user access to Form and Report Pages.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool CheckAccess(IdentityUser user, MenuItem item)
    {
        if (item.IsPublic)
            return true;
        if (user == null)
            return false;
        if (string.IsNullOrEmpty(item.Area))
        {
            return item.IsDivider
                ? false
                : item.subMenues == null ||
                     item.subMenues.Any(menuItem => CheckAccess(user, menuItem));
        }

        string namespaceId = GetMenuItemParamValue(item, eMenuItemParameter.NamespaceId);
        string entityId = GetMenuItemParamValue(item, eMenuItemParameter.EntityId);
        switch (item.Area)
        {
            case "Form":
                string formId = GetMenuItemParamValue(item, eMenuItemParameter.FormId);
                Form form = item.Reference as Form ?? ProjectDefinition.Project.GetUiEntity(
                                      namespaceId, entityId)
                                  ?
                                  .GetEntityForm(formId,
                                      GetFormType(item.action),
                                      GetMenuItemParamValue(item, eMenuItemParameter.FormSubjectId));
                item.Reference = form;
                return form != null && accessServices.CheckFormAccess(user, form, out _);
            case "Report":
                Report report = item.Reference as Report ?? ProjectDefinition.Project.GetUiEntity(
                                         namespaceId, entityId)
                                     ?
                                     .GetReport(GetMenuItemParamValue(item, eMenuItemParameter.ReportId));
                item.Reference = report;
                return report != null && accessServices.CheckReportAccess(user, report);
            case "Dashboard":
                Dashboard dashboard = item.Reference as Dashboard ?? ProjectDefinition.Project.GetUiEntity(
                                             namespaceId, entityId)
                                         ?
                                         .GetDashboard(GetMenuItemParamValue(item, eMenuItemParameter.DashboardId));
                item.Reference = dashboard;
                return dashboard != null && accessServices.CheckDashboardAccess(user, dashboard);
            case "Process":
                if (item.action == "MyWorkItems" || item.action == "MyProcesses")
                {
                    return true;
                }
                else if (item.action == "WorkItems" || item.action == "IndexWorkItems")
                {
                    string processId = GetMenuItemParamValue(item, eMenuItemParameter.ProcessId);
                    if (!string.IsNullOrEmpty(processId))
                        return accessServices.CheckProcessAccess(user, processId);
                }
                else if (item.action?.ToLower() == "form")
                {
                    string processId = GetMenuItemParamValue(item, eMenuItemParameter.ProcessId);
                    if (string.IsNullOrEmpty(processId))
                        return accessServices.CheckControllerActionAccess(user, item.Area, item.action);
                    Form taskForm = item.Reference as Form ??
                                        GetProcessTaskForm(processId, GetMenuItemParamValue(item, eMenuItemParameter.TaskId));
                    if (taskForm == null) return false;
                    item.Reference = taskForm;
                    return accessServices.CheckFormAccess(user, taskForm, out _);
                }
                else if (item.action?.ToLower() == "bpmndesign")
                {
                    return accessServices.CheckSystemFeatureAccess(user, SystemFeatureId.ProcessDesign);
                }
                return accessServices.CheckControllerActionAccess(user, item.Area, item.action);

            default:
                return accessServices.CheckControllerActionAccess(user, item.Area, item.action);
        }
    }

    private static Form GetProcessTaskForm(string processId, string taskId)
    {
        return ProjectDefinition.Project.GetProcessTaskForm(processId, taskId);
    }

    /// <summary>
    /// get Form Type.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private static Form.eFormType GetFormType(string action)
    {
        return action switch
        {
            "Index" => Form.eFormType.Index,
            "Create" => Form.eFormType.Create,
            "Edit" => Form.eFormType.Edit,
            "Delete" => Form.eFormType.Delete,
            "VirtualDelete" => Form.eFormType.VirtualDelete,
            "Details" => Form.eFormType.Detail,
            "WorkItem" => Form.eFormType.WorkItem,//It's been Authorization.eFormType.ProcessForm
            "SpecificURL" => Form.eFormType.SpecificURL,
            _ => Form.eFormType.Index,
        };
    }

    /// <summary>
    /// get current Page identification to select current Menu Item from Navigation Menu
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private static List<string> GetPagePackIds(MenuItem item)
    {
        List<string> list = [];
        switch (item.Area)
        {
            case "Form":
                {
                    Form form = item.Reference as Form;
                    list.Add(form?.PagePackId);
                    list.Add(form?.IndexPagePackId);
                    break;
                }

            case "Report":
                {
                    Report report = item.Reference as Report;
                    list.Add(report?.PagePackId);
                    break;
                }

            case "Dashboard":
                {
                    Dashboard dashboard = item.Reference as Dashboard;
                    list.Add(dashboard?.PagePackId);
                    break;
                }

            case "Process":
                {
                    string processId = GetMenuItemParamValue(item, eMenuItemParameter.ProcessId);
                    string pageType = GetMenuItemParamValue(item, eMenuItemParameter.PageType);
                    if (item.action?.ToLower() == "myworkitems" || item.action?.ToLower() == "myprocesses" ||
                         pageType == "MyWorkItems")
                        list.Add("Process_MyWorkItems");
                    else if (item.action?.ToLower() == "workitems" ||
                                item.action?.ToLower() == "indexworkitems" && pageType == "WorkItems")
                    {
                        list.Add("Process_WorkItems" +
                                    (!string.IsNullOrEmpty(processId) ? "_" + processId : string.Empty));
                    }
                    else if (item.action?.ToLower() == "form")
                    {
                        Form taskForm = item.Reference as Form;
                        list.Add(taskForm?.PagePackId);
                        list.Add(taskForm?.IndexPagePackId);
                    }
                    else if (item.action?.ToLower() == "bpmndesign")
                    {
                        list.Add("Process_BpmnDesign");
                    }
                }
                break;
            default:
                list.Add("/" + item.Area + "/" + item.action);
                break;
        }

        return list;
    }

    /// <summary>
    /// get Parameters related to Menu Item of Navigation Menu
    /// </summary>
    /// <param name="item"></param>
    /// <param name="eMenuItemParameter"></param>
    /// <returns></returns>
    private static string GetMenuItemParamValue(MenuItem item, eMenuItemParameter eMenuItemParameter)
    {
        if (item.parameters == null) return null;
        MenuParam prm = item.parameters.FirstOrDefault(p => p.Parameter == eMenuItemParameter);
        return prm?.value != null ? prm.value.ToString() : string.Empty;
    }
}
