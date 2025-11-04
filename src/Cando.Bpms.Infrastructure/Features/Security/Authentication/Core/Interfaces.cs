using System.Security.Claims;

namespace Neo.Bpms.Infrastructure.Features.Security.Authentication.Core;

#region IIdentityValidator
/// <summary>
///     Used to validate an item
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IIdentityValidator<in T>
{
    /// <summary>
    ///     Validate the item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IdentityResult Validate(T item);
}
#endregion IIdentityValidator

#region IPasswordHasher
/// <summary>
///     Abstraction for password hashing methods
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    ///     Hash a password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    string HashPassword(string password);

    /// <summary>
    ///     Verify that a password matches the hashed password
    /// </summary>
    /// <param name="hashedPassword"></param>
    /// <param name="providedPassword"></param>
    /// <returns></returns>
    PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
}
#endregion IPasswordHasher

#region IIdentityMessageService
/// <summary>
///     Expose a way to send messages (i.e. email/sms)
/// </summary>
public interface IIdentityMessageService
{
    /// <summary>
    ///     This method should send the message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    void Send(IdentityMessage message);
}
#endregion IIdentityMessageService

#region IClaimsIdentityFactory
/// <summary>
///     Interface for creating a ClaimsIdentity from a user
/// </summary>
/// <typeparam name="TUser"></typeparam>
public interface IClaimsIdentityFactory<TUser> where TUser : class, IUser
{
    /// <summary>
    ///     Create a ClaimsIdentity from an user using a UserManager
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="user"></param>
    /// <param name="authenticationType"></param>
    /// <returns></returns>
    ClaimsIdentity Create(TUser user, string authenticationType);
}
#endregion IClaimsIdentityFactory
/// <summary>
///     Interface to generate user tokens
/// </summary>
public interface IUserTokenProvider
{
    /// <summary>
    ///     Generate a token for a user with a specific purpose
    /// </summary>
    /// <param name="purpose"></param>
    /// <param name="manager"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    string Generate(string purpose, IdentityUser user);

    /// <summary>
    ///     Validate a token for a user with a specific purpose
    /// </summary>
    /// <param name="purpose"></param>
    /// <param name="token"></param>
    /// <param name="manager"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    bool Validate(string purpose, string token, IdentityUser user);

    /// <summary>
    ///     Notifies the user that a token has been generated, for example an email or sms could be sent, or 
    ///     this can be a no-op
    /// </summary>
    /// <param name="token"></param>
    /// <param name="manager"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    void Notify(string token, IdentityUser user);

    /// <summary>
    ///     Returns true if provider can be used for this user, i.e. could require a user to have an email
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    bool IsValidProviderForUser(IdentityUser user);
}
