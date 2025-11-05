using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Data.Base;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;

namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    public Dictionary<string, ColumnDefinition> fields = [];
    public List<FormulaDefinition> formulaInfos;
    public bool IfNotSelected => fields.Count == 0 && (GroupBys == null || GroupBys.Count == 0) && (formulaInfos == null || formulaInfos.Count == 0);

    public QueryUtility SelectField(string fieldId, string overFieldName = null)
    {
        if (Entity == null) return this;
        var fieldItems = fieldId.Split(' ');
        if (fieldItems.Length > 0)
            fieldId = fieldItems[0];
        if (fieldItems.Length > 1 && string.IsNullOrEmpty(overFieldName))
            overFieldName = fieldItems[1];
        var field = Entity.GetField(fieldId);
        if (field != null)
            SelectField(field, !string.IsNullOrEmpty(overFieldName) ? overFieldName : fieldId);
        else
        {
            var associationIds = fieldId.Split('.');
            if (associationIds.Length <= 1)
            {
                if (fieldId == "*")
                    SelectFieldsOfEntity(overFieldName);
                else
                    Logger.LogTrace($"Select unknown field {fieldId} in query {Name}");
                return this;
            }

            var queryUtility = this;
            var subFieldIds = "";
            for (var ia = 0; ia < associationIds.Length; ia++)
            {
                var subFieldId = associationIds[ia];
                if (subFieldId == "*")
                {
                    queryUtility?.SelectFieldsOfEntity(subFieldIds);
                    field = null;
                    break;
                }

                subFieldIds += subFieldId + ".";
                field = queryUtility?.Entity?.GetField(subFieldId);
                if (ia == associationIds.Length - 1) break;
                if (field?.AssociationEntity?.Entity() == null) break;
                queryUtility = queryUtility.LeftOuterJoin(field)?.Query;
            }

            if (field != null && queryUtility != null)
                queryUtility.SelectField(field, !string.IsNullOrEmpty(overFieldName) ? overFieldName : fieldId);
            else
                Logger.LogTrace($"Select unknown field {fieldId} in query {queryUtility?.Name ?? Name}");
        }

        return this;
    }

    public QueryUtility SelectFields(params string[] fieldList)
    {
        foreach (var item in fieldList)
            SelectField(item);
        return this;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public QueryUtility SelectFormulaField(string fieldId, ExpressionNode formula)
    {
        addFormula(new FormulaDefinition
        {
            asFieldName = "" + fieldId,
            formula = formula
        });
        return this;
    }

    public QueryUtility SelectFormulaField(string fieldId, string formula)
    {
        SelectFormulaField(fieldId, Parser.Parse(formula));
        return this;
    }

    public QueryUtility SelectFieldsOfEntity(string overPrefix = null)
    {
        if (Entity == null) return this;
        foreach (var field in Entity.entityFields.Values)
        {
            if (field.NotMap || field.AuditField) continue;
            var overFieldName = string.IsNullOrEmpty(overPrefix) ? field.Id : overPrefix + field.Id;
            SelectField(field, overFieldName);
        }

        return this;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public QueryUtility AddPkFields()
    {
        foreach (var field in Entity.KeyFields ?? [])
            SelectField(field);
        return this;
    }

    public QueryUtility AddBasicFields(string culture)
    {
        if (Entity?.DisplayStrings != null)
        {
            foreach (var item in Entity.DisplayStrings)
            {
                if (!item.CheckCulture(culture))
                    continue;
                if (!string.IsNullOrEmpty(item.OtherFieldThatIgnoreMe))
                    SelectField(item.OtherFieldThatIgnoreMe);
                SelectField(item.FieldId);
            }
        }

        return this;
    }

    public string AddBasicFieldsFormula(string culture)
    {
        if (Entity == null)
            return null;
        var basicFields = new List<string>();
        if (Entity.DisplayStrings != null)
        {
            foreach (var item in Entity.DisplayStrings.Where(item => item.CheckCulture(culture)))
            {
                if (!item.CheckCulture(culture) && basicFields.Count > 1)
                    continue;
                AddBasicFieldFormulaItem(item, basicFields);
            }
            if (basicFields.Count == 0 && Entity.DisplayStrings.Count != 0)
                AddBasicFieldFormulaItem(Entity.DisplayStrings.FirstOrDefault(), basicFields);
        }

        if (basicFields.Count == 0)
            basicFields.Add(Entity.entityFields.Values.FirstOrDefault(f => f.CSharpType == typeof(string))?.Id ??
                                 Entity.KeyFields.FirstOrDefault()?.Id ?? "");

        var formula = basicFields.Count > 1
            ? $"Concat({string.Join(",' ',", basicFields)})"
            : basicFields[0];
        SelectFormulaField("_BS", formula);
        return formula;
    }

    private static void AddBasicFieldFormulaItem(BasicField item, List<string> basicFields)
    {
        var f = item.FieldId;
        if (!item.IgnoreIfNull)
            f = $"ISNULL({f},'')";
        if (!string.IsNullOrEmpty(item.OtherFieldThatIgnoreMe))
            f = $"ISNULL({item.OtherFieldThatIgnoreMe},{f})";
        basicFields.Add(f);
    }

    private void addFormula(FormulaDefinition formula)
    {
        formulaInfos ??= [];
        formulaInfos.Add(formula);
    }

    public void SelectField(EntityField field, string overFieldName = null)
    {
        if (field.Formula?.FormulaBody != null)
            SelectFormulaField(string.IsNullOrEmpty(overFieldName) ? field.Id : overFieldName,
                field.Formula?.FormulaBody.clone());
        else if (field.AssociationEntity?.Maps != null)
        {
            foreach (var map in field.AssociationEntity.Maps)
            {
                if (!string.IsNullOrEmpty(overFieldName))
                {
                    var overFieldIds = overFieldName.Split('.');
                    overFieldName = overFieldIds.Length > 1
                        ? string.Join(".", overFieldIds.Take(overFieldIds.Length - 1)) + "." + map.SourceField
                        : map.SourceField;
                }

                SelectField(map.SourceField, overFieldName);
            }
        }
        else if (field.AssociationEntity == null)
            SelectFieldId(field, field.Id, overFieldName);
    }

    private void SelectFieldId(EntityField field, string fieldId, string overFieldName = null)
    {
        fields ??= [];
        if (field.IsForParent && field.CheckFlag(EntityFieldFlags.IncludeInPKV) && 
            field.ReferenceFields?.FirstOrDefault(r => r.Relationship is ParentEntity) != null)
            return;
        if (!fields.ContainsKey(fieldId))
            fields.Add(fieldId, new ColumnDefinition(field)
            {
                fieldName = fieldId,
                overFieldName = overFieldName
            });
    }

    private void SelectFieldsOfEntityIfNotSelected()
    {
        if (IfNotSelected)
            SelectFieldsOfEntity();
    }

    private void SelectFieldsOfEntityIfNotSelected<T>() where T : new()
    {
        if (IfNotSelected)
            SelectFieldsOfEntity<T>();
    }

    private void SelectFieldsOfEntity<T>() where T : new()
    {
        var type = typeof(T);
        if (Entity != null)
        {
            foreach (var item in Entity.entityFields.Values)
            {
                if (item.NotMap || item.AuditField) continue;
                try
                {
                    if (type.GetField(item.Id) == null && type.GetProperty(item.Id) == null) continue;
                }
                catch 
                { 
                    continue; 
                }
                SelectField(item);
            }
        }
    }
}
