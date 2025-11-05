namespace Neo.Bpms.Domain.Models.Cmmn;

public class ExceptionInfo
{
    public ExceptionInfo()
    {
    }

    public ExceptionInfo(string forField, Exception exception)
    {
        ForField = forField;
        Exception = exception;
    }

    public string ForField { get; set; }
    public Exception Exception { get; set; }
}

public class ExceptionInfos : List<ExceptionInfo>
{
    public ExceptionInfos Add(string forField, string message)
    {
        return Add(forField, new Exception(message));
    }

    public ExceptionInfos Add(string forField, Exception exception)
    {
        Add(new ExceptionInfo
        {
            ForField = forField,
            Exception = exception
        });
        return this;
    }

    public static void AddError(ref ExceptionInfos errors, string forField, string message)
    {
        errors ??= [];
        errors.Add(forField, message);
    }

    public static void AddError(ref ExceptionInfos errors, string forField, Exception exception)
    {
        errors ??= [];
        errors.Add(forField, exception);
    }

    public static void AppendErrors(ref ExceptionInfos errors, IList<ExceptionInfo> newErrors)
    {
        if (newErrors != null && newErrors.Any())
        {
            errors ??= [];
            errors.AddRange(newErrors);
        }
    }

    public static void AppendError(ref ExceptionInfos errors, ExceptionInfo error)
    {
        errors ??= [];
        errors.Add(error);
    }
}
