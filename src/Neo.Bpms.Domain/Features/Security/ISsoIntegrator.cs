using Neo.Bpms.Domain.Features.Security.Dto;

namespace Neo.Bpms.Domain.Features.Security;

public interface ISsoIntegratorParams
{
    string RedirectUrl(string baseUrl);
}
public interface ISsoIntegrator
{
    /// <summary>
    /// این متد بعد از ریدایرکت شدن از صفحه‌یِ لاگینِ خارجی فراخوانی می‌شود.
    /// در پیاده‌سازیِ آن لازم است اعتبارِ لاگین بررسی شود.
    /// </summary>
    /// <param name="parameters">پارامترهایی که سرویسِ لاگینِ خارجی به سیستمِ ما پاس کرده</param>
    /// <param name="httpContext">بعضی کتابخانه‌هایِ اعتبارسنجی به خودِ کانتکست نیاز داشته‌اند</param>
    /// <returns></returns>
    Task<VerificationResult> VerifyRedirectionAsync(IDictionary<string, string> parameters, object httpContext);

    /// <summary>
    /// آدرسِ صفحه‌یِ لاگینِ خارجی
    /// </summary>
    /// <param name="baseUrl">
    /// آدرسِ ریشه‌یِ سیستمِ ما
    /// </param>
    /// <returns></returns>
    string LoginPageUrl(string baseUrl);
    Task<dynamic> Signout(IDictionary<string, string> parameters);
}
