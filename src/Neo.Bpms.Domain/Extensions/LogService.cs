namespace Neo.Bpms.Domain.Extensions;

public interface ILogContainer<T>
{
    ILogger<T> Logger { get; }
}
public class LogContainer(ILogger<LogContainer> logger) : ILogContainer<LogContainer>
{
    public ILogger<LogContainer> Logger { get; set; } = logger;
}
