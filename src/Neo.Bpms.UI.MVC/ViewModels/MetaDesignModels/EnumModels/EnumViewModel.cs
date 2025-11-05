namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EnumModels;

public class EnumViewModel
{
    public EnumViewModel()
    {

    }
    public EnumViewModel(Enumeration @enum)
    {
        id = @enum.Id;
        name = @enum.Name;
        enName = @enum.EnName;
        namespaceId = @enum.NamespaceId;
        values = @enum.items?.Values.Select(e => new EnumValueViewModel(e));
    }

    public string id { get; set; }
    public string name { get; set; }
    public string enName { get; set; }
    public string namespaceId { get; set; }
    public IEnumerable<EnumValueViewModel> values { get; set; }

    public void Modify(Enumeration enumeration)
    {
        enumeration.Id = id;
        enumeration.Name = name;
        //enumeration.EnName = enName;
        //todo enumeration.DeleteExtraItems = deleteExtraItems;
        enumeration.items = [];
        foreach (EnumValueViewModel enumValueViewModel in values)
        {
            enumeration.items.Add(enumValueViewModel.id,
                new EnumerationItem
                {
                    Id = enumValueViewModel.id,
                    Name = enumValueViewModel.persianName
                    //todo = v.persianName,
                });
        }
    }
}
