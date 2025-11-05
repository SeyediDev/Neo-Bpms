using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;
using Microsoft.Extensions.Configuration;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Extensions;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class NeoHtmlControl(
    IFormLogicHelper formLogicHelper, InputFieldDefinition field, ControlsRendererData controlsRendererData, 
    ISBVRRenderer sbvrRenderer)
    : BaseNeoHtmlControl(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    public override NeoStringBuilder Render()
    {
        throw new NotImplementedException();
    }

    public NeoStringBuilder RenderCheckBoxList(string value)
    {
        NeoStringBuilder stringBuilder = new();
        stringBuilder += $"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip +
                              "\" class=\"" +
                              ControlsClassString +
                              CommonProperties.NarrowColumnClasses + CommonProperties.ShowHideRelatedClass +
                              "\">";
        RenderDesignIcons(stringBuilder);
        stringBuilder += "<div class=\"dWrapper\">";
        stringBuilder += RenderBulkEditCheckbox();
        RenderLabel(stringBuilder);
        if (Structure.CombosData.TryGetValue(Field.FieldName, out ComboData list))
        {
            ulong flags = ulong.Parse(value);

            foreach (FormDataRow item in list.Rows)
            {
                if (string.IsNullOrEmpty(item.Ids)) continue;
                int index = Convert.ToInt32(item.Ids);
                ulong vs = (ulong)1 << index;
                bool selected = (flags & vs) != 0;
                stringBuilder += "<div>";
                if (selected)
                {
                    stringBuilder += "<input " + CommonProperties.ReadOnlyRelatedAttribute +
                                          " type=\"checkbox\" onchange=\"checkboxlistChanged(this)\" checked=\"checked\" name=\"" +
                                          Field.FieldName + "_chk\" value=\"" + vs + "\"" + LogicString + "/>" +
                                          " <span>" + item.DisplayValue + "</span>";
                }
                else
                {
                    stringBuilder += "<input " + CommonProperties.ReadOnlyRelatedAttribute +
                                          " type=\"checkbox\" onchange=\"checkboxlistChanged(this)\" name=\"" +
                                          Field.FieldName + "_chk\" value=\"" + vs + "\"" + LogicString + "/>" +
                                          " <span>" + item.DisplayValue + "</span>";
                }

                stringBuilder += "</div>";
            }

            //flags=0;
            //for (int i = 0; i < length; i++)
            //{
            //	if (sel)
            //		Flags |= it.value;
            //}
            // todo req and checkboxlist attributes were used in form.js. I disabled them!!
            stringBuilder += "<input faName=\"" + Field.Label + "\" name=\"" + Field.FieldName + "\" " +
                                  (CommonProperties.IsRequired ? "req=true" : "") +
                                  " checkboxlist=true type=\"hidden\" value=\"" + flags +
                                  "\" />";
        }

        stringBuilder += "</div>";
        stringBuilder += "</div>";
        return stringBuilder;
    }
    
    public NeoStringBuilder RenderCheckBox(string? value)
    {
        NeoStringBuilder stringBuilder = new();
        stringBuilder.Append($"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip +
                              "\" class=\"" + ControlsClassString +
                              CommonProperties.NarrowColumnClasses +
                              CommonProperties.ShowHideRelatedClass +
                              "\">");
        RenderDesignIcons(stringBuilder);

        stringBuilder.Append("<div style=\"margin-bottom:7px;\" class=\"dWrapper\">");
        stringBuilder.Append($"<div>" + RenderBulkEditCheckbox());
        RenderLabel(stringBuilder);
        stringBuilder.Append("</div>");

        bool bValue = ConvUtill.ToBoolean(value);
        stringBuilder.Append(
            $"<input id=\"field-{Field.FieldName}\" associated-hidden-name=\"{Field.FieldName}\" type=\"checkbox\" onchange=\"checkboxChanged(this)\" "
            + CommonProperties.ReadOnlyRelatedAttribute +
            " " +
            (bValue ? "checked" : "") + " />");
        stringBuilder.Append("<input type=\"hidden\" name=\"" + Field.FieldName + "\"  " + LogicString +
                              (bValue ? " checked value=\"true\"" : "value=\"false\"") +
                              CommonProperties.ReadOnlyRelatedAttribute + " />");
        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
        return stringBuilder;
    }
    public NeoStringBuilder RenderTextInput(string value)
    {
        NeoStringBuilder stringBuilder = new();
        stringBuilder += $"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip +
                              "\" class=\"" +
                              ControlsClassString +
                              CommonProperties.NarrowColumnClasses + CommonProperties.ShowHideRelatedClass +
                              "\">";
        RenderDesignIcons(stringBuilder);
        stringBuilder += "<div class=\"dWrapper\">";
        stringBuilder += RenderBulkEditCheckbox();
        RenderLabel(stringBuilder);
        string inputType = Field.PropertyBoolean(eControlPropertyId.IsPassword) ?
            "type=\"password\" autocomplete=\"new-password\"" : "type=\"text\"";
        stringBuilder += $"<input id=\"field-{Field.FieldName}\" dir=\"" + CommonProperties.Direction + "\" " +
                              CommonProperties.ReadOnlyRelatedAttribute + " " +
                              (CommonProperties.IsRequired
                                  ? "required=\"required\" oninvalid = \"InvalidMsg(this);\" "
                                  : "") +
                              " title=\"" + CommonProperties.Tooltip + $"\" {inputType} name=\"" + Field.FieldName +
                              "\" value=\"" +
                              value + "\" class=\"form-control tField cBox \"" +
                              LogicString + " />";
        stringBuilder += "</div>";
        stringBuilder += "</div>";
        return stringBuilder;
    }
    public NeoStringBuilder RenderNumberInput(string value)
    {
        NeoStringBuilder stringBuilder = new();
        stringBuilder += $"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip +
                              "\" class=\"" + ControlsClassString +
                              CommonProperties.NarrowColumnClasses + CommonProperties.ShowHideRelatedClass +
                              "\">";
        RenderDesignIcons(stringBuilder);
        stringBuilder += "<div class=\"dWrapper\">";
        stringBuilder += RenderBulkEditCheckbox();
        RenderLabel(stringBuilder);
        stringBuilder += $"<input id=\"field-{Field.FieldName}\"" + CommonProperties.ReadOnlyRelatedAttribute + " " +
                              (CommonProperties.IsRequired
                                  ? "required=\"required\" oninvalid=\"InvalidMsg(this);\" "
                                  : "") + " title=\"" + CommonProperties.Tooltip +
                              "\" type=\"number\" step=\"any\" name=\"" +
                              Field.FieldName + "\" value=\"" + value +
                              "\" class=\"form-control digit cBox\"" +
                              LogicString + " />";
        stringBuilder += "</div>";
        stringBuilder += "</div>";
        return stringBuilder;
    }
    
    public NeoStringBuilder RenderRadioButton(string value)
    {
        NeoStringBuilder stringBuilder = new();
        stringBuilder += $"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip +
                              "\" class=\"" + ControlsClassString +
                              CommonProperties.NarrowColumnClasses + CommonProperties.ShowHideRelatedClass +
                              "\">";
        RenderDesignIcons(stringBuilder);
        stringBuilder += "<div class=\"dWrapper\">";
        stringBuilder += RenderBulkEditCheckbox();
        RenderLabel(stringBuilder);
        bool bValue1 = ConvUtill.ToBoolean(value);

        stringBuilder += "<input " + CommonProperties.ReadOnlyRelatedAttribute + " " +
                              (CommonProperties.IsRequired
                                  ? "required=\"required\" oninvalid = \"InvalidMsg(this);\" "
                                  : "") +
                              " title=\"" + CommonProperties.Tooltip + "\" type=\"radio\" name=\"" + Field.FieldName +
                              "" +
                              (bValue1 ? "checked" : "") + " class=\"form-control rButton\"" +
                              LogicString + " />";
        stringBuilder += "</div>";
        stringBuilder += "</div>";
        return stringBuilder;
    }
    
    public NeoStringBuilder RenderMultilineTextInput(string value)
    {
        NeoStringBuilder stringBuilder = new();
        stringBuilder += "<br/>";
        stringBuilder += $"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip +
                              "\" class=\"" + ControlsClassString +
                              CommonProperties.CustomColumnClasses(CommonProperties.IsDoubleWidth
                                  ? "col-12 "
                                  : "col-md-6 ") + CommonProperties.ShowHideRelatedClass +
                              "\">";
        RenderDesignIcons(stringBuilder);
        stringBuilder += "<div class=\"dWrapper\">";
        stringBuilder += RenderBulkEditCheckbox();
        RenderLabel(stringBuilder);
        stringBuilder += $"<textarea id=\"field-{Field.FieldName}\"" + CommonProperties.ReadOnlyRelatedAttribute + " " +
                              (CommonProperties.IsRequired
                                  ? "required=\"required\" oninvalid = \"InvalidMsg(this);\" "
                                  : "") +
                              " title=\"" + CommonProperties.Tooltip + "\"  name=\"" + Field.FieldName + "\" value=\"" +
                              value +
                              "\" class=\"form-control rText cBox\"" +
                              LogicString + " >" + value +
                              "</textarea>";
        stringBuilder += "</div>";
        stringBuilder += "</div>";
        return stringBuilder;
    }
    
    public NeoStringBuilder RenderTimeInput(string value)
    {
        NeoStringBuilder stringBuilder = new();
        stringBuilder += $"<div data-id=\"{Field.FieldName}\" title=\"" + CommonProperties.Tooltip +
                              "\" class=\"" +
                              ControlsClassString +
                              CommonProperties.NarrowColumnClasses + CommonProperties.ShowHideRelatedClass +
                              "\">";
        RenderDesignIcons(stringBuilder);

        stringBuilder += "<div class=\"dWrapper\">";
        stringBuilder += RenderBulkEditCheckbox();
        RenderLabel(stringBuilder);
        stringBuilder += $"<input id=\"field-{Field.FieldName}\"" + CommonProperties.ReadOnlyRelatedAttribute + " " +
                              (CommonProperties.IsRequired
                                  ? "required=\"required\" oninvalid = \"InvalidMsg(this);\" "
                                  : "") +
                              "dir=\"ltr\" data-inputmask=\"'mask': '[99][:99][:99][:99][.999]', 'greedy' : false\" title=\""
                              + CommonProperties.Tooltip +
                              $"\" placeholder=\"[{ViewTexts.Day.ToLower()}]:[{ViewTexts.Hour}]:[{ViewTexts.Minutes}]:[{ViewTexts.Seconds}].[{ViewTexts.Milliseconds}]\"" +
                              " type=\"text\" name=\"" + Field.FieldName + "\" value=\"" +
                              value + "\" class=\"form-control timeText cBox\"" +
                              LogicString + " />";
        stringBuilder += "</div>";
        stringBuilder += "</div>";
        return stringBuilder;
    }

    public NeoStringBuilder RenderMap()
    {
        NeoStringBuilder stringBuilder = new();
        RenderDesignIcons(stringBuilder);
        stringBuilder += "<BR\\><div class=\"col-md-2\" ></div>";
        stringBuilder += "<div title=\"" + CommonProperties.Tooltip + "\" name=\"" + Field.FieldName + "\" id=\"" +
                              Field.FieldName + "\" style=\"height:400px\" class=\"" +
                              CommonProperties.WideColumnClasses +
                              CommonProperties.ShowHideRelatedClass +
                              "\">";
        stringBuilder += "</div>";
        //						str += "<div id=\"" + modalMap + "\"></div>";
        return stringBuilder;
    }
    
    public NeoStringBuilder RenderSystemPageLink()
    {
        string AcquireSystemPageLink(InputFieldDefinition fi)
        {
            if (!fi.HasProperty(eControlPropertyId.SystemLinkAddress))
                return "/";
            string[] linkParamObj = fi.PropertyValue(eControlPropertyId.SystemLinkAddress)?.Split(',');
            string namespaceId = null, entityId = null, entityItemId = null;
            if (linkParamObj.Length == 2)
            {
                entityId = linkParamObj[0];
                entityItemId = linkParamObj[1];
            }
            else if (linkParamObj.Length == 3)
            {
                namespaceId = linkParamObj[0];
                entityId = linkParamObj[1];
                entityItemId = linkParamObj[2];
            }

            UiEntity uEntity = ProjectDefinition.Project.GetEntity(namespaceId, entityId) as UiEntity;
            Form form = uEntity?.getForm(entityItemId);
            string u = "";
            var urlObj = new
            {
                NamespaceId = namespaceId,
                EntityId = entityId,
                FormId = entityItemId
            };
            switch (form?.FormType)
            {
                case Form.eFormType.Create:
                    u = Utils.Url.Action("Create", "Form",
                        ControlsRendererData.Url, urlObj);
                    break;
                case Form.eFormType.Delete:
                case Form.eFormType.VirtualDelete:
                    u = Utils.Url.Action("Delete", "Form",
                        ControlsRendererData.Url, urlObj);
                    break;
                case Form.eFormType.Detail:
                    u = Utils.Url.Action("Details", "Form",
                        ControlsRendererData.Url, urlObj);
                    break;
                case Form.eFormType.ProcessCreate:
                case Form.eFormType.Edit:
                case Form.eFormType.WorkItem:
                case Form.eFormType.ActiveProcessInstance:
                case Form.eFormType.ProcessInstance:
                case Form.eFormType.ProcessCreateOnExistingRecord:
                    u = Utils.Url.Action("Edit", "Form",
                        ControlsRendererData.Url, urlObj);
                    break;
                case Form.eFormType.Index:
                    u = Utils.Url.Action("Index", "Form",
                        ControlsRendererData.Url, urlObj);
                    break;
                case Form.eFormType.Report:
                    u = Utils.Url.Action("Index", "Report",
                        ControlsRendererData.Url,
                        new
                        {
                            NamespaceId = namespaceId,
                            EntityId = entityId,
                            ReportId = entityItemId
                        });
                    break;
            }
            return u;
        }

        NeoStringBuilder stringBuilder = new();
        stringBuilder +=
            $"<div style=\"margin-top:2px;margin-bottom:3px;\" data-id=\"{Field.FieldName}\" title=\"" +
            CommonProperties.Tooltip +
            "\" class=\"" + ControlsClassString +
            CommonProperties.NarrowColumnClasses +
            CommonProperties.ShowHideRelatedClass +
            "\">";
        RenderDesignIcons(stringBuilder);

        stringBuilder += "<div class=\"dWrapper\">";
        //stringBuilder += RenderBulkEditCheckbox();
        stringBuilder += "<span> </span>";
        stringBuilder += "<button name=\"" + Field.FieldName +
                              "\" type=\"button\" dir=\"" +
                              CommonProperties.Direction + "\" " +
                              " title=\"" + CommonProperties.Tooltip + "\" systemLinkAddress=\"" +
                              AcquireSystemPageLink(Field) +
                              "\" linkParameter=\"" + Field.PropertyValue(eControlPropertyId.LinkParameter) +
                              $"\" class=\"btn {CommonProperties.ButtonStyleClass(ContextualStyle.Info)} btn-sm btn-block tField\" onclick=\"sysLink(this)\" " + LogicString +
                              ">" + Field.Label + "</button>";
        stringBuilder += "</div>";
        stringBuilder += "</div>";
        return stringBuilder;
    }
    public NeoStringBuilder RenderAdvancedUpload(ICmmnDocument cmmnFileManager, ILogger logger, IConfiguration configuration)
    {
        NeoStringBuilder stringBuilder = new();
        stringBuilder += new FileHtmlControl(cmmnFileManager, _formLogicHelper, Field, ControlsRendererData,
            logger, configuration, _sbvrRenderer).SetAnotherUploader().Render();
        ControlsRendererData.AddIncludeNeed(PluginInclude.FineUploader);
        stringBuilder +=
            $"<div class=\"{CommonProperties.WideColumnClasses}\"><div class=\"fine-uploader-element\" id=\"{Field.FieldName}\">" +
            "</div></div></div></div>";
        return stringBuilder;
    }

    public NeoStringBuilder RenderTerminal()
    {
        NeoStringBuilder stringBuilder = new();
        ControlsRendererData.AddIncludeNeed(PluginInclude.Terminal);
        stringBuilder +=
            $"<div class=\"col-12\"><div id=\"{Field.FieldName}\">" +
            "</div></div>";
        return stringBuilder;
    }
    public NeoStringBuilder RenderOperationButton()
    {
        NeoStringBuilder stringBuilder = new();
        stringBuilder.Append(
            $@"<div class=""pt-4 {CommonProperties.NarrowColumnClasses}""><button type=""button"" class=""operation-button btn {CommonProperties.ButtonStyleClass(ContextualStyle.Primary)} {ControlsClassString} 
                 {CommonProperties.ShowHideRelatedClass}""
				data-confirm-message=""{Field.PropertyValue(eControlPropertyId.ConfirmMessage)}"" 
				data-style=""zoom-in"" data-spinner-color=""black"" data-id=""{Field.FieldName}""
				id=""{Field.FieldName}"" >
                    {Field.Label}
                </button></div><div id=""modal-place""></div>");
        return stringBuilder;
    }
}
