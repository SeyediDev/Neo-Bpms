using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.UI.MVC.ViewModels.ElementViewModels;

public class DataStoreViewModel
{
    public DataStoreViewModel()
    {

    }

    public DataStoreViewModel(DataStore dataStore)
    {
        id = dataStore.Id;
        name = dataStore.Name;
        isUnLimited = dataStore.isUnlimited;
        capacity = dataStore.capacity;
        entityId = dataStore.itemSubjectRef?.structure?.Id;
        namespaceId = dataStore.itemSubjectRef?.structure?.NamespaceId;
    }

    public string id { get; set; }
    public string name { get; set; }
    public int capacity { get; set; }
    public bool isUnLimited { get; set; }
    /// <summary>
    /// structureRef
    /// </summary>
    public string namespaceId { get; set; }
    public string entityId { get; set; }
    public List<DataStateViewModel> dataState { get; set; }

    public DataStore ToDataStore()
    {
        return new DataStore(ProjectDefinition.Project.BpmnDefinitions, id, name,
            ProjectDefinition.Project.BpmnDefinitions.AddOrGetItemDefinition(false,
                ProjectDefinition.Project.GetEntity(namespaceId, entityId)),
            isUnLimited, capacity);
    }
}