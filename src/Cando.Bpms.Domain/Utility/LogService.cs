namespace Neo.Bpms.Domain.Utility;

public interface ILogContainer<T>
{
    ILogger<T> Logger { get; }
}
public class LogContainer(ILogger<LogContainer> logger) : ILogContainer<LogContainer>
{
    public ILogger<LogContainer> Logger { get; set; } = logger;
}
