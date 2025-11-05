namespace Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

/// <summary>
/// Form Field
/// </summary>
public class FormField : BaseModelClass
{
    public enum Type
    {
        Field = 1,
        FilterField,
        SubTable,
        ColumnField,
        SubjectField,
        TooltipField,
        IconField,

        Control = 11,
        UIBlock = 21,
        //CssBlock=4
    };

    public Type FieldOrControlType;

    [XmlIgnore]
    public EntityField Field;

    public string ControlId => Id;

    public string ParentControlId;

    public eControlTypeId ControlTypeId { get; set; }

    public List<FormProperty> Properties { get; set; }

    public bool bFromUIBlock;

    [XmlIgnore]
    public UiEntity TableEntity { get; set; }

    public string AssociationId { get; set; }

    public string TableEntityId;

    [XmlIgnore]
    public Association TableAssociation;

    public string TableAssociationId;

    /// <summary>
    /// Form Field
    /// </summary>
    /// <param name="form">form</param>
    /// <param name="field">field</param>
    /// <returns></returns>
    public FormField(Form form, EntityField field)
        : base(form, field.Id, field.Name)
    {
        FieldOrControlType = Type.Field;
        Field = field;
        //			this.isWritable = isWritable;
        //			this.isNew = isNew;			
        ControlTypeId = eControlTypeId.None;
        bFromUIBlock = false;
    }

    /// <summary>
    /// Form Field
    /// </summary>
    /// <param name="form">form</param>
    /// <param name="field">field</param>
    /// <param name="type">type</param>
    /// <returns></returns>
    public FormField(Form form, EntityField field, eControlTypeId type)
        : base(form, field.Id, field.Name)
    {
        FieldOrControlType = Type.Field;
        Field = field;
        //			this.isWritable = isWritable;
        //			this.isNew = isNew;			
        ControlTypeId = type;
        bFromUIBlock = false;
    }

    /// <summary>
    /// Form Field
    /// </summary>
    /// <param name="form">form</param>
    /// <param name="type">type</param>
    /// <param name="id">id</param>
    /// <returns></returns>
    public FormField(Form form, eControlTypeId type, string id)
        : base(form, id, type.ToString())
    {
        FieldOrControlType = Type.Control;
        Field = null;
        //			this.isWritable = isWritable;
        //			this.isNew = isNew;
        //			controlId = id;
        ControlTypeId = type;
        bFromUIBlock = false;
    }

    /// <summary>
    /// Form Field
    /// </summary>
    /// <param name="form">form</param>
    /// <param name="formSubjectId">Form Subject</param>
    /// <returns></returns>
    public FormField(Form form, string formSubjectId)
        : base(form, formSubjectId, formSubjectId)
    {
        FieldOrControlType = Type.Control;
        Field = null;
        //			this.isWritable = isWritable;
        //			this.isNew = isNew;
        ParentControlId = null;
        ControlTypeId = eControlTypeId.None;
        bFromUIBlock = false;
    }

    /// <summary>
    /// Form Field
    /// </summary>
    /// <param name="f">f</param>
    /// <returns></returns>
    public FormField(FormField f)
        : base(f.Parent, f.Field.Id, f.Field.Name)
    {
        Field = f.Field;
        //			this.isWritable = f.isWritable;
        //			this.isNew = f.isNew;
        ParentControlId = f.ParentControlId;
        //			controlId = f.controlId;
        ControlTypeId = f.ControlTypeId;
        bFromUIBlock = true;
        if (f.Properties != null)
        {
            foreach (FormProperty p in f.Properties)
                AddProperty(new FormProperty(p));
        }
        //			if (f.labelProperties != null)
        //			{
        //				foreach (var p in f.labelProperties)
        //					addLabelProperty(new FormProperty(p));
        //			}
    }

    public FormField()
    {
    }

    public static FormField NewSubTableInstance(Form form, string parentControlId, string tableEntityId,
        string tableAssociationId, string tableIndexFormSubjectId, string associationId, UiEntity tableEntity,
        Association tableAssociation, string labelName, string enLabelName)
    {
        FormField newSubTableInstance = new()
        {
            Parent = form,
            ParentControlId = parentControlId,
            FieldOrControlType = Type.SubTable,
            ControlTypeId = eControlTypeId.IndexTable,

            Name = tableAssociation.DestEntityId,
            Id = SubTableInstanceId(tableEntityId, tableAssociationId, tableIndexFormSubjectId),

            TableEntityId = tableEntity.Id,
            TableEntity = tableEntity,
            TableAssociationId = tableAssociation.Id,
            TableAssociation = tableAssociation,
            AssociationId = associationId,

            Field = null,
            bFromUIBlock = false
        };

        if (!string.IsNullOrWhiteSpace(tableIndexFormSubjectId))
            newSubTableInstance.AddProperty(eControlPropertyId.Subject, tableIndexFormSubjectId);
        if (!string.IsNullOrWhiteSpace(labelName))
            newSubTableInstance.AddProperty(eControlPropertyId.LabelName, labelName);
        if (!string.IsNullOrWhiteSpace(enLabelName))
            newSubTableInstance.AddProperty(eControlPropertyId.EnLabelName, enLabelName);
        return newSubTableInstance;
    }
    public static FormField NewListInstance(Form form, EntityField field, string parentControlId,
        string subFormSubjectId, UiEntity subTableEntity)
    {
        FormField instance = new()
        {
            Parent = form,
            ParentControlId = parentControlId,
            FieldOrControlType = Type.SubTable,
            ControlTypeId = eControlTypeId.IndexTable,
            Name = field.Name,
            Id = field.Id,
            TableEntityId = subTableEntity.Id,
            TableEntity = subTableEntity,
            Field = field,//todo was null
            bFromUIBlock = false
        };
        if (!string.IsNullOrWhiteSpace(subFormSubjectId))
            instance.AddProperty(eControlPropertyId.Subject, subFormSubjectId);

        return instance;
    }

    public static string SubTableInstanceId(string tableEntityId, string tableAssociationId, string tableIndexFormSubjectId)
    {
        return tableEntityId + "_" + (tableAssociationId ?? "null") +
                              (string.IsNullOrEmpty(tableIndexFormSubjectId) ? "" : "_" + tableIndexFormSubjectId);
    }

    public void SubTableToMultipleCombo(string multipleForeignKeyFieldId)
    {
        ControlTypeId = eControlTypeId.MultipleSelectableCombo;
        AddProperty(eControlPropertyId.MultipleForeignKeyFieldId, multipleForeignKeyFieldId);
        Id += "_" + multipleForeignKeyFieldId;
    }

    public void AddProperty(eControlPropertyId id, object value)
    {
        AddProperty(new FormProperty(id, value));
    }

    /// <summary>
    /// add Property
    /// </summary>
    /// <param name="property">property</param>
    /// <returns></returns>
    public void AddProperty(FormProperty property)
    {
        Properties ??= [];
        if (property.Id == eControlPropertyId.LabelName)
        {
            FormProperty prop = GetProperty(property.Id);
            if (prop != null)
            {
                prop.Value = property.Value;
                return;
            }
        }
        Properties.Add(property);
    }

    public void AddProperties(IList<FormProperty> addingProperties)
    {
        Properties ??= [];
        Properties.AddRange(addingProperties);
    }

    public IList<FormProperty> GetProperties()
    {
        return Properties;
    }
    public void SetProperties(List<FormProperty> settingProperties)
    {
        Properties = settingProperties;
    }

    /// <summary>
    /// Check Property
    /// </summary>
    /// <param name="propertyId">property</param>
    /// <returns></returns>
    public bool CheckProperty(eControlPropertyId propertyId)
    {
        var prop = Properties?.FirstOrDefault(p => p.Id == propertyId);
        return prop != null;
    }

    public FormProperty GetProperty(eControlPropertyId propertyId)
    {
        var prop = Properties?.FirstOrDefault(p => p.Id == propertyId);
        return prop;
    }

    public string Property(eControlPropertyId propertyId) => GetProperty(propertyId)?.Value?.ToString();
    public string GetNameWithCulture(string culture)
    {
        string value;
        if (culture == "en")
        {
            value = Property(eControlPropertyId.EnLabelName);
            if (string.IsNullOrEmpty(value))
                value = Field?.EnName ?? Property(eControlPropertyId.LabelName) ?? Field?.Id ?? Name ?? "";
        }
        else
        {
            value = Property(eControlPropertyId.LabelName) ?? Name ?? Field?.Id ?? "";
        }

        return value;
    }
}
