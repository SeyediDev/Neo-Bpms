using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.DataStores;

internal static class DataStoreXmlConvertor
{
    internal static void Export(dynamic bpmnElement)
    {
        foreach (var dataStore in ProjectDefinition.Project.DataStores ?? [])
        {
            var element = bpmnElement.dataStore();
            RootElementXmlConvertor.Export(element, dataStore);
            element.name = dataStore.Name;
            element.isUnlimited = dataStore.isUnlimited;
            element.capacity = dataStore.capacity;
            ItemAwareElementXmlConvertor.Export(bpmnElement, element, dataStore);
        }
    }

    internal static void Import(ElasticObject bpmnElement)
    {
        foreach (var element in bpmnElement.GetElements("dataStore") ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id");
            var dataStore = ProjectDefinition.Project.GetDataStore(id);
            if (dataStore != null) continue;
            var isUnlimited = element.GetBool("isUnlimited", true);
            var capacity = element.GetInteger("capacity", 0);
            dataStore = new DataStore(null, id, element.GetString("name"), null, isUnlimited, capacity);
            RootElementXmlConvertor.Import(element, dataStore);
            ItemAwareElementXmlConvertor.Import(ProjectDefinition.Project.BpmnDefinitions,
                bpmnElement, element, dataStore);
            ProjectDefinition.Project.AddDataStore(dataStore);
        }
    }
}
