using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Data;

public static partial class DataStorage
{
    internal static void SaveDataRecord(ProcessInstance pi, FlowNodeInstance ai, string code,
        out int recordsAffected)
    {
        Process proc = pi.Process;
        recordsAffected = 0;
        if (proc.Entity?.NotMapped ?? false)
        {
            return;
        }

        ApplyUtility au = new(proc.EntityNamespaceId, proc.EntityId, pi.AuditTrail, null);
        ElasticObject recordForSave = new();

        if (ai != null)
        {
            SaveDataRecordFields(au, recordForSave, ai.FlowNode, ai);
        }

        SaveDataRecordFields(au, recordForSave, pi.Process, pi);
        SetKeyInRecord(pi, au, recordForSave);
        SaveStateProperty(pi, au, recordForSave);
        SetAuditFields(pi, ai, code);

        bool hasKey = CheckHasKeyInRecord(au, recordForSave);
        bool applied = hasKey ? au.Update(recordForSave) : au.Insert(recordForSave);
        if (applied)
        {
            recordsAffected = au.RecordsAffected;
            LogSaveDataRecord(pi, ai, code, recordForSave);
        }
        else
        {
            LogErrorInSaveDataRecord(pi, ai, au);
        }

        if (string.IsNullOrEmpty(pi.EntityPkv) && !hasKey && applied)
        {
            pi.FetchData(false, recordForSave, pi.Process);
        }
    }

    private static void SetKeyInRecord(ProcessInstance pi, ApplyUtility au, ElasticObject recordForSave)
    {
        foreach (var keyField in au.Entity.KeyFields ?? [])
        {
            if (!string.IsNullOrEmpty(pi.EntityPkv))
            {
                au.AddField(keyField.Id, pi.EntityPkv);
                recordForSave.SetField(keyField.Id, pi.EntityPkv);
            }
            else
            {
                if (pi.GetData(keyField.Id, out object value))
                {
                    au.AddField(keyField.Id, value);
                    recordForSave.SetField(keyField.Id, value);
                }
            }
            break;
        }
    }

    private static void SaveStateProperty(ProcessInstance pi, ApplyUtility au, IExpressionValue recordForSave)
    {
        if (!pi.GetData(pi.Process.StateProperty, out object value))
        {
            return;
        }

        _ = au.AddField(pi.Process.StateProperty, value);
        _ = recordForSave.SetField(pi.Process.StateProperty, value);
    }

    private static void SetAuditFields(ProcessInstance pi, FlowNodeInstance ai, string code)
    {
        if (pi.AuditTrail == null)
        {
            return;
        }

        pi.AuditTrail.ProcessInstanceId = pi.Id;
        if (ai != null)
        {
            pi.AuditTrail.FlowNodeInstanceId = ai.Id;
        }

        if (!string.IsNullOrEmpty(code))
        {
            pi.AuditTrail.TraceCode = code;
        }
    }

    private static bool CheckHasKeyInRecord(ApplyUtility au, ElasticObject recordForSave)
    {
        bool hasKey = true;
        foreach (var keyField in au.Entity.KeyFields ?? [])
        {
            if (recordForSave.GetField(keyField.Id, out object value) && value != null)
            {
                continue;
            }

            hasKey = false;
            break;
        }
        return hasKey;
    }

    private static void SaveDataRecordFields(ApplyUtility au, ElasticObject recordForSave,
        BaseElement container, IPropertyValueContainer propertyValueContainer)
    {
        object value;
        IPropertyContainer propertyContainer = container as IPropertyContainer;
        foreach (Property property in propertyContainer?.properties ?? Enumerable.Empty<Property>())
        {
            if (propertyValueContainer.GetData(property.Name, out value))
            {
                _ = au.AddField(property.fieldId, value);
                _ = recordForSave.SetField(property.fieldId, value);
            }
        }
        IFlowElementsContainer flowElementContainer = container as IFlowElementsContainer;
        foreach (DataObject dataObject in flowElementContainer?.flowElements?.Values.OfType<DataObject>() ??
                                            [])
        {
            if (propertyValueContainer.GetData(dataObject.fieldId, out value))
            {
                _ = au.AddField(dataObject.fieldId, value);
                _ = recordForSave.SetField(dataObject.fieldId, value);
            }
        }
        if (container?.Parent is IFlowElementsContainer parentFlowElementContainer)
        {
            SaveDataRecordFields(au, recordForSave,
                            parentFlowElementContainer as BaseElement, propertyValueContainer);
        }
    }

    private static void LogSaveDataRecord(ProcessInstance pi, FlowNodeInstance ai, string code,
        ElasticObject recordForSave)
    {
        if (!pi.IsActiveLog)
        {
            return;
        }

        string text = $"Save Record {recordForSave}. process:{pi.Process.Id}, code:{code}, PI:{pi.Id}";
        if (ai != null)
        {
            text += ", Activity:" + ai.FlowNode?.Id;
            text += ", AI:" + ai.Id;
        }
        if (pi.AuditTrail != null)
        {
            text += ", AT:" + pi.AuditTrail.Id;
        }

        pi.LogTrace(text);
    }

    private static void LogErrorInSaveDataRecord(ProcessInstance pi, FlowNodeInstance ai, ApplyUtility au)
    {
        pi.LogError(
            $"can not save data record in entity {au.Entity.Id}. process:{pi.Process.Id}, PI:{pi.Id}, AI:{ai?.Id} command : {au.CommandTxt}");
    }
}
