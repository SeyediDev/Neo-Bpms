namespace Neo.Bpms.UI.MVC.Controllers;

public class SaveTheFormModel
{
    public FormAddress FormAddress { get; set; }
    public List<ControlModel> Controls { get; set; }
}

public partial class FormController
{
    public ActionResult Design(FormAddress formAddress)
    {
        IdentityUser user = CheckFormDesignAccess(formAddress, false, out Form form);
        ViewBag.user = user;

        string culture = CultureHelper.GetCurrentNeutralCulture();

        CommonFormStructure structure = formStructRoutines.GetCommonFormStructure(culture, formAddress.NamespaceId,
            formAddress.EntityId, formAddress.FormId, form?.FormType ?? Form.eFormType.Create, null, form, user, null,
            true);
        if (structure == null || form == null)
            return Error("امکان طراحی این صفحه وجود ندارد.", form?.FormType.ToName());

        ViewBag.structure = structure;
        structure.FormType = form.FormType;
        ViewBag.ContainerClass = "container-fluid";
        SetPagePackId(form);
        return View(new ElasticObject());
    }

    [HttpPost]
    public JsonResult SaveTheForm([FromBody] SaveTheFormModel model)
    {
        CheckFormDesignAccess(model.FormAddress, true, out Form form);
        string randomString = new Guid().ToString();
        if (model.FormAddress.IsAddressingColumns)
        {
            form.formFields.RemoveAll(SuitableForIndex);
            form.formFields.AddRange(model.Controls.Select(c => c.ToFormField(form, randomString)).ToList());
        }
        else
        {
            form.formFields.Clear();
            List<FormField> columnFields = form.formFields.Where(SuitableForIndex).ToList();
            if (model.Controls != null)
                form.formFields = [.. model.Controls.Select(c => c.ToFormField(form, randomString))];
            form.formFields.AddRange(columnFields);
        }

        formLayout.SaveFormLayout(form);
        return Json(new { });
    }

    private static bool SuitableForIndex(FormField f)
    {
        return f.FieldOrControlType == FormField.Type.ColumnField ||
            f.FieldOrControlType == FormField.Type.SubjectField;
    }

    [HttpGet]
    public JsonResult GetControllers(FormAddress formAddress)
    {
        CheckFormDesignAccess(formAddress, false, out Form form);
        var controllers = form.UiRules.Select(c => new
        {
            id = c.Id,
            name = c.Name
        });
        return Json(controllers);
    }

    [HttpPost]
    public JsonResult GetController([FromBody] ControllerAddress controllerAddress)
    {
        CheckFormDesignAccess(controllerAddress.FormAddress, true, out Form form);
        ControllerViewModel controller = form.UiRules
                                    .Where(c => c.Id == controllerAddress.ControllerId)
                                    .Select(c => new ControllerViewModel(c))
                                    .FirstOrDefault();
        return Json(controller);
    }

    [HttpGet]
    public JsonResult Targets(FormAddress formAddress)
    {
        CheckFormDesignAccess(formAddress, false, out Form form);
        var targets = form.formFields.Select(ff => new
        {
            id = ff.Id,
            label = ff.Name
        }).ToList();
        return Json(targets);
    }

    [HttpPost]
    public JsonResult SaveController([FromBody] ControllerSaveModel model)
    {
        CheckFormDesignAccess(model.controllerAddress.FormAddress, true, out Form form);
        form.UiRules.RemoveAll(c => c.Id == model.controllerAddress.ControllerId);
        form.UiRules.Add(model.controller.ToUiRule());
        formLayout.SaveFormLayout(form);
        return Json(new { });
    }

    [HttpPost]
    public JsonResult NewController([FromBody] FormAddress formAddress)
    {
        CheckFormDesignAccess(formAddress, true, out Form form);
        dynamic newController = new
        {
            Id = Guid.NewGuid().ToString(),
            Name = "کنترلر جدید"
        };
        form.UiRules.Add(new UIRule(form, newController.Id, newController.Name));
        formLayout.SaveFormLayout(form);
        return Json(newController);
    }

    [HttpPost]
    public JsonResult RemoveController([FromBody] ControllerAddress controllerAddress)
    {
        CheckFormDesignAccess(controllerAddress.FormAddress, true, out Form form);
        form.UiRules.RemoveAll(c => c.Id == controllerAddress.ControllerId);
        formLayout.SaveFormLayout(form);
        return Json(new { });
    }

    private IdentityUser CheckFormDesignAccess(FormAddress formAddress, bool toApplyChanges, out Form form)
    {
        CheckDesignFeature(toApplyChanges);
        IdentityUser user = GetUser(User);
        if (user == null || !User.Identity.IsAuthenticated)
            throw new HttpException("لطفا ابتدا وارد سیستم شوید.");
        if (!CheckAccess(user, SystemFeatureId.FormDesign))
            throw new HttpException("شما دسترسی طراحی فرم ندارید.");

        if (formAddress == null)
        {
            form = null;
            return user;
        }

        UiEntity uiEntity = (UiEntity)ProjectDefinition.Project.GetEntity(formAddress.NamespaceId, formAddress.EntityId);
        form = uiEntity?.getForm(formAddress.FormId) ??
            uiEntity?.GetReport(formAddress.FormId); // todo formAddress should specify where we should look for the form
        return form == null ? throw new HttpException("خطا: چنین فرمی موجود نیست.") : user;
    }
}
