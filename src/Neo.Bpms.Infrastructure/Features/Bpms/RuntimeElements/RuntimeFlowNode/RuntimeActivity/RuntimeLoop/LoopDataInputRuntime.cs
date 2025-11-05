using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Models.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.LoopCharacteristic;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.Infrastructure.Features.Bpms;
public abstract partial class ActivityRuntime
{
    private void CreateMultiInstancesOnLoopDataInput(ProcessInstance pi, LocalParameters inputData,
        MultiInstanceLoopCharacteristics mi, out List<LoopInstance> instances)
    {
        var list = FetchLoopDataInput(mi.loopDataInputRef, pi, inputData);
        AuditTrace($"Multi instance record count : {list?.Count} in activity {Activity.Id}", pi, null);
        instances = null;
        if ((list?.Count ?? 0) <= 0) return;
        instances = [];
        var loopCounter = 0L;
        foreach (var item in list)
            CreateInstanceOfLoop(pi, instances, loopCounter++, item);
    }

    private static List<ElasticObject> FetchLoopDataInput(IItemAwareElement loopDataInputRef,
        ProcessInstance pi, LocalParameters inputData)
    {
        switch (loopDataInputRef)
        {
            case null:
            case DataStoreReference dataStoreReference
                when dataStoreReference.dataStore?.itemSubjectRef?.structure?.entityFields == null:
                return null;
            case DataStoreReference dataStoreReference:
                {
                    var list = dataStoreReference.dataStore.itemSubjectRef.structure.entityFields.Values
                        .Where(f => !f.NotMap)
                        .Select(f => f.Id)
                        .ToList();
                    return DataStorage.ReadDataStoreList(dataStoreReference, pi, list);
                }
        }

        var dataElement = loopDataInputRef as DataElement;
        var dataFlowElement = loopDataInputRef as DataFlowElement;
        var name = dataElement?.Name ?? dataFlowElement?.Name ?? (loopDataInputRef as BaseElement)?.Id ?? "";
        if (pi.Data.HasAttribute(name))
            return GetList(pi.Data[name])?.ToList();
        if (pi.ParentAi != null && pi.ParentAi.Data.HasAttribute(name))
            return GetList(pi.ParentAi.Data[name])?.ToList();
        return inputData.TryGetValue(name, out object value) ? GetList(value)?.ToList() : null;
    }

    private static IEnumerable<ElasticObject> GetList(object list)
    {
        switch (list)
        {
            case ICollection<ElasticObject> objects:
                return objects.ToList();
            case ICollection<object> collection:
                return collection.Select(item => new ElasticObject("", item)).ToList();
            default:
                return ReflectionTools.GetEnumerableValue(list)
                    ?.Select(ElasticObject.ToElastic).ToList();
        }
    }
}
