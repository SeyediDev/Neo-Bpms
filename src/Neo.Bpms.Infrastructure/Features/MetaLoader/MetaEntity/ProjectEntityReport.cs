using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.UI;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;

public static class ProjectEntityReport
{
    private const string Folder = "Reports";
    private static string FolderPath(string entityPath) => $"{entityPath}\\{Folder}";
    private static string FileName(Report report) => $"{report.Id}.xml";

    public static void LoadAll(UiEntity entity, string entityPath)
    {
        var folder = FolderPath(entityPath);
        if (!Directory.Exists(folder)) return;

        var files = Directory.EnumerateFiles(folder);
        foreach (var file in files)
            Load(entity, folder, file);
    }

    public static void SaveAllReports(UiEntity uiEntity, string entityPath)
    {
        foreach (var report in uiEntity.GetReports() ?? [])
            Save(report, entityPath);
    }

    private static void Load(UiEntity entity, string folder, string fileName)
    {
        var id = ProjectMetaFile.FetchFileName(fileName);
        var report = XMLUtill.GetObjectFromXmlFile<Report>(fileName);
        if (report == null)
        {
            Directory.CreateDirectory($"{folder}\\InvalidReports");
            File.Move(fileName, $"{folder}\\InvalidReports\\{id}.xml");
            return;
        }

        ReConfig(report, entity);
        entity.AddReport(report, true);
    }

    public static void Save(Report report, string entityPath = null)
    {
        var folder =
            FolderPath(!string.IsNullOrEmpty(entityPath) ? entityPath : ProjectEntity.EntityPath(report.entity));
        ProjectMetaFile.SaveObjectToFile(report, folder, FileName(report));
    }

    public static void Remove(string namespaceId, string entityId, string id)
    {
        var entity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        var report = entity?.GetReport(id);
        if (report == null) return;
        var folder = FolderPath(ProjectEntity.EntityPath(report.entity));
        //ProjectMetaFile.DeleteItem(folder, id, report.Name, entity.DeleteReport);
        entity.DeleteReport(id);
        ProjectMetaFile.DeletePhysicalFile(folder, $"{id}.xml");
    }

    public static void ReConfig(Report report, Entity entity)
    {
        //todo
        report.Parent = entity;
        ProjectEntityForm.ReConfig(report, entity);
    }
}