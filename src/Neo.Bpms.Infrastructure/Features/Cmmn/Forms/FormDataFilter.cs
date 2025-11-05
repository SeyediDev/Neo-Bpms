using Neo.Bpms.Domain.Expressions.FunctionImplementations;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms;

public static class FormDataFilter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="qd">Query</param>
    /// <param name="form"></param>
    /// <param name="filterValues"></param>
    /// <param name="distinct"></param>
    public static void AddFilters(QueryUtility qd, Form form, ElasticObject filterValues, out bool distinct)
    {
        distinct = false;
        if (form == null) return;
        CorrectArrayValuesInFilterRecord(filterValues);
        AddFormFilter(qd, form, filterValues, false, out distinct);
        if (filterValues == null || !filterValues.Attributes.Any()) return;
        foreach (FormField formField in form.formFields.Where(f => f.FieldOrControlType == FormField.Type.FilterField))
        {
            if (formField.CheckProperty(eControlPropertyId.SpecialFilter))
                continue;
            FilterParameter filterParameter = FetchFilterParameterId(filterValues, formField.Id, FilterParameter.IsEqualTo);
            bool fetchFilterValue = filterValues.GetField(formField.Id, out object filterValue) && filterValue != null;
            if (filterParameter != FilterParameter.IsNull && filterParameter != FilterParameter.IsNotNull)
            {
                if (!fetchFilterValue)
                    continue;
                if (ControlFunctionsImplementation.IsNullOrEmpty(filterValue))
                    continue;
            }

            bool writeQInFilter = true;
            QueryUtility queryUtility = qd;
            string[] fieldIds = formField.Id.Split('.');
            EntityField field = formField.Field;
            if (fieldIds.Length > 1)
            {
                field = FetchLastField(fieldIds, field);
                writeQInFilter = false;
            }

            if (field == null || queryUtility == null) continue;
            if (field.AssociationEntity != null && field.AssociationEntity.Maps.FirstOrDefault()?.DestField !=
                field.AssociationEntity.Entity().KeyFields.FirstOrDefault().Id)
            {
                ElasticObject relationDoc = QueryUtility.New(field.AssociationEntity.Entity())
                    .SelectFields(string.Join(",", field.AssociationEntity.Maps.Select(m => m.DestField)))
                    .AddPkFilter(filterValue?.ToString())
                    .FirstOrDefault();
                filterValue = string.Join("#", field.AssociationEntity.Maps.Select(m => relationDoc.GetObject(m.DestField)));
                writeQInFilter = false;
            }

            string filter = GenerateFilter(field, writeQInFilter, formField.Id, filterValue, filterParameter);
            if (string.IsNullOrEmpty(filter))
                continue;
            if (fieldIds.Length > 1)
                field = LeftOuterJoin(formField, ref queryUtility, fieldIds);
            if (field == null || queryUtility == null) continue;
            if (field.AssociationEntity == null)
                AddFilterToQuery(queryUtility, formField.TableEntityId, field, writeQInFilter, formField, filterValue,
                    filterParameter, ref distinct);
            else if (field.AssociationEntity.Maps != null)
                AddAssociationFilterToQuery(queryUtility, formField, writeQInFilter, field, filterValue, filterParameter,
                    ref distinct);
        }
    }

    private static void CorrectArrayValuesInFilterRecord(ElasticObject filterValues)
    {
        List<KeyValuePair<string, ElasticObject>> list = [.. filterValues.Attributes];
        for (int i = 0; i < list.Count; i++)
        {
            KeyValuePair<string, ElasticObject> item = list[i];
            if(item.Key.EndsWith("[]"))
            {
                filterValues.SetField(item.Key[..^2], item.Value);
                continue;
            }
        }
    }

    private static EntityField FetchLastField(string[] fieldIds, EntityField field)
    {
        for (int ii = 1; ii < fieldIds.Length; ii++)
        {
            Entity associationEntity = field?.AssociationEntity?.Entity();
            if (associationEntity == null) break;
            field = field.AssociationEntity?.Entity()?.GetField(fieldIds[ii]);
        }

        return field;
    }

    private static EntityField LeftOuterJoin(FormField formField, ref QueryUtility queryUtility, string[] fieldIds)
    {
        EntityField field = formField.Field;
        for (int ii = 1; ii < fieldIds.Length; ii++)
        {
            Entity associationEntity = field?.AssociationEntity?.Entity();
            if (associationEntity == null) break;
            QueryUtility.SubQueryDefinition jq = queryUtility?.LeftOuterJoin(associationEntity, null);
            if ((jq?.fieldMappings?.Count ?? 0) == 0)
                jq?.SetMapping(field);
            queryUtility = jq?.Query;
            field = field.AssociationEntity?.Entity()?.GetField(fieldIds[ii]);
        }

        return field;
    }

    private static void AddAssociationFilterToQuery(QueryUtility queryUtility, FormField formField,
        bool writeQInFilter, EntityField field, object value, FilterParameter filterParameter, ref bool distinct)
    {
        if (field.AssociationEntity.Maps.Count == 1)
        {
            AddFilterToQuery(queryUtility, formField.TableEntityId, field, writeQInFilter, formField, value,
                filterParameter, ref distinct);
        }
        else
        {
            string[] ids = value?.ToString().Split('#');
            int iId = 0;
            foreach (Domain.Entities.Cmmn.Relationship.EntityRelationMap map in field.AssociationEntity.Maps)
            {
                EntityField sf = field.Entity.GetField(map.SourceField);
                if (sf != null)
                    AddFilterToQuery(queryUtility, formField.TableEntityId, sf, false, formField, ids?[iId],
                        filterParameter, ref distinct);
                iId++;
            }
        }
    }

    public static FilterParameter FetchFilterParameterId(ElasticObject record, string formFieldId,
        FilterParameter defaultFilterParameter)
    {
        return record.GetEnum(FilterParameterMethods.FilterParameterName(formFieldId), defaultFilterParameter);
    }

    public static void AddFormFilter(QueryUtility qd, Form form,
        ElasticObject filterValues, bool onlyNoCondition, out bool distinct)
    {
        distinct = false;
        LocalParameters el = FormDataRoutines.GetLocalParamValues(filterValues?["user"] ?? qd.LocalParameters?["user"],
            qd.Entity.model.Id, qd.Entity.Id, form.Id, filterValues);
        if (form.InputRecordsFilters == null) return;
        foreach (EntityFormFilter filter in form.InputRecordsFilters)
        {
            if (filter.filter == null) continue;
            if (onlyNoCondition && filter.condition != null)
                continue;
            if (filter.condition?.Root != null)
            {
                if (!ExpressionNode.CheckIfTrue(GetExpression(filter.condition.Root, el, qd).Eval(null, el)))
                    continue;
            }

            ExpressionNode exp = GetExpression(filter.filter.Root, el, qd);
            if (string.IsNullOrEmpty(filter.entityId) || filter.entityId == qd.Entity.Id)
                qd.Where(exp);
            else
            {
                Entity associationEntity = ProjectDefinition.Project.GetEntityByEntityId(filter.entityId, qd.Entity.model.Id);
                if (associationEntity == null) continue;
                EntityField association = associationEntity.GetField(filter.Association);
                QueryUtility.SubQueryDefinition jq = qd.LeftOuterJoin(associationEntity, null);
                jq?.SetReverseMapping(association);
                jq?.Query?.Where(exp);
                qd.SetDistinct(true);
                distinct = true;
            }
        }
    }

    private static ExpressionNode GetExpression(ExpressionNode root, LocalParameters el, QueryUtility qd)
    {
        ExpressionNode exp = root.clone();
        exp = exp.replace(el);
        exp = exp.processAndReplace(EntityConversions.ConvertFormulaFields, qd);
        exp = exp.processAndReplace(EntityConversions.ConvertNames, qd);
        return exp;
    }

    private static void AddFilterToQuery(QueryUtility qd, string entityId,
        EntityField field, bool writeQ, FormField formField, object value,
        FilterParameter filterParameter, ref bool distinct)
    {
        //if (field.NotMap) return;
        if (string.IsNullOrEmpty(entityId) || entityId == qd.Entity.Id)
            GenerateFilter(qd, field, writeQ, formField, value, filterParameter);
        else
        {
            Entity associationEntity = ProjectDefinition.Project.GetEntityByEntityId(entityId, qd.Entity.model.Id);
            if (associationEntity == null) return;
            FormProperty propAssociation = formField.GetProperty(eControlPropertyId.Association);
            if (propAssociation == null) return;
            EntityField association = associationEntity.GetField(propAssociation.Value?.ToString());
            QueryUtility.SubQueryDefinition jq = qd.LeftOuterJoin(associationEntity, null);
            if (jq?.Query != null)
            {
                jq.SetReverseMapping(association);
                GenerateFilter(jq.Query, field, writeQ, formField, value, filterParameter);
                if (jq.Query.Entity.IsStateBase)
                    jq.Query.ActiveStates();
            }

            qd.SetDistinct(true);
            distinct = true;
        }
    }

    private static void GenerateFilter(QueryUtility qd, EntityField field, bool writeQ,
        FormField formField, object value, FilterParameter filterParameter)
    {
        string filter = GenerateFilter(field, writeQ, formField.Id, value, filterParameter);
        if (!string.IsNullOrEmpty(filter))
            qd?.Where(filter);
    }

    private static string GenerateFilter(
        EntityField field, bool writeQ, string formFieldId, object value, FilterParameter filterParameter)
    {
        if (field.IsForeignParam())
        {
            return GenerateAssociationParamFilter(field, writeQ, formFieldId, value, filterParameter);
        }

        if (field.IsTextParam())
        {
            return GenerateTextParamFilter(field, writeQ, formFieldId, value, filterParameter);
        }

        if (field.IsBoolParam())
        {
            return GenerateBooleanParamFilter(field, value);
        }

        if (field.IsDoubleParam())
        {
            return GenerateDoubleParamFilter(field, writeQ, formFieldId, value, filterParameter);
        }

        if (field.IsLongParam())
        {
            return GenerateLongParamFilter(field, writeQ, formFieldId, value, filterParameter);
        }

        if (field.IsDateTime())
        {
            return GenerateDateTimeParamFilter(field, writeQ, formFieldId, value, filterParameter);
        }

        return field.IsTimeSpan()
            ? field.Id + "==" + (writeQ ? $"q[{formFieldId}]" : value)
            : field.Id + "==" + (writeQ ? $"q[{formFieldId}]" : value);
    }

    private static string GenerateBooleanParamFilter(EntityField field, object value)
    {
        BooleanItem valueItem = BooleanEntityField.GetValueItem(false, value);
        return valueItem switch
        {
            BooleanItem.True => "ISNULL(" + field.Id + ",0)==1",
            BooleanItem.False => "ISNULL(" + field.Id + ",0)==0",
            BooleanItem.Null => field.Id + "==null",
            _ => "",
        };
    }

    private static string GenerateAssociationParamFilter(EntityField field, bool writeQ, string formFieldId,
        object value, FilterParameter filterParameter)
    {
        switch (filterParameter)
        {
            case FilterParameter.IsEqualTo:
            case FilterParameter.Contains:
                return GenerateAssociationParamEqualToFilter(field, value);
            case FilterParameter.IsNotEqualTo:
            case FilterParameter.DoesNotContain:
                return $"!({GenerateAssociationParamEqualToFilter(field, value)})";
            case FilterParameter.StartsWith:
                if (writeQ)
                    return $"{field.Id}>=q[{formFieldId}]";
                return $"{field.Id}>='{value?.ToString().Replace("\'", "\'\'")}'";
            case FilterParameter.EndsWith:
                if (writeQ)
                    return $"{field.Id}<=q[{formFieldId}]";
                return $"{field.Id}<='{value?.ToString().Replace("\'", "\'\'")}'";
            case FilterParameter.IsNull:
                return $"{field.Id} == null";
            case FilterParameter.IsNotNull:
                return $"{field.Id} != null";
        }

        return "";
    }

    private static string GenerateAssociationParamEqualToFilter(EntityField field, object value)
    {
        if (value == null)
            return field.Id + " == null";
        if (!value.ToString().Contains(","))
            return field.Id + "=='" + value + "'";
        if (value.ToString().StartsWith("{"))
            return field.Id + " In(List(" + value + "))";
        string stringifyParams = string.Join(",", value.ToString().Split(',').Select(f => $"'{f}'").ToList());
        return field.Id + " In(" + stringifyParams + ")";
    }

    private static string GenerateTextParamFilter(EntityField field, bool writeQ, string formFieldId, object value,
        FilterParameter filterParameter)
    {
        switch (filterParameter)
        {
            case FilterParameter.IsEqualTo:
                if (writeQ)
                    return $"{field.Id} == q[{formFieldId}]";
                return $"{field.Id} == '{value?.ToString().Replace("\'", "\'\'")}'";
            case FilterParameter.IsNotEqualTo:
                if (writeQ)
                    return $"{field.Id} != q[{formFieldId}]";
                return $"{field.Id} != '{value?.ToString().Replace("\'", "\'\'")}'";
            case FilterParameter.Contains:
                if (writeQ)
                    return $"strany({field.Id},q[{formFieldId}])";
                return $"strany({field.Id},'{value?.ToString().Replace("\'", "\'\'")}')";
            case FilterParameter.DoesNotContain:
                if (writeQ)
                    return $"!(strany({field.Id},q[{formFieldId}]))";
                return $"!(strany({field.Id},'{value?.ToString().Replace("\'", "\'\'")}'))";
            case FilterParameter.StartsWith:
                if (writeQ)
                    return $"strstart({field.Id},q[{formFieldId}])";
                return $"strstart({field.Id},'{value?.ToString().Replace("\'", "\'\'")}')";
            case FilterParameter.EndsWith:
                if (writeQ)
                    return $"strend({field.Id},q[{formFieldId}])";
                return $"strend({field.Id},'{value?.ToString().Replace("\'", "\'\'")}')";
            case FilterParameter.IsNull:
                return $"{field.Id} == null";
            case FilterParameter.IsNotNull:
                return $"{field.Id} != null";
        }

        return "";
    }

    private static string GenerateDateTimeParamFilter(EntityField field, bool writeQ, string formFieldId,
        object value, FilterParameter filterParameter)
    {
        return filterParameter switch
        {
            FilterParameter.IsEqualTo or FilterParameter.Contains => GenerateDateTimeParamEqualToFilter(field, writeQ, formFieldId, value),
            FilterParameter.IsNotEqualTo or FilterParameter.DoesNotContain => $"!({GenerateDateTimeParamEqualToFilter(field, writeQ, formFieldId, value)})",
            FilterParameter.StartsWith => $"({field.Id}>={(writeQ ? $"q[{formFieldId}]" : $"DateTimeOf('{value}')")})",
            FilterParameter.EndsWith => $"({field.Id}<AddDays({(writeQ ? $"q[{formFieldId}]" : $"DateTimeOf('{value}')")},1))",
            FilterParameter.IsNull => $"{field.Id} == null",
            FilterParameter.IsNotNull => $"{field.Id} != null",
            _ => "",
        };
    }

    private static string GenerateDateTimeParamEqualToFilter(EntityField field, bool writeQ, string formFieldId,
        object value)
    {
        return $"({field.Id}>={(writeQ ? $"q[{formFieldId}]" : $"DateTimeOf('{value}')")})" +
               $" and ({field.Id}<AddDays({(writeQ ? $"q[{formFieldId}]" : $"DateTimeOf('{value}')")},1))";
    }

    private static string GenerateLongParamFilter(EntityField field, bool writeQ, string formFieldId, object value,
        FilterParameter filterParameter)
    {
        return filterParameter == FilterParameter.IsEqualTo || filterParameter == FilterParameter.Contains
            ? !value.ToString().Contains(",")
                ? $"ISNULL({field.Id},0)==({(writeQ ? $"q[{formFieldId}]" : value)})"
                : $"{field.Id} In({(writeQ ? $"q[{formFieldId}]" : value)})"
            : GenerateDoubleParamFilter(field, writeQ, formFieldId, value, filterParameter);
    }

    private static string GenerateDoubleParamFilter(EntityField field, bool writeQ, string formFieldId, object value,
        FilterParameter filterParameter)
    {
        switch (filterParameter)
        {
            case FilterParameter.IsEqualTo:
            case FilterParameter.Contains:
                return $"ISNULL({field.Id},0)=={(writeQ ? $"q[{formFieldId}]" : value)}";
            case FilterParameter.IsNotEqualTo:
            case FilterParameter.DoesNotContain:
                return $"ISNULL({field.Id},0)!={(writeQ ? $"q[{formFieldId}]" : value)}";
            case FilterParameter.StartsWith:
                if (writeQ)
                    return $"{field.Id}>=q[{formFieldId}]";
                return $"{field.Id}>='{value?.ToString().Replace("\'", "\'\'")}'";
            case FilterParameter.EndsWith:
                if (writeQ)
                    return $"{field.Id}<=q[{formFieldId}]";
                return $"{field.Id}<='{value?.ToString().Replace("\'", "\'\'")}'";
            case FilterParameter.IsNull:
                return $"{field.Id} == null";
            case FilterParameter.IsNotNull:
                return $"{field.Id} != null";
        }

        return "";
    }
}
