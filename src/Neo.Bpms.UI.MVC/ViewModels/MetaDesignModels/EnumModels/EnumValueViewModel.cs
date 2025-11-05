using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Cmmn;
using Neo.Bpms.Domain.Entities.Cmmn.Entities;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EnumModels;

public class EnumValueViewModel
{
    public EnumValueViewModel()
    {

    }

    public EnumValueViewModel(EnumerationItem item)
    {
        id = item.Id;
        persianName = item.Name;
        englishName = item.EnName;
        description = item.Description;
        englishDescription = item.EnDescription;
    }

    public EnumValueViewModel(EntityState item)
    {
        id = item.Id;
        persianName = item.Name;
        englishName = item.enName;
        stateCategory = (EntityStateCategory)item.category;
        //            description = item.Description;
        //            englishDescription = item.Description;
    }

    public string id { get; set; }
    public string persianName { get; set; }
    public string englishName { get; set; }
    public string? description { get; set; }
    public string englishDescription { get; set; }
    public EntityStateCategory stateCategory { get; set; }

    public EnumerationItem ToEnumerationItem(Enumeration enumeration)
    {
        EnumerationItem enumerationItem = new(enumeration, Convert.ToInt32(id),
            persianName, stateCategory)
        {
            EnName = englishName,
            EnDescription = englishDescription,
            Name = persianName,
            Description = description
        };
        return enumerationItem;
    }

    public EntityState ToEntityState(Entity entity)
    {
        return new EntityState(entity, Convert.ToInt32(id),
                persianName, englishName, stateCategory);
    }
}
