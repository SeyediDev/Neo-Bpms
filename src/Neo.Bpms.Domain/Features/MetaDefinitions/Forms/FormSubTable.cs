namespace Neo.Bpms.Domain.Features.Definitions.Entities;

/// <summary>
/// Base class to define forms and their details. All form definitions in the business and meta models are sub classes of this object.
/// </summary>
public abstract partial class FormDefinition
{
    /// <summary>
    /// Adds the sub table.
    /// </summary>
    /// <param name="tableEntityId">The entity identifier.</param>
    /// <param name="tableAssociation">The association.</param>
    /// <param name="tableIndexFormSubjectId">The index form subject identifier.</param>
    /// <param name="labelName">The label name</param>
    /// <param name="multipleForeignKeyFieldId">The multiple Foreign Key Field identifier</param>
    /// <returns></returns>
    public FormField AddSubTableWithMultipleCombo(string tableEntityId, string tableAssociation,
        string tableIndexFormSubjectId,
        string labelName, string multipleForeignKeyFieldId, string association = null, string enLabelName = null)
    {
        var formField = AddSubTable(tableEntityId, tableAssociation, tableIndexFormSubjectId, labelName,
            association, true, ContainerControl.None, enLabelName);
        if (formField != null && !string.IsNullOrWhiteSpace(multipleForeignKeyFieldId))
        {
            formField.SubTableToMultipleCombo(multipleForeignKeyFieldId);
        }
        return formField;
    }

    /// <summary>
    /// Adds the sub table.
    /// </summary>
    /// <param name="tableIndexFormSubjectId">The index form subject identifier.</param>
    /// <param name="labelName">The label name</param>
    /// <param name="multipleForeignKeyFieldId">The multiple Foreign Key Field Identifier</param>
    /// <param name="enLabelName"></param>
    /// <param name="association">The association. if null is Id</param>
    /// <param name="enLabelName">The EN label name</param>
    /// <returns></returns>
    public FormField AddSubTableWithMultipleCombo<TTableEntity, TTableAssociation>(string tableIndexFormSubjectId,
        string labelName, string multipleForeignKeyFieldId, string association = null, string enLabelName = null)
    {
        var formField = AddSubTable<TTableEntity, TTableAssociation>(tableIndexFormSubjectId, labelName, association
        , true, ContainerControl.None, enLabelName);
        if (formField != null && !string.IsNullOrWhiteSpace(multipleForeignKeyFieldId))
        {
            formField.SubTableToMultipleCombo(multipleForeignKeyFieldId);
        }
        return formField;
    }

    /// <summary>
    /// Adds the sub table.
    /// </summary>
    /// <param name="tableIndexFormSubjectId">The index form subject identifier.</param>
    /// <param name="labelName">The label name</param>
    /// <param name="association">The association. if null is Id</param>
    /// <param name="editable">if set to <c>true</c> [is editable]</param>
    /// <param name="containerControl"></param>
    /// <param name="enLabelName">The english label name</param>
    /// <returns></returns>
    public FormField AddSubTable<TTableEntity, TTableAssociation>(
        string tableIndexFormSubjectId, string labelName, string association = null,
        bool editable = false, ContainerControl containerControl = ContainerControl.None, string enLabelName = null, string filter = null)
    {
        return AddSubTable(typeof(TTableEntity).Name, typeof(TTableAssociation).Name,
            tableIndexFormSubjectId, labelName, association, editable, containerControl, enLabelName, null, filter);
    }

    /// <summary>
    /// Adds the sub table.
    /// </summary>
    /// <param name="tableAssociation"></param>
    /// <param name="tableIndexFormSubjectId">The index form subject identifier.</param>
    /// <param name="labelName"></param>
    /// <param name="association">The association. if null is Id</param>
    /// <param name="editable"></param>
    /// <param name="containerControl"></param>
    /// <param name="enLabelName"></param>
    /// <returns></returns>
    public FormField AddSubTable<TTableEntity>(string tableAssociation,
        string tableIndexFormSubjectId, string labelName, string association = null,
        bool editable = false, ContainerControl containerControl = ContainerControl.None,
        string enLabelName = null, string filter = null)
    {
        return AddSubTable(typeof(TTableEntity).Name, tableAssociation,
            tableIndexFormSubjectId, labelName, association, editable, containerControl, enLabelName, null, filter);
    }

    /// <summary>
    /// Adds the sub table.
    /// </summary>
    /// <param name="tableEntityId">The entity identifier.</param>
    /// <param name="tableAssociationId"></param>
    /// <param name="tableIndexFormSubjectId">The index form subject identifier.</param>
    /// <param name="labelName"></param>
    /// <param name="associationId"></param>
    /// <param name="editable"></param>
    /// <param name="containerControl"></param>
    /// <param name="enLabelName"></param>
    /// <returns></returns>
    public FormField AddSubTable(string tableEntityId, string tableAssociationId = null,
        string tableIndexFormSubjectId = null, string labelName = null, string associationId = null,
        bool editable = false, ContainerControl containerControl = ContainerControl.None, 
        string enLabelName = null, eControlPropertyId? controlPropertyId=null, string filter = null)
    {
        if (!GetSubTable(form, tableEntityId, tableAssociationId, out var tableEntity, out var tableAssociation))
        {
            return null;
            throw new Exception("Invalid sub table " + tableEntityId + "-" + tableAssociationId + "-" +
                                      tableIndexFormSubjectId);
        }
        if (tableAssociation == null)
            throw new Exception("Invalid association " + tableEntityId + "-" + tableAssociationId);
        var tableControlId = FormField.SubTableInstanceId(tableEntityId, tableAssociationId, tableIndexFormSubjectId);
        //const ContainerControl containerControl = ContainerControl.Accordion; //ContainerControl.MultiTab
        var notTabular = containerControl != ContainerControl.None && string.IsNullOrEmpty(_parentControlId);
        if (notTabular)
        {
            var fmtId = "SubTablesTab" + containerControl;
            var fmt = form?.formFields.FirstOrDefault(f => f.ControlTypeId == (eControlTypeId)containerControl && f.Id == fmtId);
            if (fmt == null)
            {
                AddControl((eControlTypeId)containerControl, fmtId);
                StartSubControls();
            }
            else
            {
                _currentFormField = fmt;
                _parentControlId = fmt.ControlId;
            }
            AddControl(containerControl == ContainerControl.Accordion ? eControlTypeId.AccordionItem : eControlTypeId.MultiTabItem,
                $"{tableControlId}_TabItem",
                labelName ?? tableAssociation.Name, enLabelName ?? tableEntity.EnName);
            // Set entity info on tab for icon display
            AddProperty(eControlPropertyId.NamespaceId, tableEntity.NamespaceId);
            AddProperty(eControlPropertyId.EntityId, tableEntityId);
            StartSubControls();
        }
        var formField = FormField.NewSubTableInstance(form, _parentControlId, tableEntityId, tableAssociationId, tableIndexFormSubjectId, associationId, tableEntity, tableAssociation, labelName, enLabelName);
        AddField(formField);
        if (editable)
            AddProperty(eControlPropertyId.Editable);
        if(controlPropertyId!=null)
            AddProperty(controlPropertyId.Value);
        if( !string.IsNullOrEmpty(filter) )
        {
            AddProperty(eControlPropertyId.FilterFormula, filter);
        }
        if (notTabular)
        {
            EndSubControls();
            EndSubControls();
            _currentFormField = formField;
        }
        return formField;
    }

    private static bool GetSubTable(Form form, string entityId, string association, out UiEntity uiEntity,
        out Association aso)
    {
        var entity = GetSubTableEntity(form, entityId, out uiEntity);
        aso = null;
        if (uiEntity == null) return false;
        if (association != null)
            aso = entity.GetAssociation(association);
        else
        {
            //find the first association to this entity
            foreach (var item in entity.Associations)
            {
                if (item.DestEntityId == form.entity.Id && item.DestNamespaceId == form.NamespaceId)
                {
                    aso = item;
                    break;
                }
            }
        }
        return aso != null;
    }

    private static Entity GetSubTableEntity(Form form, string entityId, out UiEntity uiEntity)
    {
        var entity = form.entity.model.GetEntity(entityId)
                      ?? ProjectDefinition.Project.GetEntityByEntityId(entityId);
        uiEntity = entity as UiEntity;
        return entity;
    }
}
