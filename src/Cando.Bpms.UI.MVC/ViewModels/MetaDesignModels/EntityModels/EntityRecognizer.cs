using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityModels;

public class EntityRecognizer
{
    public EntityRecognizer(Entity entity)
    {
        id = entity.Id;
        name = entity.Name;
        idName = entity.Id + " " + entity.Name;
        namespaceId = entity.NamespaceId;
        notMapped = entity.NotMapped;
        keyGroups =
            [
                new KeyGroupsViewModel
                {
                    isPrimary = true,
                    title = "کلید اصلی",
                    keys = entity.KeyFields?.Select(keyField => new KeyFieldsViewModel(keyField)).ToList()
                }
            ];
        foreach (EntityIndex entityIndex in entity.indexes?.Where(i => i.IsUnique) ?? [])
        {
            keyGroups.Add(new KeyGroupsViewModel
            {
                isPrimary = false,
                title = entityIndex.Name,
                keys = [.. entityIndex.Fields.Select(k => new KeyFieldsViewModel(entity.GetField(k.FieldName)))]
            });
        }
    }

    public string id { get; set; }
    public string name { get; set; }
    public string idName { get; set; }
    public string namespaceId { get; set; }
    public bool notMapped { get; set; }

    /// <summary>
    /// first fill by primary keys titled 'کلیداصلی'
    /// second fill by unique index titled 'name of index' note in this case tick not map
    /// </summary>
    public List<KeyGroupsViewModel> keyGroups { get; set; }
}

public class KeyGroupsViewModel
{
    public string title { get; set; }
    public bool isPrimary { get; set; }
    public List<KeyFieldsViewModel> keys { get; set; }
}

public class KeyFieldsViewModel
{
    public KeyFieldsViewModel(EntityField keyField)
    {
        id = keyField.Id;
        name = keyField.Name;
        type = keyField.FieldType;
        maxLength = keyField.MaxLen;
    }

    public string id { get; set; }
    public string name { get; set; }
    public TVariableTypes type { get; set; }
    public long maxLength { get; set; }
}
