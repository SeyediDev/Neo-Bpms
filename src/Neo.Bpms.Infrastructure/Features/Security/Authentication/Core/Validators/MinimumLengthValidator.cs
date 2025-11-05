namespace Neo.Bpms.Infrastructure.Features.Security.Authentication.Core.Validators;

/// <summary>
///     Used to validate that passwords are a minimum length
/// </summary>
/// <remarks>
///     Constructor
/// </remarks>
/// <param name="requiredLength"></param>
public class MinimumLengthValidator(int requiredLength) : IIdentityValidator<string>
{

    /// <summary>
    ///     Minimum required length for the password
    /// </summary>
    public int RequiredLength { get; set; } = requiredLength;

    /// <summary>
    ///     Ensures that the password is of the required length
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IdentityResult Validate(string item)
    {
        if (string.IsNullOrWhiteSpace(item) || item.Length < RequiredLength)
        {
            return IdentityResult.Failed(string.Format(CultureInfo.CurrentCulture, Resources.PasswordTooShort, RequiredLength));
        }
        return IdentityResult.Success;
    }
}
