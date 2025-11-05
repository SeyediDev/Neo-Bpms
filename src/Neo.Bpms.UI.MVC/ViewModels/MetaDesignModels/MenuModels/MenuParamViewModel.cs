namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.MenuModels;

public class MenuParamViewModel
{
    public MenuParamViewModel()
    {

    }

    public MenuParamViewModel(MenuParam menuParam)
    {
        type = (int)menuParam.Parameter;
        value = menuParam.value.ToString();
    }
    public MenuParamViewModel(eMenuItemParameter parameter, string value)
    {
        type = (int)parameter;
        this.value = value;
    }

    public int type { get; set; }
    public string value { get; set; }

    public MenuParam ToMenuParam()
    {
        return new MenuParam((eMenuItemParameter)type, value);
    }
}
