using Neo.Bpms.Domain.Model.BPMN.Processes;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Bpmn.Extensions;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Infrastructure.Features.Bpms.Engine;

namespace Neo.Bpms.UI.MVC.Controllers;

/// <summary>
/// The form controller.
/// </summary>
public partial class FormController
{
    private async Task UpdateRecordsAndCreateProcess(string namespaceId, string entityId,
        List<string> recordsIds, Form indexForm, ElasticObject indexFilterValues,
        ElasticObject formData, Form form, string taskId, string processId, string processVersion,
        IdentityUser user, long userGroupId, string culture, bool createProcess,
        string associationFieldId = null)
    {
        if (form == null) return;
        AuditTrail auditTrail = new(TriggerTypeId.CreateAndCompleteTaskByUser, form.Name, user, userGroupId)
        {
            MetaEntityId = indexForm?.entity.DbId,
            MetaFormId = indexForm?.DbId,
            FormId = indexForm?.Id ?? form.Id
        };

        Process process = null;
        if (createProcess && !FetchProcess(entityId, form, taskId, processId, processVersion, culture, out process))
            return;
        long itemCount = 0;
        if (string.IsNullOrEmpty(associationFieldId))
            itemCount = await SaveFromDataPerQueryItems(auditTrail, namespaceId, entityId,
                recordsIds, indexForm, indexFilterValues, formData,
                form, taskId, culture, associationFieldId, process);
        if (createProcess)
            itemCount = await CreateProcessPerQueryItems(auditTrail, namespaceId, entityId,
                recordsIds, indexForm, indexFilterValues, formData, form, taskId, processId,
                processVersion, culture, associationFieldId, process);
        DataStorage.SaveAudit(auditTrail);
        if (itemCount == 0)
        {
            ModelState.AddModelError("", culture == "en"
                ? "Can not find any instance to create process " + taskId + " of entity " + entityId
                : "سندی جهت شروع فرآیند " + taskId + " در موجودیت " + entityId +
                  " یافت نشد. احتمالا پایگاه داده یکسان نیست. با مدیر سامانه در‌میان بگذارید");
        }
    }

    private Task<long> CreateProcessPerQueryItems(AuditTrail auditTrail, string namespaceId, string entityId,
        List<string> recordsIds,
        Form indexForm, ElasticObject indexFilterValues, ElasticObject formData, Form form, string taskId,
        string processId, string processVersion, string culture,
        string associationFieldId, Process process)
    {
        return DoPerQueryItems(auditTrail, namespaceId, entityId, recordsIds, indexForm, indexFilterValues, formData,
            false,
            form, taskId, processId, processVersion, culture, true, associationFieldId, process);
    }

    private Task<long> SaveFromDataPerQueryItems(AuditTrail auditTrail, string namespaceId, string entityId,
        List<string> recordsIds,
        Form indexForm, ElasticObject indexFilterValues, ElasticObject formData, Form form, string taskId,
        string culture, string associationFieldId, Process process)
    {
        return DoPerQueryItems(auditTrail, namespaceId, entityId, recordsIds, indexForm, indexFilterValues,
            formData, true, form, taskId, null, null,
            culture, false, associationFieldId, process);
    }

    private async Task<long> DoPerQueryItems(AuditTrail auditTrail, string namespaceId, string entityId,
        List<string> recordsIds, Form indexForm,
        ElasticObject indexFilterValues, ElasticObject formData, bool saveFromData, Form form, string taskId,
        string processId, string processVersion, string culture, bool createProcess, string associationFieldId,
        Process process)
    {
        int? maxRows = 5000;
        if (!FetchQuery(auditTrail?.User, namespaceId, entityId, recordsIds, indexForm, indexFilterValues,
            formData, form, taskId, culture, associationFieldId, maxRows, process, out QueryUtility q))
            return 0;
        long itemCount = (long)0;
        object lockObject = new();
        string workDescription = formData.GetString(RenderingForm.WorkDescription);
        LocalParameters lpFormData = formData.ToLocalParameters();
        await Task.WhenAll(q.GetRecords().Select(record =>
        {
            lock (lockObject)
                itemCount++;
            return DoPerQueryItem(auditTrail, namespaceId, entityId, formData, lpFormData, saveFromData, form,
                taskId, processId, processVersion, culture, createProcess, associationFieldId,
                process, q, record, workDescription);
        }));
        q.ReleaseQuery();
        return itemCount;
    }

    private async Task DoPerQueryItem(AuditTrail auditTrail, string namespaceId, string entityId,
        ElasticObject inFormData, LocalParameters lpFormData, bool saveFromData, Form form,
        string taskId, string processId, string processVersion,
        string culture, bool createProcess, string associationFieldId, Process process,
        QueryUtility q, ElasticObject record, string workDescription)
    {
        string entityPkv;
        ElasticObject formData = inFormData.Clone();
        string keyId = q.Entity?.KeyFields?.FirstOrDefault()?.Id;
        if (!string.IsNullOrEmpty(associationFieldId))
        {
            formData[associationFieldId] = record[keyId];
            formData[associationFieldId + "Id"] = record[keyId];
            if (createProcess)
                await CreateRecordOfProcess(auditTrail, namespaceId, entityId, formData, form, associationFieldId);
            entityPkv = formData[keyId]?.ToString() ?? formData["Ids"]?.ToString();
        }
        else
        {
            entityPkv = record[keyId]?.ToString() ?? record["Ids"]?.ToString();
            formData[keyId] = entityPkv;
        }

        Property keyProperty = process?.properties?.FirstOrDefault(p => p.fieldId == keyId);
        if (keyProperty != null)
        {
            record[keyProperty.fieldId] = record.GetString(keyProperty.Name);
            if (string.IsNullOrEmpty(entityPkv))
                entityPkv = record.GetString(keyProperty.fieldId);
        }
        if (saveFromData)
        {
            ExceptionInfos errors = []; 
            if (!await applyFormData.UpdateRecord(auditTrail, namespaceId, entityId, form,
                formData, record, null, errors))
            {
                AddModelError((IReadOnlyCollection<ExceptionInfo>)errors);
                q.ReleaseQuery();
                return;
            }
        }

        if (createProcess)
            CreateAndCompleteTaskByUser(auditTrail, form, processId, processVersion,
                taskId, culture, entityPkv, lpFormData, workDescription);
    }

    private void AddModelError(IReadOnlyCollection<ExceptionInfo> errors)
    {
        if (errors == null || errors.Count == 0)
            ModelState.AddModelError("Edit", Messages.SaveWasNotSuccessful);
        else
        {
            foreach (ExceptionInfo error in errors)
                ModelState.AddModelError(error.ForField, error.Exception.Message);
        }
    }

    private bool FetchQuery(IdentityUser user, string namespaceId, string entityId, List<string> recordsIds,
        Form indexForm, ElasticObject indexFilterValues, ElasticObject formData,
        Form form, string taskId, string culture,
        string associationFieldId, int? maxRows, Process process,
        out QueryUtility q)
    {
        LocalParameters localParameters = FormDataRoutines.GetLocalParamValues(user, namespaceId, entityId, form.Id,
            indexFilterValues ?? formData);
        q = !string.IsNullOrEmpty(associationFieldId)
            ? new QueryUtility(indexForm.NamespaceId, indexForm.EntityId)
            : process != null
                ? DataStorage.QueryProcessData(process)
                : new QueryUtility(namespaceId, entityId);
        if (indexFilterValues != null && indexFilterValues["user"] == null)
            indexFilterValues["user"] = user;
        if (recordsIds?.Count > 0)
            q.Where(q.Entity.entityFields.Values.FirstOrDefault(f => f.IncludeInPkv)?.Id +
                    " In(" + string.Join(",", recordsIds) + ")");
        else
        {
            ArgumentNullException.ThrowIfNull(indexForm);
            q.ActiveStates();
            FormDataFilter.AddFilters(q, indexForm as Form, indexFilterValues, out _);
        }

        q.AddPkFields();
        if (maxRows != null)
            q.SetPage(1, maxRows.Value);
        foreach (FormField mapFormField in form.formFields.Where(ff => !(ff.Field?.NotMap ?? true)))
            q.SelectField(mapFormField.Field);
        // ReSharper disable once InvertIf
        if (!q.GetDocuments(localParameters))
        {
            ModelState.AddModelError("", culture == "en"
                ? "Can not find any instance to create process " + taskId + " of entity " + q.Entity.Id
                : "سندی جهت شروع فرآیند " + taskId + " در موجودیت " + q.Entity.Name +
                  " یافت نشد. احتمالا پایگاه داده سینک نیست. با مدیر سامانه درمیان بگذارید");
            return false;
        }

        return true;
    }

    private bool FetchProcess(string entityId, Form form, string taskId, string processId, string processVersion,
        string culture, out Process process)
    {
        process = null;
        if (string.IsNullOrEmpty(processId) || string.IsNullOrEmpty(taskId))
        {
            ModelState.AddModelError("", culture == "en"
                ? "Can not find task for form " + form.Id + " of entity " + entityId
                : "برای فرم " + form.Name + " موجودیت " + entityId + " فرآیندی یافت نشد. ");
            return false;
        }

        process =
            ProjectDefinition.Project.GetBpmnDefinition(processId, processVersion)?.GetRootElement(processId) as
                Process;
        if (process == null)
        {
            ModelState.AddModelError("", culture == "en"
                ? "Can not find task for form " + form.Id + " of entity " + entityId
                : "برای فرم " + form.Name + " موجودیت " + entityId + " فرآیندی یافت نشد. ");
            return false;
        }

        return true;
    }

    private async Task CreateRecordOfProcess(AuditTrail auditTrail, string namespaceId, string entityId,
        ElasticObject record, Form form, string associationFieldId)
    {
        ExceptionInfos errors = []; 
        if (!await applyFormData.CreateRecord(auditTrail, namespaceId, entityId, form,
            record, [associationFieldId], errors))
            AddModelError((IReadOnlyCollection<ExceptionInfo>)errors);
    }

    private void CreateAndCompleteTaskByUser(AuditTrail auditTrail, Form form,
        string processId, string processVersion, string taskId,
        string culture, string entityPkv, LocalParameters lpFormData, string workDescription)
    {
        if (!((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).CreateProcessInstanceAndCompleteUserTask(auditTrail, processId, processVersion,
            taskId, entityPkv, lpFormData, workDescription, TimeSpan.Zero, out _))
            ModelState.AddModelError("", culture == "en"
                ? "Can not create process " + taskId + " for record " + entityPkv + " of entity " + form.entity.Id
                : "برای رکورد " + entityPkv + " موجودیت " + form.entity.Name + " نمی توان فرآیند " + taskId +
                  " را شروع کرد. ");
    }

    private void CheckPossibility(CommonFormStructure structure, ElasticObject formData)
    {
        //			var notOk = formData.Attributes?.FirstOrDefault(a => a.Key.Contains('.') && !string.IsNullOrEmpty(a.Value?.ToString()));
        //			if(notOk?.Key != null)
        //				throw new HttpException(501, $"عملیات گروهی با استفاده از فیلتر {notOk.Value.Key} به صورت چند صفحه ای امکان پذیر نیست.");
    }
}
