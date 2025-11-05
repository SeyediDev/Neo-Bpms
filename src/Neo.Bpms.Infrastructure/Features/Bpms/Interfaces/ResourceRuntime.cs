namespace Neo.Bpms.Infrastructure.Features.Bpms.Interfaces;

public class OperationResourceRuntime
{
    public object defintion;
    public int Progress { get; protected set; }

    public virtual void Free()
    {
    }

    public virtual bool IsWorking()
    {
        throw new NotImplementedException();
    }

    public virtual string GetMachineId()
    {
        return null;
    }
}