using Neo.Bpms.Domain.Features.Definitions.Entities.Processes;
using Neo.Bpms.Domain.Models.Base;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.MetaProcess;

public interface IProjectProcess
{
    void LoadAll();
    void SaveLoadedProcess(ProjectContext projectContext, string pathPrefix = null);
    BpmnDefinitions LoadBpmn(BusinessProcess businessProcess,
        BusinessProcessVersion businessProcessVersion,
        string fileName, bool isMainBpmn);
}
public class ProjectProcess(/*ILogger logger*/) : IProjectProcess
{
    #region public methods
    private const string Folder = "Processes";

    private static string ProcessFolder(string pathPrefix = null) =>
        $"{ProjectMetaFile.MetaPath}\\{pathPrefix ?? ""}{Folder}";

    private static string BusinessProcessFolder(BusinessProcess businessProcess, string pathPrefix = null)
    {
        return $"{ProcessFolder(pathPrefix)}\\{businessProcess.Id}";
    }

    private static string ProcessVersionFileName(string processId, string versionId)
    {
        return $"{processId}_V{versionId}.xml";
    }

    private static string ProcessVersionDiagramFileName(string processId, string versionId)
    {
        return $"{processId}_V{versionId}.di.xml";
    }

    public void LoadAll()
    {
        var folder = ProcessFolder();
        if (!Directory.Exists(folder)) return;
        var directories = Directory.EnumerateDirectories(folder);
        foreach (var directory in directories)
        {
            try
            {
                LoadProcessViaPath(directory);
            }
            catch //(Exception e)
            {
                //logger.LogError(e, "LoadAll Error {message}", e.Message);
            }
        }
    }

    public BpmnDefinitions LoadBpmn(BusinessProcess businessProcess,
        BusinessProcessVersion businessProcessVersion,
        string fileName, bool isMainBpmn)
    {
        return File.Exists(fileName)
            ? BpmnImporter.ImportFromBpmnFile(businessProcess, businessProcessVersion,
                fileName, BpmnImporter.ImportType.Bpmn, isMainBpmn)
            : null;
    }

    private void LoadProcessViaPath(string businessProcessPath)
    {
        var id = ProjectMetaFile.LastDirectoryName(businessProcessPath);
        BusinessProcess newItem = null;
        try
        {
            newItem = XMLUtill.GetObjectFromXmlFile<BusinessProcess>($"{businessProcessPath}\\{id}.xml");
        }
        catch
        {
        }

        newItem ??= new BusinessProcess(id, id);
        var businessProcess = ProjectDefinition.Project.GetBusinessProcess(id);
        if (businessProcess == null)
            businessProcess = ProjectDefinition.Project.AddBusinessProcess(newItem.Id, newItem.Name);
        else
        {
            businessProcess.Name = newItem.Name;
        }

        var versionFolder = $"{businessProcessPath}\\Versions";
        var directories = Directory.EnumerateDirectories(versionFolder);
        foreach (var directory in directories)
            LoadProcessVersionViaPath(businessProcess, directory);
    }

    private void LoadProcessVersionViaPath(BusinessProcess businessProcess, string versionPath)
    {
        var versionId = ProjectMetaFile.LastDirectoryName(versionPath);
        var businessProcessVersion = businessProcess.GetVersion(versionId);
        if (businessProcessVersion == null)
        {
            var v = XMLUtill.GetObjectFromXmlFile<BusinessProcessVersion>($"{versionPath}\\{versionId}.xml")
                    ?? new BusinessProcessVersion(businessProcess, versionId, null);
            businessProcessVersion = businessProcess.AddVersion(businessProcess, v.Id, null);
        }

        var fileName = $"{versionPath}\\{ProcessVersionFileName(businessProcess.Id, versionId)}";
        var bpmnDefinitions = LoadBpmn(businessProcess, businessProcessVersion, fileName, false);
        if (bpmnDefinitions == null) return;
        businessProcessVersion.Name = bpmnDefinitions.Name + "-" + businessProcessVersion.Id;
    }

    public static void RemoveProcess(string processId)
    {
        //			var businessProcess = ProjectDefinition.Project.GetBusinessProcess(processId);
        //			ProjectMetaFile.DeleteItem(ProcessFolder(), processId, 
        //				businessProcess.Name, ProjectDefinition.Project.DeleteProcess);
        ProjectDefinition.Project.DeleteProcess(processId);
        ProjectMetaFile.DeletePhisycalDirectory(ProcessFolder(), processId);
    }

    public static void RemoveProcessVersion(string processId, string versionId)
    {
        var businessProcess = ProjectDefinition.Project.GetBusinessProcess(processId);
        //			ProjectMetaFile.DeleteItem(BusinessProcessFolder(businessProcess), versionId, 
        //				businessProcess.Name, businessProcess.DeleteVersion);
        businessProcess.DeleteVersion(versionId);
        ProjectMetaFile.DeletePhisycalDirectory(BusinessProcessFolder(businessProcess), versionId);
    }

    public static BpmnDefinitions ImportBpmnDefinitions(
        BusinessProcess businessProcess, BusinessProcessVersion businessProcessVersion,
        string bpmnXml)
    {
        var bpmnDefinitions = BpmnImporter.Import(businessProcess, businessProcessVersion,
            bpmnXml, BpmnImporter.ImportType.Bpmn, true);
        if (!bpmnDefinitions?.ErrorInfos.Any(e => e.Type <= eErrorLevel.Error) ?? false)
            SaveBpmnDefinitionsXml(businessProcess, businessProcessVersion, bpmnXml, true);
        return bpmnDefinitions;
    }

    public void SaveLoadedProcess(ProjectContext projectContext, string pathPrefix = null)
    {
        foreach (var businessProcess in projectContext.BusinessProcesses.Values)
        {
            foreach (var businessProcessVersion in businessProcess.Versions.Values)
            {
                SaveBusinessProcessVersion(businessProcess, businessProcessVersion, pathPrefix);
            }
        }
    }

    public static void SaveBusinessProcessVersion(BusinessProcess businessProcess,
        BusinessProcessVersion businessProcessVersion, string pathPrefix = null)
    {
        var bpmnXml = BpmnExporter.Export(businessProcessVersion.BpmnDefinitions, null, BpmnExporter.ExportType.Bpmn);
        SaveBpmnDefinitionsXml(businessProcess, businessProcessVersion,
            bpmnXml, false, pathPrefix);
    }

    public static ElasticObject GetDiagramBpmnDefinition(string processId, string versionId)
    {
        if (string.IsNullOrEmpty(versionId))
            versionId = "1.0";
        var businessProcess = ProjectDefinition.Project.GetBusinessProcess(processId);
        var businessProcessVersion = businessProcess?.GetVersion(versionId);
        if (businessProcessVersion == null)
            return null;
        string diagramFolder =
            $"{BusinessProcessFolder(businessProcess, "Diagrams")}\\Versions\\{businessProcessVersion.Id}";
        string fileName = $"{diagramFolder}\\{ProcessVersionDiagramFileName(businessProcess.Id, versionId)}";
        return File.Exists(fileName) ? BpmnImporter.ReadProcessFile(fileName) : null;
    }

    public static string SaveBusinessProcess(
        BusinessProcess businessProcess, BusinessProcessVersion businessProcessVersion,
        string pathPrefix = null)
    {
        string folder = BusinessProcessFolder(businessProcess, pathPrefix);
        ProjectMetaFile.InitDirectory(folder);
        ProjectMetaFile.SaveObjectToFile(businessProcess, folder, $"{businessProcess.Id}.xml");
        string versionFolder =
            $"{BusinessProcessFolder(businessProcess, pathPrefix)}\\Versions\\{businessProcessVersion.Id}";
        ProjectMetaFile.InitDirectory(versionFolder);
        ProjectMetaFile.SaveObjectToFile(businessProcessVersion, versionFolder, $"{businessProcessVersion.Id}.xml");
        return versionFolder;
    }

    public static void SaveBpmn(BpmnDefinitions bpmnDefinitions, string path, string fileName)
    {
        var bpmnXml = BpmnExporter.Export(bpmnDefinitions, null, BpmnExporter.ExportType.Bpmn);
        ProjectMetaFile.SaveFile(bpmnXml, path, fileName);
    }

    public static void AddBusinessProcessIfIsNull(
        ref BusinessProcess businessProcess, ref BusinessProcessVersion businessProcessVersion,
        string processId, string name, Entity entity, string versionId)
    {
        businessProcess ??= ProjectDefinition.Project.GetBusinessProcess(processId);
        if (businessProcess == null)
            businessProcess = ProjectDefinition.Project.AddBusinessProcess(processId, name);
        else
        {
            businessProcess.Name = name;
            // businessProcess.CurrentVersion
        }

        businessProcessVersion ??= businessProcess.GetVersion(versionId);
        if (businessProcessVersion == null)
        {
            businessProcessVersion = businessProcess.AddVersion(businessProcess, versionId,
                ProcessDefinition.CreateBpmnDefinitionsAndProcess(processId, name, entity, out var process));
            process.isExecutable = true;
            process.Status = Process.ProcessStatus.Active;
            process.laneSets =
            [
                new LaneSet(process, processId + ".LaneSet", "lane set", null)
                {
                    lanes =
                    [
                        new Lane(process, processId + ".Lane", "lane")
                        {
                            flowNodeRefs = ["Start"]
                        }
                    ]
                }
            ];
            process.flowElements.Add("Start", new StartEvent(process, "Start", "start"));
            SaveBusinessProcessVersion(businessProcess, businessProcessVersion);
        }

        SaveBusinessProcess(businessProcess, businessProcessVersion);
    }

    public static void FetchBusinessProcessIfIsNull(ref BusinessProcess businessProcess,
        ref BusinessProcessVersion businessProcessVersion, string processId, string versionId)
    {
        versionId ??= "1.0";
        businessProcess ??= ProjectDefinition.Project.GetBusinessProcess(processId);
        businessProcessVersion ??= businessProcess?.GetVersion(versionId);
    }
    #endregion

    #region private methods
    private static void SaveBpmnDefinitionsXml(
        BusinessProcess businessProcess, BusinessProcessVersion businessProcessVersion,
        string bpmnXml, bool saveDiagram, string pathPrefix = null)
    {
        var versionFolder = SaveBusinessProcess(businessProcess, businessProcessVersion,
            pathPrefix);
        var fileName = ProcessVersionFileName(businessProcess.Id, businessProcessVersion.Id);
        ProjectMetaFile.SaveFile(bpmnXml, versionFolder, fileName);
        ProjectMetaFile.SaveFile(bpmnXml, versionFolder + "\\History", DateTime.UtcNow.ToString("s") + "_" + fileName);
        if (!saveDiagram) return;
        var diagramFolder =
            $"{BusinessProcessFolder(businessProcess, (string.IsNullOrEmpty(pathPrefix) ? "" : pathPrefix + "\\") + "Diagrams")}\\Versions\\{businessProcessVersion.Id}";
        ProjectMetaFile.SaveFile(bpmnXml, diagramFolder,
            ProcessVersionDiagramFileName(businessProcess.Id, businessProcessVersion.Id));
    }
    #endregion
}
