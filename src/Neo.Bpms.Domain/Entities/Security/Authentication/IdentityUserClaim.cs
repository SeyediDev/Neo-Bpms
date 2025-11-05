using Neo.Bpms.Domain.Entities.Cmmn.Storage;

namespace Neo.Bpms.Domain.Entities.Security.Authentication;

public class IdentityUserClaim : CategorizedLogicModel //: System.Security.Claims.Claim
{
    public IdentityUserClaim()
    {
    }

    public IdentityUserClaim(string type, string value)
    {
        Type = type;
        ClaimValue = value;
    }

    public string Type; //ClaimType.Name
    public long ClaimTypeId;

    public string UserId;
    public string ClaimValue;

    public override string CategorizedKey(string name)
    {
        return name switch
        {
            nameof(UserId) => UserId,
            "ClaimType" => Type,
            nameof(Type) => Type,
            nameof(ClaimTypeId) => ClaimTypeId.ToString(),
            _ => throw new CategorizedKeyException(name, name),
        };
    }
}

public class LogicClaimType : LogicModel
{
    public string EnName;
}