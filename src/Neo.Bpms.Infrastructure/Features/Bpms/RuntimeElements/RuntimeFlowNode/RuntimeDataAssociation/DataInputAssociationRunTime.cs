using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataAssociation;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;

namespace Neo.Bpms.Infrastructure.Features.Bpms;
public abstract partial class FlowNodeRunTime
{
    protected internal LocalParameters FetchInputData(ProcessInstance pi, FlowNodeInstance ai, LocalParameters inputParams)
    {
        var outputParams = new LocalParameters();
        if (flowNode is not IDataInputAssociationContainer dataInputAssociationContainer) return outputParams;
        var dataInputAssociations = dataInputAssociationContainer.dataInputAssociations;
        FetchDataInputAssociations(pi, ai, dataInputAssociations, inputParams, outputParams);

        if (flowNode is not IDataInputContainer dataInputContainer)
        {
            var ioSpecificationContainer = flowNode as IIoSpecificationContainer;
            dataInputContainer = ioSpecificationContainer?.ioSpecification;
        }

        if (dataInputContainer?.dataInputs != null)
        {
            foreach (var di in dataInputContainer.dataInputs)
            {
                if (ai.GetData(di.Name, out var value))
                    outputParams.AddOrUpdate(di.Name, value);
            }
        }

        return outputParams;
    }

    private void FetchDataInputAssociations(ProcessInstance pi, FlowNodeInstance ai, List<DataInputAssociation> dataInputAssociations,
        LocalParameters inputParams, LocalParameters outputParams)
    {
        if (dataInputAssociations?.Count > 0)
        {
            var propertyAssociations = dataInputAssociations.Where(
                    doa =>
                        doa.TargetDataElement?.type == DataElement.eDataElementTypes.Property ||
                        doa.TargetDataElement?.type == DataElement.eDataElementTypes.DataInput ||
                        doa.TargetDataFlow?.type == DataFlowElement.eDataFlowElementTypes.DataObject ||
                        doa.TargetDataFlow?.type == DataFlowElement.eDataFlowElementTypes.DataObjectRef)
                .ToList();
            var saveProcessInstance = false;
            if (propertyAssociations.Count != 0)
            {
                foreach (var dia in propertyAssociations)
                {
                    RunDataAssociation(dia, ai, inputParams, outputParams);
                }

                DataStorage.SaveProcessInstance(pi, ai, "DAR.1", true, DataStorage.LockChangeRequest.NoChange);
            }

            foreach (var dia in dataInputAssociations.Where(
                dia1 =>
                    dia1.TargetDataFlow?.type == DataFlowElement.eDataFlowElementTypes.DataStoreRef))
            {
                RunDataAssociation(dia, ai, outputParams, outputParams);
                var dataStoreReference = dia.targetRef as DataStoreReference;
                if (!string.IsNullOrEmpty(dataStoreReference?.SaveOnPrimaryKeyProperty ??
                                          dataStoreReference?.SaveOnPrimaryKeyProperty))
                    saveProcessInstance = true;
            }

            if (saveProcessInstance)
                DataStorage.SaveProcessInstance(pi, ai, "DAR.2", true, DataStorage.LockChangeRequest.NoChange);
        }
    }
}
