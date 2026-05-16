namespace Neo.Bpms.Infrastructure.Features.Security.Authentication.Core;

/// <summary>
///     Implements password hashing methods
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    ///     Hash a password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public virtual string HashPassword(string password)
    {
        return Crypto.HashPassword(password);
    }
}
