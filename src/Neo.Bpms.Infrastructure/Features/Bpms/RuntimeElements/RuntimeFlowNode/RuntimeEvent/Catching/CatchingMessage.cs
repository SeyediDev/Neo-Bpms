global using Neo.Bpms.Domain.Utility.Extensions;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;
internal class CatchingMessage : CatchingEvent
{
    private MessageEventDefinition _message => EventDefinition as MessageEventDefinition;

    internal CatchingMessage(FlowNodeRunTime flowNodeRuntime, MessageEventDefinition messageDefinition,
        ExecutionInstance execution, ProcessInstance pi, FlowNodeInstance ai, LocalParameters inputData)
        : base(flowNodeRuntime, messageDefinition, execution, pi, ai, inputData)
    {
    }

    #region start
    protected override bool CatchingInStartEvent()
    {
        CorrelateMessageParams(out var dataOutputs);
        var entityPkv = ProcessVersion.FetchEntityPkv(InputData);
        if (!flowNode.InSubProcess)
        {
            Pi = CreateProcessInstanceAndCatching(dataOutputs, entityPkv);
        }
        else
        {
            Pi = DataStorage.LoadProcessInstances(ProcessVersion, Execution, entityPkv)?.Values.FirstOrDefault();
            try
            {
                Pi = DataStorage.LockControl(Pi, "catch message in event sub process start", out _);
                CatchingInStartEventInPi();
            }
            catch
            {
                // ignored
            }
        }

        return Pi != null;
    }
    #endregion

    protected override bool CatchingFlowNode(FlowNodeRunTime flowNodeRuntime)
    {
        return CatchCorrelatedFlowInstance(flowNodeRuntime, $"Catching message {_message.Code}");
    }

    protected override bool CatchingInBoundaryEvent(BoundaryEvent boundaryEvent)
    {
        return ProcessVersion.TryGetActivityRuntime(boundaryEvent.attachedToRef, out var activityRunTime) &&
               CatchCorrelatedFlowInstance(activityRunTime, $"Catching in boundaryEvent {_message.Code}");
    }

    private bool CatchCorrelatedFlowInstance(FlowNodeRunTime runTime, string traceCode)
    {
        var entityFilters = GenerateCorrelateMessageFilter(out var queryMessageParams);
        Ai = DataStorage.CorrelateFlowNodeInstances(Execution, runTime, queryMessageParams, [.. entityFilters])
            ?.FirstOrDefault();
        Ai = DataStorage.LockControl(Ai, traceCode);
        Pi = Ai?.pi;
        if (Pi == null)
        {
            Execution.AuditTrail.AddDetail((long)BPMNAuditDetailTypeId.Info,
            $"Correlated PI not found {traceCode}. filters: {string.Join(" and ", entityFilters)}",
            ProcessVersion.DbId, Runtime.DbId, Pi?.Id, Ai?.Id);
            return false;
        }
        CatchAndSavePi();
        return true;
    }

    private void CorrelateMessageParams(out List<DataOutput> dataOutputs)
    {
        var catchEvent = flowNode as CatchEvent;
        dataOutputs = catchEvent?.dataOutputs;
        if (_message.messageRef?.itemRef?.structure?.entityFields == null) return;
        if ((dataOutputs?.Count ?? 0) == 0)
        {
            dataOutputs = [.. _message.messageRef.itemRef.structure.entityFields.Values.Select(entityField =>
                {
                    var property = ProcessVersion.definition.GetProperty(entityField.DbFieldName);
                    if (property == null)
                        return null;
                    var id = catchEvent?.Id + ".DataOutputAssociation." + entityField.Id;
                    var dataOutput = new DataOutput(catchEvent, id, entityField.Id,
                        new ItemDefinition(null, id + ".ItemDefinition", false, entityField), false);
                    return dataOutput;
                })
                .Where(d => d != null)];
        }

        foreach (var entityField in _message.messageRef.itemRef.structure.entityFields.Values)
        {
            if (!string.IsNullOrEmpty(entityField.DbFieldName) && entityField.DbFieldName != entityField.Id)
            {
                var v = InputData.GetItem(entityField.Id);
                if (v != null)
                    InputData.AddOrUpdate(entityField.DbFieldName, v);
            }
        }
    }

    private List<string> GenerateCorrelateMessageFilter(out LocalParameters queryMessageParams)
    {
        queryMessageParams = [];
        var filters = new List<string>();
        if (InputData == null || InputData.Count == 0) return filters;
        if (_message.messageRef?.itemRef?.structure?.entityFields == null) return filters;
        foreach (var messageKeyField in _message.messageRef.itemRef.structure.KeyFields)
        {
            var id = $"__{messageKeyField.Id}";
            filters.Add($"{messageKeyField.DbFieldName}=={id}");
            queryMessageParams.Add(id, InputData.Get(messageKeyField.Id));
        }

        return filters;
    }
}
