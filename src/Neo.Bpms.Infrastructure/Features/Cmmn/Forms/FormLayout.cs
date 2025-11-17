using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms;

public class FormLayout
{
    public FormItemPropertiesViewModel GetPropertiesViewModel(string namespaceId, string entityId,
        string formSubjectId, string formId, Form.eFormType formType,
        FormField.Type formItemType, string fieldId, IdentityUser user)
    {
        Form form = FormStructRoutines.GetForm(namespaceId, entityId, formId, formType, formSubjectId);
        FormField field =
            form?.formFields?.FirstOrDefault(
                f => f.FieldOrControlType == formItemType && (f.Id ?? f.ControlId) == fieldId);
        return field != null
            ? new FormItemPropertiesViewModel
            {
                FormItemType = field.FieldOrControlType,
                Id = fieldId,
                ParentControlId = field.ParentControlId,
                Label = field.Name,
                ControlTypeId = field.ControlTypeId != eControlTypeId.None
                    ? field.ControlTypeId
                    : FormStructRoutines.GetDefaultControlTypeId(field.Field, false),
                Properties =
                    field.GetProperties()?.Select(p => new UIComponentProperty { PropertyId = p.id, Value = p.value }).ToList()
            }
            : null;
    }

    public void SetPropertiesViewModel(string namespaceId, string entityId, string formSubjectId,
        string formId, Form.eFormType formType,
        FormItemPropertiesViewModel viewModel, IdentityUser user)
    {
        if (viewModel.Id == null)
            return; //todo validation in other layers or other conditions
        Form form = FormStructRoutines.GetForm(namespaceId, entityId, formId, formType, formSubjectId);
        if (form == null) return;
        FormField oldField =
            form.formFields?.FirstOrDefault(
                f => (f.Id ?? f.ControlId) == viewModel.Id && f.FieldOrControlType == viewModel.FormItemType);
        if (oldField == null)
        {
            EntityField f = form.entity.GetField(viewModel.Id);
            oldField = f != null
                ? new FormField(form, f, viewModel.ControlTypeId)
                : new FormField(form, viewModel.ControlTypeId, viewModel.Id);
            form.AddFormField(oldField);
        }
        oldField.FieldOrControlType = viewModel.FormItemType;
        oldField.ControlTypeId = viewModel.ControlTypeId;
        oldField.Name = viewModel.Label;
        oldField.ParentControlId = viewModel.ParentControlId;
        oldField.SetProperties(viewModel.Properties?.Select(p =>
            new FormProperty(p)).ToList());
        ProjectEntityForm.Save(form);
    }

    public void SaveFormLayout(IEntityPage page)
    {
        switch (page)
        {
            case Dashboard dashboard:
                ProjectEntityDashboard.ReConfig(dashboard, dashboard.entity);
                ProjectEntityDashboard.Save(dashboard);
                break;
            case Report report:
                ProjectEntityReport.ReConfig(report, report.entity);
                ProjectEntityReport.Save(report);
                break;
            case Form form:
                ProjectEntityForm.ReConfig(form, form.entity);
                ProjectEntityForm.Save(form);
                break;
        }
    }

    public void SaveFormLayout(List<List<FormElementViewModel>> elements, Form form, IdentityUser user)
    {
        List<FormField> formFields = [];
        SaveFormLayout(null, elements[0], form, formFields);
        form.formFields = formFields;
        SaveFormLayout(form);
    }

    private void SaveFormLayout(string parentsId,
            List<FormElementViewModel> elements, Form form, List<FormField> formFields)
    {
        foreach (FormElementViewModel element in elements)
        {
            FormField formField = form.formFields.FirstOrDefault(ff => ff.Id == element.id);
            if (formField != null)
                formField.ParentControlId = parentsId;
            else
            {
                EntityField entityField = null;
                const string newId = "recently-added-control";
                eControlTypeId controlTypeId = eControlTypeId.None;
                if (string.IsNullOrEmpty(element.id) ||
                    element.id.Length > newId.Length &&
                     element.id[..newId.Length] == newId)
                {
                    Enum.TryParse(element.id[(newId.Length + 1) /*for dash*/..], out controlTypeId);
                    element.id = Guid.NewGuid().ToString();
                }
                else
                    entityField = form.entity.GetField(element.id);
                formField = entityField != null
                    ? new FormField(form, entityField, controlTypeId)
                    : new FormField(form, controlTypeId, element.id);
            }
            if (string.IsNullOrEmpty(formField.Name))
            {
                FormProperty prop = formField.GetProperty(eControlPropertyId.LabelName);
                if (prop != null)
                    formField.Name = prop.value?.ToString();
            }
            if (string.IsNullOrEmpty(formField.TableEntityId))
            {
                FormProperty prop = formField.GetProperty(eControlPropertyId.EntityId);
                if (prop != null)
                    formField.TableEntityId = prop.value?.ToString();
            }
            if (string.IsNullOrEmpty(formField.TableAssociationId))
            {
                FormProperty prop = formField.GetProperty(eControlPropertyId.Association);
                if (prop != null)
                    formField.TableAssociationId = prop.value?.ToString();
            }
            formFields.Add(formField);

            if (element.children != null && element.children.Count > 0)
                SaveFormLayout(element.id, element.children[0], form, formFields);
        }
    }
}
