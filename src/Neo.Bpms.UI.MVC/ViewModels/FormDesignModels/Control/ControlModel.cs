using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.ViewModels.FormDesignModels.Control;

public class ControlModel
{
    public ControlModel()
    {
    }

    public ControlModel(InputFieldDefinition f)
    {
        ControlId = f.FieldName;
        ParentControlId = f.parentControlId;
        Label = f.Label;
        Properties = f.GetProperties()?.Where(NotHandledInOtherWays).ToList();
        ControlTypeId = f.ControlType;
        ControlGroups = f.ControlType.GetControlGroup();
        FormItemType = f.FormFieldType;
        if (FormItemType == FormField.Type.SubTable && f is TableDefinition)
        {
            TableDefinition tableDefinition = (TableDefinition)f;
            SubTable = new SubTableControlModel(tableDefinition);
        }
    }

    public ControlModel(FormStructRoutines formStructRoutines, FormField f, bool isFilter)
    {
        FormStructRoutines.ReformFieldDefinition(f, "fa", false, out EntityField field, out string label); //todo fa!s
        ControlId = f.ControlId;
        ParentControlId = f.ParentControlId;
        Label = label;
        Properties = f.GetProperties()?.Where(NotHandledInOtherWays).Select(FormPropertyToProperty).ToList();
        ControlTypeId = f.ControlTypeId == eControlTypeId.None && field != null
            ? FormStructRoutines.GetDefaultControlTypeId(field, isFilter)
            : f.ControlTypeId; // todo review!
        ControlGroups = ControlTypeId.GetControlGroup();
        FormItemType = f.FieldOrControlType;
        if (FormItemType == FormField.Type.SubTable)
            SubTable = new SubTableControlModel(f);
        if (ControlTypeId == eControlTypeId.SystemPageLink)
            SystemLinkAddress = new SystemLinkAddress(f.Property(eControlPropertyId.SystemLinkAddress));
    }

    public string ControlId { get; set; }
    public string ParentControlId { get; set; }
    public string Label { get; set; }
    public eControlTypeId ControlTypeId { get; set; }
    public FormField.Type FormItemType { get; set; }
    public IList<UIComponentProperty> Properties { get; set; }
    public IEnumerable<ControlGroup> ControlGroups { get; set; }

    public SubTableControlModel SubTable { get; set; }
    public SubjectColumnControlModel Subject { get; set; }
    public SystemLinkAddress SystemLinkAddress { get; set; }

    public FormField ToFormField(Form form, string randomString)
    {
        FormField formField;
        if (FormItemType == FormField.Type.SubjectField && Subject != null)
        {
            formField = ToSubjectField(form);
        }
        else if ((ControlTypeId == eControlTypeId.IndexTable ||
            ControlTypeId == eControlTypeId.MultipleSelectableCombo) && SubTable != null)
        {
            formField = ToSubTableField(form);
        }
        else
            formField = ToNormalFormField(form, randomString);

        AddProperties(formField);
        return formField;
    }

    private void AddProperties(FormField formField)
    {
        List<FormProperty> properties = Properties?.Select(p => new FormProperty(p)).ToList();
        if (properties?.Any() == true)
        {
            formField.AddProperties(properties);
        }
    }

    private FormField ToNormalFormField(Form form, string randomString)
    {
        HandleSystemPageLink();
        FormField formField = new()
        {
            Id = NormalizeRecent(ControlId, randomString),
            ParentControlId = NormalizeRecent(ParentControlId, randomString), // todo merge normalizes
            FieldOrControlType = FormItemType,
            Name = Label,
            ControlTypeId = ControlTypeId,
            Parent = form,
            Field = form.GetEntityField(ControlId)
        };
        formField.AddProperty(eControlPropertyId.LabelName, Label);
        return formField;
    }

    private string NormalizeRecent(string id, string randomString)
    {
        return id?.Replace("recently-", randomString);
    }

    private void HandleSystemPageLink()
    {
        if (ControlTypeId == eControlTypeId.SystemPageLink)
        {
            if (SystemLinkAddress == null || !SystemLinkAddress.IsValid)
            {
                throw new ValidationException($"آدرس فرم در کنترل {ControlId} به درستی تنظیم نشده است.");
            }
            Properties ??= [];
            Properties.Add(new UIComponentProperty(eControlPropertyId.SystemLinkAddress,
                    SystemLinkAddress.ToCommaSeparatedForm()));
        }
    }

    private FormField ToSubTableField(Form form)
    {
        UiEntity tableEntity = ProjectDefinition.Project.GetUiEntity(SubTable.NamespaceId, SubTable.EntityId) ?? throw new ValidationException($"موجودیت مربوط به جدول {ControlId} یافت نشد.");
        Association tableAssociation = (tableEntity.GetField(SubTable.TableAssociationId)?.AssociationEntity) ?? throw new ValidationException($"فیلد ارتباطی مربوط به موجودیت {SubTable.EntityId} یافت نشد.");
        FormField formField = FormField.NewSubTableInstance(form, ParentControlId, SubTable.EntityId,
                SubTable.TableAssociationId, SubTable.IndexFormSubjectId, SubTable.AssociationId,
                tableEntity, tableAssociation, Label, null);
        if (ControlTypeId == eControlTypeId.MultipleSelectableCombo)
        {
            UIComponentProperty multipleForeignKeyFieldIdProperty =
                (Properties?.FirstOrDefault(p => p.Id == eControlPropertyId.MultipleForeignKeyFieldId)) ?? throw new ValidationException(
                    $"کلید خارجی موجودیت متناظر برای لیست قابل انتخاب چندگانه‌ی {Label ?? ControlId} مشخص نشده است.");
            if (tableEntity.GetField(multipleForeignKeyFieldIdProperty.Value?.ToString()) == null)
            {
                throw new ValidationException(
                    $"کلید خارجی موجودیت متناظر {multipleForeignKeyFieldIdProperty.Value} در {Label ?? ControlId} معتبر نیست.");
            }

            formField.SubTableToMultipleCombo(multipleForeignKeyFieldIdProperty.Value?.ToString());
            Properties.Remove(multipleForeignKeyFieldIdProperty);
        }

        return ControlId != formField.Id && form.formFields.Any(f => f.Id == formField.Id)
                ? throw new ValidationException(
                    $"جدول یا لیست قابل انتخاب چندگانه با ویژگی های تکراری {ControlId} {formField.Id}")
                : formField;
    }

    private FormField ToSubjectField(Form form)
    {
        FormField formField = new(form, Subject.FormSubjectId) { FieldOrControlType = FormField.Type.SubjectField };
        if (!Subject.HasEdit)
            formField.AddProperty(eControlPropertyId.NoEditIcon, true);
        if (!Subject.HasDetails)
            formField.AddProperty(eControlPropertyId.NoDetailsIcon, true);
        if (!string.IsNullOrWhiteSpace(Subject.FormSubjectId))
            formField.AddProperty(eControlPropertyId.Subject, Subject.FormSubjectId);
        if (!string.IsNullOrWhiteSpace(Label))
            formField.AddProperty(eControlPropertyId.LabelName, Label);
        return formField;
    }

    private static UIComponentProperty FormPropertyToProperty(FormProperty p)
    {
        return new UIComponentProperty
        {
            PropertyId = p.Id,
            Value = p.value
        };
    }

    private static bool NotHandledInOtherWays(FormProperty p)
    {
        return p.Id != eControlPropertyId.LabelName && p.Id != eControlPropertyId.Subject &&
            p.Id != eControlPropertyId.SystemLinkAddress;
    }

    private static bool NotHandledInOtherWays(UIComponentProperty p)
    {
        return p.Id != eControlPropertyId.LabelName && p.Id != eControlPropertyId.Subject &&
            p.Id != eControlPropertyId.SystemLinkAddress;
    }
}
