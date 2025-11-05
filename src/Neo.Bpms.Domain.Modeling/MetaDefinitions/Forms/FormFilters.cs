using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Components;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities;

public abstract partial class FormDefinition
{
    /// <summary>
    /// Define Form Filters
    /// </summary>
    /// <returns></returns>
    public virtual bool DefineFilters()
    {
        return true;
    }

    /// <summary>
    /// Define Form Filters
    /// </summary>
    /// <returns></returns>
    protected virtual void Filters()
    {
    }

    #region filter field

    /// <summary>
    /// Adds the filter field.
    /// </summary>
    /// <param name="fieldId">Name of the field.</param>
    /// <param name="controlProperties">The control properties</param>
    /// <returns></returns>
    public FormField AddFilterField(string fieldId, params eControlPropertyId[] controlProperties)
    {
        return AddFilterField(fieldId, null, controlProperties);
    }

    /// <summary>
    /// Adds the filter field.
    /// </summary>
    /// <param name="fieldId">Name of the field.</param>
    /// <param name="label">The Label</param>
    /// <param name="controlProperties">The control properties</param>
    /// <returns></returns>
    public FormField AddFilterField(string fieldId, string label = null,
        params eControlPropertyId[] controlProperties)
    {
        if (form?.formFields.Any(f =>
            f.Id == fieldId && string.IsNullOrEmpty(f.TableEntityId) &&
            f.FieldOrControlType == FormField.Type.FilterField) ?? false)
            throw new Exception("Duplicate Id: " + fieldId);
        var fieldIds = fieldId.Split('.');
        var fld = form?.entity.GetField(fieldIds[0]);
        if (fld == null)
        {
            return null;
        }

        var notTabular = string.IsNullOrEmpty(_parentControlId);
        if (notTabular)
            CreateDefaultFilterTab(fieldIds, fld);

        var ff = new FormField(form, fld)
        {
            ParentControlId = _parentControlId,
            FieldOrControlType = FormField.Type.FilterField,
            Id = fieldId
        };
        var filterField = AddField(ff);
        if (!string.IsNullOrEmpty(label))
            AddProperty(eControlPropertyId.LabelName, label);
        if (notTabular)
        {
            EndSubControls();
            EndSubControls();
            _currentFormField = filterField;
        }

        foreach (var controlProperty in controlProperties ?? Enumerable.Empty<eControlPropertyId>())
            filterField.AddProperty(new FormProperty(controlProperty, true));
        return filterField;
    }

    public FormField SelectFilterField(string fieldId, params eControlPropertyId[] controlProperties)
    {
        var filterField = form?.formFields.FirstOrDefault(f =>
            f.Id == fieldId && string.IsNullOrEmpty(f.TableEntityId) &&
            f.FieldOrControlType == FormField.Type.FilterField);

        _currentFormField = filterField;
        if (filterField != null)
        {
            foreach (var controlProperty in controlProperties ?? Enumerable.Empty<eControlPropertyId>())
                filterField.AddProperty(new FormProperty(controlProperty, true));
        }

        return filterField;
    }

    private void CreateDefaultFilterTab(string[] fieldIds, EntityField fld)
    {
        GetOrAddFilterMultiTab();
        var tabItemId = fieldIds.Length > 1 ? $"tab-{fld.Id}" : "PublicFilters";
        var tabItem = form?.formFields.FirstOrDefault(
            f => f.ControlId == tabItemId && f.ControlTypeId == eControlTypeId.MultiTabItem);
        if (tabItem == null)
        {
            AddControl(eControlTypeId.MultiTabItem, tabItemId,
                fieldIds.Length > 1 ? fld.Name : "عمومی", fieldIds.Length > 1 ? fld.EnName : "General");
            StartSubControls();
        }
        else
        {
            _currentFormField = tabItem;

            _parentControlId = tabItem.ControlId;
        }
    }

    protected void GetOrAddFilterMultiTab()
    {
        var fmtId = "RFTM";
        var fmt = form?.formFields.FirstOrDefault(
            f => f.ControlTypeId == eControlTypeId.MultiTab);
        if (fmt == null)
        {
            AddControl(eControlTypeId.MultiTab, fmtId);
            StartSubControls();
        }
        else
        {
            _currentFormField = fmt;
            _parentControlId = fmt.ControlId;
        }
    }

    /// <summary>
    /// Adds the filter field.
    /// </summary>
    /// <param name="fieldId">Name of the field.</param>
    /// <param name="label"></param>
    /// <param name="controlProperties">The control properties</param>
    /// <returns></returns>
    public FormField AddSpecialFilterField(string fieldId, string label = null,
        params eControlPropertyId[] controlProperties)
    {
        var ff = AddFilterField(fieldId, label, controlProperties);
        AddProperty(eControlPropertyId.SpecialFilter, true);
        AddProperty(eControlPropertyId.IsNotMultiple, true);
        return ff;
    }

    /// <summary>
    /// Add Filtering field(s) to the report
    /// </summary>
    /// <param name="fieldIds">fields</param>
    /// <returns></returns>
    public bool AddFilterFields(params string[] fieldIds)
    {
        var allIsOk = true;
        foreach (var fldId in fieldIds)
        {
            _currentFormField = AddFilterField(fldId);
            if (_currentFormField == null)
                allIsOk = false;
        }

        return allIsOk;
    }

    /// <summary>
    /// Adds Special Filter Fields
    /// </summary>
    /// <param name="fieldIds">fields</param>
    /// <returns></returns>
    public bool AddSpecialFilterFields(params string[] fieldIds)
    {
        var allIsOk = true;
        foreach (var fldId in fieldIds)
        {
            _currentFormField = AddSpecialFilterField(fldId);
            if (_currentFormField == null)
                allIsOk = false;
        }

        return allIsOk;
    }

    /// <summary>
    /// Add association field for join filter
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="tableEntityId"></param>
    /// <param name="tableAssociationId"></param>
    /// <param name="label"></param>
    /// <param name="associationId"></param>
    /// <returns></returns>
    protected FormField AddReverseAssociationFilterField(string tableEntityId, string tableAssociationId,
        string fieldName, string label = null, string associationId = null)
    {
        if (form.formFields.Any(f =>
            f.Id == fieldName && f.TableEntityId == tableEntityId && f.TableAssociationId == tableAssociationId &&
            f.FieldOrControlType == FormField.Type.FilterField))
            throw new Exception("Duplicate Id: " + fieldName);
        var associationEntity = ProjectDefinition.Project.GetEntityByEntityId(tableEntityId, entity.model.Id);
        var field = associationEntity?.GetField(fieldName);
        if (field == null)
            return null;
        var fieldIds = fieldName.Split('.');
        var notTabular = string.IsNullOrEmpty(_parentControlId);
        if (notTabular)
            CreateDefaultFilterTab(fieldIds, field);
        var filterField = new FormField(form, field)
        {
            ParentControlId = _parentControlId,
            FieldOrControlType = FormField.Type.FilterField,
            TableEntityId = tableEntityId,
            TableAssociationId = tableAssociationId,
            AssociationId = associationId,
            Id = tableEntityId + "_" + tableAssociationId + "_" + field.Id
        };
        AddField(filterField);
        AddProperty(eControlPropertyId.EntityId, tableEntityId);
        AddProperty(eControlPropertyId.Association, tableAssociationId);
        if (!string.IsNullOrEmpty(label))
            AddProperty(eControlPropertyId.LabelName, label);
        if (notTabular)
        {
            EndSubControls();
            EndSubControls();
            _currentFormField = filterField;
        }

        return filterField;
    }


    /// <summary>
    /// Adds the filter field.
    /// </summary>
    /// <param name="fieldId">Name of the field.</param>
    /// <param name="type">The type.</param>
    /// <param name="controlProperties">The control properties</param>
    /// <returns></returns>
    public FormField AddFilterField(string fieldId, eControlTypeId type,
        params eControlPropertyId[] controlProperties)
    {
        if (form.formFields.Any(f => f.Id == fieldId && f.FieldOrControlType == FormField.Type.FilterField))
            throw new Exception("Duplicate Id: " + fieldId);
        var fieldIds = fieldId.Split('.');
        var fld = form.entity.GetField(fieldIds[0]);
        if (fld == null)
            return null;
        var notTabular = string.IsNullOrEmpty(_parentControlId);
        if (notTabular)
            CreateDefaultFilterTab(fieldIds, fld);
        var filterField = new FormField(form, fld)
        {
            ParentControlId = _parentControlId,
            FieldOrControlType = FormField.Type.FilterField,
            ControlTypeId = type
        };
        AddField(filterField);
        if (notTabular)
        {
            EndSubControls();
            EndSubControls();
            _currentFormField = filterField;
        }

        foreach (var controlProperty in controlProperties ?? Enumerable.Empty<eControlPropertyId>())
            filterField.AddProperty(new FormProperty(controlProperty, true));
        return filterField;
    }

    #endregion

    #region add filter

    /// <summary>
    /// Adds the input records filter.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="association"></param>
    /// <returns></returns>
    private EntityFormFilter AddFilter(ExpressionTree filter, ExpressionTree condition, string entityId = null,
        string association = null)
    {
        if (form is not Form entityForm) return null;
        if (filter?.Root == null)
            throw new Exception($"Incorrect syntax {filter?.ExpressionString}");
        var irf = new EntityFormFilter(filter, condition, entityId, association);
        entityForm.AddInputRecordsFilter(irf);
        return irf;
    }

    /// <summary>
    /// Adds the input records filter.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <returns></returns>
    public EntityFormFilter AddFilter(string filter, string condition = null, string entityId = null)
    {
        return AddFilter(Parser.ParseTree(filter), Parser.ParseTree(condition), entityId);
    }

    /// <summary>
    /// Adds Reverse Association Filter
    /// </summary>
    /// <param name="filter">The filter</param>
    /// <param name="condition">The condition</param>
    /// <param name="entityId">The entity identifier</param>
    /// <param name="association">The association</param>
    /// <returns></returns>
    protected EntityFormFilter AddReverseAssociationFilter(string filter, string condition, string entityId,
        string association)
    {
        return AddFilter(Parser.ParseTree(filter), Parser.ParseTree(condition), entityId, association);
    }

    #endregion

    #region filter specific input states

    /// <summary>
    /// Add Input State
    /// </summary>
    /// <param name="stateIds">State Ids</param>
    /// <returns></returns>
    protected bool SetInputStateIds(params Enum[] stateIds)
    {
        if (stateIds.All(st => form.entity?.GetStateCollection()?.States.ContainsKey("" + st.ToInt()) ?? false))
        {
            var s = "StateId in (" + string.Join(",", stateIds.Select(i => i.ToInt())) + ")";
            return AddFilter(s) != null;
        }

        //else
        //    throw new Exception("Invalid StateIds : " + stateIds.Select(i => i.ToString()));
        return false;
    }

    /// <summary>
    /// Form Don't Force Active States Filter
    /// </summary>
    /// <returns></returns>
    public bool DontForceActiveStatesFilter()
    {
        if (form is Form entityForm) // todo correct?
            entityForm.DontForceActiveStatesFilter = true;
        return false; // todo correct?!
    }

    /// <summary>
    /// Set Input State Ids
    /// </summary>
    /// <param name="stateIds">State Ids</param>
    /// <returns></returns>
    public bool SetInputStateIds(params int[] stateIds)
    {
        if (stateIds.All(st => form.entity?.GetStateCollection()?.States.ContainsKey("" + st) ?? false))
        {
            var s = "StateId in (" + string.Join(",", stateIds.Select(i => i.ToString())) + ")";
            return AddFilter(s) != null;
        }

        //else
        //    throw new Exception("Invalid StateIds : " + stateIds.Select(i => i.ToString()));
        return false;
    }

    #endregion
}
