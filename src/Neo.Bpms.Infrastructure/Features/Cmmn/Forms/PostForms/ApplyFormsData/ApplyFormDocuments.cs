using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Infrastructure.Features.Cmmn.ObjectStorage;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;

public interface IApplyFormDocuments
{
    Task ApplyDocuments(Entity entity, Form form, ElasticObject record, ElasticObject keyValues,
        string ids, CancellationToken cancellationToken= default);
}

public class ApplyFormDocuments(ICmmnDocument cmmnDocument)
    : IApplyFormDocuments
{
    public async Task ApplyDocuments(Entity entity, Form form, ElasticObject record, ElasticObject keyValues, 
        string ids, CancellationToken cancellationToken=default)
    {
        Dictionary<string, object> changes = [];
        foreach (FormField formField in form.formFields)
        {
            if (formField?.Field?.FieldType != TVariableTypes.File)
            {
                continue;
            }
            bool bExist = record.GetField(formField.Id, out object value);

            if ((!bExist || value == null) && formField.Id.EndsWith("Id"))
            {
                bExist = keyValues.GetField(formField?.Id[..^2], out value);
            }
            if (bExist)
            {
                if (value is string documentId && !documentId.Contains(";"))
                {
                    value = documentId.Split(',').FirstOrDefault(g => g.Length > 0);
                }
            }
            value = await ApplyDocument(entity, formField, ids, value, record, cancellationToken);
            changes.Add(formField.Id, value);
        }
        if(changes.Count != 0)
        {
            ApplyUtility apply =new(entity);
            foreach (KeyValuePair<string, object> change in changes)
            {
                _ = record.SetField(change.Key, change.Value);
                _ = (apply?.AddField(change.Key, change.Value));
            }
            apply.Update(record, keyValues, form.DataOperation,
                UserSecurityAccessFlags.Update, form.FormSubjectId, form.FormType==Form.eFormType.VirtualDelete, true);
        }
    }
    
    private async Task<object> ApplyDocument(Entity entity, FormField formField, string ids,
        object value, ElasticObject record, CancellationToken cancellationToken)
    {
        FileSubmitAction fileSubmitAction = record.GetEnumText($"{formField.Id}__Action", FileSubmitAction.Nothing);
        if (fileSubmitAction == FileSubmitAction.Move)
        {
            var documentType = formField.GetProperty(eControlPropertyId.DocumentType)?.Value?.ToInt();
            value = await cmmnDocument.MoveAndSaveFile(
                entity, formField.Id, documentType, ids, null, value?.ToString(), cancellationToken);
        }
        else if (fileSubmitAction == FileSubmitAction.Upload && !string.IsNullOrEmpty(value?.ToString()))
        {
            var documentType = formField.GetProperty(eControlPropertyId.DocumentType)?.Value?.ToInt();
            value = await cmmnDocument.SaveFileData(entity, formField.Id, documentType, ids, value, cancellationToken);
        }
        else if (fileSubmitAction == FileSubmitAction.Remove)
        {
            await cmmnDocument.DeleteFileData(
                entity, formField.Id, ids, value?.ToString(), cancellationToken);
            value = null;
        }

        return value;
    }
}
