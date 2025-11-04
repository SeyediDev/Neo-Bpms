using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Data;

public static partial class DataStorage
{
    public static void LoadDataRecord(ProcessVersionRuntime processVersionRuntime, string ids, ProcessInstance pi)
    {
        if (string.IsNullOrEmpty(ids))
        {
            pi.LogError($"Missing pkv record pi:{pi.Id}");
            throw new Exception($"ایراد DS313, pi:{pi.Id}");
        }

        ElasticObject data = QueryProcessData(processVersionRuntime)
            .AddPkFilter(ids)
            .FirstOrDefault();
        if (data != null)
        {
            pi.FetchData(true, data, processVersionRuntime.definition);
        }
    }

    private static QueryUtility QueryProcessData(ProcessVersionRuntime versionRuntime)
    {
        return QueryProcessData(versionRuntime.definition);
    }

    public static QueryUtility QueryProcessData(Process process)
    {
        QueryUtility q = new(process.EntityNamespaceId, process.EntityId);
        return QueryProcessData(process, q);
    }

    private static QueryUtility QueryProcessData(Process process, QueryUtility q)
    {
        bool any = false;
        foreach (Property property in process.properties ?? Enumerable.Empty<Property>())
        {
            if (q.Entity.GetField(property.fieldId) != null)
            {
                _ = q.SelectField(property.fieldId, property.Name);
                any = true;
            }
        }

        foreach (DataObject dataObject in process.flowElements.Values.OfType<DataObject>())
        {
            if (q.Entity.GetField(dataObject.fieldId) != null)
            {
                _ = q.SelectField(dataObject.fieldId);
                any = true;
            }
        }

        foreach (InputSet inputSet in process.ioSpecification?.inputSets ?? Enumerable.Empty<InputSet>())
        {
            foreach (DataInputRef input in inputSet.dataInputRefs)
            {
                if (q.Entity.GetField(input.dataInput.fieldId) != null)
                {
                    _ = q.SelectField(input.dataInput.fieldId);
                    any = true;
                }
            }
        }

        if (!string.IsNullOrEmpty(process.StateProperty))
        {
            _ = q.SelectField(process.StateProperty);
            any = true;
        }

        if (!any)
        {
            _ = q.AddPkFields();
        }

        return q;
    }
}
