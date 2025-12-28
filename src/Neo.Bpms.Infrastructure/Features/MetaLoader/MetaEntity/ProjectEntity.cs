namespace Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;

public interface IProjectEntity 
{
    void LoadStructure(ModelNamespace model, string namespacePath);
}
public class ProjectEntity: IProjectEntity
{
    private const string Folder = "Entities";
    private const string FieldSuffix = ".fields";
    private static string FolderPath(string namespacePath) => $"{namespacePath}\\{Folder}";

    public void LoadStructure(ModelNamespace model, string namespacePath)
    {
        var folder = FolderPath(namespacePath);
        if (!Directory.Exists(folder)) return;

        var directories = Directory.EnumerateDirectories(folder);
        foreach (var directory in directories)
            LoadEntityStructure(model, folder, directory);
    }

    public static void LoadUi(ModelNamespace model, string namespacePath)
    {
        var folder = FolderPath(namespacePath);
        if (!Directory.Exists(folder)) return;

        var directories = Directory.EnumerateDirectories(folder);
        foreach (var directory in directories)
            LoadUiEntity(model, directory);
    }

    public static void SaveAllEntities(ModelNamespace projectNamespace, string namespacePath)
    {
        foreach (var entity in projectNamespace.GetEntities().Values)
            SaveAll(entity, namespacePath);
    }

    public static void Save(Entity entity, string entityPath = null)
    {
        if (string.IsNullOrEmpty(entityPath))
            entityPath = EntityPath(entity);
        SaveStructure(entity, entityPath);
    }

    public static void Save(UiEntity entity, string entityPath = null)
    {
        if (string.IsNullOrEmpty(entityPath))
            entityPath = EntityPath(entity);
        SaveStructure(entity, entityPath);
    }

    private static void SaveStructure(Entity entity, string entityPath)
    {
        ProjectMetaFile.SaveObjectToFile(entity, entityPath, $"{entity.Id}.xml");
        ProjectMetaFile.SaveObjectToFile((entity.entityFields?.Values ?? Enumerable.Empty<EntityField>()).ToList(),
            entityPath, $"{entity.Id}{FieldSuffix}.xml");
    }

    public static void Remove(string namespaceId, string entityId)
    {
        var modelNamespace = ProjectDefinition.Project.GetModel(namespaceId);
        var entity = modelNamespace.GetEntity(entityId);
        if (entity == null) return;
        var folder = FolderPath(ProjectNamespace.NamespacePath(modelNamespace));
        //ProjectMetaFile.DeleteItem(folder, entityId, entity.Name, modelNamespace.DeleteEntity);
        modelNamespace.DeleteEntity(entityId);
        ProjectMetaFile.DeletePhisycalDirectory(folder, entityId);
    }

    public static string EntityPath(Entity entity, string namespacePath = null)
    {
        if (string.IsNullOrEmpty(namespacePath))
            namespacePath = ProjectNamespace.NamespacePath(entity.model);
        return $"{FolderPath(namespacePath)}\\{entity.Id}";
    }

    private static void LoadEntityStructure(ModelNamespace model, string folder, string entityPath)
    {
        var id = ProjectMetaFile.FetchFileName(entityPath);
        if (id == "InvalidEntities") return;
        var entityFileName = $"{entityPath}\\{id}.xml";
        var newEntity = XMLUtill.GetObjectFromXmlFile<UiEntity>(entityFileName);
        if (newEntity == null)
        {
            if (File.Exists(entityFileName))
            {
                Directory.CreateDirectory($"{folder}\\InvalidEntities");
                Directory.Move(entityPath, $"{folder}\\InvalidEntities\\{id}");
            }
            return;
        }
        var newEntityFields = XMLUtill.GetObjectFromXmlFile<List<EntityField>>($"{entityPath}\\{id}{FieldSuffix}.xml");
        foreach (var entityField in newEntityFields ?? Enumerable.Empty<EntityField>())
            newEntity.entityFields.TryAdd(entityField.Id, entityField);
        var entity = model.GetEntity(id);
        if (entity != null)
            newEntity.SetUiComponents(entity as UiEntity);
        model.AddEntity(newEntity, true);
        ReConfig(newEntity, model);
    }

    private static void LoadUiEntity(ModelNamespace model, string entityPath)
    {
        var id = ProjectMetaFile.FetchFileName(entityPath);
        if (model.GetEntity(id) is not UiEntity uiEntity) return;
        ProjectEntityForm.LoadAll(uiEntity, entityPath);
        ProjectEntityReport.LoadAll(uiEntity, entityPath);
        ProjectEntityDashboard.LoadAll(uiEntity, entityPath);
    }

    private static void SaveAll(Entity entity, string namespacePath)
    {
        var entityPath = EntityPath(entity, namespacePath);
        if (entity is UiEntity uiEntity)
        {
            Save(uiEntity, entityPath);
            ProjectEntityForm.SaveAllForms(uiEntity, entityPath);
            ProjectEntityReport.SaveAllReports(uiEntity, entityPath);
            ProjectEntityDashboard.SaveAllDashboards(uiEntity, entityPath);
        }
        else
            Save(entity, entityPath);
    }

    public static void ReConfig(Entity entity, ModelNamespace model)
    {
        entity.Parent = model;
        foreach (var entityField in entity.entityFields?.Values ?? Enumerable.Empty<EntityField>())
        {
            entityField.Parent = entity;
            entityField.AssociationEntity?.Parent = entity;
            entityField.Formula?.FormulaBody = Parser.Parse(entityField.Formula.FormulaText);//todo
        }
        foreach (var autoCalc in entity.AutoCalcs?.Calculations ?? Enumerable.Empty<AutoCalc>())
        {
            Parser.ParseTree(autoCalc.Formula);
            if (autoCalc.Condition != null)
                Parser.ParseTree(autoCalc.Condition);
        }
    }
}
