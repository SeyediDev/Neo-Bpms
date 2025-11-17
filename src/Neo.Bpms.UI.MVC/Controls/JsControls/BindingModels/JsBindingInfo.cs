using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Newtonsoft.Json.Converters;

namespace Neo.Bpms.UI.MVC.Controls.JsControls.BindingModels;

public class JsBindingInfo
{
    public JsBindingInfo()
    {
        tableInfo = [];
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public JsBindingType type { get; set; }
    public string name { get; set; }
    public string relatedDataField { get; set; }
    public List<JsBindingInfo> tableInfo { get; set; }

    public void FormTableMappings(TableDefinition table)
    {
        foreach (InputFieldDefinition col in table.Columns)
        {
            string nameSuffix = (col.FieldType == TVariableTypes.Association) ? "Id" : "";
            JsBindingInfo jsBindingInfo = new()
            {
                // type is based on controlType and not fieldType because multi combo binding mechanism is different
                type = col.ControlType == eControlTypeId.IndexTable ? JsBindingType.table : JsBindingType.field,
                name = col.FieldName + nameSuffix,
                relatedDataField = col.PropertyValue(eControlPropertyId.FieldMapping) ?? col.FieldName /*+ nameSuffix*/
            };
            if (col.ControlType == eControlTypeId.IndexTable)
            {
                jsBindingInfo.FormTableMappings((TableDefinition)col);
            }
            tableInfo.Add(jsBindingInfo);
        }
    }
}