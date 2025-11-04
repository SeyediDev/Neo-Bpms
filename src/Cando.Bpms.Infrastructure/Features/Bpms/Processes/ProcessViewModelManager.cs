using Neo.Bpms.Domain.Entities.Bpmn.Extensions.ResourceRoles;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Gateways;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.CallActivity;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.ThrowEvent;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes;

public static class ProcessViewModelManager
{
    public static List<ProcessViewModel> GetProcessViewModels(string processId)
    {
        List<ProcessViewModel> res = [];
        foreach (ProcessVersionRuntime process in ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.processesRunTimes.Values.SelectMany(p => p.Versions.Values))
        {
            if (!string.IsNullOrEmpty(processId) && processId != process.definition.Id)
                continue;
            ProcessViewModel processViewModel = new()
            {
                Id = process.definition.Id,
                Name = process.definition.Name,
                EntityId = process.definition.EntityId,
                InputParams = process.definition.ioSpecification?.dataInputs?.Select(d => d.Name).ToList() ??
                              [],
                OuputParams = process.definition.ioSpecification?.dataOutputs?.Select(d => d.Name).ToList() ??
                              [],
                Properties = process.definition.properties?.Select(prop => prop.Name).ToList() ??
                              []
            };
            foreach (ResourceRole resourceRole in process.definition.resources ?? Enumerable.Empty<ResourceRole>())
            {
                if ((resourceRole as ProcessManagerResource)?.resourceRef is not HumanResource resource) continue;
                ResourceViewModel managerViewModel = new()
                {
                    Type = resource.type.ToString(),
                    Code = resource.Id ?? "",
                    Name = GetResourceName(resource),
                    Claim = resource.UserClaimRestrictions?.FirstOrDefault()
                };
                processViewModel.Managers.Add(managerViewModel);
            }
            foreach (Domain.Entities.Bpmn.Processes.Lanes.LaneSet laneSet in process.definition.laneSets)
                foreach (Domain.Entities.Bpmn.Processes.Lanes.Lane lane in laneSet.lanes)
                {
                    LaneViewModel laneViewModel = new()
                    {
                        Id = lane.Id ?? laneSet.Id,
                        Name = lane.Name ?? laneSet.Name,
                        ParentId = laneSet.parentLane?.Id ?? "",
                    };
                    int colorIndex = 0;
                    foreach (string flowNodeRef in lane.flowNodeRefs)
                    {
                        if (!process.definition.flowElements.TryGetValue(flowNodeRef, out FlowElement flowElement))
                            continue;
                        FlowNode element = flowElement as FlowNode;
                        ElementViewModel elementViewModel = new()
                        {
                            Id = element.Id,
                            Name = element.Name,
                            Type = element.nodeType.ToString(),
                            ColorIndex = colorIndex++,
                        };
                        laneViewModel.Elements.Add(elementViewModel);
                        switch (element.nodeType)
                        {
                            case FlowNode.eFlowNodeType.Gateway:
                                elementViewModel.Type = (element as Gateway)?.gatewayType.ToString() ?? elementViewModel.Type;
                                break;
                            case FlowNode.eFlowNodeType.Activity:
                                Activity activity = element as Activity;
                                elementViewModel.Type = activity?.ActivityType.ToString() ?? elementViewModel.Type;

                                switch (activity?.ActivityType)
                                {
                                    case Activity.eActivityType.CallActivitySubProcess:
                                        elementViewModel.ProcessLink = (activity as CallActivity).CalledElementId;
                                        break;
                                    case Activity.eActivityType.AdhocSubProcess:
                                        elementViewModel.ProcessLink = (activity as SubProcess).Id;
                                        break;
                                    case Activity.eActivityType.TransactionSubProcess:
                                        elementViewModel.ProcessLink = (activity as SubProcess).Id;
                                        break;
                                    case Activity.eActivityType.EventSubProcess:
                                        elementViewModel.ProcessLink = (activity as SubProcess).Id;
                                        break;
                                    case Activity.eActivityType.EmbeddedSubProcess:
                                        elementViewModel.ProcessLink = (activity as SubProcess).Id;
                                        break;
                                }
                                if (activity?.resources != null)
                                {   //todo
                                    HumanResource resource = activity.resources?.FirstOrDefault()?.resourceRef as HumanResource;
                                    laneViewModel.Performer.Type = resource?.type.ToString() ?? "";
                                    laneViewModel.Performer.Code = resource?.Id ?? "";
                                    laneViewModel.Performer.Name = GetResourceName(resource);
                                    laneViewModel.Performer.Claim = resource?.UserClaimRestrictions?.FirstOrDefault();
                                }
                                break;
                            case FlowNode.eFlowNodeType.Event:
                                elementViewModel.Type = ((element as Event)?.TriggerType.ToString() ?? elementViewModel.Type) +
                                                        "-" + ((element as CatchEvent)?.Location.ToString() ?? (element as ThrowEvent)?.Location.ToString() ?? "");
                                break;
                            case FlowNode.eFlowNodeType.ChoreographyActivity:
                                break;
                        }
                    }
                    processViewModel.Lanes.Add(laneViewModel);
                }
            foreach (Domain.Entities.Bpmn.Processes.Lanes.LaneSet laneSet in process.definition.laneSets)
                foreach (Domain.Entities.Bpmn.Processes.Lanes.Lane lane in laneSet.lanes)
                {
                    foreach (string flowNodeRef in lane.flowNodeRefs)
                    {
                        if (!process.definition.flowElements.TryGetValue(flowNodeRef, out FlowElement flowElement))
                            continue;
                        FlowNode element = flowElement as FlowNode;
                        ElementViewModel elementViewModel = processViewModel.GetElement(element.Id);
                        if (elementViewModel == null) continue;
                        foreach (SequenceFlow sequenceFlow in element.incoming)
                        {
                            ElementViewModel sequenceElementViewModel = processViewModel.GetElement(sequenceFlow.sourceRef.Id);
                            if (sequenceElementViewModel == null) continue;
                            elementViewModel.Inputs.Add(new FlowElementViewModel
                            {
                                //Name = sequenceFlow.Id,
                                StateId = sequenceFlow.StateId,
                                Condition = (sequenceFlow.conditionExpression as FormalExpression)?.Expression.ExpressionString,
                                Element = sequenceElementViewModel
                            });
                        }
                        foreach (SequenceFlow sequenceFlow in element.outgoing)
                        {
                            ElementViewModel sequenceElementViewModel = processViewModel.GetElement(sequenceFlow.targetRef.Id);
                            if (sequenceElementViewModel == null) continue;
                            elementViewModel.Outputs.Add(new FlowElementViewModel
                            {
                                //Name = sequenceFlow.Id,
                                StateId = sequenceFlow.StateId,
                                Condition = (sequenceFlow.conditionExpression as FormalExpression)?.Expression.ExpressionString,
                                Element = sequenceElementViewModel
                            });
                        }
                    }
                }
            res.Add(processViewModel);
            if (!string.IsNullOrEmpty(processId))
                break;
        }
        return res;
    }

    private static string GetResourceName(HumanResource resource)
    {
        return (resource?.type) switch
        {
            HumanResourceType.User => resource.Name,
            HumanResourceType.UserGroup => resource.Id,
            _ => "",
        };
    }
}
