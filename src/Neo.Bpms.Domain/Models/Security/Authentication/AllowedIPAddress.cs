using Neo.Bpms.Domain.Models.Cmmn.Storage;

namespace Neo.Bpms.Domain.Models.Security.Authentication;

/// <summary>
///     EntityType that represents one specific user claim
/// </summary>
public class IdentityUserAllowedIPAddress : IdentityUserAllowedIPAddress<string>
{
}

/// <summary>
///     EntityType that represents one specific user claim
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class IdentityUserAllowedIPAddress<TKey> : CategorizedLogicModel, IBaseClassId<long?>
{
    ///// <summary>
    /////     Primary key
    ///// </summary>
    //public virtual int Id { get; set; }

    /// <summary>
    ///     User Id for the user who owns this login
    /// </summary>
    public virtual TKey UserId { get; set; }

    public virtual string IPAddress { get; set; }
    public override string CategorizedKey(string name)
    {
        return name switch
        {
            nameof(UserId) => UserId.ToString(),
            nameof(IPAddress) => IPAddress,
            _ => throw new CategorizedKeyException(name, name),
        };
    }
}
