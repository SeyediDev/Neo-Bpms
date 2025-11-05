using System.Net.Mime;

namespace Neo.Bpms.UI.MVC.Controllers.Public;

public class BpmsController : ControllerBaseMVC
{
    protected static bool HasDesignFeatures()
    {
        return ProjectDefinition.Project.HasDesignFeatures;
    }

    private static bool? _dynamicallyDesignable;
    protected static bool CanApplyDesignChanges()
    {
        if (_dynamicallyDesignable == null)
        {
            _ = bool.TryParse(DependencyInjectionHolder.Instance.Configuration["IsDynamicallyDesignable"],
                out bool isDynamicallyDesignable);
            _dynamicallyDesignable = isDynamicallyDesignable;
        }
        return _dynamicallyDesignable.Value;
    }
    protected static void CheckDesignFeature(bool toApplyChanges)
    {
        if (!HasDesignFeatures() ||
            toApplyChanges && !CanApplyDesignChanges())
        {
            throw new Exception(Messages.DesignFeatureIsNotEnabled);
        }
    }
    protected static bool CanDesignForms(IdentityUser user)
    {
        return HasDesignFeatures() &&
               (user?.CheckSystemFeatureAccess(SystemFeatureId.FormDesign) ?? false);
    }

    protected void CheckProcessesDesignAccess(IdentityUser user, bool toApplyChanges)
    {
        CheckDesignFeature(toApplyChanges);
        if (!CheckAccess(user, SystemFeatureId.ProcessDesign))
        {
            throw new Exception(Messages.YouCantDesignProcesses);
        }
    }

    protected ActionResult GetFileStreamResult(string fileName, string extension, Stream stream, string contentType)
    {
        ContentDisposition cd = new()
        {
            FileName = fileName + extension,
            //Document.FileName;
            Inline = false
        };
        Response.Headers.Append("Content-Disposition", cd.ToString());
        stream.Seek(0, SeekOrigin.Begin);
        return new FileStreamResult(stream, contentType);
    }

    internal static LocalParameters GetLocalParameters(IdentityUser user, ElasticObject record)
    {
        LocalParameters lp = new() { { "user", user }, { "userId", user?.Id } };
        if (record != null)
            lp.Add("q", record);
        return lp;
    }
}
