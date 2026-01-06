using Neo.Bpms.Domain.Models.Cmmn.Data;
using Neo.Bpms.Engine.DDL;
using Neo.Bpms.Infrastructure.Features.Bpms.Engine;
using Neo.Bpms.Infrastructure.Features.Bpms.MetaDataPart;
using Neo.Bpms.Infrastructure.Features.MetaLoader;
using Neo.Bpms.Infrastructure.Features.Orm.DDL;

namespace Neo.Bpms.UI.MVC.Controllers;

public class MigrationController : ControllerBaseMVC
{
    public ActionResult Index()
    {
        CheckAccess();
        SetPagePackId("/Migration/Index");

        MigrationOptions optionsModel = new();
        return View(optionsModel);
    }

    private void CheckAccess()
    {
        ViewBag.user = GetUser(User);
    }
}

public class MigrationActionsController(MigrationSingleton migrationManager) 
    : ControllerBaseMVC
{
    [HttpPost]
    public JsonResult Migrate(MigrationOptions options)
    {
        //CheckAccess();

        if (migrationManager.IsBusy)
            return Json(new { WasBusy = true });
        migrationManager.StartMigration(options);

        string messages = migrationManager.PopMessages();
        List<DDLManager.Log> errors = migrationManager.Errors;
        if (options.SyncMetaData)
        {
            try
            {
                EntityMetaDataManager entityMetaDataManager = new();
                //TODO entityMetaDataManager.SaveMetadataToDatabase();
                if (entityMetaDataManager.Errors != null)
                    errors.AddRange(entityMetaDataManager.Errors.Select(e =>
                        new DDLManager.Log
                        {
                            code = "EntityMetaDataManagerErrors",
                            className = e.ForField,
                            comment = e.Exception?.Message
                        }));
                if (ProjectDefinition.Project.HasProcess)
                {
                    BPMNMetaDataManager metaDataManager = new(((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository);
                    metaDataManager.SaveMetadataToDatabase();
                    if (metaDataManager.Errors != null)
                    {
                        errors.AddRange(metaDataManager.Errors.Select(e =>
                            new DDLManager.Log
                            {
                                code = "BPMNMetaDataManager",
                                className = e.ForField,
                                comment = e.Exception?.Message
                            }));
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.LogCritical(e,e.Message);
                errors.Add(new DDLManager.Log
                {
                    code = "SyncMetaDataException",
                    className = "SyncMetaData",
                    comment = e.Message
                });
            }
        }

        return Json(new
        {
            WasBusy = false,
            Messages = messages,
            migrationManager.Commands,
            migrationManager.Errors
        });
    }

    [HttpPost]
    public JsonResult SetEnumerationItems(MigrationOptions options)
    {
        CheckAccess();

        if (migrationManager.IsBusy)
            return Json(new { WasBusy = true });
        migrationManager.SetEnumerationItems(options);

        string messages = migrationManager.PopMessages();

        return Json(new
        {
            WasBusy = false,
            Messages = messages,
            migrationManager.Commands,
            migrationManager.Errors
        });
    }
    [HttpPost]
    public JsonResult Stop()
    {
        CheckAccess();

        migrationManager.Stop();

        return null;
    }

    [HttpGet]
    public JsonResult MigrationStatus()
    {
        CheckAccess();
        string messages = migrationManager.PopMessages();
        return Json(new { Messages = messages, migrationManager.IsBusy });
    }

    private static void CheckAccess()
    {
        return;
        /*Domain.Entities.Security.Authentication.IdentityUser user = GetUser();
        if (!AccessServices.CheckControllerActionAccess(user, "Migration", "Index"))
            throw new HttpException(Messages.PageAccessDenied);
        */
    }
}
