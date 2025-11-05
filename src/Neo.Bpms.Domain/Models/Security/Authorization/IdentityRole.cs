using Neo.Bpms.Domain.Models.Cmmn.Storage;

namespace Neo.Bpms.Domain.Models.Security.Authorization;

public class IdentityRole : CategorizedLogicModel, ICanBeDisable
{
    public string Name;
    public string Code;

    public bool Disable { get; set; }

    public override string CategorizedKey(string name)
    {
        return name switch
        {
            nameof(Code) => Code,
            _ => throw new CategorizedKeyException(name, name),
        };
    }
}
