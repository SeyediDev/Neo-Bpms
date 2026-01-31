using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Common;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.JoinQuery;

public static class FetchJoinQuery
{
    public static void FetchJoinQueriesData(string culture,
        LocalParameters lp, JoinQueriesData joinQueries,
        QueryInfo queryInfo = null)
    {
        List<FetchJoinQueryData> queries = [.. joinQueries.List.Select(joinQuery =>
                new FetchJoinQueryData
                {
                    Culture = culture,
                    LocalParameters = lp?.Clone(),
                    JoinQueryData = joinQuery
                })];
        Parallel.ForEach(queries, fetchJoinQueryData => fetchJoinQueryData.Fetch(queryInfo));
        foreach (FetchJoinQueryData fetchJoinQueryData in queries)
        {
            fetchJoinQueryData.Set();
        }
    }

    public static void SetJoinQueryReference(string culture,
        JoinQueriesData joinQueries,
        QueryUtility q, Dictionary<string, FormField> referFormFields,
        ElasticObject record, bool setDisplayValue, IdentityUser user)
    {
        foreach (FormField referFormField in referFormFields.Values)
        {
            EntityField field = referFormField.Field;
            if (field.Id != referFormField.Id)
            {
                string[] fieldIds = referFormField.Id.Split('.');
                for (int i = 1; i < fieldIds.Length; i++)
                    field = field?.AssociationEntity?.Entity()?.GetField(fieldIds[i]);
            }

            if (field?.AssociationEntity?.Entity() == null) continue;
            joinQueries.TryGetValue(field.AssociationEntity, out JoinQueryData joinQueryData);
            if (joinQueryData == null) continue;
            string[] overFieldIds = referFormField.Id.Split('.');
            if (field.AssociationEntity.Entity().IsEntityState)
            {
                string overFieldName = overFieldIds.Length > 1
                    ? string.Join(".", overFieldIds.Take(overFieldIds.Length - 1)) + ".StateId"
                    : "StateId";
                string stateId = record.GetString(overFieldName);
                record.SetField(referFormField.Id, q.Entity.GetState(stateId)?.Name ?? stateId);
                if (!string.IsNullOrEmpty(stateId))
                    joinQueryData.Add(stateId, referFormField.Id, record);
                continue;
            }

            if (field.AssociationEntity.Entity().DisplayStrings == null)
                continue;
            string fkIds = "";
            if (field.AssociationEntity.Maps != null)
            {
                foreach (EntityRelationMap map in field.AssociationEntity.Maps)
                {
                    string overFieldName = overFieldIds.Length > 1
                        ? string.Join(".", overFieldIds.Take(overFieldIds.Length - 1)) + "." + map.SourceField
                        : map.SourceField;
                    record.GetField(overFieldName, out object fk);
                    fkIds += (string.IsNullOrEmpty(fkIds) ? "" : "#") + fk;
                }
            }

            if (!string.IsNullOrEmpty(fkIds))
            {
                joinQueryData.Add(fkIds, referFormField.Id, record);
            }

            if (!setDisplayValue)
            {
                record.SetField(referFormField.Id, fkIds);
                continue;
            }

            string displayValue = "";
            foreach (BasicField item in field.AssociationEntity.Entity().DisplayStrings)
            {
                if (!item.CheckCulture(culture))
                    continue;
                if (!string.IsNullOrEmpty(item.OtherFieldThatIgnoreMe))
                {
                    string key1 = referFormField.Id + "." + item.FieldId;
                    if (record.GetField(key1, out object obj1) || obj1 == null)
                        continue;
                }

                string key = referFormField.Id + "." + item.FieldId;
                if (!string.IsNullOrEmpty(displayValue)) displayValue += "-";
                displayValue += record.GetString(key) ?? "";
            }

            if (!string.IsNullOrEmpty(displayValue))
                record.SetField(referFormField.Id, displayValue);
            else if (user?.IsAdmin ?? false)
                record.SetField(referFormField.Id, fkIds);
        }
    }
}
