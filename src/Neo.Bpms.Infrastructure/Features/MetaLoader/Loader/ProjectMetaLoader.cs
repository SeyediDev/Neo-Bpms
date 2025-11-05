using Neo.Bpms.Domain.Entities.Cmmn.Data.Provider;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Interfaces;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Projects;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.Loader;

public interface IProjectMetaLoader
{
    void Load(bool loadMenu = true, bool loadUi = true, bool loadBpmn = true, bool syncMetaData = true);
}

public class ProjectMetaLoader<TProjectMetaDefinition, TServiceDefinition, TMenuDefinition>
    (IDataProviderContainer dataProviderContainer, IConfiguration configuration,
    ISpecificEntitiesLoader specificEntitiesLoader, IMetaDataLoader metaDataLoader,
    IEntityLoader<TProjectMetaDefinition> entityLoader,
    ILogger<ProjectMetaLoader<TProjectMetaDefinition, TServiceDefinition, TMenuDefinition>> logger)
    : IProjectMetaLoader
    where TProjectMetaDefinition : ProjectMetaDefinition, new()
    where TServiceDefinition : ServiceDefinition, new()
    where TMenuDefinition : MenuDefinition, new()
{
    public void Load(bool loadMenu = true, bool loadUi = true, bool loadBpmn = true, bool syncMetaData = true)
    {
        DataSourceProviderManager.ProviderContainer = dataProviderContainer;
        SetConfig();
        LoadDataSourceProviders();

        TProjectMetaDefinition metaDefinition = LoadEntityDefinitions(loadUi);
        LoadedMetaDefinition(metaDefinition);
        new TServiceDefinition().DefineAll();

        if (loadBpmn)
        {
            LoadBpmnDefinition(metaDefinition);
        }

        if (loadMenu)
        {
            LoadMenu();
        }

        if (ProjectDefinition.SaveLoadedMeta)
        {
            specificEntitiesLoader.SaveLoadedMeta();
        }

        metaDataLoader.LoadAndSyncMetaData(syncMetaData);
        logger.LogTrace("Project Loaded.");
    }

    protected virtual void LoadedMetaDefinition(TProjectMetaDefinition metaDefinition)
    {
    }

    private void LoadDataSourceProviders()
    {
        logger.LogTrace("Setup Provider Container.");
        DataSourceProviderManager.ProviderContainer.Init();

        foreach (IDataProvider provider in DataSourceProviderManager.ProviderContainer)
        {
            logger.LogTrace("Initialize {providerName} provider.", provider.Name);
            provider.Init();
        }

        //ConnectionManager.InitializeBaseConnection();//todo
    }

    private TProjectMetaDefinition LoadEntityDefinitions(bool loadUi)
    {
        logger.LogTrace("Start Load Meta.");
        TProjectMetaDefinition metaDefinition = new();
        ProjectDefinition.Project = metaDefinition.Identify();
        entityLoader.DefineAllNamespaces(metaDefinition);
        entityLoader.LoadEntity(metaDefinition, loadUi);
        return metaDefinition;
    }

    private void LoadMenu()
    {
        ProjectDefinition.Project.MainMenuItem = new TMenuDefinition().DefineAll();
        specificEntitiesLoader.LoadSpecificMenu();
    }

    private void LoadBpmnDefinition(TProjectMetaDefinition metaDefinition)
    {
        InterfaceLoader.DefineInterfacesToBPMN(ProjectDefinition.Project);
        metaDefinition.LoadBPMNDefinitions();
        specificEntitiesLoader.LoadSpecificProcess();
    }
    
    protected void SetConfig()
    {
        ProjectDefinition.SaveLoadedMeta = FetchBooleanConfig("SaveLoadedMeta");
        ProjectDefinition.DontSyncMetaData = FetchBooleanConfig("DontSyncMetaData");
    }

    protected static string GetConfig(IConfiguration config, string name, string defaultValue = "")
    {
        try
        {
            string c = config[name];
            if (string.IsNullOrEmpty(c))
            {
                c = defaultValue;
            }

            return c;
        }
        catch
        {
            return defaultValue;
        }
    }

    private bool FetchBooleanConfig(string key)
    {
        string value = configuration[key];
        return !string.IsNullOrEmpty(value) && Convert.ToBoolean(value);
    }
}
