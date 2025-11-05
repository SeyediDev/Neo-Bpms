namespace Neo.Bpms.UI.MVC.Controllers;
public partial class FormController
{
    /// <summary>
    /// get controller's client operations to manage whole client operations of specific page.
    /// check if user is Valid and authorized to get operations for page, otherwise the user will get redirected to Error page with appropriate message.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public JsonResult GetOperations([FromBody] GetOperationsModel model)
    {
        try
        {
            IdentityUser user = GetUser(User);
            if (user == null || !User.Identity.IsAuthenticated)
                throw new HttpException("خطا");
            UiEntity entity = ProjectDefinition.Project.GetEntity(model.NamespaceId, model.EntityId) as UiEntity ?? throw new Exception("Bad Request");
            Form form = entity.getForm(model.FormId) ?? entity.GetReport(model.FormId);
            model.LCs ??= [];
            if (form == null)
            {
                Dashboard dashboard = entity.GetDashboard(model.FormId) ?? throw new Exception("کد داشبورد صحیح نیست.");
                if (!CheckAccess(user, dashboard))
                    throw new Exception(Messages.PageAccessDenied);
                ServerOperationResult res = formLogicHelper.GetEventServerOperations(model.NamespaceId, model.EntityId, model.FormId, model.source,
                    model.eventName, user, model.Qs,
                    model.LCs, CultureHelper.GetCurrentNeutralCulture());

                return Json(res);
            }
            if (form.FormType == Form.eFormType.Report)
            {
                if (!CheckAccess(user, form as Report))
                    throw new Exception(Messages.PageAccessDenied);
            }
            else
            {
                if (!CheckAccess(user, form))
                    throw new Exception(Messages.PageAccessDenied);
            }
            ServerOperationResult result = formLogicHelper.GetEventServerOperations(model.NamespaceId, model.EntityId, model.FormId, model.source,
                model.eventName, user, model.Qs,
                model.LCs, CultureHelper.GetCurrentNeutralCulture());

            return Json(result);
        }
        catch (Exception e)
        {
            return Json("Error :" + e.Message);
        }
    }
}
public class GetOperationsModel
{
    public string NamespaceId;
    public string EntityId;
    public string FormId;
    public string source;
    public string eventName;
    public List<QField> Qs;
    public List<LCField> LCs;
}
