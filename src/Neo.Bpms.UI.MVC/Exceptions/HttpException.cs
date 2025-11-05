using System.Net;

namespace Neo.Bpms.UI.MVC.Exceptions;

public class HttpException : Exception
{
    public HttpException(int httpStatusCode)
    {
        ErrorCode = httpStatusCode;
    }

    public HttpException(string message) : base(message)
    {
    }

    public HttpException(HttpStatusCode httpStatusCode)
    {
        ErrorCode = (int)httpStatusCode;
    }

    public HttpException(int httpStatusCode, string message) : base(message)
    {
        ErrorCode = httpStatusCode;
    }

    public HttpException(HttpStatusCode httpStatusCode, string message) : base(message)
    {
        ErrorCode = (int)httpStatusCode;
    }

    public HttpException(int httpStatusCode, string message, Exception inner) : base(message, inner)
    {
        ErrorCode = httpStatusCode;
    }

    public HttpException(HttpStatusCode httpStatusCode, string message, Exception inner) : base(message, inner)
    {
        ErrorCode = (int)httpStatusCode;
    }

    public int ErrorCode { get; } = 500;

}
