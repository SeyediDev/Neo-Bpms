using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class ReportFilterName(ReportConfigManager reportConfigManager)
{
    public async Task<string> GetConfigFilterNames(string namespaceId,
        string entityId, string reportId, string configId,
        ElasticObject filterValuesArgument, string parentReportIds,
        IDictionary<string, ComboData> combosData, ElasticObject parentFilters,
        List<InputFieldDefinition> fields, IdentityUser user, string culture)
    {
        GetReportConfigResult getReportConfigResult = new(configId);
        await reportConfigManager.GetReportConfig(namespaceId, entityId, reportId,
            getReportConfigResult, user);
        return GetFilterNames(namespaceId, entityId, filterValuesArgument,
            parentReportIds, combosData, parentFilters,
            getReportConfigResult.Config?.Parent?.ParentConfiguredReport, fields,
            getReportConfigResult.Config?.Parent, culture);
    }

    /// <summary>
    /// get Filter Names for report Headers in print preview.
    /// </summary>
    /// <param name="filterValuesArgument"></param>
    /// <param name="parentReportIds"></param>
    /// <param name="parentFilters"></param>
    /// <param name="parentConfig"></param>
    /// <param name="fields"></param>
    /// <param name="subReport"></param>
    /// <param name="combosData"></param>
    /// <param name="namespaceId"></param>
    /// <param name="entityId"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public string GetFilterNames(string namespaceId, string entityId,
        ElasticObject filterValuesArgument, string parentReportIds,
        IDictionary<string, ComboData> combosData, ElasticObject parentFilters,
        ConfiguredReport parentConfig, List<InputFieldDefinition> fields,
        ConfiguredReport.ConfiguredSubReport subReport, string culture)
    {
        string name = "";
        ElasticObject filters = filterValuesArgument;
        Entity entity = ProjectDefinition.Project.GetEntity(namespaceId, entityId);
        if (entity == null) return name;
        if (!string.IsNullOrEmpty(parentReportIds) && parentConfig != null && subReport != null)
        {
            List<object> ids = [.. parentReportIds.Split(',')];
            int j = 0;
            //check if parent entity is not the same as entity, in this case,  1. the association relation is meaningful 2. the parent config view type is report list
            if (!Equals(parentConfig.Report.entity, entity))
            {
                EntityField relField = entity.GetField(subReport.AssociationName);
                if (relField?.AssociationEntity != null)
                    name += GetFilterNames(combosData, entity, ids, ref j, relField.Id, culture);
            }
            else
                if (parentConfig.ViewType != ReportViewType.List)
            {
                //parent and sub reports are both from the same entity and the group by fields can be used as the filter.
                if (parentConfig.Fields != null)
                {
                    foreach (ConfiguredReport.SelectedField field in parentConfig.Fields.Values.OrderBy(f => f.Order))
                    {
                        if (field.type != ConfiguredReport.eFieldSelectionType.asGroupBy) continue;
                        Entity entity1 = parentConfig.Report.entity;
                        Entity colEntity = entity1;
                        if (entity1.Id != field.entityId)
                        {
                            colEntity = ProjectDefinition.Project.GetEntityByEntityId(field.entityId);
                            if (colEntity == null) continue;
                        }
                        EntityField colField = colEntity?.GetField(field.fieldId);
                        name += colField?.AssociationEntity != null
                            ? GetFilterNames(combosData, colEntity, ids, ref j, colField.Id, culture)
                            : GetFilterNames(combosData, colEntity, ids, ref j, field.fieldId, culture);
                        if (j > ids.Count) break;
                    }
                }
            }
            else
            {
                //parent and sub reports are both from the same entity and the primary key can be used as the filter.
                foreach (KeyValuePair<string, EntityField> parentField in parentConfig.Report.entity.entityFields)
                {
                    if (!parentField.Value.IncludeInPkv) continue;
                    name += GetFilterNames(combosData, parentConfig.Report.entity, ids, ref j, parentField.Key, culture);
                    if (j > ids.Count) break;
                }
            }
        }
        if (filters == null && parentFilters == null) return name;
        bool firstIteration = true;
        foreach (InputFieldDefinition f in fields)
        {
            if (string.IsNullOrEmpty(f.FieldName)) continue;
            if (filters?[f.FieldName] != null && !string.IsNullOrEmpty(filters[f.FieldName].ToString().Trim()))
            {
                string fv = filters[f.FieldName].ToString();
                string itemName = GetFilterName(combosData, entity, f.FieldName, fv, culture);
                if (string.IsNullOrEmpty(itemName)) continue;
                if (firstIteration)
                    firstIteration = false;
                else
                    name += "،";
                name += itemName;
            }
            else if (parentFilters?[f.FieldName] != null && !string.IsNullOrEmpty(parentFilters[f.FieldName].ToString().Trim()))
            {
                string fv = parentFilters[f.FieldName].ToString();
                if (firstIteration)
                    firstIteration = false;
                else
                    name += "،";
                name += GetFilterName(combosData, entity, f.FieldName, fv, culture);
            }
        }
        return name;
    }
    private string GetFilterName(IDictionary<string, ComboData> combosData,
        Entity entity, string fieldId, string fv, string culture)
    {
        string name = "";
        string[] associationItems = fieldId.Split('.');
        int ia = 0;
        EntityField fld = null;
        string fieldName = "";
        foreach (string associationItem in associationItems)
        {
            fld = entity.GetField(associationItem);
            fieldName += (!string.IsNullOrEmpty(fieldName) ? fieldName + "-" : "") +
                         (culture == "en" ? fld?.EnName : fld?.Name) + " ";
            if (fld?.AssociationEntity != null)
            {
                if (ia != associationItems.Length - 1)
                    entity = fld.AssociationEntity.Entity();
            }
            ia++;
        }
        if (fld == null) return name;
        if (fld.AssociationEntity != null)
        {
            if (!combosData.TryGetValue(fieldId, out ComboData comboData))
            {
                comboData = ComboDataRoutines.GetRecords(fld.Entity, fld.AssociationEntity.Entity(),
                    null /*todo*/, /*culture*/$"{fld.AssociationEntity.Entity().KeyFields?.FirstOrDefault()?.Id}=='{fv}'",
                    1, //todo
                    fld.AssociationEntity.Constraint, null, null, null,
                    1, false, null);
                combosData.Add(fieldId, comboData);
            }

            if (fld.AssociationEntity.CheckFlag(EntityFieldFlags.IsBitMask))
                fv = GetBitmaskValue(comboData, Convert.ToInt32(fv));
            else
            {
                string fvDisplayValue = "";
                foreach (string fvItem in fv.Split(','))
                {
                    FormDataRow row = comboData.GetRow(fvItem);
                    if (row == null && fvItem != "0")
                        row = GetNewValueRecord(entity, fld, fvItem);
                    if (row != null)
                        fvDisplayValue += (!string.IsNullOrEmpty(fvDisplayValue) ? "," : "") + row.DisplayValue;
                    else if (fvItem == "0") return name;
                }

                fv = fvDisplayValue;
            }
        }
        else
        {
            switch (fld.FieldType)
            {
                case TVariableTypes.DateTime:
                case TVariableTypes.Date:
                    {
                        if (!NormalizeDateValue(ref fv, fld))
                            return name;
                        break;
                    }
                case TVariableTypes.DateStr:
                    break;
                case TVariableTypes.HourMinute:
                    break;
                case TVariableTypes.DayHourMinute:
                    break;
                case TVariableTypes.DoubleMinuteSecond:
                    break;
                case TVariableTypes.DurHourMinute:
                    break;
                case TVariableTypes.BOOL:
                    {
                        BooleanEntityField booleanField = fld.Boolean;
                        BooleanItem valueItem = BooleanEntityField.GetValueItem(false, fv);
                        switch (valueItem)
                        {
                            case BooleanItem.True:
                                fv = booleanField?.TrueTitle ?? "بلی";
                                break;
                            case BooleanItem.False:
                                fv = booleanField?.FalseTitle ?? "خیر";
                                break;
                            case BooleanItem.Null:
                                fv = booleanField?.NullTitle ?? "تهی";
                                break;
                            default:
                                return name;
                        }
                        break;
                    }
            }
        }
        name += " " + fieldName + ": " + (fld.Id == "Iso2" ? "^" : "") + fv + " " + (fld.Id == "Iso2" ? "~" : "");//todo very bad practice
        return name;
    }

    private bool NormalizeDateValue(ref string fv, EntityField fld)
    {
        string[] sDates0 = fv.Split(' ');
        string[] sDates = sDates0[0].Split('/');
        if (sDates.Length >= 3)
        {
            short year = Convert.ToInt16(sDates[0]);
            short month = Convert.ToInt16(sDates[1]);
            short day = Convert.ToInt16(sDates[2]);
            if (day > 31)
            {
                year = day;
                day = month;
                month = Convert.ToInt16(sDates[0]);
            }
            DateTime dt = new(year, month, day);
            if (dt > DateTime.MinValue)
            {
                if (ProjectDefinition.Project.DefaultCalendar == "shamsi")
                {
                    PersianCalendar pc = new();
                    fv = pc.GetYear(dt).ToString() + '/' + Get2Digit(pc.GetMonth(dt)) + '/'
                          + Get2Digit(pc.GetDayOfMonth(dt));

                }
                else
                {
                    fv = dt.Year.ToString() + '/' + dt.Month + '/' + dt.Day;
                    if (fld.FieldType == TVariableTypes.DateTime && sDates0.Length > 1)
                        fv += " " + sDates0[1];
                }
            }
            else
                return false;
        }
        else
            return false;
        return true;
    }

    private string Get2Digit(int d)
    {
        return d < 10 ? "0" + d : d.ToString();
    }

    private FormDataRow GetNewValueRecord(Entity entity, EntityField fld, string fvItem)
    {
        string pkFilter = "";
        List<EntityField> keys = [.. fld.AssociationEntity.Entity().KeyFields];
        if (keys.Count == 1)
            pkFilter += "(" + fld.AssociationEntity.Entity().KeyFields.FirstOrDefault()?.Id + "='" +
                        fvItem + "'" + ")";
        else
        {
            string[] ids = fvItem.Split('#');
            int i = 0;
            foreach (EntityField pk in keys)
            {
                if (!string.IsNullOrEmpty(pkFilter)) pkFilter += "And";
                pkFilter += "(" + pk.Id + "='" + ids[i++] + "'" + ")";
                if (i >= ids.Length) break;
            }
        }
        ComboData cd0 = ComboDataRoutines.GetRecords(entity, fld.AssociationEntity.Entity(), "fa",
            pkFilter, 1, fld.AssociationEntity.Constraint, null, /*displayFields:*/ null, "", 1, false);
        return cd0.Rows.Count > 0 ? cd0.Rows[0] : null;
    }

    private string GetBitmaskValue(ComboData comboData, int v)
    {
        string newValue = "";
        for (int i = 0; i < 64; i++)
        {
            int id = 1 << i;
            if ((id & v) == 0) continue;
            string bId = i.ToString();
            FormDataRow row = comboData.GetRow(bId);
            if (!string.IsNullOrEmpty(newValue)) newValue += " - ";
            newValue += row?.DisplayValue ?? bId;
        }
        return newValue;
    }

    private string GetFilterNames(IDictionary<string, ComboData> combosData, Entity entity,
        IList<object> ids, ref int j, string fName, string culture)
    {
        string name = "";
        if (j >= ids.Count) return name;
        object obj = ids[j];
        name += GetFilterName(combosData, entity, fName, "" + obj, culture);
        j++;
        return name;
    }
}
