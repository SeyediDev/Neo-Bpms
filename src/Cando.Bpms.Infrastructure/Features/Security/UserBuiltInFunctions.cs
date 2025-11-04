namespace Neo.Bpms.Infrastructure.Features.Security;
public class UserBuiltInFunctions : IFunctionImplementations
{
    private static readonly IAccessServices AccessServices = DependencyInjectionHolder.Instance.AccessServices;

    /// <summary>
    /// دریافت شناسه کاربر
    /// </summary>
    /// <param name="user">کاربر</param>
    /// <returns></returns>
    public static string UserId(IdentityUser user)
    {
        return user?.Id;
    }

    /// <summary>
    /// دریافت شناسه کاربر
    /// </summary>
    /// <param name="user">کاربر</param>
    /// <returns></returns>
    [Obsolete]
    public static string userid(IdentityUser user)
    {
        return user?.Id;
    }
    /// <summary>
    /// بررسی میشود ایا کاربر در گروه کاربری وجود دارد یا نه
    /// </summary>
    /// <param name="user">کاربر</param>
    /// <param name="userGroupCode">کد گروه کاربری</param>
    /// <returns></returns>
    public bool InUserGroup(IdentityUser user, string userGroupCode)
    {
        return AccessServices.InUserGroup(user, userGroupCode);
    }
    /// <summary>
    /// بررسی میشود ایا کاربر در گروه کاربری وجود دارد یا نه
    /// </summary>
    /// <param name="user">شناسه کاربر</param>
    /// <param name="userGroupCode">کد گروه کاربری</param>
    /// <returns></returns>
    public bool UserIdInUserGroup(string userId, string userGroupCode)
    {
        return AccessServices.InUserGroup(userId, userGroupCode);
    }
    /// <summary>
    /// کلیم های کاربر
    /// </summary>
    /// <param name="user">کاربر</param>
    /// <param name="claimType">نوع کلیم</param>
    /// <returns></returns>
    public static List<string> UserClaims(IdentityUser user, string claimType)
    {
        return AccessServices.UserClaims(user, claimType);
    }
    /// <summary>
    /// اولین کلیم کاربر 
    /// </summary>
    /// <param name="user">کاربر</param>
    /// <param name="claimType">نوع کلیم</param>
    /// <returns></returns>
    public static string UserClaim(IdentityUser user, string claimType)
    {
        return AccessServices.UserClaims(user, claimType)?.FirstOrDefault();
    }

    /// <summary>
    /// کلیم های کاربر
    /// </summary>
    /// <param name="user">شناسه کاربر</param>
    /// <param name="claimType">نوع کلیم</param>
    /// <returns></returns>
    public List<string> UserIdClaims(string userId, string claimType)
    {
        return AccessServices.UserClaims(userId, claimType);//todo
    }
}
