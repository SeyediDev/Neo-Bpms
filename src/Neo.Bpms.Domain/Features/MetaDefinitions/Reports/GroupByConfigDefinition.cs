using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Reports;

public abstract class GroupByConfigDefinition : ReportConfigDefinition
{
    protected virtual string HavingCondition { get; }
    protected override ReportViewType ViewType => ReportViewType.GroupByList;
    public override bool DefineAll(Report r)
    {
        base.DefineAll(r);
        DefineGroupBy();
        return true;
    }
    protected override void DefineExtra()
    {
        base.DefineExtra();
        if (!string.IsNullOrEmpty(HavingCondition))
        {
            reportConfig.HavingCondition = HavingCondition;
        }
    }
    protected abstract void DefineGroupBy();
    /// <summary>
    /// Add group by column
    /// </summary>
    /// <param name="field">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void GroupBy(string field, string alias = null, bool addAsDisplayColumn = true)
    {
        string[] fieldIds = field.Split('.');
        if (fieldIds.Length > 2)
        {
            return;//todo more than one dot is not supported in this method
        }

        if (fieldIds.Length == 1)
        {
            AddField(ConfiguredReport.eFieldSelectionType.asGroupBy, field, alias);
        }
        else
        {
            EntityField associationField = report?.entity.GetField(fieldIds[0]);

            if (associationField == null)
            {
                return;
            }

            EntityField f = associationField.AssociationEntity.GetField(fieldIds[1]);
            if (f == null)
            {
                return;
            }

            AddIncludedField(ConfiguredReport.eFieldSelectionType.asGroupBy, f.Id, associationField.AssociationEntity.Id
                , associationField.Id, alias);
        }
        if (addAsDisplayColumn)
        {
            DisplayColumn(field, alias);
        }
    }

    protected void GroupBys(params string[] fields)
    {
        foreach (string field in fields)
        {
            AddField(ConfiguredReport.eFieldSelectionType.asGroupBy, field, null);
        }
    }
    /// <summary>
    /// Add group by formula column
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void GroupByFormula(string formula, string alias = null, bool addAsDisplayColumn = true)
    {
        AddFormulaField(ConfiguredReport.eFieldSelectionType.asGroupBy, null, formula, alias);
        if (addAsDisplayColumn)
        {
            AddFormulaField(ConfiguredReport.eFieldSelectionType.asColumn, selectedField?.fieldId, formula, alias);
        }
    }
    /// <summary>
    /// Add Selected Field
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="aggregationType"></param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Aggregation(string fieldId, AggregationType aggregationType, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AddField((ConfiguredReport.eFieldSelectionType)aggregationType, fieldId, alias, properties);
    }

    /// <summary>
    /// Add Selected Field
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="aggregationType"></param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void AggregationFormula(string formula, AggregationType aggregationType, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AddFormulaField((ConfiguredReport.eFieldSelectionType)aggregationType, null, formula, alias, properties);
    }

    /// <summary>
    /// Add Selected Field
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Sum(string fieldId, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        Aggregation(fieldId, AggregationType.Sum, alias, properties);
    }

    /// <summary>
    /// Add Selected Field
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Average(string fieldId, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        Aggregation(fieldId, AggregationType.Average, alias, properties);
    }

    /// <summary>
    /// Add sum formula column
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void SumFormula(string formula, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AggregationFormula(formula, AggregationType.Sum, alias, properties);
    }

    /// <summary>
    /// Add count column
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Count(string fieldId = null, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        if (string.IsNullOrEmpty(fieldId))
        {
            fieldId = "*";
        }

        Aggregation(fieldId, AggregationType.Count, alias, properties);
    }

    /// <summary>
    /// Add sum formula column
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void CountFormula(string formula, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AggregationFormula(formula, AggregationType.Count, alias, properties);
    }

    /// <summary>
    /// Add max column
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Max(string fieldId, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        Aggregation(fieldId, AggregationType.Max, alias, properties);
    }

    /// <summary>
    /// Add max formula column
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void MaxFormula(string formula, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AggregationFormula(formula, AggregationType.Max, alias, properties);
    }

    /// <summary>
    /// Add min column
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void Min(string fieldId, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        Aggregation(fieldId, AggregationType.Min, alias, properties);
    }

    /// <summary>
    /// Add min formula column
    /// </summary>
    /// <param name="formula">formula</param>
    /// <param name="alias">Alias</param>
    /// <returns></returns>
    protected void MinFormula(string formula, string alias = null,
        params (eControlPropertyId propertyId, object value)[] properties)
    {
        AggregationFormula(formula, AggregationType.Min, alias, properties);
    }

    /// <summary>
    /// Add group by column with category for range-based categorization
    /// </summary>
    /// <param name="field">Field name</param>
    /// <param name="alias">Display alias</param>
    /// <param name="category">Category identifier for this field</param>
    /// <param name="addAsDisplayColumn">Whether to add as display column</param>
    protected void GroupByWithCategory(string field, string alias, string category, bool addAsDisplayColumn = true)
    {
        string[] fieldIds = field.Split('.');
        if (fieldIds.Length > 2)
        {
            return; // More than one dot is not supported
        }

        if (fieldIds.Length == 1)
        {
            AddField(ConfiguredReport.eFieldSelectionType.asGroupBy, field, alias,
                [(eControlPropertyId.Category, category)]);
        }
        else
        {
            EntityField associationField = report?.entity.GetField(fieldIds[0]);
            if (associationField == null)
            {
                return;
            }

            EntityField f = associationField.AssociationEntity.GetField(fieldIds[1]);
            if (f == null)
            {
                return;
            }

            AddIncludedField(ConfiguredReport.eFieldSelectionType.asGroupBy, f.Id, associationField.AssociationEntity.Id,
                associationField.Id, alias,
                [(eControlPropertyId.Category, category)]);
        }

        if (addAsDisplayColumn)
        {
            DisplayColumn(field, alias, [(eControlPropertyId.Category, category)]);
        }
    }

    /// <summary>
    /// Add category ranges for a specific category
    /// </summary>
    /// <param name="category">Category identifier</param>
    /// <param name="ranges">List of ranges to add</param>
    protected void AddCategoryRanges(string category, params CategoryRange[] ranges)
    {
        foreach (var range in ranges)
        {
            reportConfig.AddProperty(ReportConfigProperty.CategoryRange, range.ToCategoryRangeString(category));
        }
    }
}
