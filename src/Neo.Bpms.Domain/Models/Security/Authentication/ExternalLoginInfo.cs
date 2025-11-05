using System.Security.Claims;

namespace Neo.Bpms.Domain.Models.Security.Authentication;

/// <summary>
///     Used to return information needed to associate an external login
/// </summary>
public class ExternalLoginInfo
{
    /// <summary>
    ///     Associated login data
    /// </summary>
    public IdentityUserLogin Login { get; set; }

    /// <summary>
    ///     Suggested user name for a user
    /// </summary>
    public string DefaultUserName { get; set; }

    /// <summary>
    ///     Email claim from the external identity
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///     The external identity
    /// </summary>
    public ClaimsIdentity ExternalIdentity { get; set; }
}
