namespace Neo.Bpms.Domain.Entities.Base;

/// <summary>
/// Base class for all modeling info
/// </summary>
public abstract class BaseClass : IBaseClass
{
    public string Name { get; set; }

    public string Id { get; set; }
    
    protected BaseClass()
    {
    }

    protected BaseClass(BaseClass parent, string id, string name)
    {
        Parent = parent;
        Name = name;
        Id = id;
    }

    /// <summary>
    /// The parent
    /// </summary>
    [XmlIgnore]
    public BaseClass Parent;
}
