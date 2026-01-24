using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.Domain.Features.Definitions.Entities;

/// <summary>
/// Base class to define forms and their details. All form definitions in the business and meta models are sub classes of this object.
/// </summary>
public abstract partial class FormDefinition
{
    protected FormField _currentFormField;
    protected string _parentControlId;
    /// <summary>
    /// Defines View Model
    /// </summary>
    /// <returns></returns>
    protected virtual bool DefineViewModel()
    {
        return true;
    }

    /// <summary>
    /// Defines View Model
    /// </summary>
    /// <returns></returns>
    protected virtual void ViewModel()
    {
    }
    protected virtual void FormOperation()
    {
    }

    /// <summary>
    /// Adds the field.
    /// </summary>
    /// <param name="formField">The form field.</param>
    /// <returns></returns>
    protected FormField AddField(FormField formField)
    {
        _currentFormField = formField;
        //			currentBaseElement = formField;
        form.AddFormField(formField);
        return _currentFormField;
    }

    /// <summary>
    /// Adds the field.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="controlProperties"></param>
    /// <returns></returns>
    public FormField AddField(string fieldName, params eControlPropertyId[] controlProperties)
    {
        _currentFormField = null;
        if (form.formFields.Any(f => f.Id == fieldName && f.FieldOrControlType == FormField.Type.Field))
        {
            throw new Exception("Duplicate Id: " + fieldName);
        }

        string[] fieldIds = fieldName.Split('.');
        EntityField fld = form.entity.GetField(fieldIds[0]);
        if (fld == null)
        {
            return null;
        }

        FormField ff = new(form, fld)
        {
            Id = fieldName,
            ParentControlId = _parentControlId,
            FieldOrControlType = FormField.Type.Field
        };
        for (int i = 1; i < fieldIds.Length; i++)
        {
            fld = fld?.AssociationEntity?.Entity()?.GetField(fieldIds[i]);
        }

        if (fld == null)
        {
            throw new Exception("invalid field Id:" + fieldName);
        }

        foreach (eControlPropertyId controlProperty in controlProperties ?? Enumerable.Empty<eControlPropertyId>())
        {
            ff.AddProperty(new FormProperty(controlProperty, true));
        }

        return AddField(ff);
    }

    public FormField AddListTable(string fieldName, string subFormSubjectId = null)
    {
        _currentFormField = null;
        if (form.formFields.Any(f => f.Id == fieldName && f.FieldOrControlType == FormField.Type.SubTable))
        {
            throw new Exception("Duplicate Id: " + fieldName);
        }

        EntityField fld = form.entity.GetField(fieldName) ??
            throw new Exception("invalid field Id:" + fieldName);


        string subTableEntityId = fld.CSharpType.GetGenericArguments().FirstOrDefault()?.Name;
        _ = GetSubTableEntity(form, subTableEntityId, out UiEntity subTableEntity);
        return AddField(FormField.NewListInstance(form, fld, _parentControlId, subFormSubjectId, subTableEntity));
    }
    /// <summary>
    /// Adds the field.
    /// </summary>
    /// <param name="fieldNames">Name of the fields.</param>
    /// <returns></returns>
    public void AddFields(params string[] fieldNames)
    {
        foreach (string fieldName in fieldNames ?? Enumerable.Empty<string>())
        {
            _ = AddField(fieldName);
        }
    }

    /// <summary>
    /// Adds the association field.
    /// </summary>
    /// <param name="controlProperties"></param>
    /// <returns></returns>
    public FormField AddField<TAssociation>(params eControlPropertyId[] controlProperties)
    {
        return AddField(typeof(TAssociation).Name, controlProperties);
    }

    /// <summary>
    /// Adds the field.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="controlTypeId">The type.</param>
    /// <param name="controlProperties"></param>
    /// <returns></returns>
    public FormField AddField(string fieldName, eControlTypeId controlTypeId,
        params eControlPropertyId[] controlProperties)
    {
        _currentFormField = null;
        if (form.formFields.Any(f => f.Id == fieldName && f.FieldOrControlType == FormField.Type.Field))
        {
            throw new Exception("Duplicate Id: " + fieldName);
        }

        string[] fieldIds = fieldName.Split('.');
        EntityField fld = form.entity.GetField(fieldIds[0]);
        if (fld == null)
        {
            return null;
        }

        FormField ff = new(form, fld)
        {
            Id = fieldName,
            ParentControlId = _parentControlId,
            FieldOrControlType = FormField.Type.Field,
            ControlTypeId = controlTypeId
        };
        for (int i = 1; i < fieldIds.Length; i++)
        {
            fld = fld?.AssociationEntity?.Entity()?.GetField(fieldIds[i]);
        }

        if (fld == null)
        {
            return null;
        }

        foreach (eControlPropertyId controlProperty in controlProperties ?? Enumerable.Empty<eControlPropertyId>())
        {
            if (controlProperty != eControlPropertyId.None)
            {
                ff.AddProperty(new FormProperty(controlProperty, true));
            }
        }

        return AddField(ff);
    }
    /// <summary>
    /// Adds the control.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public FormField AddControl(eControlTypeId type, string id)
    {
        _currentFormField = null;
        if (form.formFields.Any(f => f.ControlId == id))
        {
            //id += new Random(10);
            throw new Exception("Duplicate Id: " + id);
        }
        FormField ff = new(form, type, id)
        {
            ParentControlId = _parentControlId
        };
        return AddField(ff);
    }
    /// <summary>
    /// Adds The Control
    /// </summary>
    /// <param name="type">The type</param>
    /// <param name="id">The identifier</param>
    /// <param name="name">The name</param>
    /// <param name="enName">The English name</param>
    /// <returns></returns>
    public FormField AddControl(eControlTypeId type, string id, string name, string enName = null)
    {
        FormField f = AddControl(type, id);
        _ = AddProperty(eControlPropertyId.LabelName, name);
        if (enName != null)
        {
            _ = AddProperty(eControlPropertyId.EnLabelName, enName);
        }

        return f;
    }

    /// <summary>
    /// Adds the field column.
    /// </summary>
    /// <param name="fieldIdList">Name of the field.</param>
    /// <returns></returns>
    public void AddColumns(params string[] fieldIdList)
    {
        foreach (string fldId in fieldIdList)
        {
            _ = AddColumn(fldId);
        }
    }

    /// <summary>
    /// Adds the field column.
    /// </summary>
    /// <param name="fieldId">Name of the field.</param>
    /// <param name="controlTypeId"></param>
    /// <returns></returns>
    public FormField AddColumn(string fieldId, eControlTypeId controlTypeId= eControlTypeId.None)
    {
        _currentFormField = null;
        if (form.formFields.Any(f => f.Id == fieldId && f.FieldOrControlType == FormField.Type.ColumnField))
        {
            throw new Exception("Duplicate Id: " + fieldId);
        }

        string[] fieldIds = fieldId.Split('.');
        EntityField fld = form.entity.GetField(fieldIds[0]);
        if (fld == null)
        {
            return null;
        }

        FormField ff = new(form, fld)
        {
            Id = fieldId,
            ParentControlId = _parentControlId,
            FieldOrControlType = FormField.Type.ColumnField,
            ControlTypeId = controlTypeId
        };
        for (int i = 1; i < fieldIds.Length; i++)
        {
            fld = fld?.AssociationEntity?.Entity()?.GetField(fieldIds[i]);
        }

        return fld == null ? null : AddField(ff);
    }
    /// <summary>
    /// Starts the sub controls.
    /// </summary>
    public void StartSubControls()
    {
        if (_currentFormField != null)
        {
            _parentControlId = _currentFormField.ControlId;
        }
    }

    /// <summary>
    /// Ends the sub controls.
    /// </summary>
    public void EndSubControls()
    {
        if (form != null && _parentControlId != null)
        {
            FormField parent = form.formFields.FirstOrDefault(ff => ff.ControlId == _parentControlId);
            _parentControlId = parent?.ParentControlId;
        }
    }
    
    public FormField AddSubjectColumn<TForm>()
        where TForm : FormDefinition, ISubjectFormDefinition, new()
    {
        var name = typeof(TForm).Name;
        var subjectForm = entity.GetEntityForm(name);
        return AddSubjectColumn(subjectForm?.Name??name, true, false, name, subjectForm?.EnName??name);
    }
    /// <summary>
    /// Adds the subject column.
    /// </summary>
    /// <param name="label">The label.</param>
    /// <param name="hasEdit">if set to <c>true</c> [has edit].</param>
    /// <param name="hasDetails">if set to <c>true</c> [has details].</param>
    /// <param name="formSubjectId">The form subject identifier.</param>
    /// <returns></returns>
    public FormField AddSubjectColumn(string label, bool hasEdit, bool hasDetails, string formSubjectId, string enLabel = null)
    {
        _currentFormField = null;
        FormField ff = new(form, formSubjectId) { FieldOrControlType = FormField.Type.SubjectField };
        _ = AddField(ff);
        if (!hasEdit)
        {
            _ = AddProperty(eControlPropertyId.NoEditIcon, true);
        }

        if (!hasDetails)
        {
            _ = AddProperty(eControlPropertyId.NoDetailsIcon, true);
        }

        if (!string.IsNullOrWhiteSpace(formSubjectId))
        {
            _ = AddProperty(eControlPropertyId.Subject, formSubjectId);
        }

        if (!string.IsNullOrWhiteSpace(label))
        {
            _ = AddProperty(eControlPropertyId.LabelName, label);
        }

        if (!string.IsNullOrWhiteSpace(enLabel))
        {
            _ = AddProperty(eControlPropertyId.EnLabelName, enLabel);
        }

        return ff;
    }

    /// <summary>
    /// Adds the description field.
    /// </summary>
    /// <param name="fieldName">The description field identifier.</param>
    /// <param name="forFieldId">For field identifier.</param>
    /// <param name="asIcon">if set to <c>true</c> [as icon].</param>
    /// <returns></returns>
    public FormField AddDescriptionField(string fieldName, string forFieldId, bool asIcon)
    {
        _currentFormField = null;
        string[] fieldIds = fieldName.Split('.');
        EntityField fld = form.entity.GetField(fieldIds[0]);
        if (fld == null)
        {
            return null;
        }

        FormField ff = new(form, fld)
        {
            Id = fieldName,
            FieldOrControlType = asIcon ? FormField.Type.IconField : FormField.Type.TooltipField
        };
        for (int i = 1; i < fieldIds.Length; i++)
        {
            fld = fld?.AssociationEntity?.Entity()?.GetField(fieldIds[i]);
        }

        if (fld == null)
        {
            return null;
        }

        _ = AddField(ff);
        _ = AddProperty(eControlPropertyId.ForFieldId, forFieldId);
        return ff;
    }

    /// <summary>
    /// Adds the property.
    /// </summary>
    /// <param name="fieldId">The control property.</param>
    /// <returns></returns>
    public bool SelectField(string fieldId)
    {
        _currentFormField = null;
        if (form == null)
        {
            return false;
        }

        _currentFormField =
            form.formFields.FirstOrDefault(f => f.Id == fieldId && f.FieldOrControlType == FormField.Type.Field);
        return _currentFormField != null;
    }

    /// <summary>
    /// Adds the property.
    /// </summary>
    /// <param name="controlProperty">The control property.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public bool AddProperty(eControlPropertyId controlProperty, object value = null)
    {
        if (_currentFormField == null)
        {
            return false;
        }

        value ??= true;

        _currentFormField.AddProperty(new FormProperty(controlProperty, value));
        return true;
    }

    /// <summary>
    /// Adds the property.
    /// </summary>
    /// <param name="controlProperties"></param>
    /// <returns></returns>
    protected bool AddProperties(params eControlPropertyId[] controlProperties)
    {
        if (_currentFormField == null || controlProperties == null)
        {
            return false;
        }

        foreach (eControlPropertyId controlProperty in controlProperties)
        {
            _currentFormField.AddProperty(new FormProperty(controlProperty, true));
        }

        return true;
    }
    //todo: support of UI Blocks
    /// <summary>
    /// Adds the UI block.
    /// </summary>
    /// <param name="uiBlock">The UI block.</param>
    protected void AddUiBlock(Form uiBlock)
    {
        form.AddUiBlock(uiBlock, _parentControlId);
    }

    /// <summary>
    /// Adds All Fields
    /// </summary>
    /// <param name="formFieldType"></param>
    /// <param name="showParentAssociation"></param>
    /// <param name="dontAddEntityPkv"></param>
    public void AddAllFields(FormField.Type formFieldType, bool showParentAssociation = false,
        bool dontAddEntityPkv = false)
    {
        Dictionary<string, string> mapFields = entity.GetAssociationMapFields();
        foreach (EntityField field in entity.entityFields.Values.OrderBy(f => f.Order))
        {
            if (field.FieldType!= TVariableTypes.File)
            {
                AddOneField(formFieldType, showParentAssociation, dontAddEntityPkv, mapFields, field);
            }
        }
        foreach (EntityField field in entity.entityFields.Values.OrderBy(f => f.Order))
        {
            if (field.FieldType == TVariableTypes.File)
            {
                AddOneField(formFieldType, showParentAssociation, dontAddEntityPkv, mapFields, field);
            }
        }

        static bool IdInDescription(EntityField field)
        {
            return field.Id.In("Description", "EnDescription", "Text"); //todo
        }

        void AddOneField(FormField.Type formFieldType, bool showParentAssociation, bool dontAddEntityPkv, 
            Dictionary<string, string> mapFields, EntityField field)
        {
            if (field.AuditField)
            {
                return;
            }

            if (field.NotMap && field.AssociationEntity == null && field.Formula != null && formFieldType != FormField.Type.ColumnField)
            {
                return;
            }

            if (field.IsAutoIncrement() && formFieldType != FormField.Type.ColumnField)
            {
                return;
            }

            if (formFieldType == FormField.Type.ColumnField && IdInDescription(field) )
            {
                return;
            }

            if (dontAddEntityPkv && field.IncludeInPkv)
            {
                return;
            }

            if (mapFields.ContainsKey(field.Id))
            {
                return;
            }

            if (entity.IsStateBase)
            {
                if (field.Id == "StateId" || (field.IsForParent && field.Id == "State"))
                {
                    return;
                }
            }
            if (!showParentAssociation && field.AssociationEntity != null && field.AssociationEntity.GetType() == typeof(WeakEntityAssociation))
            {
                return;
            }

            if (field.AssociationEntity != null && field.AssociationEntity.GetType() == typeof(ParentEntity))
            {
                return;
            }

            if (field.Association != null && field.Association.Hidden)
            {
                return;
            }

            if (field.IsForParent && field.CheckFlag(EntityFieldFlags.IncludeInPKV))
            {
                return;
            }
            
            if (entity.EntityType.IsInBaseInterface<ISoftDelete>())
            {
                if (field.Id == nameof(ISoftDelete.ExpireDate) || field.Id == nameof(ISoftDelete.IsDeleted))
                {
                    return;
                }
            }
 
            eControlTypeId control = eControlTypeId.None;
            if (field.FieldType == TVariableTypes.File)
            {
                control = eControlTypeId.File; //eControlTypeId.AdvancedUpload
            }
            else if (IdInDescription(field))
            {
                control = eControlTypeId.MultilineTextInput;
            }

            eControlPropertyId controlPropertyId = eControlPropertyId.None;
            if (field.Association != null)
            {
                controlPropertyId = eControlPropertyId.RemoteData;
            }

            switch (formFieldType)
            {
                case FormField.Type.Field:
                    _ = control == eControlTypeId.None
                        ? AddField(field.Id, controlPropertyId)
                        : AddField(field.Id, control, controlPropertyId);

                    if (field.Required ||
                         (field.AssociationEntity?.Maps?.Any(m => entity.GetField(m.SourceField)?.Required ?? false) ?? false))
                    {
                        _ = AddProperty(eControlPropertyId.Required);
                    }
                    break;
                case FormField.Type.FilterField:
                    if (control == eControlTypeId.File || control == eControlTypeId.AdvancedUpload)
                    {
                        return;
                    }

                    _ = control == eControlTypeId.None
                        ? AddFilterField(field.Id, controlPropertyId)
                        : AddFilterField(field.Id, control, controlPropertyId);

                    break;
                case FormField.Type.ColumnField:
                    AddColumnInAddAll(field, control);
                    break;
            }
            if (field.FieldType == TVariableTypes.File)
            {
                if (field.HasProperty(EntityFieldPropertyId.Multiple))
                {
                    _ = AddProperty(eControlPropertyId.IsMultiple);
                }
                if (field.HasProperty(EntityFieldPropertyId.UseFileServer))
                {
                    _ = AddProperty(eControlPropertyId.UseFileServer);
                }
                if (field.HasProperty(EntityFieldPropertyId.ShowDocumentInPage))
                {
                    _ = AddProperty(eControlPropertyId.ShowDocumentInPage);
                }
                if (field.HasProperty(EntityFieldPropertyId.DocumentType))
                {
                    _ = AddProperty(eControlPropertyId.DocumentType, field.GetPropertyValue(EntityFieldPropertyId.DocumentType));
                }
            }
        }
    }

    /// <summary>
    /// Adds Column In Add All
    /// </summary>
    /// <param name="field"></param>
    /// <param name="control"></param>
    protected virtual void AddColumnInAddAll(EntityField field, eControlTypeId control)
    {
        AddColumn(field.Id, control);

        if (field.Required ||
            (field.AssociationEntity?.Maps?.Any(m => entity.GetField(m.SourceField)?.Required ?? false) ?? false))
        {
            _ = AddProperty(eControlPropertyId.Required);
        }
    }
}
