using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;

public interface IProjectNamespace
{
    void LoadStructure();
    void LoadUi();
    void SaveAllNamespaces(string pathPrefix = null);
}
public class ProjectNamespace(/*ILogger logger,*/ IProjectEntity projectEntity, IProjectEnum projectEnum) 
    : IProjectNamespace
{
    private const string Folder = "Namespaces";

    public void LoadStructure()
    {
        string folder = $"{ProjectMetaFile.MetaPath}\\{Folder}";
        if (!Directory.Exists(folder))
        {
            return;
        }

        IEnumerable<string> directories = Directory.EnumerateDirectories(folder);
        foreach (string directory in directories)
        {
            LoadStructure(directory);
        }
    }

    public void LoadUi()
    {
        string folder = $"{ProjectMetaFile.MetaPath}\\{Folder}";
        if (!Directory.Exists(folder))
        {
            return;
        }

        IEnumerable<string> directories = Directory.EnumerateDirectories(folder);
        foreach (string directory in directories)
        {
            LoadUiOfNamespace(directory);
        }
    }

    public static string NamespacePath(ModelNamespace projectNamespace, string pathPrefix = null)
    {
        return $"{ProjectMetaFile.MetaPath}\\{pathPrefix ?? ""}{Folder}\\{projectNamespace.Id}";
    }

    public void SaveAllNamespaces(string pathPrefix = null)
    {
        foreach (ModelNamespace projectNamespace in ProjectDefinition.Project.Namespaces.Values)
        {
            SaveAll(projectNamespace, pathPrefix);
        }
    }

    public static void Remove(string namespaceId)
    {
        ModelNamespace modelNamespace = ProjectDefinition.Project.GetModel(namespaceId);
        string folder = NamespacePath(modelNamespace);
        //ProjectMetaFile.DeleteItem(folder, namespaceId, modelNamespace.Name, ProjectDefinition.Project.DeleteNamespace);
        _ = ProjectDefinition.Project.DeleteNamespace(namespaceId);
        _ = ProjectMetaFile.DeletePhisycalDirectory(folder, namespaceId);
    }

    public static void Save(ModelNamespace projectNamespace, string pathPrefix = null)
    {
        string path = NamespacePath(projectNamespace, pathPrefix);
        ProjectMetaFile.SaveObjectToFile(projectNamespace, path, $"{projectNamespace.Id}.xml");
    }

    private static void SaveAll(ModelNamespace projectNamespace, string pathPrefix = null)
    {
        Save(projectNamespace, pathPrefix);
        string namespacePath = NamespacePath(projectNamespace, pathPrefix);
        ProjectEntity.SaveAllEntities(projectNamespace, namespacePath);
        ProjectEnum.SaveAllEnums(projectNamespace, namespacePath);
    }

    private void LoadStructure(string namespacePath)
    {
        string id = ProjectMetaFile.LastDirectoryName(namespacePath);
        ModelNamespace model = ProjectDefinition.Project.GetModel(id);
        ModelNamespace newItem = XMLUtill.GetObjectFromXmlFile<ModelNamespace>($"{namespacePath}\\{id}.xml");
        if (newItem != null)
        {
            if (model != null)
            {
                newItem.SetComponents(model);
            }

            _ = ProjectDefinition.Project.AddNamespace(newItem, true);
            ReConfig(newItem, model);
            model = newItem;
        }

        if (model == null)
        {
            //logger.LogCritical("Model not found for {0}", id);
            return;
        }

        projectEntity.LoadStructure(model, namespacePath);
        projectEnum.LoadAll(model, namespacePath);
    }

    private void LoadUiOfNamespace(string namespacePath)
    {
        string id = ProjectMetaFile.LastDirectoryName(namespacePath);
        ModelNamespace model = ProjectDefinition.Project.GetModel(id);
        if (model == null)
        {
            //logger.LogCritical("Model not found for {0}", id);
            return;
        }

        ProjectEntity.LoadUi(model, namespacePath);
    }
    //todo: function usage
    private static void ReConfig(ModelNamespace newItem, ModelNamespace model)
    {

    }
}
