using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormTemplate;

public class FormTemplate(Form form, string culture)
{
    public string Culture = culture;
    public Form Form = form;
    public string FileName { get; set; }
    public void Export(FormStructRoutines formStructRoutines, MemoryStream stream)
    {
        StreamWriter streamWriter = new(stream);
        CreateFormFields(formStructRoutines, streamWriter);
        FileName = $"{Form.entity.Id}.{Form.Id}";
    }

    private void CreateTables(FormStructRoutines formStructRoutines, StreamWriter stream, FormField tableField)
    {
        NeoStringBuilder result = new();
        Form tableForm = FormStructRoutines.GetForm(tableField.TableEntity.NamespaceId, tableField.TableEntity.Id, null,
            Form.eFormType.Detail, null/*todo*/);
        result += $"<table style =\"margin-left:auto; margin-right:0; border: 1px solid black;\"  dir = \"rtl\"> <caption>{tableForm.GetName(Culture)}</caption> <thead> <tr>";
        foreach (FormField formField in tableForm.formFields.OrderBy(f => f.Name))
        {
            result += $"<th>{formField.GetName(Culture)}</th>";
        }
        result += $"</tr> </thead> <tbody> <tr _list = \"{tableField.Id}\">";
        foreach (FormField formField in tableForm.formFields.OrderBy(f => f.Name))
        {
            result += $"<td _field = \"{formField.Id}\"> </td>";
        }
        result += "</tr> </tbody> </table>";

        stream.Write(result);
    }

    private void CreateFormFields(FormStructRoutines formStructRoutines, StreamWriter stream)
    {
        NeoStringBuilder result = new();
        result += "<!DOCTYPE html> <html> <head> </head> <body>";
        foreach (FormField field in Form.formFields)
        {
            if (field.ControlTypeId.In(eControlTypeId.Accordion, eControlTypeId.AccordionItem, eControlTypeId.FieldSet, eControlTypeId.MultiTab, eControlTypeId.MultiPage, eControlTypeId.MultiTabItem))
                continue;
            if (field.ControlTypeId == eControlTypeId.IndexTable)
                CreateTables(formStructRoutines, stream, field);
            else
            {
                result += $"<div dir = \"rtl\" class=\"" +
                             (field.GetProperty(eControlPropertyId.DoubleWidth) != null
                                 ? "col-lg-6 col-md-6 col-sm-12 " : "col-lg-3 col-md-3 col-sm-6 ") + "\">";
                result += $"<div class=\"dWrapper\"> <label>{field.GetName(Culture)}: </label>";
                result += $"<input _fieldValue = \"{field.Id}\" dir = \"rtl\" disabled=\"\" title=\"\" type=\"text\" name=\"{field.Id}\">";
                result += " </div> </div>";
            }
        }
        result += "</body> </html>";

        stream.Write(result.ToString(), 0, result.Length);
    }
}
