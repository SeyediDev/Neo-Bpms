using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices;

public class MultipleSelectableCombo(IFormLogicHelper formLogicHelper, InputFieldDefinition field,
    ControlsRendererData controlsRendererData, ISBVRRenderer sbvrRenderer)
    : Combo(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    public override NeoStringBuilder Render()
    {
        if (ControlsRendererData.Options.IsInToolBox)
            return BlackBox();
        ElasticObject record = ControlsRendererData.Record;
        TableDefinition table = (TableDefinition)Field;
        string multipleForeignKeyFieldId = Field.GetProperty(eControlPropertyId.MultipleForeignKeyFieldId)?.Value?.ToString();
        Domain.Entities.Cmmn.Entities.Entity entity = ProjectDefinition.Project.GetEntity(table.NamespaceId, table.EntityId);
        EntityField multipleForeignKeyField = entity?.GetField(multipleForeignKeyFieldId);
        bool isRemoteData = Field.PropertyBoolean(eControlPropertyId.RemoteData);
        Dictionary<string, string> idsList = GetIdsList(record, table, multipleForeignKeyField);
        NeoStringBuilder result = new();
        RenderHeader(result, isRemoteData, true, [.. idsList.Values], true);
        //todo Check OnDemand and RemoteData Properties?
        if (multipleForeignKeyFieldId != null && table.CombosData.TryGetValue(multipleForeignKeyFieldId, out ComboData value))
        {
            ComboData list = value;
            foreach (FormDataRow item in list.Rows)
            {
                result += "<option";
                if (idsList.ContainsKey(item.Ids) && !string.IsNullOrEmpty(item.Ids))
                    result += " selected=\"selected\"";
                if (!string.IsNullOrEmpty(item.Ids))
                    result += " value=\"" + item.Ids + "\"";
                result += ">" + item.DisplayValue + "</option>";
            }
        }
        RenderFooter(result);
        return result;
    }

    private static Dictionary<string, string> GetIdsList(ElasticObject record, TableDefinition table, EntityField multipleForeignKeyField)
    {
        record.GetField(table.FieldName, out object tableData);

        if (tableData is IList<ElasticObject> tableDataRows)
        {
            return GetIdsListFromElastic(multipleForeignKeyField, tableDataRows);
        }

        return tableData is IList<string> dataRows ? GetIdsListFromString(dataRows) : [];
        ;
    }
    private static Dictionary<string, string> GetIdsListFromString(IList<string> tableDataRows)
    {
        return tableDataRows.ToDictionary(s => s);
    }

    private static Dictionary<string, string> GetIdsListFromElastic(EntityField multipleForeignKeyField, IList<ElasticObject> tableDataRows)
    {
        Dictionary<string, string> idsList = [];

        if (tableDataRows != null && multipleForeignKeyField?.AssociationEntity?.Maps != null)
        {
            foreach (ElasticObject row in tableDataRows)
            {
                string rowIds = "";
                foreach (Domain.Entities.Cmmn.Relationship.EntityRelationMap map in multipleForeignKeyField.AssociationEntity.Maps)
                {
                    string rowId = row.GetString(map.SourceField);
                    if (string.IsNullOrEmpty(rowId)) continue;
                    if (!string.IsNullOrEmpty(rowIds)) rowIds += "#";
                    rowIds += rowId;
                }

                if (!string.IsNullOrEmpty(rowIds))
                {
                    if (!idsList.ContainsKey(rowIds))
                        idsList.Add(rowIds, rowIds);
                    //else
                    //	Logger.LogError()//todo
                }
            }
        }

        return idsList;
    }

    public override NeoStringBuilder RenderRelatedLinks(NeoStringBuilder result, bool recordBase)
    {
        TableDefinition table = (TableDefinition)Field;
        if (table.MultipleForeignKeyField == null)
            return result;
        var formIdentifier = new
        {
            table.MultipleForeignKeyField.NamespaceId,
            table.MultipleForeignKeyField.EntityId,
            Field.FormSubjectId,
        };
        table.MultipleForeignKeyField.ControlType = eControlTypeId.MultipleSelectableCombo;
        return RenderRelatedLinksUrl(table.MultipleForeignKeyField, result, recordBase, formIdentifier);
    }
}
