namespace Neo.Bpms.Domain.Entities.Cmmn.Storage;

public abstract class LogicModel : IdentityBase<long?>
{
    public long GetId()
    {
        return Id ?? 0;
    }
}
