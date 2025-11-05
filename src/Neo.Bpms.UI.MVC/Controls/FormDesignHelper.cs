using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;
using Neo.Bpms.UI.MVC.ViewModels.FormDesignModels.Control;
using Microsoft.AspNetCore.Html;

namespace Neo.Bpms.UI.MVC.Controls;

public class FormDesignHelper(FormStructRoutines formStructRoutines, IControlsRenderer controlsRenderer)
{
    public List<OtherInputField> CreateOtherFields(CommonFormStructure structure,
        bool isFilter, ElasticObject record, bool isDesignMode,
        bool isInToolBox, IUrlHelper urlHelper)
    {
        RendererOptions options = new()
        {
            IsFilter = isFilter,
            IsDesignMode = isDesignMode,
            IsInToolBox = isInToolBox
        };

        ControlsRendererData controlsRendererData = new(structure, record, options, null, urlHelper);
        List<OtherInputField> otherInputFields =
            AcquireOtherInputFields(structure, isFilter ? FormField.Type.FilterField : FormField.Type.Field, isFilter);
        foreach (OtherInputField input in otherInputFields)
        {
            input.Html = controlsRenderer.CreateHtmlField(controlsRendererData, input.InputFieldDefinition);
        }

        return otherInputFields;
    }

    public List<OtherInputField> AcquireOtherInputFields(CommonFormStructure structure,
        FormField.Type formFieldType, bool isFilter)
    {
        List<OtherInputField> result = [];
        UiEntity entity =
            ProjectDefinition.Project.GetUiEntity(structure.NamespaceId, structure.EntityId);
        Domain.Entities.Cmmn.Fields.EntityFields entityFields = entity?.entityFields;
        if (entityFields == null) return result;
        foreach (Domain.Entities.Cmmn.Fields.EntityField field in entityFields.Values)
        {
            if (field.AuditField || field.IsStatic)
                continue;
            Domain.Entities.Cmmn.Fields.EntityField classField = entityFields.Values.FirstOrDefault(
                f => f.AssociationEntity?.Maps?.FirstOrDefault(
                    map => map.SourceField == field.Id) != null);
            bool alreadyInTheForm =
                formFieldType == FormField.Type.ColumnField &&
                structure.ColumnInfos.Any(f => f.ColumnName == field.Id || f.ColumnName == classField?.Id)
                || (formFieldType == FormField.Type.Field || formFieldType == FormField.Type.FilterField) &&
                structure.Fields.Any(f => f.FieldName == field.Id || f.FieldName == classField?.Id);
            if (!alreadyInTheForm)
            {
                result.Add(new OtherInputField(formStructRoutines, field, formFieldType, isFilter));
            }
        }

        return result;
    }

    public List<string> CreateToolBoxControls(CommonFormStructure structure, bool isFilter, IUrlHelper urlHelper)
    {
        RendererOptions options = new()
        {
            IsInToolBox = true
        };
        ControlsRendererData controlsRendererData = new(structure, null, options, null, urlHelper);

        List<InputFieldDefinition> fields = GetToolBoxControlsList(isFilter);
        return [.. fields.Select(field => controlsRenderer.CreateControl(controlsRendererData, null, field).ToString())];
    }

    public static List<IndexFormSubjectId> AcquireOtherSubjects(CommonFormStructure structure)
    {
        UiEntity entity = ProjectDefinition.Project.GetUiEntity(structure.NamespaceId, structure.EntityId);
        Dictionary<string, IndexFormSubjectId> subjectForms = [];
        foreach (Form subjectForm in entity.getForms()?.Where(form => !string.IsNullOrEmpty(form.FormSubjectId)) ??
                                    [])
        {
            if (subjectForm.FormSubjectId == structure.FormSubjectId || subjectForm.FormSubjectId == null)
                continue;
            if (structure.Subjects.Any(s => string.Equals(s.Name, subjectForm.FormSubjectId, StringComparison.Ordinal)))
                continue;
            if (subjectForms.TryGetValue(subjectForm.FormSubjectId, out IndexFormSubjectId indexFormSubjectId))
                continue;
            indexFormSubjectId = new IndexFormSubjectId(subjectForm.FormSubjectId)
            {
                Alias = subjectForm.FormSubjectId,
                Name = subjectForm.FormSubjectId
            };
            subjectForms.Add(subjectForm.FormSubjectId, indexFormSubjectId);
        }

        return [.. subjectForms.Values];
    }

    private static List<InputFieldDefinition> GetToolBoxControlsList(bool isFilter)
    {
        List<InputFieldDefinition> list =
        [
            CreateToolboxControl("مجموعه فیلد", eControlTypeId.FieldSet,
                FormField.Type.Control),
            CreateToolboxControl("فیلد موجودیت‌های مرتبط", eControlTypeId.None,
                isFilter ? FormField.Type.FilterField : FormField.Type.Field),
            CreateToolboxControl("لینک به فرم", eControlTypeId.SystemPageLink,
                FormField.Type.Control)
        ];
        if (!isFilter)
        {
            list.Add(CreateToolboxControl("جدول داخلی", eControlTypeId.IndexTable,
                FormField.Type.Control));
            list.Add(CreateToolboxControl("لیست قابل انتخاب چندگانه", eControlTypeId.MultipleSelectableCombo,
                FormField.Type.Control));
        }
        return list;
    }

    private static InputFieldDefinition CreateToolboxControl(string title, eControlTypeId controlType,
        FormField.Type formFieldType)
    {
        InputFieldDefinition result = new(title)
        {
            FieldName = $"recently-added-control-{controlType}",
            parentControlId = null,
            ControlType = controlType,
            FormFieldType = formFieldType,
            IconField = "",
            TooltipField = ""
        };
        result.AddProperty(eControlPropertyId.LabelName, title);
        return result;
    }

    public HtmlString RenderPropertiesObject(CommonFormStructure structure, bool isFilter)
    {
        List<FormField> formFields = GetFormFields(structure, isFilter);

        List<ControlModel> controlsList = GetToolBoxControlsList(isFilter).Select(f => new ControlModel(f)).ToList();
        IEnumerable<ControlModel> otherFields = AcquireOtherInputFields(structure, GetFieldType(structure, isFilter), isFilter)
            .Select(f => new ControlModel(f.InputFieldDefinition));
        controlsList.AddRange(otherFields);
        if (formFields != null)
            controlsList.AddRange(formFields.Select(f => new ControlModel(formStructRoutines, f, isFilter)));

        string controlsModel = JsonConvert.SerializeObject(controlsList,
            Formatting.None,
            new JsonSerializerSettings
            {
                Converters =
                [
                    new Newtonsoft.Json.Converters.StringEnumConverter()
                ]
            });
        if (string.IsNullOrEmpty(controlsModel))
            controlsModel = "[]";
        return new HtmlString(controlsModel);
    }

    private static FormField.Type GetFieldType(CommonFormStructure structure, bool isFilter)
    {
        return isFilter ? FormField.Type.FilterField :
            structure.FormType == Form.eFormType.Index ? FormField.Type.ColumnField : FormField.Type.Field;
    }

    private static List<FormField> GetFormFields(CommonFormStructure structure, bool isFilter)
    {
        UiEntity entity = ProjectDefinition.Project.GetUiEntity(structure.NamespaceId, structure.EntityId);
        if (structure.FormType == Form.eFormType.Report)
        {
            Report report = entity?.GetReport(structure.Form_ReportId);
            return report?.formFields.Where(ff =>
                    ff.FieldOrControlType == FormField.Type.FilterField ||
                    ff.FieldOrControlType == FormField.Type.Control)
                .ToList();
        }

        Form form = entity?.GetEntityForm(structure.Form_ReportId);
        return form != null
            ? form.FormType == Form.eFormType.Index
                    ? [.. form.formFields.Where(f =>
                        isFilter && f.FieldOrControlType != FormField.Type.ColumnField ||
                        !isFilter && f.FieldOrControlType != FormField.Type.FilterField)]
                    : form.formFields
            : (entity?.GetDashboard(structure.Form_ReportId)?.formFields);
    }
}
