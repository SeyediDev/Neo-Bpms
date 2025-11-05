using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    private DataStoreReference currentDataStoreReference;
    /// <summary>
    /// Adds Data Store Reference
    /// </summary>
    /// <typeparam name="T">entity type</typeparam>
    /// <param name="name">the name.</param>
    /// <param name="isCollection">if set to <c>true</c> [is collection]</param>
    /// <param name="filterExpression">The filter expression</param>
    /// <param name="saveOnPrimaryKeyProperty">The primary key property</param>
    /// <returns></returns>
    protected DataStoreReference AddDataStoreReference<T>(string name, bool isCollection,
        string filterExpression = null, string saveOnPrimaryKeyProperty = null)
    {
        if (definitions == null) return null;
        var entity = ProjectDefinition.Project.GetEntity<T>();
        var id = $"{entity.NamespaceId}.{entity.Id}";
        var dataStore = ProjectDefinition.Project.GetDataStore(id);
        if (dataStore == null)
        {
            var itemDef = AddItemDefinition<T>(isCollection, id, false);
            dataStore = new DataStore(definitions, id, id, itemDef, true, 0);
            ProjectDefinition.Project.AddDataStore(dataStore);
        }
        currentDataStoreReference = new DataStoreReference(process, name, name, dataStore)
        {
            SaveOnPrimaryKeyProperty = saveOnPrimaryKeyProperty,
            FilterExpression = new FormalExpression(id + "_filter", Parser.ParseTree(filterExpression))
        };
        AddFlowElement(currentDataStoreReference);
        return currentDataStoreReference;
    }
    /// <summary>
    /// Adds Filter Mapping To Data Store Reference
    /// </summary>
    /// <param name="dataStoreField">The data store field</param>
    /// <param name="processProperty">The process property</param>
    protected void AddDataStoreReferenceFilterMapping(string dataStoreField, string processProperty)
    {
        currentDataStoreReference?.AddFilterMapping(dataStoreField, processProperty);
    }
}
