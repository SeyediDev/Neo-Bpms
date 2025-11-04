using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;

public static class ProjectEntityDashboard
{
    private const string Folder = "Dashboards";
    private static string FolderPath(string entityPath) => $"{entityPath}\\{Folder}";
    private static string FileName(Dashboard dashboard) => $"{dashboard.Id}.xml";

    public static void LoadAll(UiEntity entity, string entityPath)
    {
        var folder = FolderPath(entityPath);
        if (!Directory.Exists(folder)) return;

        var files = Directory.EnumerateFiles(folder);
        foreach (var file in files)
            Load(entity, folder, file);
    }

    public static void SaveAllDashboards(UiEntity uiEntity, string entityPath)
    {
        foreach (var dashboard in uiEntity.GetDashboards()?.Values ?? Enumerable.Empty<Dashboard>())
            Save(dashboard, entityPath);
    }

    private static void Load(UiEntity entity, string folder, string fileName)
    {
        var id = ProjectMetaFile.FetchFileName(fileName);
        var dashboard = XMLUtill.GetObjectFromXmlFile<Dashboard>(fileName);
        if (dashboard == null)
        {
            Directory.CreateDirectory($"{folder}\\InvalidDashboards");
            File.Move(fileName, $"{folder}\\InvalidDashboards\\{id}.xml");
            return;
        }
        ReConfig(dashboard, entity);
        entity.AddDashboard(dashboard, true);
    }

    public static void Save(Dashboard dashboard, string entityPath = null)
    {
        var folder = FolderPath(!string.IsNullOrEmpty(entityPath) ? entityPath : ProjectEntity.EntityPath(dashboard.Entity));
        ProjectMetaFile.SaveObjectToFile(dashboard, folder, FileName(dashboard));
    }


    public static void Remove(string namespaceId, string entityId, string id)
    {
        var entity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        var dashboard = entity?.GetDashboard(id);
        if (dashboard == null) return;
        var folder = FolderPath(ProjectEntity.EntityPath(dashboard.Entity));
        //ProjectMetaFile.DeleteItem(folder, id, dashboard.Name, entity.DeleteDashboard);
        entity.DeleteDashboard(id);
        ProjectMetaFile.DeletePhysicalFile(folder, $"{id}.xml");
    }

    public static void ReConfig(Dashboard dashboard, Entity entity)
    {
        ProjectEntityForm.ReConfigForm(dashboard, entity);
        dashboard.Parent = entity;
        //todo ...
    }
}