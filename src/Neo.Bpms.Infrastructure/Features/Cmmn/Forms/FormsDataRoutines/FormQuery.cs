using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormsDataRoutines;

public class FormQuery(Form form, CommonFormStructure structure, CancellationToken cancellationToken)
{
    public Form Form { get; set; } = form;
    public CommonFormStructure Structure { get; set; } = structure;
    public CancellationToken CancellationToken { get; set; } = cancellationToken;

    public QueryUtility Q { get; set; }
    private Entity Entity => Form?.Entity;

    public QueryUtility EstablishQuery()
    {
        Q = new QueryUtility(Entity, "FormQuery.1") { CancellationToken = CancellationToken };
        return Q;
    }

    public void SetQueryByForm(string ids,
        LocalParameters lp, Dictionary<string, FormField> referFormFields, JoinQueriesData joinQueries, string culture)
    {
        SelectFields(
            Form.formFields.Where(f =>
                f.FieldOrControlType == FormField.Type.Field && f.Field != null &&
                !f.CheckProperty(eControlPropertyId.DontLoadData)), referFormFields, joinQueries);
        FormDataFilter.AddFormFilter(Q, Form, GenerateFilterValuesRecord(lp), true, out _);
        Q.AddPkFields();
        if (!string.IsNullOrEmpty(ids))
        {
            Q.AddFilter(FormDataRoutines.GetPKFilter(ids, Entity));
        }
        foreach (TableDefinition table in Structure.Tables)
        {
            if (!string.IsNullOrEmpty(table.TableDef.AssociationId))
            {
                Q.SelectField(table.TableDef.AssociationId);
            }
        }
        if (!string.IsNullOrEmpty(Form.FormSubjectId))
        {
            Q.AddBasicFields(culture);
        }
    }

    public Dictionary<string, FormField> EstablishEntityQueryForIndex(ElasticObject filterValues, string sortFields,
        IEnumerable<string> filterList, bool justForCount, JoinQueriesData joinQueries,
        bool dontForceActiveStatesFilter, bool checkfilterValues)
    {
        foreach (string filter in filterList ?? [])
        {
            Q.Where(filter);
        }
        if (!(Form.DontForceActiveStatesFilter || dontForceActiveStatesFilter))
        {
            Q.ActiveStates();
        }
        if (ReflectionTools.IsInBaseInterface<ISoftDelete>(Form.Entity.EntityType))
        {
            Q.Where($"({nameof(ISoftDelete.IsDeleted)} == false) Or ({nameof(ISoftDelete.IsDeleted)} == null)");
        }
        if (!justForCount)
        {
            AddOrderBy(sortFields);
        }
        Dictionary<string, FormField> referFormFields = [];
        if (checkfilterValues)
        {
            FormDataFilter.AddFilters(Q, Form, filterValues, out bool distinct);
            if (justForCount && !distinct)
            {
                return referFormFields;
            }
        }
        SelectFields(
            Form.formFields.Where(f =>
                (f.FieldOrControlType == FormField.Type.ColumnField ||
                 f.FieldOrControlType == FormField.Type.SubTable) && f.Field != null &&
                !f.CheckProperty(eControlPropertyId.DontLoadData)), referFormFields, joinQueries);
        Q.AddPkFields();
        return referFormFields;
    }

    public Dictionary<string, FormField> EstablishEntityQuery(ElasticObject filterValues,
        IEnumerable<string> filterList, bool justForCount, JoinQueriesData joinQueries,
        bool dontForceActiveStatesFilter = false)
    {
        foreach (string filter in filterList ?? [])
            Q.Where(filter);
        if (!(Form.DontForceActiveStatesFilter || dontForceActiveStatesFilter))
        {
            Q.ActiveStates();
        }
        if (ReflectionTools.IsInBaseInterface<ISoftDelete>(Form.Entity.EntityType))
        {
            Q.Where($"({nameof(ISoftDelete.IsDeleted)} == false) Or ({nameof(ISoftDelete.IsDeleted)} == null)");
        }

        Dictionary<string, FormField> referFormFields = [];
        FormDataFilter.AddFilters(Q, Form, filterValues, out bool distinct);
        if (justForCount && !distinct)
            return referFormFields;
        SelectFields(
            Form.formFields.Where(f =>
                (f.FieldOrControlType == FormField.Type.Field ||
                 f.FieldOrControlType == FormField.Type.SubTable) && f.Field != null &&
                !f.CheckProperty(eControlPropertyId.DontLoadData)), referFormFields, joinQueries);
        Q.AddPkFields();
        return referFormFields;
    }

    private void AddOrderBy(string sortFields)
    {
        bool any = false;
        if (!string.IsNullOrEmpty(sortFields))
        {
            if (sortFields.StartsWith("#"))
                sortFields = sortFields[1..];
            string[] sfs = sortFields.Split('#');
            foreach (string item in sfs)
            {
                if (string.IsNullOrEmpty(item)) continue;
                string[] sf = item.Split(' ');
                any = true;
                Q.OrderBy(sf[0],
                    sf.Length > 1 && sf[1].ToUpper() == "DESC"
                        ? SortType.Descending
                        : SortType.Ascending, sf.Length > 2 && ConvUtill.ToBoolean(sf[2]));
            }
        }

        if (AddFormOrderBy())
        {
            any = true;
        }
        if (any)
        {
            return;
        }

        // Auto order by field with "order" or "ترتیب" in alias/name
        ColumnFieldDefinition orderField = Structure.ColumnInfos.FirstOrDefault(col =>
            (!string.IsNullOrEmpty(col.Alias) &&
             (col.Alias.Contains("ترتیب", StringComparison.OrdinalIgnoreCase) ||
              col.Alias.Contains("order", StringComparison.OrdinalIgnoreCase))) ||
            (!string.IsNullOrEmpty(col.ColumnName) &&
             col.ColumnName.Contains("order", StringComparison.OrdinalIgnoreCase)));

        if (orderField != null)
        {
            EntityField field = Entity.GetField(orderField.ColumnName);
            if (field != null && field.AssociationEntity == null)
            {
                Q.OrderBy(orderField.ColumnName, SortType.Ascending);
                return;
            }
        }

        foreach (ColumnFieldDefinition item in Structure.ColumnInfos)
        {
            EntityField field = Entity.GetField(item.ColumnName);
            if (field != null && field.AssociationEntity == null)
                Q.OrderBy(item.ColumnName);
            break;
        }
    }

    public void SelectField(FormField formField, IDictionary<string, FormField> referFormFields, JoinQueriesData joinQueries)
    {
        SelectFields([formField], referFormFields, joinQueries);
    }

    private void SelectFields(IEnumerable<FormField> formFields, IDictionary<string, FormField> referFormFields, JoinQueriesData joinQueries)
    {
        int fCount = 0;
        const int maxFieldCount = 200;
        foreach (FormField formField in formFields)
        {
            if (++fCount > maxFieldCount) break;
            EntityField field = formField.Field;
            if (field.AssociationEntity == null)
            {
                Q.SelectField(field);
            }
            else
            {
                if (referFormFields.ContainsKey(formField.Id))
                {
                    continue;
                }
                referFormFields.Add(formField.Id, formField);
                if (field.Id != formField.Id)
                {
                    string[] fieldIds = formField.Id.Split('.');
                    for (int i = 1; i < fieldIds.Length; i++)
                        field = field?.AssociationEntity?.Entity()?.GetField(fieldIds[i]);
                }

                if (field?.AssociationEntity?.Entity() == null)
                {
                    Q.SelectField(formField.Id);
                    continue;
                }

                string displayFields = formField.Property(eControlPropertyId.DisplayFields);
                if (string.IsNullOrEmpty(displayFields))
                {
                    displayFields = field?.Property(EntityFieldPropertyId.DisplayFields);
                }

                joinQueries.TryAdd(field.AssociationEntity, displayFields?.Split(",").ToList());
                Q.SelectField(formField.Id);
                if (field.AssociationEntity.Entity()?.Id == "FileInfo")
                {
                    Q.SelectField(formField.Id + ".ContentType");
                }
            }
        }
    }

    private bool AddFormOrderBy()
    {
        bool any = false;
        foreach (FormOrderBy orderBy in Form.OrderBys ?? Enumerable.Empty<FormOrderBy>())
        {
            Q.OrderBy(orderBy.FieldId, orderBy.SortType, orderBy.ById);
            any = true;
        }

        if (any)
        {
            EntityField keyField = Q.Entity.KeyFields?.FirstOrDefault();
            if (keyField != null)
            {
                OrderByDefinition keyOrder = Q.OrderBys?.FirstOrDefault(o => o.fieldName == keyField.Id);
                if (keyOrder == null)
                {
                    Q.OrderBy(keyField.Id);
                }
            }
        }

        return any;
    }

    private static ElasticObject GenerateFilterValuesRecord(LocalParameters lps)
    {
        ElasticObject filterValues = new();
        if (lps != null)
        {
            foreach (KeyValuePair<string, object> lp in lps)
                filterValues.SetField(lp.Key, lp.Value);
        }
        return filterValues;
    }
}
