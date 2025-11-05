namespace Neo.Bpms.Domain.Expressions.Model.DataStructures;

public class Interval(eEndpointType fromType, object from, eEndpointType toType, object to)
{
    private readonly eEndpointType _fromType = fromType;
    private readonly eEndpointType _toType = toType;
    private readonly object _from = from;
    private readonly object _to = to;

    public object GetValue()
    {
        return _fromType == eEndpointType.Unlimited ? _to : _toType == eEndpointType.Unlimited ? _from : this;
    }

    internal bool Contains(object obj)
    {
        switch (_fromType)
        {
            case eEndpointType.Open:
                if ((dynamic)obj <= (dynamic)_from) return false;
                break;
            case eEndpointType.Close:
                if ((dynamic)obj < (dynamic)_from) return false;
                break;
            case eEndpointType.Unlimited:
                break;
        }
        switch (_toType)
        {
            case eEndpointType.Open:
                if ((dynamic)obj >= (dynamic)_to) return false;
                break;
            case eEndpointType.Close:
                if ((dynamic)obj > (dynamic)_to) return false;
                break;
            case eEndpointType.Unlimited:
                break;
        }
        return true;
    }
}
public enum eEndpointType
{
    Open,
    Close,
    Unlimited
}
