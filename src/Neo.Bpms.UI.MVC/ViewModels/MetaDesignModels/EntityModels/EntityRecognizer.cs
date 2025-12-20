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

public class KeyFieldsViewModel(EntityField keyField)
{
    public string id { get; set; } = keyField.Id;
    public string name { get; set; } = keyField.Name;
    public TVariableTypes type { get; set; } = keyField.FieldType;
    public long maxLength { get; set; } = keyField.MaxLen;
}
