using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class IndexTable(IFormLogicHelper formLogicHelper, 
    FormStructRoutines formStructRoutines, InputFieldDefinition field, 
    ControlsRendererData controlsRendererData, IControlsRenderer controlsRenderer, ISBVRRenderer sbvrRenderer)
    : BaseNeoHtmlControl(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    public override NeoStringBuilder Render() //todo caller, pid, wid, taskId, processId
    {
        if (ControlsRendererData.Options.IsInToolBox)
            return BlackBox();
        ElasticObject record = ControlsRendererData.Record;
        CommonFormStructure structure = ControlsRendererData.Structure;

        TableDefinition table = (TableDefinition)Field;
        NeoStringBuilder result = new();
        record.GetField(table.FieldName, out object tableData);

        string recordId = record.GetString("Ids");
        Dictionary<string, string> logics = [];
        var associationId = table.TableDef.AssociationId ?? ControlsRendererData.Structure.EntityId;
        List<ColumnFieldDefinition> columns =
            [.. table.ColumnInfos.Where(x => x.ColumnName != associationId)];
        foreach (ColumnFieldDefinition col in columns)
        {
            string logicStr = _formLogicHelper.GetLogicEvent(table.Logic, table.FieldName, col.ColumnName);
            if (string.IsNullOrEmpty(logicStr)) continue;
            string key = table.FieldName + '_' + col.ColumnName;
            logics.Add(key, logicStr);
        }

        bool isEditable = table.Editable && !ControlsRendererData.Options.IsReadOnly;
        string editable = "";
        if (isEditable)
            editable = "editable";
        string tableColInfo = columns.Aggregate("",
            (current, item) =>
                current + ((!string.IsNullOrEmpty(current) ? "|" : "") + item.ColumnName + ',' + item.FieldType));
        result.Append("<div class=\"w-100\"></div>"); // Force new line
        result.Append(
            $"<div class=\"{CommonProperties.WideColumnClasses}\">" +
            $"<div class=\"row \" data-id=\"{Field.FieldName}\">");
        result.Append("<div class=\"col-md-12\">");
        RenderDesignIcons(result);
        result.Append(TitleMessage(ControlsRendererData.Options.IsReadOnly, table));
        if (ControlsRendererData.Options.IsReadOnly)
            result.Append("<br>");
        IList<ElasticObject> tableDataRows = tableData as IList<ElasticObject> ?? [];
        result.Append(
            $@"<div class=""t-table-wrapper {(tableDataRows.Count == 0 ? "no-min-height" : "")} custom-scroll"">");

        result.Append(
            $@"<table editable='{editable}' colinfo=""{tableColInfo}"" data-original-count=""{tableDataRows.Count}"" 
                      name=""{table.FieldName}"" class=""{table.FieldName} table table-striped table-bordered"">
				    <thead>");

        if (ControlsRendererData.Options.IsReadOnly)
        {
            int colSpan = columns.Count + (table.Subjects ?? Enumerable.Empty<IndexFormSubjectId>()).Count() + 1;
            result.Append(
                $"<tr>" +
                $"<th class=\"cColumn\" colspan=\"{colSpan}\">{table.Label}</th>" +
                $"</tr>");
        }

        result.Append("<tr class=\"sticky-row\">");
        foreach (ColumnFieldDefinition col in columns)
        {
            string logicKey = table.FieldName + '_' + col.ColumnName;
            string colLogicEvent = logics.TryGetValue(logicKey, out string value) ? value : "";
            string eventString = colLogicEvent.Split('=')[0].Trim();

            string alias = col.Alias;
            result.Append($"<th class=\"cColumn\" fieldname=\"{col.ColumnName}\" ")
                  .Append($"namespaceid=\"{structure.NamespaceId}\" ")
                  .Append($"entityid=\"{structure.EntityId}\" ")
                  .Append($"eventtype=\"{eventString}\" {IndexTableHelpers.GetStyle(col, isEditable)}>")
                  .Append($"{alias}&nbsp;");
            if (isEditable && col.PropertyBoolean(eControlPropertyId.Required))
            {
                result.Append("<span style=\"color: red;\">*</span>");
            }
            if (col.FieldType == TVariableTypes.Association)
            {
                ComboBox colRenderer = new(_formLogicHelper, col, ControlsRendererData, _sbvrRenderer);
                colRenderer.RenderRelatedLinks(result, false);
            }

            result.Append("</th>");
        }

        foreach (IndexFormSubjectId sbj in table.Subjects ?? Enumerable.Empty<IndexFormSubjectId>())
        {
            result.Append($"<th class=\"cColumn sub-table-actions-column\">{sbj.Alias}</th>");
        }

        if (isEditable)
        {
            result.Append($"<th class=\"ShowHide cColumn\"></th>");
        }
        result.Append($"<th class=\"cColumn sub-table-actions-column\">{CreateLinkHtml(table, structure, recordId, isEditable)}</th></tr>")
              .Append("</thead><tbody>");
        int counter = 0;
        int maxRows = tableDataRows.Count > 0 && isEditable ? tableDataRows.Count + 1 : tableDataRows.Count;
        for (int i = 0; i < maxRows && i < 600; i++)
        {
            bool hide = false;
            ElasticObject row = new();
            string rowIds = "";
            if (i < tableDataRows.Count)
            {
                row = tableDataRows[i];
                rowIds = IndexTableHelpers.FetchRowId(table, row);
            }
            else
            { 
                hide = true; 
            }

            if (hide)
            {
                counter = -1;
            }
            result.Append($"<tr identity=\"{rowIds}\" {(hide ? "doppelganger=true class=ShowHide" : "")}>");
            //result.Append($"<tr identity=\"{rowIds}\" >");

            if (isEditable && !hide)
            {
                result.Append($"<td class=\"ShowHide\"><input type=\"hidden\" id=\"{table.FieldName + "_" + counter + "__Ids"}\" ")
                      .Append($"name=\"{table.FieldName + "[" + counter + "].__Ids"}\" value=\"{rowIds}\"/></td>");
            }
            foreach (ColumnFieldDefinition col in columns)
            {
                RenderColumn(table, col, logics, row, isEditable, counter, result);
            }
            foreach (IndexFormSubjectId sbj in table.Subjects ?? Enumerable.Empty<IndexFormSubjectId>())
                RenderSubject(table, sbj, row, rowIds, ref result);

            if (isEditable)
            {
                result.Append("<td class=\"rowStatus ShowHide\">")
                          .Append("<input type=\"hidden\" removeStatus=\"true\"")
                              .Append($" id=\"{table.FieldName + "_" + counter + "__Deleted"}\"")
                              .Append($" name=\"{table.FieldName + "[" + counter + "].__Deleted"}\"")
                              .Append(" value=\"false\"/>")
                      .Append("</td>");
            }
            counter++;
            RenderLinks(result, table, structure, recordId, rowIds, ControlsRendererData.Options.IsReadOnly, isEditable);
            result.Append("</tr>");
        }

        result.Append("</tbody></table></div></div></div></div>");
        return result;
    }

    protected override NeoStringBuilder RenderDesignIcons(NeoStringBuilder result)
    {
        base.RenderDesignIcons(result);
        if (ControlsRendererData.Options.IsDesignMode)
        {
            result.Append(
                $@"<a class=""text-decoration-none"" href=""{Url.Action("Index", "Form", ControlsRendererData.Url, 
                new { Field.NamespaceId, Field.EntityId, Field.FormId, Field.FormSubjectId })}"">
					<i class=""fa fa-link""></i>
					</a>");
        }

        return result;
    }

    private void RenderSubject(TableDefinition table, IndexFormSubjectId subjectInfo,
        ElasticObject rowData, string rowIds, ref NeoStringBuilder result)
    {
        object subjectText = "";
        bool subjectTextExists = subjectInfo.HasText && rowData.GetField(subjectInfo.Name, out subjectText);
        if (!subjectTextExists)
            subjectText = " ";
        object subjectTooltip = "";
        bool subjectTooltipExists =
            subjectInfo.HasTooltip && rowData.GetField(subjectInfo.Name + "_Tooltip", out subjectTooltip);
        if (!subjectTooltipExists)
            subjectTooltip = " ";
        result.Append($"<td class=\"cColumn\"><span title=\"{subjectTooltip}\">{subjectText}</span>&nbsp;");
        if (subjectInfo.HasDetailsForm)
        {
            result.Append(
                "<a " +
IndexTableHelpers.FormLinkClass("text-info") +
                $" href=\"{GetFormLink(subjectInfo.DetailAction, table, subjectInfo.DetailFormId, subjectInfo.Name, rowIds)}\">" +
                $"<span class=\"text-info\" title=\"{ViewTexts.Details}\">" +
                "<i class=\"fa fa-info\"></i>" +
                "</span>" +
                "</a>");
        }

        if (subjectInfo.HasEditForm)
        {
            result.Append("<a " +
IndexTableHelpers.FormLinkClass("text-success") +
                      $"href=\"{GetFormLink(subjectInfo.EditAction, table, subjectInfo.EditFormId, subjectInfo.Name, rowIds)}\">" +
                          $"<span class=\"text-success\" title=\"{ViewTexts.Edit}\">" +
                                "<i class=\"fa fa-pencil\"></i>" +
                          "</span>" +
                      "</a>");
        }

        result.Append("</td>");
    }
    private string GetFormLink(string requestedAction, TableDefinition table, string formId, string formSubjectId, string rowIds)
    {
        string action = ResolveAction(requestedAction);

        return Url.Action(action, "Form", ControlsRendererData.Url, new
        {
            table.NamespaceId,
            EntityId = table.PropertyValue(eControlPropertyId.FormEntityId) ?? table.EntityId,
            FormId = formId,
            FormSubjectId = formSubjectId,
            ids = rowIds,
            Caller="",
            isReturnable = true
        });
    }
    private string GetAction(string requestedAction) => ResolveAction(requestedAction);

    private void RenderLinks(NeoStringBuilder result, TableDefinition table, CommonFormStructure structure,
        string recordId, string rowIds, bool isInDetailsForm, bool isEditable)
    {
        result.Append("<td class=\"cColumn\">");
        if (table.HasDetailsForm)
        { 
            result.Append($"<a {IndexTableHelpers.FormLinkClass("text-info")} href=\"{Url.Action(GetAction(table.DetailAction), "Form", ControlsRendererData.Url, IndexTableHelpers.GetUrlObject(table, structure, recordId, rowIds, table.DetailFormId))}\">" +
                          $"<span class=\"text-info\" title=\"{ViewTexts.Details}\"><i class=\"fa fa-info\"></i></span></a>");
        }
        if (table.HasEditForm && !isInDetailsForm && !isEditable)
        {
            result.Append($"<a {IndexTableHelpers.FormLinkClass("text-success")} href=\"{Url.Action(GetAction(table.EditAction), "Form", ControlsRendererData.Url, IndexTableHelpers.GetUrlObject(table, structure, recordId, rowIds, table.EditFormId))}\">")
                  .Append($"<span class=\"text-success\" title=\"{ViewTexts.Edit}\">")
                  .Append("<i class=\"fa fa-pencil\"></i></span></a>");
        }

        if (table.HasDeleteForm && !isInDetailsForm)
        {
            if (isEditable)
                result.Append($"<span {IndexTableHelpers.FormLinkClass("text-danger")} style=\"cursor: pointer;\" onclick=\"tableOp.setDelColStatus(this)\" title=\"{ViewTexts.Delete}\"><i class=\"fa text-danger fa-times\"></i></span>");
            else
                result.Append($"<a {IndexTableHelpers.FormLinkClass(("text-danger"))} href=\"{Url.Action(GetAction("Delete"), "Form", ControlsRendererData.Url, IndexTableHelpers.GetUrlObject(table, structure, recordId, rowIds, table.DeleteFormId))}\">")
                      .Append($"<span style=\"cursor: pointer;\" class=\"text-danger\" title=\"{ViewTexts.Delete}\">")
                      .Append("<i class=\"fa fa-times\"></i></span></a>");
        }

        result.Append("</td>");
    }

    private string ResolveAction(string requestedAction)
    {
        string action = string.IsNullOrWhiteSpace(requestedAction) ? "Edit" : requestedAction;
        return !ControlsRendererData.Options.IsIframe
            ? action
            : action is "Edit" or "Details" or "Delete" or "Create"
            ? "IframeForm"
            : action;
    }

    private void RenderColumn(TableDefinition table, ColumnFieldDefinition col, Dictionary<string, string> logics,
        ElasticObject row,
        bool isEditable, int counter, NeoStringBuilder result)
    {
        bool isNull = false;
        string logicKey = table.FieldName + '_' + col.ColumnName;
        string colLogicEvent = logics.TryGetValue(logicKey, out string value) ? value : "";
        string colColumnName = col.ColumnName;
        if (col.FieldType == TVariableTypes.Association)
            colColumnName += "Id";
        if (!row.GetField(colColumnName, out object cellValue))
        {
            isNull = true;
            cellValue = "";
        }

        if (isEditable)
        {
            RenderEditableColumn(table, col, counter, result, isNull, cellValue, colLogicEvent, row);
        }
        else
        {
            RenderReadOnlyColumn(table, col, result, cellValue);
        }
    }

    private void RenderReadOnlyColumn(TableDefinition table, ColumnFieldDefinition col, NeoStringBuilder result,
        object cellValue)
    {
        bool cell = false;
        switch (col.FieldType)
        {
            case TVariableTypes.BOOL:
                cell = RenderReadOnlyBooleanColumn(col, ref cellValue);
                break;
            case TVariableTypes.Association:
                if (!string.IsNullOrEmpty(cellValue?.ToString()) && table.CombosData.TryGetValue(col.ColumnName, out ComboData value))
                {
                    FormDataRow r = value.GetRow(cellValue.ToString());
                    if (r != null)
                        cellValue = r.DisplayValue;
                }
                cellValue = ControlsRendererData.Encoder.Encode(cellValue?.ToString() ?? "");
                break;
            case TVariableTypes.DayHourMinute:
            case TVariableTypes.DurHourMinute:
                cellValue = FormDataRoutines.GetTimeSpanDisplayValue(cellValue);
                break;
            case TVariableTypes.Date:
                cellValue = FormDataRoutines.GetCellElementValue(cellValue, TVariableTypes.Date,
                    ControlsRendererData.Options.CalendarType);
                break;
            case TVariableTypes.File:
                cellValue = ControlsRendererData.Encoder.Encode(cellValue?.ToString() ?? "");
                break;
            default:
                cellValue = ControlsRendererData.Encoder.Encode(cellValue?.ToString() ?? "");
                break;
        }

        result.Append("<td class=\"cColumn\">" +
                      (!cell && col.FieldType == TVariableTypes.BOOL
                          ? "<input type=\"checkbox\" disabled=\"disabled\"/>"
                          : cellValue) + "</td>");
    }

    private bool RenderReadOnlyBooleanColumn(ColumnFieldDefinition col,
        ref object cellValue)
    {
        bool cell;
        if (col.ControlType == eControlTypeId.BooleanCombo)
        {
            BooleanItem valueItem = BooleanEntityField.GetValueItem(false, cellValue);
            switch (valueItem)
            {
                case BooleanItem.True:
                    cellValue = (object)col.Property(eControlPropertyId.TrueTitle)
                                ?? TableHelper.CreateCellElement(true, col, false, ControlsRendererData.Options.CalendarType);
                    cell = true;
                    break;
                case BooleanItem.False:
                    cellValue = col.Property(eControlPropertyId.FalseTitle) ?? "";
                    cell = true;
                    break;
                case BooleanItem.Null:
                    cellValue = col.Property(eControlPropertyId.NullTitle) ?? "-";
                    cell = true;
                    break;
                default:
                    cellValue = "";
                    cell = false;
                    break;
            }
        }
        else
        {
            try
            {
                cell = ConvUtill.ToBoolean(cellValue);
            }
            catch (Exception)
            {
                cell = false;
            }

            if (cell)
            {
                cellValue = TableHelper.CreateCellElement(cellValue ?? false, col,
                    false, ControlsRendererData.Options.CalendarType);
            }
        }

        return cell;
    }

    private void RenderEditableColumn(TableDefinition table, ColumnFieldDefinition col, int counter,
        NeoStringBuilder result, bool isNull, object cellValue, string colLogicEvent, ElasticObject record)
    {
        string value = "";
        CommonProperties commonProperties = new(col.GetProperties());

        if (isNull)
            value = commonProperties.DefaultValue;
        if (!string.IsNullOrEmpty(value))
            cellValue = value;

        string rowColName = table.FieldName + "[" + counter + "]." + col.ColumnName;
        string fullColumnName = table.FieldName + "_" + col.ColumnName;
        string rowColId = table.FieldName + "_" + counter + "__" + col.ColumnName;
        result.Append("<td class=\"cColumn\" style=\"opacity: 0.9\">");
        RenderCellControl(table, col, result, cellValue, colLogicEvent, commonProperties, rowColId, fullColumnName,
            rowColName, record);
        result.Append("</td>");
    }

    private ControlsRendererData _controlsRendererData;

    private void RenderCellControl(TableDefinition table, ColumnFieldDefinition col, NeoStringBuilder result,
        object cellValue, string colLogicEvent, CommonProperties commonProperties, string rowColId,
        string fullColumnName,
        string rowColName, ElasticObject record)
    {
        void RenderSelectBox()
        {
            bool isRemoteData = col.GetProperty(eControlPropertyId.RemoteData)?.GetValueAsBoolean() ?? false;
            result.Append(
                    $"<select {commonProperties.ReadOnlyRelatedAttribute} {(commonProperties.IsRequired ? "required oninvalid=\"InvalidMsg(this);\"" : "")}")
                .Append(
                    $" data-namespace=\"{table.NamespaceId}\" data-entity=\"{table.EntityId}\" data-form=\"{table.FormId}\" data-column=\"{col.ColumnName}\" ")
                .Append(
                    $" dir=\"{commonProperties.Direction}\" title=\"{commonProperties.Tooltip}\" class=\"{commonProperties.ShowHideRelatedClass}\" ")
                .Append(isRemoteData
                    ? $"data-isremote=\"true\" initValue=\"{cellValue}\" filter-formula=\"{col.PropertyValue(eControlPropertyId.FilterFormula)}\""
                    : "")
                .Append($" full-column-name=\"{fullColumnName}\" ")
                .Append(
                    $" id=\"{rowColId}\" name=\"{rowColName}\" {Combo.GetCustomRemoteDataUrl(col, ControlsRendererData.Url)} {colLogicEvent}>");
            List<FormDataRow> list = [];
            if (table.CombosData.TryGetValue(col.ColumnName, out ComboData value))
            {
                list.AddRange(value.Rows);
            }

            list.AddRange(Choice.PropertyOptions(col));
            foreach (FormDataRow item in list)
            {
                result.Append("<option " + (item.Ids == cellValue?.ToString() ? "selected=\"selected\" " : "") +
                              $" value=\"{ControlsRendererData.Encoder.Encode(item.Ids ?? "")}\">{ControlsRendererData.Encoder.Encode(item.DisplayValue ?? "")}</option>");
            }

            result.Append("</select>");
        }

        switch (col.FieldType)
        {
            case TVariableTypes.DayHourMinute:
            case TVariableTypes.DurHourMinute:
                if (!string.IsNullOrEmpty(cellValue?.ToString()?.Trim()))
                    cellValue = IndexTableHelpers.GetTimeSpanValue(cellValue);
                result.Append(
                        $"<input {commonProperties.ReadOnlyRelatedAttribute} {(commonProperties.IsRequired ? "required oninvalid =InvalidMsg(this);" : "")} ")
                    .Append("dir =\"ltr\" data-inputmask=\"'mask': '[99][:99][:99][:99][.999]', 'greedy' : false\" ")
                    .Append("placeholder=\"[روز]:[ساعت]:[دقیقه]:[ثانیه].[میلی ثانیه]\"")
                    .Append($" class=\"form-control {commonProperties.ShowHideRelatedClass} timeText \" ")
                    .Append($" type=\"text\" id=\"{rowColId}\"")
                    .Append($" full-column-name=\"{fullColumnName}\" ")
                    .Append($" name=\"{rowColName}\" ")
                    .Append($" value=\"{cellValue}\" {colLogicEvent} />");
                ControlsRendererData.AddIncludeNeed(PluginInclude.InputMask);
                break;
            case TVariableTypes.Association:
                RenderSelectBox();
                break;
            case TVariableTypes.Date:
            case TVariableTypes.DateStr:
            case TVariableTypes.DateTime:
                _ = $@"<input {commonProperties.ReadOnlyRelatedAttribute} {(commonProperties.IsRequired ? "required oninvalid=\"InvalidMsg(this);\"" : "")}
						 dir=""{commonProperties.Direction}"" title=""{commonProperties.Tooltip}""
					     class=""form-control dateField {commonProperties.ShowHideRelatedClass} {ControlsRendererData.Options.CalendarType}"" type=""text"" autocomplete=""off""
						 full-column-name=""{fullColumnName}"" associated-hidden-name=""{rowColName}""
						 id=""field-{rowColId}"" value=""{(cellValue as DateTime?)?.ToIso8601()}"" {colLogicEvent}/>
                        <input type=""hidden"" id=""{rowColId}"" name=""{rowColName}""  
                             {commonProperties.ReadOnlyRelatedAttribute} value=""{(cellValue as DateTime?)?.ToHtmlInputValue("miladi")}"" />
                    ";
                break;
            case TVariableTypes.BOOL:
                bool bValue = ConvUtill.ToBoolean(cellValue);
                result.Append(
                        $"<input {commonProperties.ReadOnlyRelatedAttribute} class=\"{commonProperties.ShowHideRelatedClass}\" ")
                    .Append($" onchange=\"checkboxChanged(this)\" associated-hidden-name=\"{rowColName}\"")
                    .Append(
                        $" type=\"checkbox\" {(bValue ? "checked" : "")} value=\"{(bValue ? "true" : "false")}\" {colLogicEvent}/>")
                    .Append(
                        $"<input type=\"hidden\" id=\"{rowColId}\" full-column-name=\"{fullColumnName}\" name=\"{rowColName}\" value=\"{(bValue ? "true" : "false")}\"/>");
                break;
            case TVariableTypes.Double:
                result.Append(
                        $"<input {commonProperties.ReadOnlyRelatedAttribute} {(commonProperties.IsRequired ? "required oninvalid =InvalidMsg(this);" : "")} ")
                    .Append($" class=\"form-control {commonProperties.ShowHideRelatedClass} \" ")
                    .Append(
                        $" dir=\"{commonProperties.Direction}\" type=\"{IndexTableHelpers.GetInputType(col)}\" step=\"any\" id=\"{rowColId}\"")
                    .Append($" name=\"{rowColName}\" ")
                    .Append($" full-column-name=\"{fullColumnName}\" ")
                    .Append($" value=\"{ControlsRendererData.Encoder.Encode(cellValue?.ToString() ?? "")}\" {colLogicEvent} />");
                break;
            default:
                // Check if this is an enum field that should render as ComboBox
                bool isEnumField = col.FieldType == TVariableTypes.StringListItem || 
                                 col.FieldType == TVariableTypes.StringListBitMask ||
                                 (table.CombosData.ContainsKey(col.ColumnName) && 
                                 table.CombosData[col.ColumnName].Rows.Count != 0);
                
                switch (col.ControlType)
                {
                    case eControlTypeId.None:
                        if (isEnumField)
                        {
                            RenderSelectBox();
                        }
                        else
                        {
                            result.Append(
                                    $"<input {commonProperties.ReadOnlyRelatedAttribute} {(commonProperties.IsRequired ? "required oninvalid =InvalidMsg(this);" : "")} ")
                                .Append($" class=\"form-control {commonProperties.ShowHideRelatedClass} \" ")
                                .Append($" dir=\"{commonProperties.Direction}\" type=\"{IndexTableHelpers.GetInputType(col)}\" id=\"{rowColId}\"")
                                .Append($" name=\"{rowColName}\" ")
                                .Append($" full-column-name=\"{fullColumnName}\" ")
                                .Append($" value=\"{ControlsRendererData.Encoder.Encode(cellValue?.ToString() ?? "")}\" {colLogicEvent} />");
                        }
                        break;
                    case eControlTypeId.ComboBox:
                        RenderSelectBox();
                        break;
                    default:
                        InitializeRenderer(table, record);
                        _ = controlsRenderer.CreateControl(_controlsRendererData, null, col);
                        break;
                }

                break;
        }
    }

    private void InitializeRenderer(TableDefinition table, ElasticObject record)
    {
        _controlsRendererData ??= new ControlsRendererData(formStructRoutines.GetCommonFormStructure(
                CultureHelper.GetCurrentNeutralCulture(), table.NamespaceId, table.EntityId,
                table.FormId, Form.eFormType.Edit, table.FormSubjectId,
                FormStructRoutines.GetForm(table.NamespaceId, table.EntityId, table.FormId,
                    Form.eFormType.Index, table.FormSubjectId), ControlsRendererData.User), record,
            new RendererOptions(), ControlsRendererData.User, ControlsRendererData.Url);
    }

    private string TitleMessage(bool isInDetailsForm, TableDefinition table)
    {
        if (ControlsRendererData.Options.IsIframe || isInDetailsForm || !string.IsNullOrEmpty(table.parentControlId))
            return "";

        // حفظ ساختار فعلی برای سازگاری CSS/JS، فقط بهبود بصری و امن‌سازی متن
        string title = ControlsRendererData.Encoder.Encode(table.Label ?? "");
        return $"<div class=\"col-lg-12 col-sm-12 tableTitle\">" +
               $"<i onclick=\"tableOp.toggle(this, '{table.FieldName}')\" class=\"toggle fa fa-minus\" aria-label=\"باز/بسته کردن جدول\"></i>" +
               $"<span class=\"tableTitle-label\" style=\"font-weight:600;color:#1f2937;background:transparent;\">{title}</span>" +
               $"</div>";
    }

    private string CreateLinkHtml(TableDefinition table, CommonFormStructure structure, string recordId, bool isEditable)
    {
        string title = (CultureHelper.GetCurrentNeutralCulture() == "en" ? "Add New Record To " : "افزودن سطر جدید به ")+ table.Label;
        if (isEditable)
        {
            return $"<span style=\"cursor: pointer;\" tablename=\"{table.FieldName}\" onclick=\"tableOp.addNewRow(this)\" class=\"text-success\" title=\"{title}\">" +
                        "<i class=\"fa fa-plus\" style=\"font-weight: bold; font-size: 16px;\"></i>" +
                   "</span>";
        }
        var newUrlObject =
            new
            {
                table.NamespaceId,
                EntityId = table.PropertyValue(eControlPropertyId.FormEntityId) ?? table.EntityId,
                table.FormSubjectId,
                FormId = table.CreateFormId,
                workItemFormId = structure.Form_ReportId,
                pFormId = structure.Form_ReportId,
                __parentNamespaceId = structure.NamespaceId,
                __parentEntityId = structure.EntityId,
                __parentFormSubjectId = structure.FormSubjectId,
                __subTableAssociationFieldId = table.TableDef.TableAssociation?.Id,
                __parentIds = recordId,
                isReturnable = true
            };
        string createUrl = Url.Action(GetAction("Create"), "Form", ControlsRendererData.Url, newUrlObject);
        const string addIconSvg =
            "<svg style=\"width:24px;height:24px;\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">" +
                "<line x1=\"12\" y1=\"5\" x2=\"12\" y2=\"19\" />" +
                "<line x1=\"5\" y1=\"12\" x2=\"19\" y2=\"12\" />" +
            "</svg>";
        return $"<a class=\"index-table-add-btn\" href=\"{createUrl}\" title=\"{title}\">{addIconSvg}</a>";
    }
}
