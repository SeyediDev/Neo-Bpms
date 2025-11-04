namespace Neo.Bpms.Domain.Entities.Service.ServiceOperation;

public class ServiceOperationException : Exception
{
    public string ErrorCode { get; set; }

    public ServiceOperationException(string errorCode)
    {
        ErrorCode = errorCode;
    }
    public ServiceOperationException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }
    public ServiceOperationException(string errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}