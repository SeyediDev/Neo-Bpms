using Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;
using Neo.Bpms.Infrastructure.Features.MetaLoader.MetaProcess;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.Loader;

public interface ISpecificEntitiesLoader 
{
    void SaveLoadedMeta();
    void LoadSpecificProcess();
    void LoadSpecificMenu();
    void LoadSpecificEntities();
    void LoadSpecificUi();
}
public class SpecificEntitiesLoader(
    IProjectProcess projectProcess, IProjectNamespace projectNamespace, 
    IProjectMenu projectMenu, IProjectBpmn projectBpmn
    ) : ISpecificEntitiesLoader
{
    public void LoadSpecificEntities()
    {
        projectNamespace.LoadStructure();
    }

    public void LoadSpecificUi()
    {
        projectNamespace.LoadUi();
    }

    public void LoadSpecificProcess()
    {
        projectBpmn.Load();
        projectProcess.LoadAll();
    }

    public void LoadSpecificMenu()
    {
        projectMenu.Load();
    }

    public void SaveLoadedMeta()
    {
        projectMenu.SaveAll("SaveLoaded\\");
        projectNamespace.SaveAllNamespaces("SaveLoaded\\");
        projectBpmn.Save("SaveLoaded\\");
        projectProcess.SaveLoadedProcess(ProjectDefinition.Project, "SaveLoaded\\");
    }
}
