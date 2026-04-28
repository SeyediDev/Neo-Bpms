using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms;

public class ControllerMethods(IAccessServices accessServices,
    FolderConfigBackupRestore folderConfigBackupRestore)
{
    public static string CodeFilterValues(ElasticObject filterValues)
    {
        if(filterValues ==null)
        {
            return "";
        }
        ElasticObject fv = filterValues.Clone(false);
        fv.RemoveAttribute("sortFields");
        fv.RemoveAttribute("user");
        return fv.ToJSON();
    }

    public ElasticObject DecodeFilterValues(string parentFilterValues)
    {
        if (string.IsNullOrEmpty(parentFilterValues))
        {
            return null;
        }

        if (parentFilterValues.StartsWith("\"") && parentFilterValues.EndsWith("\""))
        {
            parentFilterValues = parentFilterValues[1..^1].Replace("\\\"", "\"");
        }
        return parentFilterValues.FromJSON();
    }

    public bool CheckPageAccess(string namespaceId, string entityId,
        string formId, string reportId, string dashboardId,
        UiEntity entity, IdentityUser user,
        ref Form form, ref Report report, ref Dashboard dashboard,
        out string errorString)
    {
        errorString = "";
        if (!string.IsNullOrEmpty(formId))
        {
            form = entity?.GetEntityForm(formId);
            if (form == null)
            {
                errorString = "خطا: چنین فرمی موجود نیست.";
                return false;
            }
            if (!accessServices.CheckFormAccess(user, form, out _))
            {
                errorString = "خطا: شما به این فرم دسترسی ندارید.";
                return false;
            }
        }
        else if (!string.IsNullOrEmpty(reportId))
        {
            report = entity?.GetReport(reportId);
            if (report == null)
            {
                errorString = "خطا: شما دسترسی طراحی گزارش ندارید.";
                return false;
            }
            if (!accessServices.CheckReportAccess(user, report))
            {
                errorString = "خطا: شما به این گزارش دسترسی ندارید.";
                return false;
            }
        }
        else if (!string.IsNullOrEmpty(dashboardId))
        {
            dashboard = entity?.GetDashboard(dashboardId);
            if (dashboard == null)
            {
                errorString = "خطا: چنین داشبوردی موجود نیست.";
                return false;
            }
            if (!accessServices.CheckDashboardAccess(user, dashboard))
            {
                errorString = "خطا: شما به این داشبورد دسترسی ندارید.";
                return false;
            }
        }
        return true;
    }

    public async Task<ConfiguredFolder> FetchFolder(long folderId)
    {
        ConfiguredFolder folder = await folderConfigBackupRestore.GetConfig(folderId)
            ?? throw new Exception("خطا کد پوشه صحیح نیست");
        return folder;
    }
}
