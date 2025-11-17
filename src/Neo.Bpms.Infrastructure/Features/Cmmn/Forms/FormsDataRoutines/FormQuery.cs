using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormsDataRoutines;

public class FormQuery
{
    public FormQuery(Form form, CommonFormStructure structure, CancellationToken cancellationToken)
    {
        this.form = form;
        this.structure = structure;
        CancellationToken = cancellationToken;
    }

    public Form form { get; set; }
    public CommonFormStructure structure { get; set; }
    public CancellationToken CancellationToken { get; set; }

    public QueryUtility q { get; set; }
    private Entity entity => form?.Entity;

    public QueryUtility EstablishQuery()
    {
        q = new QueryUtility(entity, "FormQuery.1") { CancellationToken = CancellationToken };
        return q;
    }

    public void SetQueryByForm(string ids,
        LocalParameters lp, Dictionary<string, FormField> referFormFields, JoinQueriesData joinQueries)
    {
        SelectFields(
            form.formFields.Where(f =>
                f.FieldOrControlType == FormField.Type.Field && f.Field != null &&
                !f.CheckProperty(eControlPropertyId.DontLoadData)), referFormFields, joinQueries);
        FormDataFilter.AddFormFilter(q, form, GenerateFilterValuesRecord(lp), true, out _);
        q.AddPkFields();
        if (!string.IsNullOrEmpty(ids))
        {
            q.AddFilter(FormDataRoutines.GetPKFilter(ids, entity));
        }
        foreach (TableDefinition table in structure.Tables)
            if (!string.IsNullOrEmpty(table.TableDef.AssociationId))
                q.SelectField(table.TableDef.AssociationId);
    }

    public Dictionary<string, FormField> EstablishEntityQueryForIndex(ElasticObject filterValues, string sortFields,
        IEnumerable<string> filterList, bool justForCount, JoinQueriesData joinQueries,
        bool dontForceActiveStatesFilter, bool checkfilterValues)
    {
        foreach (string filter in filterList ?? [])
            q.Where(filter);
        if (!(form.DontForceActiveStatesFilter || dontForceActiveStatesFilter))
        {
            q.ActiveStates();
        }
        if (ReflectionTools.IsInBaseInterface<ISoftDelete>(form.Entity.EntityType))
        {
            q.Where($"({nameof(ISoftDelete.IsDeleted)} == false) Or ({nameof(ISoftDelete.IsDeleted)} == null)");
        }
        if (!justForCount)
            AddOrderBy(sortFields);
        Dictionary<string, FormField> referFormFields = [];
        if (checkfilterValues)
        {
            FormDataFilter.AddFilters(q, form, filterValues, out bool distinct);
            if (justForCount && !distinct)
                return referFormFields;
        }
        SelectFields(
            form.formFields.Where(f =>
                (f.FieldOrControlType == FormField.Type.ColumnField ||
                 f.FieldOrControlType == FormField.Type.SubTable) && f.Field != null &&
                !f.CheckProperty(eControlPropertyId.DontLoadData)), referFormFields, joinQueries);
        q.AddPkFields();
        return referFormFields;
    }
    public Dictionary<string, FormField> EstablishEntityQuery(ElasticObject filterValues,
        IEnumerable<string> filterList, bool justForCount, JoinQueriesData joinQueries,
        bool dontForceActiveStatesFilter = false)
    {
        foreach (string filter in filterList ?? [])
            q.Where(filter);
        if (!(form.DontForceActiveStatesFilter || dontForceActiveStatesFilter))
        {
            q.ActiveStates();
        }
        if (ReflectionTools.IsInBaseInterface<ISoftDelete>(form.Entity.EntityType))
        {
            q.Where($"({nameof(ISoftDelete.IsDeleted)} == false) Or ({nameof(ISoftDelete.IsDeleted)} == null)");
        }

        Dictionary<string, FormField> referFormFields = [];
        FormDataFilter.AddFilters(q, form, filterValues, out bool distinct);
        if (justForCount && !distinct)
            return referFormFields;
        SelectFields(
            form.formFields.Where(f =>
                (f.FieldOrControlType == FormField.Type.Field ||
                 f.FieldOrControlType == FormField.Type.SubTable) && f.Field != null &&
                !f.CheckProperty(eControlPropertyId.DontLoadData)), referFormFields, joinQueries);
        q.AddPkFields();
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
                q.OrderBy(sf[0],
                    sf.Length > 1 && sf[1].ToUpper() == "DESC"
                        ? SortType.Descending
                        : SortType.Ascending, sf.Length > 2 && ConvUtill.ToBoolean(sf[2]));
            }
        }

        if (AddFormOrderBy())
            any = true;
        if (any) return;
        foreach (ColumnFieldDefinition item in structure.ColumnInfos)
        {
            EntityField field = entity.GetField(item.ColumnName);
            if (field != null && field.AssociationEntity == null)
                q.OrderBy(item.ColumnName);
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
                q.SelectField(field);
            else
            {
                if (referFormFields.ContainsKey(formField.Id))
                    continue;
                referFormFields.Add(formField.Id, formField);
                if (field.Id != formField.Id)
                {
                    string[] fieldIds = formField.Id.Split('.');
                    for (int i = 1; i < fieldIds.Length; i++)
                        field = field?.AssociationEntity?.Entity()?.GetField(fieldIds[i]);
                }

                if (field?.AssociationEntity?.Entity() == null)
                {
                    q.SelectField(formField.Id);
                    continue;
                }

                string displayFields = formField.Property(eControlPropertyId.DisplayFields);
                if (string.IsNullOrEmpty(displayFields))
                    displayFields = field?.Property(EntityFieldPropertyId.DisplayFields);
                joinQueries.TryAdd(field.AssociationEntity, displayFields?.Split(",").ToList());
                q.SelectField(formField.Id);
                if (field.AssociationEntity.Entity()?.Id == "FileInfo")
                    q.SelectField(formField.Id + ".ContentType");
            }
        }
    }

    private bool AddFormOrderBy()
    {
        bool any = false;
        foreach (FormOrderBy orderBy in form.OrderBys ?? Enumerable.Empty<FormOrderBy>())
        {
            q.OrderBy(orderBy.FieldId, orderBy.SortType, orderBy.ById);
            any = true;
        }

        if (any)
        {
            EntityField keyField = q.Entity.KeyFields?.FirstOrDefault();
            if (keyField != null)
            {
                OrderByDefinition keyOrder = q.OrderBys?.FirstOrDefault(o => o.fieldName == keyField.Id);
                if (keyOrder == null)
                    q.OrderBy(keyField.Id);
            }
        }

        return any;
    }

    private ElasticObject GenerateFilterValuesRecord(LocalParameters lps)
    {
        ElasticObject filterValues = new();
        if (lps != null)
            foreach (KeyValuePair<string, object> lp in lps)
                filterValues.SetField(lp.Key, lp.Value);
        return filterValues;
    }
}
