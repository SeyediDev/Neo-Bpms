using System.Security.Claims;
using Neo.Bpms.Domain.Models.Cmmn.Storage;

namespace Neo.Bpms.Domain.Models.Security.Authentication;

/// <summary>
///     Default IUser implementation
/// </summary>
public sealed class IdentityUser : IdentityUser<string>,
    IUser, ICategorizedLogicModel, ICheckTimeOut
{
    /// <summary>
    ///     Constructor which creates a new Guid for the Id 
    /// </summary>
    public IdentityUser()
    {
    }
    
    public IdentityUser(object id)
    {
        Id = id?.ToString() ?? Guid.NewGuid().ToString();
    }

    /// <summary>
    ///     Constructor that takes a userName
    /// </summary>
    /// <param name="userName"></param>
    public IdentityUser(string userName, object id)
        : this(id)
    {
        UserName = userName;
    }

    public string CategorizedKey(string name)
    {
        return name switch
        {
            "Name" => UserName,//todo
            nameof(Email) => Email,
            nameof(UserName) => UserName,
            nameof(NationalNumber) => NationalNumber,
            _ => throw new CategorizedKeyException(name, nameof(name)),
        };
    }

    public bool CheckRole(string roleId)
    {
        return Roles != null && Roles.ContainsKey(roleId);
    }

    public void SetRoles(List<Claim> claims)
    {
        foreach (Claim role in claims.Where(claim => claim.Type == ClaimTypes.Role))
        {
            Roles.Add(role.Value, new IdentityRole
            {
                Code = role.Value,
                Name = role.Value
            });
        }
        IsAdmin = CheckRole(Neo.Domain.Constants.Roles.Admin);
    }
    public Dictionary<string, IdentityRole> Roles { get; set; } = [];
    public byte[] OTPSeed { get; set; }
}

/// <typeparam name="TUserKey"></typeparam>
public class IdentityUser<TUserKey> : IdentityBase<TUserKey>, IUser<TUserKey>
{
    /// <summary>
    ///     Email
    /// </summary>
    public virtual string Email { get; set; }

    /// <summary>
    ///     True if the email is confirmed, default is false
    /// </summary>
    public virtual bool EmailConfirmed { get; set; }

    /// <summary>
    ///     The salted/hashed form of the user password
    /// </summary>
    public virtual string PasswordHash { get; set; }
    public virtual bool HasPassword => PasswordHash != null;

    /// <summary>
    ///     A random value that should change whenever a users credentials have changed (password changed, login removed)
    /// </summary>
    public virtual string SecurityStamp { get; set; }

    /// <summary>
    ///     PhoneNumber for the user
    /// </summary>
    public virtual string PhoneNumber { get; set; }

    /// <summary>
    ///     True if the phone number is confirmed, default is false
    /// </summary>
    public virtual bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    ///     Is two factor enabled for the user
    /// </summary>
    public virtual bool TwoFactorEnabled { get; set; }

    /// <summary>
    ///     DateTime in UTC when lockout ends, any time in the past is considered not locked out.
    /// </summary>
    public virtual bool HasLockoutEndDateUtc { get; set; }
    public virtual DateTime LockoutEndDateUtcValue { get; set; }

    /// <summary>
    ///     Is lockout enabled for this user
    /// </summary>
    public virtual bool LockoutEnabled { get; set; }

    /// <summary>
    ///     Used to record failures for the purposes of lockout
    /// </summary>
    public virtual long AccessFailedCount { get; set; }

    /// <summary>
    ///     User name
    /// </summary>
    public virtual string UserName { get; set; }
    public virtual bool Disable { get; set; }
    public virtual string Culture { get; set; }
    public virtual bool IsAdmin { get; set; }
    public virtual string NationalNumber { get; set; }
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
    public virtual long StateId { get; set; }
    public virtual long ElectronicUserLevelId { get; set; }
    public virtual string MobileNo { get; set; }
    public virtual string AvatarPicture { get; set; }
    public virtual DateTime CreationDate { get; set; }
    public virtual DateTime? StartDate { get; set; }
    public virtual DateTime? EndDate { get; set; }
}
