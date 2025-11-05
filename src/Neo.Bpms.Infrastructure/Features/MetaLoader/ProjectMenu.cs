using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.UI;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader;

public interface IProjectMenu
{
    void Load();
    void SaveAll(string pathPrefix = null);
}
public class ProjectMenu: IProjectMenu
{
    //todo: rename to Menus
    private const string Folder = "Menues";
    private const string FileName = "Menues.xml";

    private static string MenuFolder(string pathPrefix = null) =>
        $"{ProjectMetaFile.MetaPath}\\{pathPrefix ?? ""}{Folder}";

    public void Load()
    {
        var folder = MenuFolder();
        var mainMenuItem = XMLUtill.GetObjectFromXmlFile<MenuItem>($"{folder}\\{FileName}");
        if (mainMenuItem == null) return;
        ProjectDefinition.Project.MainMenuItem = mainMenuItem;
        ReConfig(ProjectDefinition.Project.MainMenuItem);
    }

    public static void Save(MenuItem menu)
    {
        SaveMenu(menu);
    }

    public void SaveAll(string pathPrefix = null)
    {
        SaveMenu(ProjectDefinition.Project.MainMenuItem, pathPrefix);
    }

    private static void SaveMenu(MenuItem menu, string pathPrefix = null)
    {
        ProjectMetaFile.SaveObjectToFile(menu, MenuFolder(pathPrefix), FileName);
    }

    private static void ReConfig(MenuItem menuItem)
    {
        foreach (var item in menuItem?.subMenues ?? Enumerable.Empty<MenuItem>())
        {
            item.parentMenuItem = menuItem;
            ReConfig(item);
        }
    }
}
