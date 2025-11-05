global using Neo.Bpms.Infrastructure.Features.Orm.Entities.MetaDb;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Services;
using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Modeling.Entities.ProcessModel;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.MetaDataPart;

public class BPMNMetaDataManager(Repository repository) : MetaDataManager<BPMNMetaData>
{
    public Repository Repository = repository;

    protected override void SyncMetaData()
    {
        SyncInterfacesAndOperations();
        SyncProcesses();
    }
    protected override void BackupUnusedItems()
    {
        BackupUnusedItems(_metaData.Processes.Values);
        BackupUnusedItems(_metaData.ProcessVersions.Values.SelectMany(f => f.Values));
        BackupUnusedItems(_metaData.Interfaces.Values);
        BackupUnusedItems(_metaData.Operations.Values.SelectMany(f => f.Values));
        BackupUnusedItems(_metaData.FlowNodes.Values.SelectMany(f => f.Values));
    }

    private void SyncInterfacesAndOperations()
    {
        foreach (Interface @interface in ProjectDefinition.Project.Interfaces)
        {
            long interfaceId = FetchInterface(@interface.Id, @interface.Name);
            foreach (Operation operationRef in @interface.operations.Values)
            {
                FetchOperation(operationRef, interfaceId);
            }
        }
    }

    private void SyncProcesses()
    {
        foreach (var processRunTime in Repository.processesRunTimes.Values)
        {
            var bpmnProcess = FetchBPMNProcess(processRunTime);
            Dictionary<string, BPMNProcessVersion> bpmnProcessVersions = GetBPMNProcessVersions(bpmnProcess, true);
            Dictionary<string, BPMNProcessVersion> backupBpmnProcessVersions = GetBPMNProcessVersions(bpmnProcess, false);
            BusinessProcess businessProcess = ProjectDefinition.Project.GetBusinessProcess(bpmnProcess.ProcessId);
            foreach (var versionRuntime in processRunTime.Versions.Values)
            {
                SyncProcessesVersions(bpmnProcessVersions, backupBpmnProcessVersions, versionRuntime, bpmnProcess, businessProcess);
            }
        }
    }

    private void SyncProcessesVersions(IDictionary<string, BPMNProcessVersion> bpmnProcessVersions,
        Dictionary<string, BPMNProcessVersion> backupBpmnProcessVersions,
        ProcessVersionRuntime versionRuntime, BPMNProcess bpmnProcess, BusinessProcess businessProcess)
    {
        if (backupBpmnProcessVersions == null)
        {
            throw new ArgumentNullException(nameof(backupBpmnProcessVersions));
        }

        var processVersion =
            FetchProcessVersion(bpmnProcessVersions, backupBpmnProcessVersions, versionRuntime, bpmnProcess);
        Repository.ProcessVersionRunTimes.TryAdd(versionRuntime.DbId, versionRuntime);
        BusinessProcessVersion businessProcessVersion = businessProcess.GetVersion(processVersion.Version);
        if (businessProcessVersion != null)
        {
            businessProcessVersion.AdministratorLock = processVersion.AdministratorLock;
            businessProcessVersion.CheckInUserId = processVersion.CheckInUserId;
            businessProcessVersion.DbId = processVersion.Id;
        }
        if (versionRuntime.nodes == null)
        {
            return;
        }

        var bpmnFlowNodes = _metaData.FlowNodes.AddOrGetItem(processVersion.Id);
        var backupBpmnFlowNodes = _backupMetaData.FlowNodes.AddOrGetItem(processVersion.Id);
        foreach (FlowNodeRunTime nodeRunTime in versionRuntime.nodes.Values)
        {
            SyncRuntimeFlowNodes(bpmnFlowNodes, backupBpmnFlowNodes, nodeRunTime);
        }
    }

    private void SyncRuntimeFlowNodes(IDictionary<string, BPMNFlowNode> bpmnFlowNodes,
        IDictionary<string, BPMNFlowNode> backupBpmnFlowNodes, FlowNodeRunTime nodeRunTime)
    {
        try
        {
            FetchFlowNode(bpmnFlowNodes, backupBpmnFlowNodes, nodeRunTime);
            Repository.FlowNodeRunTimes.TryAdd(nodeRunTime.DbId, nodeRunTime);
        }
        catch (Exception e)
        {
            AddError("SyncRuntimeFlowNodes", "SyncRuntimeFlowNodes", e);
        }
    }

    private BPMNProcess FetchBPMNProcess(ProcessRunTime processRunTime)
    {
        var fetchItem = FetchItem(
            new BPMNProcess
            {
                ProcessId = processRunTime.Id,
                Name = processRunTime.Name
            }, t => t.ProcessId,
            (item, item2) =>
                item.Name != item2.Name ||
                item.ProcessId != item2.ProcessId,
            (item, item2) =>
            {
                item.Name = item2.Name;
                item.ProcessId = item2.ProcessId;
                return true;
            }, _metaData.Processes, _backupMetaData.Processes, processRunTime.Id);
        processRunTime.DbId = fetchItem.Id;
        return fetchItem;
    }

    private Dictionary<string, BPMNProcessVersion> GetBPMNProcessVersions(BPMNProcess process, bool bActive)
    {
        return (bActive ? _metaData : _backupMetaData).ProcessVersions.AddOrGetItem(process.Id);
    }

    private BPMNProcessVersion FetchProcessVersion(IDictionary<string, BPMNProcessVersion> processVersions,
        IDictionary<string, BPMNProcessVersion> backupProcessVersions,
        ProcessVersionRuntime versionRuntime, BPMNProcess bpmnProcess)
    {
        long? metaEntityId =
            versionRuntime.BusinessProcessVersion?.BpmnDefinitions?.Process?.Entity?.DbId;
        string name = versionRuntime.BusinessProcessVersion?.Name;
        var fetchItem = FetchItem(
            new BPMNProcessVersion
            {
                ProcessId = bpmnProcess.Id,
                Version = versionRuntime.VersionNo,
                MetaEntityId = metaEntityId,
                Name = name,
            }, t => t.Version,
            (item, item2) =>
                item.Name != item2.Name ||
                item.MetaEntityId != item2.MetaEntityId,

        (item, item2) =>
            {
                item.Name = item2.Name;
                item.MetaEntityId = item2.MetaEntityId;
                return true;
            }, processVersions, backupProcessVersions, versionRuntime.VersionNo);
        versionRuntime.DbId = fetchItem.Id;
        return fetchItem;
    }

    private void FetchFlowNode(IDictionary<string, BPMNFlowNode> flowNodes,
        IDictionary<string, BPMNFlowNode> backupFlowNodes, FlowNodeRunTime flowNodeRunTime)
    {
        FlowNodeTypeId flowNodeTypeId = flowNodeRunTime.FlowNodeTypeId;
        var operationId = FetchFlowNodeOperation(flowNodeRunTime)?.Id;
        long? eventTriggerTypeId = flowNodeRunTime.flowNode is Event @event ? (long?)@event.TriggerType : null;
        var activity = flowNodeRunTime.flowNode as Activity;
        UserTaskRuntime userTaskRuntime = flowNodeRunTime as UserTaskRuntime;
        if (userTaskRuntime != null)
        {
            userTaskRuntime.RenderingFormDbId = ((UiEntity)flowNodeRunTime.ProcessVersion.definition.Entity)?.getForm(userTaskRuntime.RenderingFormId)?.DbId;
            userTaskRuntime.RenderingIndexFormDbId = ((UiEntity)flowNodeRunTime.ProcessVersion.definition.Entity)?.getForm(userTaskRuntime.RenderingIndexFormId)?.DbId;
        }

        var fetchItem = FetchItem(
            new BPMNFlowNode
            {
                ProcessVersionId = flowNodeRunTime.ProcessVersion.DbId,
                FlowNodeId = flowNodeRunTime.flowNode.Id,
                Name = flowNodeRunTime.flowNode.Name,
                FlowNodeTypeId = flowNodeTypeId,
                OperationId = operationId,
                EventTriggerTypeId = eventTriggerTypeId,
                RenderingFormId = userTaskRuntime?.RenderingFormDbId,
                RenderingIndexFormId = userTaskRuntime?.RenderingIndexFormDbId,
                startQuantity = activity?.startQuantity,
                CriticalTimeToDo = activity?.CriticalTimeToDo,
                EmergencyTimeToDo = activity?.EmergencyTimeToDo
            }, t => t.FlowNodeId,
            (item, item2) =>
                item.Name != item2.Name ||
                item.FlowNodeTypeId != item2.FlowNodeTypeId ||
                item.OperationId != item2.OperationId ||
                item.EventTriggerTypeId != item2.EventTriggerTypeId ||
                item.startQuantity != item2.startQuantity ||
                item.RenderingFormId != item2.RenderingFormId ||
                item.RenderingIndexFormId != item2.RenderingIndexFormId ||
                item.CriticalTimeToDo != item2.CriticalTimeToDo ||
                item.EmergencyTimeToDo != item2.EmergencyTimeToDo,
            (item, item2) =>
            {
                item.Name = item2.Name;
                item.FlowNodeTypeId = item2.FlowNodeTypeId;
                item.OperationId = item2.OperationId;
                item.EventTriggerTypeId = item2.EventTriggerTypeId;
                item.RenderingFormId = item2.RenderingFormId;
                item.RenderingIndexFormId = item2.RenderingIndexFormId;
                item.startQuantity = item2.startQuantity;
                item.CriticalTimeToDo = item2.CriticalTimeToDo;
                item.EmergencyTimeToDo = item2.EmergencyTimeToDo;
                return true;
            }, flowNodes, backupFlowNodes, flowNodeRunTime.flowNode.Id);
        flowNodeRunTime.DbId = fetchItem.Id;
    }

    private BPMNOperation FetchFlowNodeOperation(FlowNodeRunTime flowNodeRunTime)
    {
        ServiceTask serviceTask = flowNodeRunTime.flowNode as ServiceTask;
        Interface @interface = (Interface)serviceTask?.operationRef?.Parent;
        string interfaceName = @interface?.Name;
        if (interfaceName == null)
        {
            return null;
        }

        if (serviceTask.operationRef == null)
        {
            return null;
        }

        long interfaceId = FetchInterface(@interface.Id, interfaceName);
        return FetchOperation(serviceTask.operationRef, interfaceId);
    }

    private BPMNOperation FetchOperation(Operation operationRef, long interfaceId)
    {
        string operationName = operationRef.Name;
        var operations = _metaData.Operations.AddOrGetItem(interfaceId);
        var backupOperations = _backupMetaData.Operations.AddOrGetItem(interfaceId);
        var fetchItem = FetchItem(
            new BPMNOperation
            {
                InterfaceId = interfaceId,
                OperationId = operationRef.Id,
                Name = operationName
            }, t => t.OperationId,
            (item, item2) =>
                item.Name != item2.Name ||
                item.OperationId != item2.OperationId,
            (item, item2) =>
            {
                item.Name = item2.Name;
                item.OperationId = item2.OperationId;
                return true;
            }, operations, backupOperations, operationRef.Id);
        return fetchItem;
    }

    private long FetchInterface(string interfaceId, string interfaceName)
    {
        var fetchItem = FetchItem(
            new BPMNInterface
            {
                InterfaceId = interfaceId,
                Name = interfaceName
            }, t => t.InterfaceId,
            (item, item2) =>
                item.Name != item2.Name ||
                item.InterfaceId != item2.InterfaceId,
            (item, item2) =>
            {
                item.Name = item2.Name;
                item.InterfaceId = item2.InterfaceId;
                return true;
            }, _metaData.Interfaces, _backupMetaData.Interfaces, interfaceId);
        return fetchItem.Id;
    }
}
