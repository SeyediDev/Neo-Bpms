using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    public JsonResult DoArithmetic(string NamespaceId, string EntityId, string FormId,
         [ModelBinder(typeof(DynamicActionGetBinder))] ElasticObject indexFormFilterValues)
    {
        Form indexForm = FormStructRoutines.GetForm(NamespaceId, EntityId, FormId, Form.eFormType.Index, null);
        if (indexForm?.formFields == null)
            throw new Exception("Invalid form.");
        IdentityUser user = GetUserAndCheckFormAccess(indexForm);
        List<FormField> arithmeticFields = indexForm.formFields.Where(f => f.GetProperty(eControlPropertyId.Summable) != null)
            .ToList();
        if (arithmeticFields.Count == 0)
            throw new Exception("Invalid form arithmetic fields.");
        Dictionary<string, double> results = [];
        foreach (FormField arithmeticField in arithmeticFields)
        {
            results.Add(arithmeticField.Id, 0);
        }

        LocalParameters localParameters = FormDataRoutines.GetLocalParamValues(user, NamespaceId, EntityId, FormId,
            indexFormFilterValues);
        QueryUtility q = new(indexForm.NamespaceId, indexForm.EntityId);
        if (indexFormFilterValues != null && indexFormFilterValues["user"] == null)
            indexFormFilterValues["user"] = user;
        q.ActiveStates();
        FormDataFilter.AddFilters(q, indexForm, indexFormFilterValues, out _);
        int? maxRows = 5000;//todo ali please.
        q.SetPage(1, maxRows.Value);
        foreach (FormField arithmeticField in arithmeticFields)
            q.Sum(arithmeticField.Id, arithmeticField.Id);
        // ReSharper disable once InvertIf
        if (!q.GetDocuments(localParameters))
        {
            //ModelState.AddModelError("", culture == "en"
            //	? "Can not find any instance to create process " + taskId + " of entity " + q.Entity.Id
            //	: "سندی جهت شروع فرآیند " + taskId + " در موجودیت " + q.Entity.Name +
            //	  " یافت نشد. احتمالا پایگاه داده سینک نیست. با مدیر سامانه درمیان بگذارید");
            throw new Exception("Invalid data.");
        }

        ElasticObject record = q.GetRecord();
        foreach (FormField arithmeticField in arithmeticFields)
            results[arithmeticField.Id] = record.GetDouble(arithmeticField.Id);

        return Json(results);
    }
}
