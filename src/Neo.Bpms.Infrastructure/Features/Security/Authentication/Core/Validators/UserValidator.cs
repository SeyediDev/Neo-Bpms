using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Neo.Bpms.Infrastructure.Features.Security.Authentication.Core.Validators;

/// <summary>
///     Validates users before they are saved
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TKey"></typeparam>
public class UserValidator : IIdentityValidator<IdentityUser>
{
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="manager"></param>
    public UserValidator()
    {
        AllowOnlyAlphanumericUserNames = true;
    }

    /// <summary>
    ///     Only allow [A-Za-z0-9@_] in UserNames
    /// </summary>
    public bool AllowOnlyAlphanumericUserNames { get; set; }

    /// <summary>
    ///     If set, enforces that emails are non empty, valid, and unique
    /// </summary>
    public bool RequireUniqueEmail { get; set; }

    /// <summary>
    ///     Validates a user before saving
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IdentityResult Validate(IdentityUser item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        var errors = new List<string>();
        ValidateUserName(item, errors);
        if (RequireUniqueEmail)
        {
            ValidateEmail(item, errors);
        }
        if (errors.Count > 0)
        {
            return IdentityResult.Failed([.. errors]);
        }
        return IdentityResult.Success;
    }

    private void ValidateUserName(IdentityUser user, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(user.UserName))
        {
            errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.PropertyTooShort, "Name"));
        }
        else if (AllowOnlyAlphanumericUserNames && !Regex.IsMatch(user.UserName, @"^[A-Za-z0-9@_\.]+$"))
        {
            // If any characters are not letters or digits, its an illegal user name
            errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.InvalidUserName, user.UserName));
        }
        else
        {
            var owner = UserAndOrganizationStorage.Instance.FindByUserName(user.UserName);
            if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            {
                errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.DuplicateName, user.UserName));
            }
        }
    }

    // make sure email is not empty, valid, and unique
    private void ValidateEmail(IdentityUser user, List<string> errors)
    {
        var email = user.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.PropertyTooShort, "Email"));
            return;
        }
        try
        {
            var m = new MailAddress(email);
        }
        catch (FormatException)
        {
            errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.InvalidEmail, email));
            return;
        }
    }
}
