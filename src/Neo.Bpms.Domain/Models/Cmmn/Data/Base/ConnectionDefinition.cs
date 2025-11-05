namespace Neo.Bpms.Domain.Models.Cmmn.Data.Base;

public abstract class ConnectionDefinition
{
    public string DatabaseName { get; set; }
    //protected ConnectionDefinition(eConnectionType type)
    //{
    //	ConnectionType = type;
    //}
    //public eConnectionType ConnectionType { get; set; }
    public virtual bool IsSequential()
    {
        return false;
    }

    public virtual bool SupportsChangeWrite()
    {
        return true;
    }

    public virtual bool SupportsFilter()
    {
        return true;
    }

    public virtual bool SupportsGroupBy()
    {
        return true;
    }

    public virtual bool SupportsOrderBy()
    {
        return true;
    }

    public virtual bool SupportsJoin()
    {
        return true;
    }

    public virtual bool MetaDataCouldBeExtracted()
    {
        return true;
    }

    public abstract void Init();
}