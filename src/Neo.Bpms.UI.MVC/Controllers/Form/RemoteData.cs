using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Service.ServiceOperation;
using Neo.Bpms.UI.MVC.ViewModels.Forms;

namespace Neo.Bpms.UI.MVC.Controllers;


public partial class FormController
{
    [HttpPost]
    public async Task<JsonResult> GetComboData(ComboDataBindingModel model)
    {
        if (model == null)
            return Json(new ComboDataViewModel(new ComboData()));
        List<LCField> lcs = model.LCs?.Select(lc => lc.ToLcField()).ToList();
        UiEntity formEntity = ProjectDefinition.Project.GetUiEntity(model.NamespaceId, model.EntityId);
        AcquireComboSpecifics(model.NamespaceId, model.FormId, model.FieldId, formEntity, model.PageType,
            model.Qs, lcs, out FormField formField, out string displayFields,
            out Entity entity, out EntityField field, out Entity associationEntity, out IdentityUser user, out (string interfaceId, string operationId) operationAddress);
        if (associationEntity == null && operationAddress.operationId == null)
            return Json(null);
        if (formField.ControlTypeId == eControlTypeId.MultipleSelectableCombo)
        {
            model.IsMandatory = true;
        }
        ElasticObject q = new();
        ElasticObject lc = new();
        LocalParameters filterValues = new()
        {
                {"q", q},
                {"lc", lc},
                {"NamespaceId", model.NamespaceId},
                {"EntityId", model.EntityId},
                {"user", user}
            };
        //var qs = record.GetSubElasticObject("Qs");
        //var lcs = record.GetSubElasticObject("LCs");
        foreach (QField item in model.Qs ?? Enumerable.Empty<QField>())
            q.SetField(item.Field, item.Value);

        if (operationAddress.operationId != null)
        {
            return await InterfaceOperationResult(operationAddress, q, model.Expression);
        }

        foreach (LCField item in lcs ?? Enumerable.Empty<LCField>())
            lc.SetField(item.Field, item.Value);
        string filter = GetFieldFilter(formField, model.Filter);
        filter = AddExpressionFilter(model, filter, displayFields, associationEntity);
        ComboData cb = ComboDataRoutines.GetRecords(entity, associationEntity,
            model.IsMandatory, CultureHelper.GetCurrentNeutralCulture(), filter, model.Page,
            field.AssociationEntity?.Constraint, displayFields, filterValues,
            formField.GetProperty(eControlPropertyId.OrderBy)?.ToString(),
            model.Count ?? 30, true, formField.GetProperties());
        return Json(new ComboDataViewModel(cb));
    }

    private async Task<JsonResult> InterfaceOperationResult(
        (string interfaceId, string operationId) operationAddress, ElasticObject q, string search)
    {
        // Enum.TryParse(
        // 	operationAddress.interfaceId,
        // 	true, out ServiceInterfaceProtocol serviceInterfaceProtocol);
        ServiceInterfaceProtocol serviceInterfaceProtocol = ServiceInterfaceProtocol.InternalLibrary;
        LocalParameters parameters = q.ToLocalParameters();
        parameters.Add("_Search", search);
        ComboDataViewModel comboDataViewModel = await CallComboServiceOperation(operationAddress, serviceInterfaceProtocol, parameters);
        return Json(comboDataViewModel);
    }

    public async Task<ComboDataViewModel> CallComboServiceOperation((string interfaceId, string operationId) operationAddress,
        ServiceInterfaceProtocol serviceInterfaceProtocol, LocalParameters parameters)
    {
        (LocalParameters outParams, System.Net.HttpStatusCode statusCode) = await serviceOperationManager.CallServiceOperation(null,
            serviceInterfaceProtocol, operationAddress.operationId, parameters);
        
        ComboDataViewModel comboDataViewModel = new(new ComboData
        {
            Rows = (outParams["Rows"] as List<LocalParameters>)?.Select(e => new FormDataRow
            { Ids = e.GetString("Id"), DisplayValue = e.GetString("Name") }).ToList()
        });
        return comboDataViewModel;
    }

    private JsonResult InterfaceOperationInitValues(string[] initValues)
    {
        return Json(new ComboDataViewModel(new ComboData
        {
            Rows = [.. initValues.Select(i => new FormDataRow
            { Ids = i, DisplayValue = i })]
        }));
    }

    public JsonResult GetComboInitValues(ComboInitValuesBindingModel model)
    {
        if (model == null)
            return Json(null);
        UiEntity uiEntity = ProjectDefinition.Project.GetUiEntity(model.NamespaceId, model.EntityId);
        AcquireComboSpecifics(model.NamespaceId, model.FormId, model.FieldId, uiEntity, model.PageType,
            model.Qs, null,
            out FormField formField, out string displayFields,
            out Entity entity, out EntityField field, out Entity associationEntity, out IdentityUser user, out (string interfaceId, string operationId) operationAddress);
        string[] initValuesList = model.InitValues?.Split(',');
        string fieldFilter = GetFieldFilter(formField, model.Filter);
        string initValueFilter = "(Id In (_initValues))";
        string filter = string.IsNullOrEmpty(fieldFilter) ? initValueFilter : $"{fieldFilter} AND {initValueFilter}";
        ElasticObject q = new();
        foreach (QField item in model.Qs ?? Enumerable.Empty<QField>())
            q.SetField(item.Field, item.Value);
        if (operationAddress.operationId != null)
        {
            return InterfaceOperationInitValues(initValuesList);
        }
        ElasticObject lc = new();
        LocalParameters filterValues = new()
        {
                {"q", q},
                {"lc", lc},
                {"NamespaceId", model.NamespaceId},
                {"EntityId", model.EntityId},
                {"user", user},
                {"_initValues", initValuesList}
            };
        string culture = CultureHelper.GetCurrentNeutralCulture();
        ComboData cb = ComboDataRoutines.GetRecords(entity, associationEntity, true, culture, filter, 1,
            field.AssociationEntity?.Constraint, displayFields,
        filterValues, formField.GetProperty(eControlPropertyId.OrderBy)?.ToString(), 5000, true, formField.GetProperties());
        return Json(new ComboDataViewModel(cb));
    }

    private void AcquireComboSpecifics(string namespaceId,
        string formId, string fieldId, UiEntity formEntity,
        PageAddress.PageTypeEnum pageType,
        IList<QField> Qs, IList<LCField> LCs,
        out FormField formField,
        out string displayFields, out Entity entity,
        out EntityField field, out Entity associationEntity, out IdentityUser user, out (string interfaceId, string operationId) operationAddress)
    {
        switch (pageType)
        {
            case PageAddress.PageTypeEnum.Form:
                Form form = formEntity.GetEntityForm(formId);
                if (!CheckPostAccess(form, out user, out _).authorized)
                    throw new Exception("Authorization");
                formField = form?.formFields?.FirstOrDefault(f => f.Id == fieldId);
                break;
            case PageAddress.PageTypeEnum.Report:
                Report report = formEntity.GetReport(formId);
                user = GetUser();
                if (!CheckAccess(user, report))
                    throw new Exception("Authorization");
                formField = report?.formFields?.FirstOrDefault(f => f.Id == fieldId);
                break;
            case PageAddress.PageTypeEnum.Dashboard:
                Dashboard dashboard = formEntity.GetDashboard(formId);
                user = GetUser();
                if (!CheckAccess(user, dashboard))
                    throw new Exception("Authorization");
                formField = dashboard?.formFields?.FirstOrDefault(f => f.Id == fieldId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pageType), pageType, null);
        }
        if (formField == null)
            throw new HttpException("Form field not found.");
        displayFields = formField.Property(eControlPropertyId.DisplayFields);
        entity = formEntity;
        field = entity.GetField(fieldId);
        if (string.IsNullOrEmpty(displayFields))
            displayFields = field?.Property(EntityFieldPropertyId.DisplayFields);
        associationEntity = field?.AssociationEntity?.Entity();
        if (field == null)
        {
            string[] obj = fieldId.Split('_');
            if (obj.Length == 3 || obj.Length == 4)
            {
                entity = ProjectDefinition.Project.GetEntityByEntityId(obj[0], namespaceId);
                fieldId = obj.Last();
                field = entity?.GetField(fieldId);
                if (field == null)
                    throw new HttpException($"Multi combo field not found. {fieldId}");
                associationEntity = field?.AssociationEntity?.Entity();
            }
        }

        string[] fieldIds = fieldId.Split('.');
        if (fieldIds.Length > 1)
        {
            field = entity.GetField(fieldIds[0]);
            for (int i = 1; i < fieldIds.Length; i++)
            {
                field = field?.AssociationEntity?.Entity()?.GetField(fieldIds[i]);
            }

            associationEntity = field?.AssociationEntity?.Entity();
        }

        FormProperty p = formField.GetProperty(eControlPropertyId.AssociatedEntityFormula);
        if (field != null && associationEntity == null && p != null)
        {
            object result = formLogicHelper.ServerEval(user, namespaceId, entity?.Id, p.value.ToString(), Qs, LCs);
            associationEntity = ProjectDefinition.Project.GetEntityByEntityId(result?.ToString());
        }
        if (field == null)
            throw new Exception($"Field not found. {fieldId}");
        operationAddress = (formField.Property(eControlPropertyId.ServiceInterfaceProtocol),
            formField.Property(eControlPropertyId.OperationName));
        if (associationEntity == null && string.IsNullOrEmpty(operationAddress.operationId))
        {
            throw new Exception($"Association entity not found. {fieldId}");
        }
    }


    private static string AddExpressionFilter(ComboDataBindingModel model, string filter, string displayFields,
        Entity associationEntity)
    {
        if (model.Expression != null)
        {
            string[] expressions = model.Expression.Split('-');
            foreach (string expressionItem in expressions)
            {
                string filterValuesStr = expressionItem.Trim();
                if (string.IsNullOrEmpty(filterValuesStr)) continue;
                if (!string.IsNullOrEmpty(filter))
                    filter += " And ";
                filter += "(";
                bool first = true;
                if (string.IsNullOrEmpty(displayFields))
                    GetBasicStringFilter("", associationEntity, ref filter, ref first,
                        filterValuesStr /*filterValueSpace*/, 1);
                else
                    GetDisplayFieldsFilter(displayFields, ref filter, ref first,
                        filterValuesStr);
                filter += ")";
            }
        }

        return filter;
    }

    private static string GetFieldFilter(FormField formField, string clientFilter)
    {
        string result = "";
        string formFieldFilter = formField.Property(eControlPropertyId.FilterFormula);
        if (!string.IsNullOrEmpty(formFieldFilter))
            result = $"({formFieldFilter})";
        if (!string.IsNullOrEmpty(clientFilter) && clientFilter != formFieldFilter)
        {
            if (!string.IsNullOrEmpty(result))
                result += " AND ";
            result += $"({clientFilter})";
        }
        return result;
    }

    private static void GetDisplayFieldsFilter(string displayFields,
        ref string filter, ref bool first, string filterValue)
    {
        string[] list = displayFields.Split(',');
        foreach (string descriptionField in list)
            GetFieldsFilter(ref filter, ref first, filterValue, descriptionField);
    }

    private static void GetBasicStringFilter(string association, Entity associationEntity,
        ref string filter, ref bool first, string filterValue, int level)
    {
        if (level > 2) return;
        foreach (BasicField bas in associationEntity.DisplayStrings)
        {
            //todo check culture
            EntityField basicField = associationEntity.GetField(bas.FieldId);
            if (basicField.AssociationEntity?.Entity() != null)
            {
                EntityField mapField =
                    basicField.Entity.GetField(basicField.AssociationEntity?.Maps.FirstOrDefault()?.SourceField ?? "");
                if (mapField == null || mapField.NotMap)
                    continue;
                GetBasicStringFilter(association + bas.FieldId + ".", basicField.AssociationEntity?.Entity(), ref filter,
                    ref first, filterValue, level + 1);
            }
            else if (!basicField.NotMap || basicField.Formula != null)
                GetFieldsFilter(ref filter, ref first, filterValue, association + bas.FieldId);
        }
    }

    private static void GetFieldsFilter(ref string filter, ref bool first, string filterValue, string fieldId)
    {
        if (!first)
            filter += " Or ";
        filter += "(strany((" + fieldId + "),'" + filterValue + "'))";
        string persianValue = GetPersianValue(filterValue);
        if (persianValue != filterValue)
        {
            filter += " Or ";
            filter += "(strany((" + fieldId + "),'" + persianValue + "'))";
        }
        first = false;
    }

    private static string GetPersianValue(string filterValue) =>
        filterValue.ToLower().Aggregate("", (current, item) => current + GetPersianChar(item));

    private static char GetPersianChar(char inChar)
    {
        return inChar switch
        {
            'q' => 'ض',
            'w' => 'ص',
            'e' => 'ث',
            'r' => 'ق',
            't' => 'ف',
            'y' => 'غ',
            'u' => 'ع',
            'i' => 'ه',
            'o' => 'خ',
            'p' => 'ح',
            '[' => 'ج',
            ']' => 'چ',
            '\\' => 'ژ',
            '|' => 'پ',
            '`' => 'پ',
            'a' => 'ش',
            's' => 'س',
            'd' => 'ی',
            'f' => 'ب',
            'g' => 'ل',
            'h' => 'ا',
            'j' => 'ت',
            'k' => 'ن',
            'l' => 'م',
            ';' => 'ک',
            '\'' => 'گ',
            'z' => 'ظ',
            'x' => 'ط',
            'c' => 'ز',
            'v' => 'ر',
            'b' => 'ذ',
            'n' => 'د',
            'm' => 'ئ',
            ',' => 'و',
            _ => inChar,
        };
    }
}
