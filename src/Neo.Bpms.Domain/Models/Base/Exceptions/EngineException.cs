namespace Neo.Bpms.Domain.Models.Base.Exceptions;

public abstract class EngineException : Exception
{
    public ExceptionInformation info;

    protected EngineException(ExceptionInformation info) :
        base(info.ErrorText)
    {
        this.info = info;
    }
    protected EngineException(ExceptionInformation info,
        Exception innerException) :
        base(info.ErrorText, innerException)
    {
        this.info = info;
    }
    public void AddParameter(object key, object value)
    {
        Data.Add(key, value);
    }
    public void AddParameter(params KeyValuePair<object, object>[] keyvalues)
    {
        foreach (KeyValuePair<object, object> param in keyvalues)
        {
            Data.Add(param.Key, param.Value);
        }
    }
}
