namespace Neo.Bpms.Domain.Model.BPMN.Processes;

public partial class DataStoreReference
{
    public string SaveOnPrimaryKeyProperty;
    public List<FilterMapping> FilterMappings { get; set; } = [];
    public string NamespaceId => dataStore?.NamespaceId;
    public string EntityId => dataStore?.EntityId;
    public FormalExpression FilterExpression { get; set; }

    public void AddFilterMapping(string dataStoreField, string processProperty)
    {
        FilterMapping fm = new()
        {
            DataStoreField = dataStoreField,
            ProcessProperty = processProperty
        };
        FilterMappings.Add(fm);
    }

    public class FilterMapping
    {
        public string DataStoreField;
        public string ProcessProperty;
    }
}