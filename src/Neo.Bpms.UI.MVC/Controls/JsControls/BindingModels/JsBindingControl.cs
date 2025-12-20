using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.JsControls.BindingModels;

public class JsBindingControl(string type, string id)
{
    public string type { get; set; } = type;
    public string id { get; set; } = id;
    public List<JsBindingInfo> bindingInfo { get; } = [];
    public Dictionary<string, List<string>> initialData { get; } = [];

    public void AddTableData(TableDefinition table, object tableData)
    {
        initialData.Add(table.FieldName, TableDataToListOfJsons(tableData, table));
    }

    private static List<string> TableDataToListOfJsons(object tableData, TableDefinition table)
    {
        if (tableData is not List<ElasticObject> list) return [];
        MakeElasticsUsable(table, list);
        return [.. list.Select(row => row.ToJson())];
    }

    private static void MakeElasticsUsable(TableDefinition table, List<ElasticObject> list)
    {
        foreach (ElasticObject item in list)
        {
            ElasticObject toBeMergedItem = new();
            foreach (TableDefinition tableColumn in table.Tables)
            {
                if (item.GetField(tableColumn.FieldName, out object tableColumnValue) &&
                    tableColumnValue is List<ElasticObject>)
                {
                    if (tableColumn.ControlType == eControlTypeId.MultipleSelectableCombo)
                    {
                        string targetField =
                            tableColumn.FieldName.Split('_')[2] + "Id"; // works conventionally but not precise.
                        ElasticObject newElastic = new()
                        {
                            InternalValue = ((List<ElasticObject>)tableColumnValue)
                                .Where(i => i.GetString(targetField) != null)
                                .Select(i => i.GetString(targetField)).ToList()
                        };
                        toBeMergedItem.AddAttribute(tableColumn.FieldName, newElastic);
                    }
                    else if (tableColumn.ControlType == eControlTypeId.IndexTable)
                    {
                        MakeElasticsUsable(tableColumn, (List<ElasticObject>)tableColumnValue);
                    }
                }
            }

            //                foreach (var keyValuePair in toBeMergedItem.Attributes)
            //                {
            //                    item.RemoveAttribute(keyValuePair.Key);
            //                }
            item.Merge(toBeMergedItem);
        }
    }

}
