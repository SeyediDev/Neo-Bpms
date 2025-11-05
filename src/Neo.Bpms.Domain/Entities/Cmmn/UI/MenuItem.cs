namespace Neo.Bpms.Domain.Entities.Cmmn.UI;

public enum eMenuItemParameter
{
    EntityId,
    ReportId,
    //		FormType,
    FormSubjectId,
    ConfigId,
    //		PageNumber,
    //		FilterValues,
    NamespaceId,
    FormId,
    ProcessId,
    TaskId,
    DashboardId,
    PageType,
    ParamValue,

    RouteName = 1000
}
/// <summary>
/// Menu Parameters class
/// </summary>
public class MenuParam
{
    public MenuParam()
    {
    }

    public MenuParam(eMenuItemParameter parameter, object value)
    {
        Parameter = parameter;
        this.value = value;
    }

    public eMenuItemParameter Parameter;
    public object value;
}
/// <summary>
/// Menu Item
/// </summary>
public class MenuItem : BaseModelClass
{
    //		public MenuItem(int id, string name, string targetAddress, string targetPlace=null): this(null, id, name, targetAddress, targetPlace){}
    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItem"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="iconName">Name of the icon.</param>
    /// <param name="area">The area.</param>
    /// <param name="action">The action.</param>
    /// <param name="targetPlace">The target place.</param>
    public MenuItem(string id, string name, string iconName, string area, string action, string targetPlace = null)
        : base(id, name)
    {
        this.iconName = iconName;
        this.action = action;
        Area = area;
        this.targetPlace = targetPlace;
        IsPublic = false;
        //			this.parentMenuItem = parentMenuItem;
    }
    public MenuItem(string id, string enName, string name, string iconName, string area, string action, string targetPlace = null)
        : base(id, name)
    {
        EnName = enName;
        this.iconName = iconName;
        this.action = action;
        Area = area;
        this.targetPlace = targetPlace;
        IsPublic = false;
    }

    public MenuItem()
    {

    }

    public string GetParameterValue(eMenuItemParameter parameter)
    {
        return parameters?.FirstOrDefault(p => p.Parameter == parameter)?.value
            ?.ToString();
    }

    [XmlIgnore]
    public MenuItem parentMenuItem = null;
    public List<MenuItem> subMenues = null;
    public List<MenuParam> parameters = null;
    public object Reference;
    /// <summary>
    /// Adds the parameter.
    /// </summary>
    /// <param name="paramId">The parameter identifier.</param>
    /// <param name="value">The value.</param>
    public void addParameter(eMenuItemParameter paramId, object value)
    {
        MenuParam p = new();
        parameters ??= [];
        p.Parameter = paramId;
        p.value = value;
        parameters.Add(p);
    }
    /// <summary>
    /// Adds the sub menu.
    /// </summary>
    /// <param name="menu">The menu.</param>
    public void addSubMenu(MenuItem menu)
    {
        menu.parentMenuItem = this;
        subMenues ??= [];
        subMenues.Add(menu);
    }
    public string Area { get; set; }
    public string targetPlace;
    public string iconName;
    public bool IsPublic;
    public string HTMLString { get; set; }
    public bool IsDivider { get; set; }
    public string action { get; set; }
}
