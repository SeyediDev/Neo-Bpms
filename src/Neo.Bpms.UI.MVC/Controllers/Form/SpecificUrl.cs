using System;
using System.Web;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    [HttpGet]
    public ActionResult SpecificURL(string NamespaceId, string EntityId, string FormSubjectId, string FormId, string ids, string Caller)
    {
        return RedirectToSpecificUrl(NamespaceId, EntityId, FormSubjectId, FormId, ids, Form.eFormType.SpecificURL);
    }

    [HttpGet]
    public ActionResult SpecificURLForRecord(string NamespaceId, string EntityId, string FormSubjectId, string FormId, string ids, string Caller)
    {
        return RedirectToSpecificUrl(NamespaceId, EntityId, FormSubjectId, FormId, ids, Form.eFormType.SpecificURLForRecord);
    }

    private ActionResult RedirectToSpecificUrl(string namespaceId, string entityId, string formSubjectId, string formId, string ids, Form.eFormType formType)
    {
        Form form = FormStructRoutines.GetForm(namespaceId, entityId, formId, formType, formSubjectId);
        if (form == null)
        {
            return Error(Messages.PageNotFound, formType.ToString());
        }

        if (!CheckAccess(form, out IdentityUser user, out _))
        {
            return Error(Messages.PageAccessDenied, formType.ToString());
        }

        string destination = BuildSpecificUrl(form, namespaceId, entityId, formSubjectId, ids);
        if (string.IsNullOrWhiteSpace(destination))
        {
            return Error(Messages.PageNotFound, formType.ToString());
        }

        return Redirect(destination);
    }

    private string BuildSpecificUrl(Form form, string namespaceId, string entityId, string formSubjectId, string ids)
    {
        string specificUrl = form.SpecificUrl?.Trim();
        if (string.IsNullOrWhiteSpace(specificUrl))
        {
            return null;
        }

        bool hasIdsPlaceholder = specificUrl.IndexOf("{ids}", StringComparison.OrdinalIgnoreCase) >= 0;
        specificUrl = specificUrl
            .Replace("{ids}", HttpUtility.UrlEncode(ids ?? string.Empty))
            .Replace("{namespaceId}", HttpUtility.UrlEncode(namespaceId ?? string.Empty))
            .Replace("{entityId}", HttpUtility.UrlEncode(entityId ?? string.Empty))
            .Replace("{formSubjectId}", HttpUtility.UrlEncode(formSubjectId ?? string.Empty))
            .Replace("{formId}", HttpUtility.UrlEncode(form?.Id ?? string.Empty));

        if (!hasIdsPlaceholder && !string.IsNullOrEmpty(ids))
        {
            specificUrl = AppendQueryString(specificUrl, "ids", ids);
        }

        if (specificUrl.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
        {
            specificUrl = specificUrl[..^7];
        }

        if (specificUrl.StartsWith("~/", StringComparison.Ordinal))
        {
            return Url.Content(specificUrl);
        }

        if (Uri.IsWellFormedUriString(specificUrl, UriKind.Absolute))
        {
            return specificUrl;
        }

        return "/" + specificUrl.TrimStart('/');
    }

    private static string AppendQueryString(string url, string key, string value)
    {
        string separator = url.Contains("?") ? "&" : "?";
        return $"{url}{separator}{key}={HttpUtility.UrlEncode(value)}";
    }
}

