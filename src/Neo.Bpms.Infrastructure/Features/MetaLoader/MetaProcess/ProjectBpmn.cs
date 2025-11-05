using Neo.Bpms.Domain.Entities.Bpmn.Extensions.BusinessProcesses;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.MetaProcess;

public interface IProjectBpmn
{
    void Load(string pathPrefix = null);
    void Save(string pathPrefix = null);
}
public class ProjectBpmn(IProjectProcess projectProcess) : IProjectBpmn
{
    private const string FileName = "bpmn.xml";
    private const string Folder = "Bpmn";
    public static string Path(string pathPrefix = null) => $"{ProjectMetaFile.MetaPath}\\{pathPrefix ?? ""}{Folder}";

    public void Load(string pathPrefix = null)
    {
        var folder = Path(pathPrefix);
        if (!Directory.Exists(folder)) return;

        var businessProcess = new BusinessProcess(null, null);
        var businessProcessVersion = businessProcess.AddVersion(businessProcess, "1.0", null);
        var fileName = $"{folder}\\{FileName}";
        var BpmnDefinitions = projectProcess.LoadBpmn(businessProcess, businessProcessVersion, fileName, true);
        if (BpmnDefinitions == null) return;
        businessProcessVersion.Name = BpmnDefinitions.Name + "-" + businessProcessVersion.Id;

        foreach (var loadedRootElement in BpmnDefinitions.GetRootElements() ??
                                                    [])
        {
            var rootElement =
             ProjectDefinition.Project.BpmnDefinitions.GetRootElement(loadedRootElement.Id);
            if (rootElement == null)
                ProjectDefinition.Project.BpmnDefinitions.AddRootElement(loadedRootElement);
            else
            {
                if (loadedRootElement.GetType() != rootElement.GetType())
                    continue;
                rootElement.Copy(loadedRootElement);
            }
        }
    }

    public void Save(string pathPrefix = null)
    {
        ProjectProcess.SaveBpmn(ProjectDefinition.Project.BpmnDefinitions,
            Path(pathPrefix), FileName);
    }
}
