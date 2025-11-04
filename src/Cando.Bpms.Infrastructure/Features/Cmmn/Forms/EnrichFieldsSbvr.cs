using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms.UIRules;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Logic;
using Neo.Common.Attributes;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms;

public interface IEnrichFieldsSbvr
{
    void EnrichSbvrWithAutoRules(InputFieldDefinition ifd, EntityField field, FormField? formField);
    void EnrichFieldsWithUIRules(CommonFormStructure structure);
}
public class EnrichFieldsSbvr: IEnrichFieldsSbvr
{
    /// <summary>
    /// Enriches InputFieldDefinition with auto-extracted business rules from EntityField
    /// </summary>
    public void EnrichSbvrWithAutoRules(InputFieldDefinition ifd, EntityField field, FormField? formField)
    {
        if (field == null) return;

        // 1. Extract MaxLength rule
        if (field.MaxLen > 0)
        {
            ifd.SBVRs.Add(new SBVR
            {
                Modality = SBVRModality.Obligatory,
                Subject = "طول متن",
                VerbPhrase = $"حداکثر {field.MaxLen} کاراکتر مجاز است",
                Condition = null
            });
        }

        // 2. Extract MinLength rule
        if (field.MinLen > 0)
        {
            ifd.SBVRs.Add(new SBVR
            {
                Modality = SBVRModality.Obligatory,
                Subject = "طول متن",
                VerbPhrase = $"حداقل {field.MinLen} کاراکتر الزامی است",
                Condition = null
            });
        }

        // 3. Extract Required (NotNull) rule
        /*bool isMandatoryChecked = false;
        if (field.Required)
        {
            if (!field.SBVRs.Any(x => x.Modality == SBVRModality.Obligatory))
            {
                isMandatoryChecked = true;

                ifd.SBVRs.Add(new SBVR
                {
                    Modality = SBVRModality.Obligatory,
                    Subject = "وارد کردن مقدار",
                    VerbPhrase = "الزامی است",
                    Condition = null
                });
            }
        }*/

        // 4. Extract DefaultValue rule
        if (field.DefaultValue != null)
        {
            ifd.SBVRs.Add(new SBVR
            {
                Modality = SBVRModality.Permitted,
                Subject = "مقدار پیش‌فرض",
                VerbPhrase = $"،مقدار '{field.DefaultValue}' تنظیم می‌شود",
                Condition = "در صورت عدم ورود"
            });
        }

        // 5. Extract Entity-level Validations for this FieldId
        if (field.Entity?.Validations != null)
        {
            var fieldValidations = field.Entity.Validations
                .Where(v => v.FieldId == field.Id)
                .ToList();

            foreach (var validation in fieldValidations)
            {
                var exceptionText = validation.GetExceptionText();
                if (!string.IsNullOrEmpty(exceptionText))
                {
                    ifd.SBVRs.Add(new SBVR
                    {
                        Modality = SBVRModality.Necessary,
                        Subject = "اعتبارسنجی",
                        VerbPhrase = exceptionText,
                        Condition = validation.Condition?.ExpressionString
                    });
                }
            }
        }
        /*if (formField != null)
        {
            if (!isMandatoryChecked)
            {
                if (!field.SBVRs.Any(x => x.Modality == SBVRModality.Obligatory))
                {
                    var mandatoryProp = formField.GetProperty(eControlPropertyId.Required);
                    if (mandatoryProp != null && mandatoryProp.Value is bool isMandatory)
                    {
                        if (isMandatory)
                        {
                            ifd.SBVRs.Add(new SBVR
                            {
                                Modality = SBVRModality.Obligatory,
                                Subject = "وارد کردن مقدار",
                                VerbPhrase = "در فرم الزامی است",
                                Condition = null
                            });
                            isMandatoryChecked = true;
                        }
                    }
                }
            }
        }*/
        ifd.SBVRs.AddRange([.. field.SBVRs ?? []]);
    }

    /// <summary>
    /// Enriches all fields with UI Rules from FormLogicDefinition as SBVR
    /// </summary>
    public void EnrichFieldsWithUIRules(CommonFormStructure structure)
    {
        if (structure?.Logic?.Logics == null) return;

        // Track processed rules to avoid duplicates (same operation can be triggered by multiple events)
        var processedRules = new HashSet<string>();

        foreach (var logicItem in structure.Logic.Logics)
        {
            // Get trigger information
            var triggerEvent = logicItem.TriggeringEvent;
            var sourceFieldId = triggerEvent?.Source;
            var eventType = triggerEvent?.EventType ?? UIRuleEvent.eEventType.invalid;

            // Find source field label for better SBVR description
            var sourceField = structure.Fields.FirstOrDefault(f => f.FieldName == sourceFieldId);
            var sourceFieldLabel = sourceField?.Label ?? sourceFieldId;

            // Build trigger description based on event type
            string triggerDescription = GetTriggerDescription(eventType, sourceFieldLabel, sourceFieldId);

            foreach (var operation in logicItem.Operations ?? [])
            {
                var targetField = structure.Fields.FirstOrDefault(f => f.FieldName == operation.TargetId);
                if (targetField == null) continue;

                // Extract ShowHide rules
                if (operation.Type == eOperationType.RemoveClass && operation.Attr == "ShowHide")
                {
                    targetField.SBVRs.Add(new SBVR
                    {
                        Modality = SBVRModality.Necessary,
                        Subject = targetField.Label ?? operation.TargetId,
                        VerbPhrase = "نمایش داده می‌شود",
                        Condition = BuildSbvCondition(triggerDescription, operation.Condition, operation.Formula)
                    });
                }
                // Extract SetProperty rules (ReadOnly, Disabled, Required, etc.)
                else if (operation.Type == eOperationType.SetProperty)
                {
                    var propertyName = operation.Attr switch
                    {
                        "ReadOnly" or "Editable" => "فقط‌خواندنی",
                        "Disabled" or "Enabled" => "غیرفعال",
                        "Required" or "Mandatory" => "الزامی",
                        "Visible" or "ShowHide" => "قابل‌مشاهده",
                        _ => operation.Attr
                    };

                    targetField.SBVRs.Add(new SBVR
                    {
                        Modality = SBVRModality.Necessary,
                        Subject = targetField.Label ?? operation.TargetId,
                        VerbPhrase = $"{propertyName} می‌شود",
                        Condition = BuildSbvCondition(triggerDescription, operation.Condition, operation.Formula)
                    });
                }
            }
        }
    }

    private static string BuildSbvCondition(string triggerDescription, string condition, string formula)
    {
        string text = triggerDescription;
        if (condition != null)
        {
            text += FormulaToString(condition);
        }
        if (formula != null)
        {
            text += FormulaToString(formula);
        }
        return text;
        static string FormulaToString(string expertion)
        {
            return " به شرط " + Parser.ParseTree(expertion).Root?.toText() ?? expertion;
        }
    }


    /// <summary>
    /// Gets a user-friendly description of when a rule is triggered
    /// </summary>
    private static string GetTriggerDescription(UIRuleEvent.eEventType eventType, string sourceFieldLabel, string sourceFieldId)
    {
        return eventType switch
        {
            UIRuleEvent.eEventType.onPageLoad => "هنگام بارگذاری صفحه",
            UIRuleEvent.eEventType.onUserChange when !string.IsNullOrEmpty(sourceFieldLabel) =>
                $"وقتی '{sourceFieldLabel}' تغییر کرد",
            UIRuleEvent.eEventType.onChange when !string.IsNullOrEmpty(sourceFieldLabel) =>
                $"وقتی '{sourceFieldLabel}' تغییر کرد",
            UIRuleEvent.eEventType.onClick when !string.IsNullOrEmpty(sourceFieldLabel) =>
                $"وقتی '{sourceFieldLabel}' کلیک شد",
            UIRuleEvent.eEventType.onFocus when !string.IsNullOrEmpty(sourceFieldLabel) =>
                $"وقتی '{sourceFieldLabel}' فوکوس شد",
            UIRuleEvent.eEventType.onBlur when !string.IsNullOrEmpty(sourceFieldLabel) =>
                $"وقتی '{sourceFieldLabel}' از فوکوس خارج شد",
            UIRuleEvent.eEventType.onSubmitForm => "هنگام ارسال فرم",
            _ when !string.IsNullOrEmpty(sourceFieldId) => $"براساس '{sourceFieldLabel ?? sourceFieldId}'",
            _ => string.Empty
        };
    }

}
