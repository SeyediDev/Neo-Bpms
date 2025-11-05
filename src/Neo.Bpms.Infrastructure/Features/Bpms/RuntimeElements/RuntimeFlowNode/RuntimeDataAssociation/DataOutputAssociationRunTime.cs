using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.DataAssociation;

namespace Neo.Bpms.Infrastructure.Features.Bpms;
public abstract partial class FlowNodeRunTime
{
    internal void RunDataOutputAssociations(ProcessInstance pi, FlowNodeInstance ai, LocalParameters outputData)
    {
        if (flowNode is not IDataOutputAssociationContainer dataOutputAssociationContainer) return;
        if (pi == null)
            return;
        var flowNodeContainer = flowNode.Parent as IFlowElementsContainer;
        while (flowNodeContainer != null && flowNodeContainer is not Process)
        {
            SetDataToProperties(flowNodeContainer as IPropertyContainer,
                (IPropertyValueContainer)ai ?? pi, outputData, pi, ai);
            flowNodeContainer = (flowNodeContainer as BaseElement)?.Parent as IFlowElementsContainer;
        }

        SetDataToProperties(flowNodeContainer as IPropertyContainer, pi, outputData, pi, ai);
        var localParameters = new LocalParameters();
        if (ai is FlowNodeInstance2 ai2 && ai2.SelectedInputSet != null)
        {
            foreach (var outputSet in ai2.SelectedInputSet.outputSetRefs)
            {
                foreach (var output in outputSet.dataOutputRefs)
                {
                    if (outputData?.ContainsKey(output.dataOutput.Id) == true)
                    {
                        ai.SetData(output.dataOutput.Id, outputData[output.dataOutput.Id]);
                        AuditTrace($"Send data to outputs {output.dataOutput.Id} = {outputData[output.dataOutput.Id]}",
                            ai.pi, ai);
                    }
                }
            }
        }

        if (dataOutputAssociationContainer.dataOutputAssociations != null &&
            dataOutputAssociationContainer.dataOutputAssociations.Count > 0)
        {
            var propertyAssociations = dataOutputAssociationContainer.dataOutputAssociations.Where(
                    doa =>
                        doa.TargetDataElement?.type == DataElement.eDataElementTypes.Property ||
                        doa.TargetDataElement?.type == DataElement.eDataElementTypes.DataOutput ||
                        doa.TargetDataFlow?.type == DataFlowElement.eDataFlowElementTypes.DataObject ||
                        doa.TargetDataFlow?.type == DataFlowElement.eDataFlowElementTypes.DataObjectRef)
                .ToList();
            if (propertyAssociations.Count != 0)
            {
                foreach (var dataAssociation in propertyAssociations)
                    RunDataAssociation(dataAssociation, ai, outputData, localParameters, pi);
                DataStorage.SaveProcessInstance(pi, ai, "DAR.0", true, DataStorage.LockChangeRequest.NoChange);
            }

            foreach (var dataAssociation in dataOutputAssociationContainer.dataOutputAssociations.Where(
                doa1 =>
                    doa1.TargetDataFlow?.type == DataFlowElement.eDataFlowElementTypes.DataStoreRef))
            {
                RunDataAssociation(dataAssociation, ai, outputData, localParameters, pi);
            }

            /*var instances = DataStorage.FetchFlowNodeInstancesOfProcessInstance(ai.pi, null, $"StateId=={(int)ActivityInstanceStateId.Ready}");
				if (instances != null)
				{
					foreach (var instance in instances.Values.OfType<ActivityInstance>())
					{
						instance.ActivityRuntime.CheckDataInputAvailabilityAndStart(instance);
					}
				}*/
        }
    }

    private void SetDataToProperties(IPropertyContainer propertyContainer,
        IPropertyValueContainer propertyValueContainer,
        LocalParameters outputData, ProcessInstance pi, FlowNodeInstance ai)
    {
        if (outputData == null || propertyContainer?.properties == null) return;
        foreach (var item in outputData)
        {
            if (propertyContainer.properties.Any(p => p.Name == item.Key))
            {
                AuditTrace($"Send data to outputs {item.Key} = {item.Value}", pi, ai);
                propertyValueContainer.SetData(item.Key, item.Value);
            }
        }
    }
}
