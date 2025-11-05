using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Resources;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;

public class RelationsData : Dictionary<string /*entity key id*/, RelationData>
{
    internal void QueryRelations(string culture)
    {
        foreach (RelationData relationData in Values)
        {
            EntityField field = relationData.Header.Field;
            Entity entity = field.Entity;
            Entity associationEntity = field.AssociationEntity.DestEntity;
            ComboData records = ComboDataRoutines.GetRecords(entity, associationEntity,
                true, culture, null, 1);
            List<string> nationalIds = [];
            if (associationEntity.Id == "Customer")
            {
                foreach (string nationalId in relationData.Requests.Keys)
                {
                    nationalIds.Add(nationalId.PadLeft(10, '0'));
                }
            }
            foreach (FormDataRow record in records.Rows)
            {
                if (!relationData.Requests.TryGetValue(record.Ids, out RelationId relationId))
                {
                    string key = GetDisplayStringKey(record.DisplayValue);
                    if (!relationData.Requests.TryGetValue(key, out relationId))
                    {
                        if (associationEntity.Id == "Customer")
                        {
                            bool flag = true;
                            foreach (string nationalId in nationalIds)
                            {
                                if (key.Contains(nationalId))
                                {
                                    string trimmedNationalCode = nationalId.TrimStart('0');
                                    while (trimmedNationalCode.Length <= 10)
                                    {
                                        if (relationData.Requests.TryGetValue(trimmedNationalCode, out relationId))
                                        {
                                            flag = false;
                                            break;
                                        }
                                        trimmedNationalCode = '0' + trimmedNationalCode;
                                    }
                                    if (!flag)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (flag)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                foreach (PrimaryKeyValue primaryKeyValue in relationId.Ids)
                    primaryKeyValue.Value = record.Ids;
            }
        }
    }

    internal void CreateRelations(List<MainExcelRecord> records, OperationResult result,
        string excelName, IdentityUser user,
        string culture)
    {
        foreach (KeyValuePair<string, RelationData> relationDataItem in this)
        {
            RelationData relationData = relationDataItem.Value;
            if (relationData.Requests == null)
                continue;
            foreach (RelationId request in relationData.Requests.Values.Where(r =>
                r.Ids.Any(i => string.IsNullOrEmpty(i.Value))))
            {
                result.AddError(excelName, relationData.Header, $"{request.DisplayString} {Texts.NotFoundAndCannotSaveAuto}");

                foreach (MainExcelRecord mainRecord in records.ToList())
                {
                    if (mainRecord.ColumnValues.TryGetValue(relationData.Header.FieldName, out ExcelColumnValue column) &&
                         column.RelationId != null && column.Value.ToString() == request.DisplayString)
                    {
                        records.Remove(mainRecord);
                    }
                }

            }

            continue;
        }
    }

    internal void AddRelationsDataRequest(HeaderColumn headerColumn,
        Association associationEntity, ExcelColumnValue excelColumnValue)
    {
        if (excelColumnValue.Value == null)
            return;
        string relationEntity = associationEntity.ReferEntityKey();
        if (!TryGetValue(relationEntity, out RelationData relationData))
        {
            relationData = new RelationData(headerColumn);
            Add(relationEntity, relationData);
        }

        string displayStringKey = GetDisplayStringKey(excelColumnValue.Value.ToString());
        if (!relationData.Requests.TryGetValue(displayStringKey, out RelationId relationId))
        {
            relationId = new RelationId
            {
                DisplayString = excelColumnValue.Value.ToString(),
                Ids = [.. headerColumn.Field.AssociationEntity.DestEntity.KeyFields.Select(keyField => new PrimaryKeyValue { KeyId = keyField.Id })]
            };
            relationData.Requests.Add(displayStringKey, relationId);
        }

        excelColumnValue.RelationId = relationId;
    }

    internal static string GetDisplayStringKey(string displayString)
    {
        return displayString.Trim().Replace("\t", "").Replace("\n", "").Replace("\r", "");
    }
}

public class RelationData
{
    public RelationData(HeaderColumn headerColumn)
    {
        Header = headerColumn;
    }

    internal HeaderColumn Header { get; set; }

    internal Dictionary<string /*display string*/, RelationId> Requests { get; set; }
        = [];
}

internal class RelationId
{
    internal List<PrimaryKeyValue> Ids { get; set; }
    internal string DisplayString { get; set; }
}

internal class PrimaryKeyValue
{
    internal string KeyId { get; set; }
    internal string Value { get; set; }
}
