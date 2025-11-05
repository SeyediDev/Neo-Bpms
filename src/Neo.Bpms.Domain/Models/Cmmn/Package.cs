namespace Neo.Bpms.Domain.Models.Cmmn;

public class Package : BaseModelClass
{
    public Package(string id, string name) : base(id, name)
    {

    }
    public Package(Entity entity, int Id, string name) :
        base(entity, "" + Id, name)
    {
        entities.Add(new PackageEntity(entity, PackageEntity.eType.Main));
    }

    public Package()
    {
    }


    public bool bAutoLayout = true;
    public List<PackageEntity> entities = [];
    public PackageEntity addEntity(Entity entity, PackageEntity.eType type)
    {
        PackageEntity pe = new(entity, type);
        entities.Add(pe);
        return pe;
    }
    public PackageEntity mainEntity
    {
        get
        {
            foreach (PackageEntity en in entities)
            {
                if (en.type == PackageEntity.eType.Main)
                {
                    return en;
                }
            }
            return null;
        }
    }
    [XmlIgnore]
    public Package parentPackage = null;
    public List<Package> subPackages = null;
    public void addSubPackage(Package package)
    {
        subPackages ??= [];
        package.parentPackage = this;
        subPackages.Add(package);
    }
}
public class PackageEntity
{
    public enum eType
    {
        Main,
        Normal,
        Referred,
        Refering
    }
    public eType type;
    public Entity entity;
    public List<EntityField> fields = [];
    public int x, y;

    public PackageEntity(Entity entity, eType type)
    {
        this.entity = entity;
        this.type = type;
        if (type is not eType.Refering and not eType.Referred)
        {
            addAllFields();
        }
    }
    public void addAllFields()
    {
        foreach (EntityField field in entity.entityFields.Values)
        {
            fields.Add(field);
        }
    }
    public void removeAllFields()
    {
        fields.Clear();
    }
    public void AddField(string fieldId)
    {
        EntityField field = entity.GetField(fieldId);
        if (field != null && getField(fieldId) == null)
        {
            fields.Add(field);
        }
    }

    public EntityField getField(string fieldId)
    {
        return fields.FirstOrDefault(f => f.Id == fieldId);
    }
    public void removeField(string fieldId)
    {
        EntityField field = fields.FirstOrDefault(f => f.Id == fieldId);
        if (field != null)
        {
            _ = fields.Remove(field);
        }
    }

}
