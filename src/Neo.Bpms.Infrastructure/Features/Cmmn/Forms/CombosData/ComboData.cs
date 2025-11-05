using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.CombosData;

public class ComboData
{
    public ComboData(string displayFields = null)
    {
        DisplayFields = displayFields;
    }
    public List<FormDataRow> Rows = [];
    public string NamespaceId;
    public string EntityId;

    public string Filter { get; set; }
    internal ConcurrentDictionary<string, FormDataRow> RecordsById { get; set; } = new ConcurrentDictionary<string, FormDataRow>();
    public bool GetQuery { get; set; }

    public string culture;
    public string DisplayFields;
    public int Count { private get; set; }
    public bool HasMore => Rows.Count >= Count && Count != 0;

    public List<UIComponentProperty> Properties { get; set; }

    public FormDataRow GetRow(string recordId)
    {
        if (recordId == null) return null;
        RecordsById.TryGetValue(recordId, out FormDataRow dataRow);
        return dataRow;
    }

    public void AddRow(FormDataRow dataRow, bool update = false)
    {
        Rows.Add(dataRow);
        if (update)
        {
            if (RecordsById[dataRow.Ids] == null)
                RecordsById[dataRow.Ids] = dataRow;
        }
        else
            RecordsById.TryAdd(dataRow.Ids, dataRow);
    }
}
