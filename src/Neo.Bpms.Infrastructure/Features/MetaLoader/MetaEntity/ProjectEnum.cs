using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;

public interface IProjectEnum
{
    void LoadAll(ModelNamespace model, string namespacePath);
}
public class ProjectEnum: IProjectEnum
{
    private const string Folder = "Enums";
    private static string FolderPath(string namespacePath) => $"{namespacePath}\\{Folder}";

    public void LoadAll(ModelNamespace model, string namespacePath)
    {
        var folder = FolderPath(namespacePath);
        if (!Directory.Exists(folder)) return;

        var files = Directory.EnumerateFiles(folder);
        foreach (var file in files)
            Load(model, file);
    }

    public static void SaveAllEnums(ModelNamespace projectNamespace, string namespacePath)
    {
        foreach (var enumValue in projectNamespace.GetEnums().Values)
            Save(enumValue, namespacePath);
    }

    private static void Load(ModelNamespace model, string file)
    {
        var newEnumeration = XMLUtill.GetObjectFromXmlFile<Enumeration>(file);
        if (newEnumeration == null) return;
        model.AddEnumeration(newEnumeration, true);
        ReConfig(newEnumeration, model);
    }

    public static void Save(Enumeration enumValue)
    {
        Save(enumValue, ProjectNamespace.NamespacePath(enumValue.Parent as ModelNamespace, ""));
    }

    private static void Save(Enumeration enumValue, string namespacePath)
    {
        var folder = FolderPath(namespacePath);
        ProjectMetaFile.SaveObjectToFile(enumValue, folder, $"{enumValue.Id}.xml");
    }

    public static void Remove(string namespaceId, string id)
    {
        var modelNamespace = ProjectDefinition.Project.GetModel(namespaceId);
        var enumeration = modelNamespace?.GetEnum(id);
        if (enumeration == null) return;
        var folder = FolderPath(ProjectNamespace.NamespacePath(modelNamespace));
        //ProjectMetaFile.DeleteItem(folder, id, enumeration.Name, modelNamespace.DeleteEnum);
        modelNamespace.DeleteEnum(id);
        ProjectMetaFile.DeletePhysicalFile(folder, $"{id}.xml");
    }

    private static void ReConfig(Enumeration enumeration, ModelNamespace model)
    {
        enumeration.Parent = model;
    }
}
