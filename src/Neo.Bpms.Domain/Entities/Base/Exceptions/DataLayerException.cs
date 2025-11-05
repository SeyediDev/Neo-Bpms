namespace Neo.Bpms.Domain.Entities.Base.Exceptions;

public class DataLayerException : EngineException
{
    public DataLayerException(DataEngineException id, string text, string enText = null) :
        base(new ExceptionInformation(id, text) { EnErrorText = string.IsNullOrEmpty(enText) ? id.ToString() : enText })
    {
    }

    public DataLayerException(Exception e) :
        base(new ExceptionInformation(DataEngineException.None, e.Message)
        {
            EnErrorText = e.Message
        })
    {
    }
}
