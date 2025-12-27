using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms.UIRules;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Resources;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;

public interface IApplyFormField
{
    bool ApplyFormRecord(ApplyUtility apply, Entity entity,
        Form form, Form.eFormType formType, ElasticObject record, ElasticObject keyValues,
        IList<string> validFieldIds, out ExceptionInfos errors);
}
public class ApplyFormField: IApplyFormField
{
    public bool ApplyFormRecord(ApplyUtility apply, Entity entity,
        Form form, Form.eFormType formType, ElasticObject record, ElasticObject keyValues,
        IList<string> validFieldIds, out ExceptionInfos errors)
    {
        errors = [];
        if (entity == null || form == null)
        {
            return false;
        }

        if (apply.AuditTrail != null)
        {
            apply.AuditTrail.TraceCode = form.Id;
        }

        if (formType == Form.eFormType.Edit || Form.IsBulk(formType) ||
             !form.AllFieldsAreReadOnly())
        {
            foreach (FormField formField in form.formFields)
            {
                if (formField.FieldOrControlType is not FormField.Type.ColumnField and
                     not FormField.Type.Field)
                {
                    continue;
                }

                if (formField.Field == null || formField.Id.Split('.').Length > 1)
                {
                    continue;
                }

                if ((form.FormType == Form.eFormType.Delete || form.FormType == Form.eFormType.VirtualDelete)
                     && !formField.Field.IncludeInPkv)
                {
                    continue;
                }

                if (formField.ControlTypeId==eControlTypeId.File)
                {
                    continue;
                }

                if (formField.CheckProperty(eControlPropertyId.ReadOnly))
                {
                    continue;
                }

                if (!ApplyFormFieldRecord(apply, entity, form, formType, record, keyValues, errors, formField))
                {
                    return false;
                }
            }
        }

        if (formType == Form.eFormType.Edit)
        {
            foreach (EntityField entityField in entity.KeyFields)
            {
                if (!ApplyFieldOfRecord(formType, apply, null, entityField, !entityField.IsAutoIncrement(),
                    record, keyValues, ref errors))
                {
                    return false;
                }
            }
        }

        if (validFieldIds != null)
        {
            foreach (string validFieldId in validFieldIds)
            {
                EntityField entityField = entity.GetField(validFieldId);
                if (entityField == null)
                {
                    ApplyFormData.AddError(ref errors, validFieldId, $"فیلد {validFieldId} تعریف نشده است.");
                    return false;
                }

                if (!ApplyFieldOfRecord(formType, apply, null, entityField,
                    !entityField.IsAutoIncrement(), record, keyValues, ref errors))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool ApplyFormFieldRecord(ApplyUtility apply, Entity entity, Form form,
        Form.eFormType formType, ElasticObject record, ElasticObject keyValues,
        ExceptionInfos errors, FormField formField)
    {
        bool bExist = FetchFormFieldValue(entity, record, formField, out object value);
        bool canBeHiddenByUiRule = form.UiRules != null && form.UiRules.Any(uiRule =>
                                              uiRule.Operations != null && uiRule.Operations.Any(o =>
                                                  o.TaskType == UIRuleTask.eTaskType.SetProperty
                                                  && o.SpecificAttribute == eControlPropertyId.ShowHide
                                                  && o.Params != null &&
                                                  o.Params.Any(p => p.field_controlId == formField.Id)));
        bool isMandatory = formField.CheckProperty(eControlPropertyId.Required);
        if (!bExist)
        {
            if (canBeHiddenByUiRule)
            {
                return true;
            }

            if (isMandatory && formField.Field.IsAutoIncrement())
            {
                return true;
            }
        }

        if (formField.ControlTypeId == eControlTypeId.File)
        {
            return ApplyFieldOfRecord(formType, apply, formField, null, isMandatory,
                record, keyValues, ref errors);
        }
        EntityField field = formField.Field;
        //todo check formField validation
        if (field.AssociationEntity == null)
        {
            if (!field.NotMap)
            {
                return ApplyFieldOfRecord(formType, apply, formField, field, isMandatory,
                    record, keyValues, ref errors);
            }
        }
        else if (field.AssociationEntity.Maps != null)
        {
            if (!AddFieldForApply(formType, apply, field, isMandatory,
                record, ref errors, bExist, value, formField))
            {
                return false;
            }
        }

        return true;
    }

    private static bool FetchFormFieldValue(Entity entity,
        IExpressionValue record, FormField formField, out object value)
    {
        bool exist = false;
        value = null;
        EntityField field = formField.Field;
        //todo check formField validation
        if (field.AssociationEntity == null)
        {
            if (!field.NotMap)
            {
                exist = record.GetField(field.Id, out value);
            }
        }
        else if (field.AssociationEntity.Maps != null)
        {
            if (field.AssociationEntity.Entity()?.Id == "FileInfo")
            {
                exist = record.GetField(field.Id, out value);
            }
            else
            {
                string ids = string.Empty;
                bool first = true;
                bool existMapValue = false;
                foreach (EntityRelationMap map in field.AssociationEntity.Maps)
                {
                    EntityField ef = entity.GetField(map.SourceField);
                    if (ef == null)
                    {
                        continue;
                    }

                    if (!first)
                    {
                        ids += "#";
                    }

                    if (record.GetField(ef.Id, out value))
                    {
                        existMapValue = true;
                    }

                    ids += value;
                    first = false;
                }

                if (existMapValue)
                {
                    value = ids;
                    _ = record.SetField(field.Id, value);
                    exist = true;
                }
                else
                {
                    exist = record.GetField(field.Id, out value);
                }
            }
        }

        return exist;
    }

    private static bool ApplyFieldOfRecord(Form.eFormType formType, ApplyUtility apply,
        FormField formField, EntityField field, bool isMandatory,
        ElasticObject record, ElasticObject keyValues, ref ExceptionInfos errors)
    {
        bool bExist = record.GetField(field?.Id ?? formField.Id, out object value);
        if ((!bExist || value == null && keyValues != null) && field != null && field.IncludeInPkv)
        {
            bExist = keyValues.GetField(field.Id, out value);
        }

        if ((!bExist || value == null) && (field != null && field.Id.EndsWith("Id") || formField.Id.EndsWith("Id")))
        {
            bExist = keyValues.GetField(field?.Id[..^2] ?? formField?.Id[..^2], out value);
        }

        if (bExist)
        {
            if (field?.IsTimeSpan() ?? false)
            {
                value = ReformFormData.ReformTimeSpan(formField, value);
            }

            if (formField?.GetProperty(eControlPropertyId.IsPassword) != null)
            {
                value = ReformFormData.ReformPassword(formField, value?.ToString());
            }
        }
        return AddFieldForApply(formType, apply, field, isMandatory,
                         record, ref errors, bExist, value, formField);
    }

    private static bool AddFieldForApply(Form.eFormType formType, ApplyUtility apply, EntityField field,
        bool isMandatory, ElasticObject record,
            ref ExceptionInfos errors, bool bExist, object value, FormField formField)
    {
        if (field != null)
        {
            if (isMandatory && !bExist && !field.IsBoolParam() && !field.IsAutoIncrement())
            {
                ApplyFormData.AddError(ref errors, field.Id, $"{Texts.MissingMandatoryMessage} {field.Id}");
                return false;
            }

            if (bExist)
            {
                if (field.IsLongParam() || field.IsDoubleParam() || field.IsForeignParam() ||
                     field.IsTimeSpan() || field.IsDateTime())
                {
                    value = ReformFormData.ReformLong(value);
                }

                if (field.IsTextParam())
                {
                    value = ReformFormData.ReformText(value);
                }

                if (field.IsBoolParam())
                {
                    value = ReformFormData.ReformBool(field, record, value);
                }

                // Validate Maximum and Minimum values
                if (value != null && !ValidateFieldRange(field, value, ref errors))
                {
                    return false;
                }
            }
        }
        if (bExist || formField?.ControlTypeId == eControlTypeId.File ||
            field != null && !field.IsAutoIncrement() && !Form.IsBulk(formType))
        {
            _ = record.SetField(field?.Id ?? formField?.Id, value);
            _ = (apply?.AddField(field.Id ?? formField?.Id, value));
        }

        return true;
    }

    private static bool ValidateFieldRange(EntityField field, object value, ref ExceptionInfos errors)
    {
        if (field.Maximum == null && field.Minimum == null)
        {
            return true;
        }

        try
        {
            // Validate based on field type
            if (field.IsDoubleParam())
            {
                return ValidateDoubleRange(field, value, ref errors);
            }
            else if (field.IsLongParam() || field.IsForeignParam() || field.IsTimeSpan())
            {
                return ValidateNumericRange(field, value, ref errors);
            }
            else if (field.IsDateTime())
            {
                return ValidateDateTimeRange(field, value, ref errors);
            }
            else if (field.IsTextParam())
            {
                // For text fields, Maximum and Minimum might be used for length validation
                // But MinLen and MaxLen already handle this, so we skip here
                return true;
            }
        }
        catch (Exception ex)
        {
            ApplyFormData.AddError(ref errors, field.Id, $"خطا در اعتبارسنجی محدوده فیلد {field.Name ?? field.Id}: {ex.Message}");
            return false;
        }

        return true;
    }

    private static bool ValidateNumericRange(EntityField field, object value, ref ExceptionInfos errors)
    {
        if (value == null)
        {
            return true;
        }

        long fieldValue = ConvUtill.ToInt64(value);
        string fieldName = field.Name ?? field.Id;

        if (field.Minimum != null)
        {
            long minValue = ConvUtill.ToInt64(field.Minimum);
            if (fieldValue < minValue)
            {
                ApplyFormData.AddError(ref errors, field.Id, 
                    $"مقدار فیلد {fieldName} باید بزرگتر یا مساوی {minValue} باشد. مقدار وارد شده: {fieldValue}");
                return false;
            }
        }

        if (field.Maximum != null)
        {
            long maxValue = ConvUtill.ToInt64(field.Maximum);
            if (fieldValue > maxValue)
            {
                ApplyFormData.AddError(ref errors, field.Id, 
                    $"مقدار فیلد {fieldName} باید کوچکتر یا مساوی {maxValue} باشد. مقدار وارد شده: {fieldValue}");
                return false;
            }
        }

        return true;
    }

    private static bool ValidateDoubleRange(EntityField field, object value, ref ExceptionInfos errors)
    {
        if (value == null)
        {
            return true;
        }

        double fieldValue = ConvUtill.ToDouble(value);
        string fieldName = field.Name ?? field.Id;

        if (field.Minimum != null)
        {
            double minValue = ConvUtill.ToDouble(field.Minimum);
            if (fieldValue < minValue)
            {
                ApplyFormData.AddError(ref errors, field.Id, 
                    $"مقدار فیلد {fieldName} باید بزرگتر یا مساوی {minValue} باشد. مقدار وارد شده: {fieldValue}");
                return false;
            }
        }

        if (field.Maximum != null)
        {
            double maxValue = ConvUtill.ToDouble(field.Maximum);
            if (fieldValue > maxValue)
            {
                ApplyFormData.AddError(ref errors, field.Id, 
                    $"مقدار فیلد {fieldName} باید کوچکتر یا مساوی {maxValue} باشد. مقدار وارد شده: {fieldValue}");
                return false;
            }
        }

        return true;
    }

    private static bool ValidateDateTimeRange(EntityField field, object value, ref ExceptionInfos errors)
    {
        if (value == null)
        {
            return true;
        }

        DateTime fieldValue;
        if (value is DateTime dt)
        {
            fieldValue = dt;
        }
        else if (value is long ticks)
        {
            fieldValue = new DateTime(ticks);
        }
        else if (DateTime.TryParse(value.ToString(), out DateTime parsed))
        {
            fieldValue = parsed;
        }
        else
        {
            ApplyFormData.AddError(ref errors, field.Id, 
                $"نمی‌توان مقدار فیلد {field.Name ?? field.Id} را به تاریخ تبدیل کرد.");
            return false;
        }

        string fieldName = field.Name ?? field.Id;

        if (field.Minimum != null)
        {
            DateTime minValue;
            if (field.Minimum is DateTime minDt)
            {
                minValue = minDt;
            }
            else if (field.Minimum is long minTicks)
            {
                minValue = new DateTime(minTicks);
            }
            else if (DateTime.TryParse(field.Minimum.ToString(), out DateTime minParsed))
            {
                minValue = minParsed;
            }
            else
            {
                return true; // Skip validation if Minimum cannot be parsed
            }

            if (fieldValue < minValue)
            {
                ApplyFormData.AddError(ref errors, field.Id, 
                    $"تاریخ فیلد {fieldName} باید بزرگتر یا مساوی {minValue:yyyy/MM/dd} باشد. تاریخ وارد شده: {fieldValue:yyyy/MM/dd}");
                return false;
            }
        }

        if (field.Maximum != null)
        {
            DateTime maxValue;
            if (field.Maximum is DateTime maxDt)
            {
                maxValue = maxDt;
            }
            else if (field.Maximum is long maxTicks)
            {
                maxValue = new DateTime(maxTicks);
            }
            else if (DateTime.TryParse(field.Maximum.ToString(), out DateTime maxParsed))
            {
                maxValue = maxParsed;
            }
            else
            {
                return true; // Skip validation if Maximum cannot be parsed
            }

            if (fieldValue > maxValue)
            {
                ApplyFormData.AddError(ref errors, field.Id, 
                    $"تاریخ فیلد {fieldName} باید کوچکتر یا مساوی {maxValue:yyyy/MM/dd} باشد. تاریخ وارد شده: {fieldValue:yyyy/MM/dd}");
                return false;
            }
        }

        return true;
    }
}
