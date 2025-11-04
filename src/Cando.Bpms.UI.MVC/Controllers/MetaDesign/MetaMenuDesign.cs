
using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.MenuModels;
using Neo.Bpms.Infrastructure.Features.MetaLoader;

namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

public partial class MetaDesignController
{
    [HttpGet]
    public JsonResult Menu()
    {
        CheckEntityDesignAccess(false);

        MenuViewModel result = new(ProjectDefinition.Project.MainMenuItem);
        return Json(result);
    }

    [HttpPut]
    public JsonResult Menu([FromBody] MenuViewModel menu)
    {
        CheckEntityDesignAccess(true);

        ProjectDefinition.Project.MainMenuItem = menu.ToMenuItem(null);
        ProjectMenu.Save(ProjectDefinition.Project.MainMenuItem);
        return Json(new { Success = true });
    }

    [HttpGet]
    public JsonResult PossibleEntityItems()
    {
        CheckEntityDesignAccess(false);

        IEnumerable<MenuEntityViewModel> possibleItems = ProjectDefinition.Project.Namespaces?.Values.SelectMany(ns =>
            ns.GetEntities()?.Values.Select(e => new MenuEntityViewModel((UiEntity)e)));

        return Json(possibleItems);
    }

    [HttpGet]
    public JsonResult PossibleProcessItems()
    {
        CheckEntityDesignAccess(false);

        IEnumerable<MenuViewModel> possibleItems = ProjectDefinition.Project.BusinessProcesses?.Values.Select(p => new MenuViewModel(p));
        return Json(possibleItems);
    }
}
