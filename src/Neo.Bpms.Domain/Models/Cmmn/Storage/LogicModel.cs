namespace Neo.Bpms.Domain.Models.Cmmn.Storage;

public abstract class LogicModel : IdentityBase<long?>
{
    public long GetId()
    {
        return Id ?? 0;
    }
}
