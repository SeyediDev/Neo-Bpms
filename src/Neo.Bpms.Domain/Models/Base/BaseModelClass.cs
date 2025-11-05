namespace Neo.Bpms.Domain.Models.Base;

public abstract class BaseModelClass : BaseClass
{
    public string EnName {  get; set; }
    public string Description {  get; set; }
    public string EnDescription {  get; set; }
    public string GetName(string culture)
    {
        return culture == "en" ? EnName : Name;
    }

    protected BaseModelClass()
    {
    }

    protected BaseModelClass(string id, string name)
        : this(null, id, name)
    {
        EnName = id;
    }

    protected BaseModelClass(BaseModelClass parent, int id, string name)
        : this(parent, "" + id, name)
    {
        EnName = id.ToString();
    }

    protected BaseModelClass(BaseModelClass parent, string id, string name)
        : base(parent, id, name)
    {
    }

    [XmlIgnore]
    public new BaseModelClass Parent
    {
        get => (BaseModelClass)base.Parent;
        set => base.Parent = value;
    }

    protected void CloneBase(BaseModelClass source)
    {
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        //       
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237  
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        if (obj is BaseModelClass o && Id == o.Id)
            return true;
        // ReSharper disable once BaseObjectEqualsIsObjectEquals
        return base.Equals(obj);
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        return base.GetHashCode();
    }
}
