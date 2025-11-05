namespace Neo.Bpms.Domain.Model.Project;

public partial class ProjectContext
{
    public Dictionary<string, BusinessProcess> BusinessProcesses { get; } = [];

    protected virtual void InitializeProcesses()
    {
    }

    public BusinessProcess GetBusinessProcess(string businessProcessId)
    {
        if (string.IsNullOrEmpty(businessProcessId))
            return null;
        BusinessProcesses.TryGetValue(businessProcessId, out var businessProcess);
        return businessProcess;
    }

    public BusinessProcess AddBusinessProcess(string businessProcessId, string businessProcessName)
    {
        var businessProcess = GetBusinessProcess(businessProcessId);
        if (businessProcess != null)
            return businessProcess;
        businessProcess = new BusinessProcess(businessProcessId, businessProcessName);
        BusinessProcesses.Add(businessProcess.Id, businessProcess);
        return businessProcess;
    }

    public bool DeleteProcess(string processId)
    {
        return BusinessProcesses.Remove(processId);
    }

    public bool AddProcessDefinition(BpmnDefinitions bpmnDefinitions, string versionId)
    {
        var businessProcess = AddBusinessProcess(bpmnDefinitions.Id, bpmnDefinitions.Name);

        var businessProcessVersion = businessProcess.TryAddVersion(businessProcess, versionId, bpmnDefinitions);
        if (businessProcessVersion != null)
            businessProcessVersion.BpmnDefinitions = bpmnDefinitions;
        return businessProcessVersion != null;
    }

    public BpmnDefinitions GetBpmnDefinition(string processId, string versionId)
    {
        if (processId == null || !BusinessProcesses.TryGetValue(processId, out var businessProcess))
            return null;

        BusinessProcessVersion businessProcessVersion;
        if (string.IsNullOrEmpty(versionId))
            businessProcessVersion = businessProcess.ActiveVersion;
        else
            businessProcess.Versions.TryGetValue(versionId, out businessProcessVersion);
        return businessProcessVersion?.BpmnDefinitions;
    }

    public bool ProcessExists(string processIdVersionId)
    {
        if (string.IsNullOrEmpty(processIdVersionId))
            return false;
        BusinessProcessVersion.FetchVersionId(processIdVersionId, out var processId, out _);
        return !string.IsNullOrEmpty(processId) && BusinessProcesses.ContainsKey(processId);
    }

    public Form GetProcessTaskForm(string processIdVersionId, string taskId)
    {
        if (!FetchProcessVersion(processIdVersionId, out var processVersion) || processVersion == null)
            return null;
        if (!FetchTask(processVersion, taskId, out var userTask) || userTask == null)
            return null;
        var entity = GetEntity(
            processVersion.BpmnDefinitions?.Process.EntityNamespaceId,
            processVersion.BpmnDefinitions?.Process.EntityId);
        return entity?.GetForm(userTask.FormId) as Form;
    }

    private bool FetchProcessVersion(string processIdVersionId, out BusinessProcessVersion processVersion)
    {
        processVersion = null;
        BusinessProcessVersion.FetchVersionId(processIdVersionId, out var processId, out var versionId);
        if (!BusinessProcesses.TryGetValue(processId, out var process))
            return false;
        processVersion = process.GetVersion(versionId);
        return processVersion != null;
    }

    private static bool FetchTask(BusinessProcessVersion processVersion, string taskId, out UserTask userTask)
    {
        userTask = processVersion.BpmnDefinitions.Process.GetActivity(taskId) as UserTask;
        return userTask != null;
    }

    /*
				public Dictionary<string, Module.Module> modules { get; } = new Dictionary<string, Module.Module>();
				public Dictionary<string, SoftwareSystem> softwareSystems { get; } = new Dictionary<string, SoftwareSystem>();
				public Dictionary<string, SubSystem> subSystems { get; } = new Dictionary<string, SubSystem>();
				public bool AddModule(Module.Module module)
				{
					if (modules.ContainsKey(module.Id)) return false;
					modules.Add(module.Id, module);
					foreach (var system in module.Systems.Values)
					{
						if (!AddSystem(system)) return false;
					}
					return true;
				}

				public bool AddSubSystem(SubSystem subsystem)
				{
					if (subSystems.ContainsKey(subsystem.Id)) return false;
					subSystems.Add(subsystem.Id, subsystem);
					foreach (var businessProcess in subsystem.BusinessProcesses.Values)
					{
						if (!AddBusinessProcess(businessProcess))
							return false;
					}
					foreach (var subSubSystem in subsystem.SubSystems.Values)
					{
						if (!AddSubSystem(subSubSystem))
							return false;
					}
					foreach (var ns in subsystem.Namespaces.Values)
					{
						if (!AddNamespace(ns)) return false;
					}
					return true;
				}

				public bool AddSystem(SoftwareSystem system)
				{
					if (softwareSystems.ContainsKey(system.Id)) return false;
					softwareSystems.Add(system.Id, system);
					foreach (var subsystem in system.subSystems.Values)
					{
						if (!AddSubSystem(subsystem)) return false;
					}
					foreach (var diagram in system.diagrams.Values)
					{
						if (!AddDiagram(diagram)) return false;
					}
					return true;
				}

				public bool AddBusinessProcess(BusinessProcess businessProcess)
				{
					foreach (var proc in businessProcess.ProcessDefinitions)
					{
						var procDef = proc as BpmnDefinitions;
						if (procDef == null) return false;
						if (!AddProcessDefinition(procDef)) return false;
					}
					return true;
				}
		*/
}
