using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;
using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage.Dto;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Common;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormsDataRoutines;

public class FormDataRoutines(FormStructRoutines formStructRoutines,
    ILogger<FormDataRoutines> logger, ICmmnDocument cmmnDocument)
{
    public async Task<IndexFormData> GetRecordsWithoutJoin(
        CommonFormStructure structure,
        Entity entity, Form form, string culture,
        ElasticObject filterValues, string sortFields,
        int pageNo, int recordsPerPage, QueryInfo queryInfo, LocalParameters lp,
        IList<string> filterList, IdentityUser user, bool checkPaging, bool setAssociationDisplay,
        CancellationToken cancellationToken)
    {
        JoinQueriesData joinQueries =
            new($"{structure.NamespaceId}.{structure.EntityId}.{Guid.NewGuid().ToString()}");
        IndexFormData result = new(filterValues);
        if (structure.hasMandatoryFilter && filterValues == null || form == null)
            return result;
        lp ??= new LocalParameters(user);
        if (user != null)
            lp.AddOrUpdate("user", user);
        if (filterValues != null && filterValues["user"] == null)
            filterValues["user"] = user;
        if (checkPaging)
        {
            result.recordCount =
                GetRecordCount(structure, entity, form, filterValues, queryInfo, lp, filterList, cancellationToken);
            if (result.recordCount <= 0) return result;
        }

        FormQuery formQuery = new(form, structure, cancellationToken);
        QueryUtility q = formQuery.EstablishQuery();
        Dictionary<string, FormField> referFormFields = formQuery.EstablishEntityQueryForIndex(filterValues,
            sortFields, filterList, false, joinQueries, false, checkPaging);
        q.SetPage(pageNo, recordsPerPage);
        if (!q.GetDocuments(filterValues, lp))
            return result;
        queryInfo?.AddByQueryUtility(q);
        List<ElasticObject> records = [.. GetRecordsRows(q, result)];
        foreach (var record in records)
        {
            lp.AddOrUpdate("q", record);
            FetchJoinQuery.SetJoinQueryReference(culture, joinQueries, q, referFormFields, record, setAssociationDisplay, user);
            SetEnumValues(culture, form, record);
            foreach (TableDefinition table in structure.Tables)
            {
                string parentIds = record.GetString("Id");
                if (!string.IsNullOrEmpty(table.TableDef.AssociationId))
                {
                    EntityField associationField = ProjectDefinition.Project.GetEntity(table.NamespaceId, table.EntityId)
                        ?.GetField(table.TableDef.AssociationId);
                    parentIds = associationField?.AssociationEntity?.Maps != null &&
                                associationField.AssociationEntity.Maps.Count > 0
                        ? record[associationField.AssociationEntity.Maps[0].SourceField]?.ToString()
                        : record[table.TableDef.AssociationId]?.ToString();
                }

                List<ElasticObject> tableRecords = await GetTableRecords(parentIds, culture, lp, table, user, record, setAssociationDisplay, cancellationToken);
                record.SetField(table.FieldName, tableRecords);
            }
            await LoadDocuments(entity, form, record, cancellationToken);
        }
        q.ReleaseQuery();
        if (setAssociationDisplay)
            FetchJoinQuery.FetchJoinQueriesData(culture, lp, joinQueries, queryInfo);
        return result;
    }

    public async Task<ElasticObject> GetRecord(Entity entity, string ids,
        CommonFormStructure structure, string culture, Form form,
        IdentityUser user, CancellationToken cancellationToken)
    {
        if (entity == null || form == null) return null;
        JoinQueriesData joinQueries = new($"{structure.NamespaceId}.{structure.EntityId}.{ids}");
        LocalParameters lp = new(user);
        if (user != null)
        {
            lp.AddOrUpdate("user", user);
            lp.AddOrUpdate("userId", user.Id);
        }

        Dictionary<string, FormField> referFormFields = [];
        FormQuery formQuery = new(form, structure, cancellationToken);
        QueryUtility q = formQuery.EstablishQuery();
        formQuery.SetQueryByForm(ids, lp, referFormFields, joinQueries, culture);
        q.SetPage(1, 1);
        if (!q.GetDocuments(lp))
        {
            logger.LogError("Can Not Get Record\r\n In command {CommandTxt}", q.CommandTxt);
            return null;
        }

        ElasticObject record = q.GetRecord();
        record.SetField("__DisplayString__", q.GetBasicFieldsData(record, culture));
        q.ReleaseQuery();
        if (record == null)
        {
            logger.LogInformation("Can Not Find Record \r\n In command {CommandTxt}", q.CommandTxt);
            return null;
        }

        EntityField keyField = q.Entity.KeyFields.FirstOrDefault();
        if (string.IsNullOrEmpty(ids) && keyField != null)
            ids = record.GetString(keyField.Id);
        record.SetField("Ids", ids);
        lp.AddOrUpdate("q", record);
        FetchJoinQuery.SetJoinQueryReference(culture, joinQueries, q, referFormFields, record, false, user);
        //SetEnumValues(culture, form, record);
        FetchJoinQuery.FetchJoinQueriesData(culture, lp, joinQueries);

        foreach (var table in structure.Tables)
        {
            string parentIds = ids;
            if (!string.IsNullOrEmpty(table.TableDef.AssociationId))
            {
                EntityField associationField = ProjectDefinition.Project.GetEntity(table.NamespaceId, table.EntityId)
                    ?.GetField(table.TableDef.AssociationId);
                parentIds = associationField?.AssociationEntity?.Maps != null &&
                            associationField.AssociationEntity.Maps.Count > 0
                    ? record[associationField.AssociationEntity.Maps[0].SourceField]?.ToString()
                    : record[table.TableDef.AssociationId + "Id"]?.ToString() ??
                       record[table.TableDef.AssociationId]?.ToString();
            }

            List<ElasticObject> tableRecords = await GetTableRecords(parentIds, culture, lp, table, user, record, true, cancellationToken);
            record.SetField(table.FieldName, tableRecords);
        }
        await LoadDocuments(entity, form, record, cancellationToken);
        return record;
    }

    #region private
    internal async Task<List<ElasticObject>> GetTableRecords(string ids, string culture,
        LocalParameters lp, TableDefinition table, IdentityUser user,
        ElasticObject parentRecord, bool setAssociationDisplay, CancellationToken cancellationToken)
    {
        UiEntity entity = table.TableDef.TableEntity;
        Form form = table.TableDef.TableEntity.GetEntityForm(null, Form.eFormType.Index, table.FormSubjectId);
        string sortFields = "";
        ElasticObject filterValues = parentRecord?.Clone();
        CommonFormStructure structure = await formStructRoutines.GetIndexStructure(culture,
            entity.NamespaceId, entity.Id, form.FormSubjectId, form.Id, sortFields, form, user);
        List<string> filterList = [];
        if (ids != null)
        {
            filterList.Add(TableAssociationFilter(table, ids));
        }
        var filterProperty = table.GetProperty(eControlPropertyId.FilterFormula)?.Value?.ToString();
        if (!string.IsNullOrEmpty(filterProperty))
        {
            filterList.Add(filterProperty);
        }
        var recordCountProperty = table.GetProperty(eControlPropertyId.MaxRecordCount)?.Value;

        IndexFormData indexData = await GetRecordsWithoutJoin(structure, entity, form, culture, filterValues,
            sortFields, 1, recordCountProperty==null?100: recordCountProperty.Int(), null, lp, filterList, user,
            false, setAssociationDisplay, cancellationToken);
        var docFields = structure.ColumnInfos.Where(f => f.ControlType == eControlTypeId.File || f.ControlType == eControlTypeId.AdvancedUpload);
        if (docFields.Any())
        {
            foreach (var row in indexData.Rows)
            {
                await LoadDocuments(entity, form, row, cancellationToken);
            }
        }
        return indexData.Rows;
    }

    internal List<ElasticObject> GetSubTableRecords(List<string> ids, string culture,
        LocalParameters lp, TableDefinition table, IdentityUser user, bool setAssociationDisplay,
        bool getTableParentFieldId, CancellationToken cancellationToken)
    {
        UiEntity entity = table.TableDef.TableEntity;
        Form form = table.TableDef.TableEntity.GetEntityForm(null, Form.eFormType.Edit, table.FormSubjectId);
        if (form is null) return null;
        CommonFormStructure structure = formStructRoutines.GetEditStructure(culture,
            entity.NamespaceId, entity.Id, form.FormSubjectId, form.Id, form, user);
        List<string> filterList = [];
        if (ids != null)
        {
            filterList.Add(TableAssociationFilter(table, ids));
        }
        var filterProperty = table.GetProperty(eControlPropertyId.FilterFormula)?.Value?.ToString();
        if (!string.IsNullOrEmpty(filterProperty))
        {
            filterList.Add(filterProperty);
        }
        var recordCount = table.GetProperty(eControlPropertyId.MaxRecordCount)?.Value;

        string parentFieldId = "";
        if (getTableParentFieldId)
        {
            parentFieldId = table.TableDef.TableAssociation.Maps.FirstOrDefault()?.SourceField;
        }

        IndexFormData indexData = GetRecords(structure, entity, form, culture, null,
            null, lp, filterList, user, setAssociationDisplay, parentFieldId,
            recordCount?.Int(), cancellationToken);
        return indexData.Rows;
    }

    private async Task LoadDocuments(Entity entity, Form form, ElasticObject record, CancellationToken cancellationToken)
    {
        foreach (var docField in form.formFields.Where(f => f.Field?.FieldType == TVariableTypes.File))
        {
            var loadData = docField.CheckProperty(eControlPropertyId.ShowDocumentInPage);
            List<DocumentView> documents = await cmmnDocument.GetDocuments(entity, docField.Id, (int)record.Id, loadData, cancellationToken);
            record[docField.Id] = documents;
        }
    }
    #endregion
    #region static methods
    public static IndexFormData GetRecords(CommonFormStructure structure, Entity entity, Form form, string culture,
        ElasticObject filterValues, QueryInfo queryInfo, LocalParameters lp,
        IList<string> filterList, IdentityUser user, bool setAssociationDisplay, string parentFieldId,
        int? recordCount, CancellationToken cancellationToken)
    {
        JoinQueriesData joinQueries = new();
        IndexFormData result = new(filterValues);
        if (form == null)
            return result;
        lp ??= new LocalParameters(user);
        if (user != null)
            lp.AddOrUpdate("user", user);
        if (filterValues != null && filterValues["user"] == null)
            filterValues["user"] = user;
        long count = GetRecordCount(structure, entity, form, filterValues, queryInfo, lp, filterList, cancellationToken);
        if (count <= 0) return result;
        result.recordCount = count;
        FormQuery formQuery = new(form, structure, cancellationToken);
        QueryUtility q = formQuery.EstablishQuery();
        Dictionary<string, FormField> referFormFields = formQuery.EstablishEntityQuery(filterValues,
            filterList, false, joinQueries);
        if (!string.IsNullOrEmpty(parentFieldId))
            q.SelectField(parentFieldId);
        if (recordCount != null)
            q.SetPage(1, recordCount.Int());
        if (!q.GetDocuments(filterValues, lp))
            return result;
        queryInfo?.AddByQueryUtility(q);
        List<ElasticObject> records = [.. GetRecordsRows(q, result)];
        Parallel.ForEach(records, record =>
        {
            FetchJoinQuery.SetJoinQueryReference(culture, joinQueries, q, referFormFields, record, setAssociationDisplay, user);
            SetEnumValues(culture, form, record);
        });
        q.ReleaseQuery();
        if (setAssociationDisplay)
        {
            FetchJoinQuery.FetchJoinQueriesData(culture, lp, joinQueries, queryInfo);
        }
        return result;
    }
    public static string GetEnumText(Type enumType, object value, string culture)
    {
        // Handle nullable enums
        Type underlyingType = Nullable.GetUnderlyingType(enumType);
        if (underlyingType != null && underlyingType.IsEnum)
        {
            enumType = underlyingType;
        }

        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Type must be an enum or nullable enum", nameof(enumType));
        }

        // Convert the value to the underlying enum type
        object enumValue;
        try
        {
            enumValue = Enum.ToObject(enumType, value);
        }
        catch
        {
            return value.ToString();
        }

        // Get the enum name
        string name = Enum.GetName(enumType, value);
        if (name == null)
        {
            return value.ToString();
        }
        if (culture == "fa")
        {
            // Get the field info for the enum value
            var fieldInfo = enumType.GetField(name);
            if (fieldInfo == null)
            {
                return name;
            }

            // Prefer Display attribute (supports resource-based localization)
            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute?.GetName() is { } displayName && !string.IsNullOrWhiteSpace(displayName))
            {
                return displayName;
            }

            // Fallback to Description attribute if available
            var descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null && !string.IsNullOrWhiteSpace(descriptionAttribute.Description))
            {
                return descriptionAttribute.Description;
            }
        }

        // Fall back to the enum member name
        return name;
    }
    public static void SetEnumValues(string culture, Form form, ElasticObject record)
    {
        foreach (var field in form.formFields.Where(f => f.Field?.IsEnum ?? false))
        {
            var value = record[field.Id];
            if (value != null)
            {
                var enumType = field.Field.CSharpType;
                var nullableType = Nullable.GetUnderlyingType(field.Field.CSharpType);
                if (nullableType != null)
                {
                    enumType = nullableType;
                }
                record[field.Id] = GetEnumText(enumType, value, culture);
            }
        }
    }

    public static void SetEnumValues(string culture, ElasticObject record, Entity entity)
    {
        foreach (var field in entity.entityFields.Values.Where(f => f.IsEnum))
        {
            var value = record[field.Id];
            if (value != null)
            {
                record[field.Id] = GetEnumText(field.CSharpType, value, culture);
            }
        }
    }

    public static string GetPKFilter(string ids, Entity entity)
    {
        List<EntityField> keys = entity?.KeyFields.ToList();
        if (keys?.Count == 1)
            return keys[0].Id + "=='" + ids + "'";
        if (keys != null)
        {
            List<string> filters = [];
            string[] sids = ids.Split('#');
            int i_sids = 0;
            foreach (EntityField item in keys)
            {
                filters.Add("(" + item.Id + "=='" + sids[i_sids] + "')");
                i_sids++;
                if (i_sids >= sids.Length)
                    break;
            }

            return string.Join(" And ", filters);
        }

        throw new Exception("Key not found");
    }

    public static ElasticObject GetKeyRecord(Entity entity, string ids)
    {
        ElasticObject r = new();
        if (string.IsNullOrEmpty(ids)) return r;
        List<EntityField> keys = [.. entity.KeyFields];
        if (keys.Count == 1)
            r.SetField(keys.FirstOrDefault()?.Id, ids);
        else
        {
            string[] sids = ids.Split('#');
            int i_sids = 0;
            foreach (EntityField item in keys)
            {
                r.SetField(item.Id, sids[i_sids]);
                i_sids++;
                if (i_sids >= sids.Length)
                    break;
            }
        }

        return r;
    }

    public static LocalParameters GetLocalParamValues(object user, string namespaceId, string entityId,
        string formId, ElasticObject record, ElasticObject record2 = null)
    {
        LocalParameters filterValues = new(user)
        {
            {"q", new ElasticObject()},
            {"NamespaceId", namespaceId},
            {"EntityId", entityId},
            {"FormId", formId}
        };
        ElasticObject q = filterValues["q"] as ElasticObject;
        q?.AddIfNot(record);
        if (record2 != null)
        {
            q?.AddIfNot(record2);
        }

        return filterValues;
    }

    public static object GetTimeSpanDisplayValue(object str)
    {
        if (string.IsNullOrEmpty(str?.ToString())) return "";
        TimeSpan timeSpan;
        try
        {
            timeSpan = (TimeSpan)str;
        }
        catch
        {
            timeSpan = new(Convert.ToInt64(str));
        }
        str = timeSpan.Days * 24 + timeSpan.Hours + ":" + timeSpan.Minutes + ":" + timeSpan.Seconds +
              (timeSpan.Milliseconds > 0 ? "." + timeSpan.Milliseconds : "");
        return str;
    }

    public static string GetCellElementValue(object value, TVariableTypes cellType, string calendar = "shamsi")
    {
        object str = value ?? "";
        switch (cellType)
        {
            case TVariableTypes.Double:
            case TVariableTypes.Decimal:
                try
                {
                    if (decimal.TryParse(value?.ToString(), out decimal v))
                    {
                        if (v == Math.Floor(v))
                        {
                            str = $"{v:n0}";
                        }
                        else
                        {
                            // Format with enough decimal places, then remove trailing zeros
                            var culture = System.Globalization.CultureInfo.CurrentCulture;
                            str = v.ToString("N10", culture);
                            // Remove trailing zeros after decimal point (culture-aware)
                            var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
                            if (str.ToString().Contains(decimalSeparator))
                            {
                                str = str.ToString().TrimEnd('0').TrimEnd(decimalSeparator.ToCharArray());
                            }
                        }
                    }
                    else
                    {
                        str = value?.ToString();
                    }
                }
                catch
                {
                    str = value?.ToString();
                }

                break;
            case TVariableTypes.Int:
            case TVariableTypes.Long:
            case TVariableTypes.Short:
                try
                {
                    if (long.TryParse(value?.ToString(), out long intVal))
                    {
                        str = $"{intVal:n0}";
                    }
                    else
                    {
                        str = value?.ToString();
                    }
                }
                catch
                {
                    str = value?.ToString();
                }
                break;
            case TVariableTypes.BOOL:
                break;
            case TVariableTypes.DayHourMinute:
            case TVariableTypes.DurHourMinute:
                if (!string.IsNullOrEmpty(str.ToString().Trim()))
                {
                    str = GetTimeSpanDisplayValue(str);
                }

                break;
            case TVariableTypes.Date:
            case TVariableTypes.DateStr:
            case TVariableTypes.DateTime:
                if (!string.IsNullOrEmpty(str.ToString().Trim()))
                {
                    DateTime dt = Convert.ToDateTime(str);
                    if (dt.Year > 1900)
                    {
                        string dStr;
                        if (calendar == "shamsi")
                        {
                            PersianCalendar pc = new();
                            dStr = pc.GetYear(dt).ToString() + '/' + pc.GetMonth(dt).ToString("D2") + '/' +
                                   pc.GetDayOfMonth(dt).ToString("D2");
                        }
                        else
                        {
                            dStr = dt.Year.ToString() + '/' + dt.Month.ToString("D2") + '/' + dt.Day.ToString("D2");
                        }

                        if (dt.Hour != 0 || dt.Minute != 0)
                            dStr += " " + dt.Hour + ":" + dt.Minute;
                        str = dStr;
                    }
                    else
                    {
                        str = "";
                    }
                }

                break;
            case TVariableTypes.Link:
                break;
        }

        return str?.ToString();
    }

    private static string TableAssociationFilter(TableDefinition table, string ids)
    {
        if (table.TableDef.TableAssociation.Maps?.Count == 1)
            return table.TableDef.TableAssociation.Maps[0].SourceField + "=='" + ids + "'";

        string[] idArray = ids.Split('#');
        int i = 0;
        List<string> associationFilters = [];
        foreach (EntityRelationMap map in table.TableDef.TableAssociation.Maps ?? Enumerable.Empty<EntityRelationMap>())
        {
            if (i >= idArray.Length) break;
            associationFilters.Add(map.SourceField + "=='" + idArray[i] + "'");
            i++;
        }

        return string.Join(" And ", associationFilters);
    }
    
    private static long GetRecordCount(CommonFormStructure structure, Entity entity, Form form,
        ElasticObject filterValues, QueryInfo queryInfo, LocalParameters lp, IEnumerable<string> filterList,
        CancellationToken cancellationToken)
    {
        if (entity.entityFields == null) return 0;
        FormQuery formQuery = new(form, structure, cancellationToken);
        QueryUtility q = formQuery.EstablishQuery();
        JoinQueriesData joinQueries = new();
        lp.AddOrUpdate("q", filterValues);
        formQuery.EstablishEntityQuery(filterValues, filterList, true, joinQueries);
        long recordCount = q.GetRecordCount(filterValues, lp);
        q.ReleaseQuery();
        queryInfo?.AddByQueryUtility(q);
        return recordCount;
    }

    private static IEnumerable<ElasticObject> GetRecordsRows(QueryUtility q, IndexFormData result)
    {
        foreach (ElasticObject record in q.GetRecords())
        {
            result.Rows.Add(record);
            yield return record;
        }
    }

    private static string TableAssociationFilter(TableDefinition table, List<string> ids)
    {
        if (table.TableDef.TableAssociation.Maps?.Count == 1)
        {
            ids = [.. ids.Select(id => $"'{id}'")];
            return table.TableDef.TableAssociation.Maps[0].SourceField + " IN (" + string.Join(',', ids) + ") ";
        }

        return null;
    }

    #endregion
}
