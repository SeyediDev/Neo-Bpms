using System.Text.RegularExpressions;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Features.FormLogic;
public class FormLogicHelper(ILogger<FormLogicHelper> logger): IFormLogicHelper
{
    private static readonly string NewLine = "\r\n";
    private static readonly string NewLine1T = $"{NewLine}\t";
    private static readonly string NewLine2T = $"{NewLine1T}\t";
    private static readonly string NewLine3T = $"{NewLine2T}\t";
    private static readonly string NewLine4T = $"{NewLine3T}\t";

    /// <summary>
    /// Gets the events for each field based on logic(UI rule definitions).
    /// </summary>
    /// <returns></returns>		
    public HtmlString HtmlLogicEvent(CommonFormStructure structure, string scope, string fieldName)
    {
        return new HtmlString(GetLogicEvent(structure, scope, fieldName));
    }

    public HtmlString HtmlLogicController(CommonFormStructure structure, string eventType)
    {
        return new HtmlString(GetLogicController(structure, eventType));
    }

    public string GetLogicEvent(CommonFormStructure structure, string scope, string fieldName)
    {
        NeoStringBuilder sb = new();
        if (string.IsNullOrEmpty(scope))
        {
            _ = sb.Append(GetLogicEvent(structure.Logic, scope, fieldName));
        }

        string tableName = scope?.Split(';')[0];
        foreach (TableDefinition tbl in structure.Tables)
        {
            if (tbl.FieldName == tableName || string.IsNullOrEmpty(scope) && string.IsNullOrEmpty(fieldName))
            {
                _ = sb.Append(GetLogicEvent(tbl.Logic, scope, fieldName));
            }
        }

        return sb.ToString();
    }

    public ServerOperationResult GetEventServerOperations(string namespaceId, string entityId, string formId,
        string source, string eventName, IdentityUser user, List<QField> qs, List<LCField> lCs, string culture)
    {
        ServerOperationResult result = new();
        if (ProjectDefinition.Project.GetEntity(namespaceId, entityId) is not UiEntity uiEntity)
        {
            return result;
        }

        Form form = uiEntity.getForm(formId)
                      ?? uiEntity.GetReport(formId)
                      ?? (Form)uiEntity.GetDashboard(formId);
        if (form.UiRules != null)
        {
            foreach (UIRule rule in form.UiRules)
            {
                if (AllOperationsAreClientSide(rule))
                {
                    continue;
                }

                if (CheckEvent(source, eventName, rule))
                {
                    foreach (UIRuleTask op in rule.Operations)
                    {
                        AddOperationToLogic(namespaceId, entityId, user, qs, lCs, result, uiEntity,
                            rule.Name, op, form.formFields, culture);
                    }
                }
            }
        }

        return result;
    }

    public object ServerEval(string formula, LocalParameters el)
    {
        try
        {
            ExpressionNode expression = Parser.Parse(formula);
            //		        expression = expression.replace(el);
            return expression.Eval(null, el);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }

        return null;
    }

    public void AddFormLogic(CommonFormStructure structure, List<UIRule> controllers, string tableName)
    {
        if (controllers == null)
        {
            return;
        }

        foreach (UIRule ctrl in controllers)
        {
            if (!string.IsNullOrEmpty(tableName) && ctrl.PagePart != PagePart.IndexList)
            {
                continue;
            }

            foreach (UIRuleEvent ev in ctrl.Events)
            {
                FormLogicDefinitionItem rule = new()
                {
                    TriggeringEvent = new FormLogicEventDefinition
                    {
                        EventType = ev.type,
                        Source = ev.exporterFieldId
                    },
                    RepeatOnTable = ev.RepeatScopeId ?? tableName
                };
                if (AllOperationsAreClientSide(ctrl))
                {
                    foreach (UIRuleTask op in ctrl.Operations)
                    {
                        FormLogicOperationDefinition rop = new(op, this);
                        switch (rop.Type)
                        {
                            case eOperationType.SetValue:
                                rop.TargetId = op.assignToFieldId;
                                rop.Value = ConvertToClientCode(op.formulaStr);
                                break;
                            case eOperationType.SetLocalParameterValue:
                                rop.TargetId = "lc_" + op.localParamId;
                                rop.Value = ConvertToClientCode(op.formulaStr);
                                break;
                            case eOperationType.ToggleClass:
                            case eOperationType.SetClass:
                            case eOperationType.RemoveClass:
                            case eOperationType.ChangeCSSAttribute:
                                if (rop.Type == eOperationType.ChangeCSSAttribute)
                                {
                                    rop.Type = ChangeOperationTypeByAttribute(op.SpecificAttribute, true);
                                }

                                if (string.IsNullOrEmpty(op.assignToFieldId))
                                {
                                    foreach (UIRuleTask.TaskParam prm in op.Params ?? Enumerable.Empty<UIRuleTask.TaskParam>())
                                    {
                                        FormLogicOperationDefinition rop1 = new(op, this);
                                        if (rop1.Type == eOperationType.ChangeCSSAttribute)
                                        {
                                            rop1.Type = ChangeOperationTypeByAttribute(op.SpecificAttribute,
                                                true);
                                        }

                                        rop1.Attr = op.SpecificAttribute.ToString();
                                        rop1.TargetId = prm.field_controlId;
                                        eControlPropertyTarget part = op.ControlPart;
                                        if (op.SpecificAttribute == eControlPropertyId.ShowHide)
                                        {
                                            part = eControlPropertyTarget.Control;
                                        }

                                        rop1.TargetArea = part switch
                                        {
                                            eControlPropertyTarget.Content => eTargetArea.Input,
                                            eControlPropertyTarget.Label => eTargetArea.Label,
                                            //case eControlPropertyTarget.Control:
                                            //	break;
                                            _ => eTargetArea.Container,
                                        };

                                        //if (rop.Type == eOperationType.ChangeCSSAttribute)
                                        rop1.Value = ConvertToClientCode(op.formulaStr);
                                        rule.Operations.Add(rop1);
                                    }

                                    rop = null;
                                }
                                else
                                {
                                    rop.Attr = op.SpecificAttribute.ToString();
                                    rop.TargetId = op.assignToFieldId;
                                    rop.TargetArea = op.ControlPart switch
                                    {
                                        eControlPropertyTarget.Content => eTargetArea.Input,
                                        eControlPropertyTarget.Label => eTargetArea.Label,
                                        //case eControlPropertyTarget.Control:
                                        //	break;
                                        _ => eTargetArea.Container,
                                    };

                                    //if (rop.Type == eOperationType.ChangeCSSAttribute)
                                    rop.Value = ConvertToClientCode(op.formulaStr);
                                }

                                break;
                            case eOperationType.ShowWarning:
                                //todo: op.exceptionInfo???????????

                                //if( culture == "fa")
                                rop.Value = string.IsNullOrEmpty(op.Validation_ConstraintWarningText)
                                    ? op.Validation_WarningText
                                    : op.Validation_ConstraintWarningText;
                                //else
                                //rop.Value = string.IsNullOrEmpty(op.Validation_EnConstraintWarningText)?op.Validation_EnWarningText:op.Validation_EnConstraintWarningText;
                                break;
                            case eOperationType.ShowError:
                                //todo: op.exceptionInfo???????????
                                //if( culture == "fa")
                                rop.Value = string.IsNullOrEmpty(op.Validation_ConstraintWarningText)
                                    ? op.Validation_WarningText
                                    : op.Validation_ConstraintWarningText;
                                //else
                                //rop.Value = string.IsNullOrEmpty(op.Validation_EnConstraintWarningText)?op.Validation_EnWarningText:op.Validation_EnConstraintWarningText;
                                if (rop.Value == null && op.exceptionInfo != null)
                                {
                                    rop.Value = string.IsNullOrEmpty(op.exceptionInfo.ErrorFormula)
                                        ? op.exceptionInfo.ErrorText
                                        : op.exceptionInfo.ErrorFormula;
                                }

                                rop.Condition = op.formulaStr;
                                rop.Loop_ConditionType = UIRuleTask.eControlType.IfCondition;
                                rop.Name = op.exceptionInfo?.Name;
                                break;
                            case eOperationType.ShowPrompt:

                                //if( culture == "fa")
                                rop.Value = string.IsNullOrEmpty(op.Validation_ConstraintWarningText)
                                    ? op.Validation_WarningText
                                    : op.Validation_ConstraintWarningText;
                                //else
                                //rop.Value = string.IsNullOrEmpty(op.Validation_EnConstraintWarningText)?op.Validation_EnWarningText:op.Validation_EnConstraintWarningText;
                                break;
                            case eOperationType.ShowModal:
                                rop.Value = "/Form/" + "Form?NamespaceId=\"" + op.modelId + "\"&EntityId=\"" +
                                                op.entityId + "\"&FormId=\"" + op.form_rerportId + "\"";
                                break;
                            case eOperationType.ShowWindow:
                                rop.Value = "/Form/" + "Form?NamespaceId=\"" + op.modelId + "\"&EntityId=\"" +
                                                op.entityId + "\"&FormId=\"" + op.form_rerportId + "\"";
                                break;
                            case eOperationType.ShowAutoHideWindow:
                                rop.Value = "/Form/" + "Form?NamespaceId=\"" + op.modelId + "\"&EntityId=\"" +
                                                op.entityId + "\"&FormId=\"" + op.form_rerportId + "\"";
                                break;
                            case eOperationType.CallController:
                                rop.Value = op.controlerId;
                                break;
                            case eOperationType.AddRow:
                                break;
                            case eOperationType.ChangeList:
                                //todo: recalc the combos data query with the new filter
                                break;
                            case eOperationType.ServerOperation:
                                if (op.TaskType == UIRuleTask.eTaskType.OneRowQuery)
                                {
                                    //todo: run the query operation and write the results (SetValue, SetLocalParameterValue)
                                }
                                else if (op.TaskType == UIRuleTask.eTaskType.CallDataOperation)
                                {
                                    //todo: call data operation and write the results (SetValue, SetLocalParameterValue)
                                }

                                break;
                        }

                        if (rop != null)
                        {
                            rule.Operations.Add(rop);
                        }
                    }
                }
                else
                {
                    FormLogicOperationDefinition rop = new()
                    {
                        Name = ctrl.Name,
                        Condition = "",
                        Loop_ConditionType = UIRuleTask.eControlType.NoCondition,
                        Type = eOperationType.ServerOperation
                    };
                    foreach (UIRuleTask op in ctrl.Operations)
                    {
                        rop.AddFormula(op);
                    }

                    rule.Operations.Add(rop);
                }

                structure.Logic.Logics.Add(rule);
            }
        }
    }

    public string GetLogicEvent(FormLogicDefinition logicDefinition, string scope, string fieldName)
    {
        string s = "";
        Dictionary<UIRuleEvent.eEventType, UIRuleEvent.eEventType> events = [];
        foreach (FormLogicDefinitionItem logic in logicDefinition.Logics)
        {
            if (events.ContainsKey(logic.TriggeringEvent.EventType))
            {
                continue;
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                if (!string.IsNullOrEmpty(logic.TriggeringEvent.Source))
                {
                    continue;
                }
            }
            else
            {
                if (logic.TriggeringEvent.Source != fieldName)
                {
                    continue;
                }
            }

            events.Add(logic.TriggeringEvent.EventType, logic.TriggeringEvent.EventType);
            switch (logic.TriggeringEvent.EventType) //todo: complete the following with html-js functions
            {
                case UIRuleEvent.eEventType.invalid:
                    break;
                case UIRuleEvent.eEventType.onPageLoad:
                    s += " onload=\"pageLoaded(this)\"";
                    break;
                case UIRuleEvent.eEventType.onPageUnLoad:
                    break;
                case UIRuleEvent.eEventType.onPageScroll:
                    break;
                case UIRuleEvent.eEventType.onPageResize:
                    break;
                case UIRuleEvent.eEventType.onSubmitForm:
                    break;
                case UIRuleEvent.eEventType.onClick:
                    s += " onclick=\"inputClicked(this, \'" + logic.TriggeringEvent.Source + "\', \'" + scope +
                          "\')\"";
                    break;
                case UIRuleEvent.eEventType.onDblClick:
                    break;
                case UIRuleEvent.eEventType.onUserChange:
                    s += " onchange=\"inputChanged(this, \'" + logic.TriggeringEvent.Source + "\', \'" + scope +
                          "\')\"";
                    break;
                case UIRuleEvent.eEventType.onFocus:
                    break;
                case UIRuleEvent.eEventType.onBlur:
                    break;
                case UIRuleEvent.eEventType.onLoad:
                    break;
                case UIRuleEvent.eEventType.onChange:
                    break;
                case UIRuleEvent.eEventType.onKeyDown:
                    break;
                case UIRuleEvent.eEventType.onKeyPress:
                    break;
                case UIRuleEvent.eEventType.onKeyUp:
                    break;
                case UIRuleEvent.eEventType.onMouseDown:
                    break;
                case UIRuleEvent.eEventType.onMouseUp:
                    break;
                case UIRuleEvent.eEventType.onMouseMove:
                    break;
                case UIRuleEvent.eEventType.onMouseOut:
                    break;
                case UIRuleEvent.eEventType.onMouseOver:
                    break;
                case UIRuleEvent.eEventType.onDrag:
                    break;
                case UIRuleEvent.eEventType.onDrop:
                    break;
                case UIRuleEvent.eEventType.onTableRowChange:
                    break;
                case UIRuleEvent.eEventType.onTableRowAdd:
                    break;
                case UIRuleEvent.eEventType.onTableRowDelete:
                    break;
            }
        }

        return s;
    }

    private UIRuleEvent.eEventType GetEventType(string eventType, out bool bHasSource)
    {
        bHasSource = true;
        switch (eventType)
        {
            case "onPageLoad":
                bHasSource = false;
                return UIRuleEvent.eEventType.onPageLoad;
            case "onPageUnLoad":
                bHasSource = false;
                return UIRuleEvent.eEventType.onPageUnLoad;
            case "onPageScroll":
                bHasSource = false;
                return UIRuleEvent.eEventType.onPageScroll;
            case "onPageResize":
                bHasSource = false;
                return UIRuleEvent.eEventType.onPageResize;
            case "onSubmitForm":
                bHasSource = false;
                return UIRuleEvent.eEventType.onSubmitForm;
            case "onClick":
                return UIRuleEvent.eEventType.onClick;
            case "onDblClick":
                return UIRuleEvent.eEventType.onDblClick;
            case "onUserChange":
                return UIRuleEvent.eEventType.onUserChange;
            case "onFocus":
                return UIRuleEvent.eEventType.onFocus;
            case "onBlur":
                return UIRuleEvent.eEventType.onBlur;
            case "onLoad":
                return UIRuleEvent.eEventType.onLoad;
            case "onChange":
                return UIRuleEvent.eEventType.onChange;
            case "onKeyDown":
                return UIRuleEvent.eEventType.onKeyDown;
            case "onKeyPress":
                return UIRuleEvent.eEventType.onKeyPress;
            case "onKeyUp":
                return UIRuleEvent.eEventType.onKeyUp;
            case "onMouseDown":
                return UIRuleEvent.eEventType.onMouseDown;
            case "onMouseUp":
                return UIRuleEvent.eEventType.onMouseUp;
            case "onMouseMove":
                return UIRuleEvent.eEventType.onMouseMove;
            case "onMouseOut":
                return UIRuleEvent.eEventType.onMouseOut;
            case "onMouseOver":
                return UIRuleEvent.eEventType.onMouseOver;
            case "onDrag":
                return UIRuleEvent.eEventType.onDrag;
            case "onDrop":
                return UIRuleEvent.eEventType.onDrop;
            case "onTableRowChange":
                return UIRuleEvent.eEventType.onTableRowChange;
            case "onTableRowAdd":
                return UIRuleEvent.eEventType.onTableRowAdd;
            case "onTableRowDelete":
                return UIRuleEvent.eEventType.onTableRowDelete;
        }

        return UIRuleEvent.eEventType.invalid;
    }

    private string GetJQuerySetValueString(string fieldName, string val)
    {
        return "Client_SetVal(This,scope,\"" + fieldName + "\",(" + val + "));";
    }

    private string GetJQuerySetLcString(string lc, string val)
    {
        return "Client_SetLC(This,scope,\"" + lc + "\",(" + val + "));";
    }

    private string GetJQuerySetCssAttrString(string fieldName, eTargetArea eTargetArea, string attr,
        string val)
    {
        return "Client_ChangeCSSAttr(This,scope,\"" + fieldName + "\",\"" + attr + "\",\"" + eTargetArea +
                 "\",(" + val + "));";
    }

    private string GetJQueryRemoveClassString(string fieldName, eTargetArea eTargetArea, string className,
        string val)
    {
        return "Client_RemoveClass(This,scope,\"" + fieldName + "\",\"" + className + "\",\"" +
                 eTargetArea + "\",(" + val + "));";
    }

    private string GetJQuerySetClassString(string fieldName, eTargetArea eTargetArea, string className,
        string val)
    {
        return "Client_AddClass(This,scope,\"" + fieldName + "\",\"" + className + "\",\"" + eTargetArea +
                 "\",(" + val + "));";
    }

    private string GetJQueryToggleClassString(string fieldName, eTargetArea eTargetArea, string className,
        string val)
    {
        return "Client_ToggleClass(This,scope,\"" + fieldName + "\",\"" + className + "\",\"" +
                 eTargetArea + "\",(" + val + "));";
    }

    private string JQueryPreProcessFormula(string formula)
    {
        return formula;
        //			return "preProcessFormula(\'" + formula + "\')";
        //if (!formula) return "";
        //var patt = /q\[[0-9]*\]|qm\[[0-9]*\]/ig;
        //var result = formula.match(patt);
        //if (result instanceof Array) {
        //	for (var i in result) {
        //		var pId = result[i].replace("q[", "").replace("qm[", "").replace("]", "");
        //		var s = getFieldValue(pId);
        //		if (!s) {
        //			s = "0";
        //		}
        //		if (isNaN(s)) {
        //			s = "'" + s + "'";
        //		}
        //		formula = formula.replace(result[i], s ? s : "0");
        //	}
        //}
        //else if (result) {
        //	var pId = result.replace("q[", "").replace("qm[", "").replace("]", "");
        //	var s = getFieldValue(pId);
        //	if (!s) {
        //		s = "0";
        //	}
        //	formula = formula.replace(result, s);
        //}
        //patt = /lc\[[0-9]*\]/ig;
        //result = formula.match(patt);
        //if (result instanceof Array) {
        //	for (var i in result) {
        //		var lcId = result[i].replace("lc[", "").replace("]", "");
        //		var s = getLCValue(lcId);
        //		if (!s)
        //			s = "0";
        //		formula = formula.replace(result[i], s)
        //	}
        //}
        //else if (result) {
        //	var lcId = result.replace("lc[", "").replace("]", "");
        //	var s = this.getLCValue(lcId);
        //	if (!s)
        //		s = "0";
        //	formula = formula.replace(result, s);
        //}
        //{
        //	var re = /`/ig;
        //	formula = String(formula).replace(re, "'");
        //	if (1 == 1)//is better find the condition for check this block
        //	{
        //		re = /;/ig;
        //		formula = String(formula).replace(re, ",");
        //	}
        //	formula = String(formula).replace(/=/g, "==");
        //	formula = String(formula).replace(/>==/g, ">=");
        //	formula = String(formula).replace(/<==/g, "<=");
        //	formula = String(formula).replace(/!==/g, "!=");

        //	formula = String(formula).replace(/and/ig, " && ");
        //	formula = String(formula).replace(/or/ig, " || ");
        //	formula = String(formula).replace(/Number/ig, "Number");
        //	formula = String(formula).replace(/equal/ig, "FormFunctions.equal");
        //	//formula = String(formula).replace(/SUM/ig, "SUM");
        //	//formula = String(formula).replace(/SUBTOTAL/ig, "SUBTOTAL");
        //	//formula = String(formula).replace(/COUNT/ig, "COUNT");
        //	//formula = String(formula).replace(/EQUAL/ig, "EQUAL");
        //	//formula = String(formula).replace(/\[\]/ig, "\"\"");
        //	//if (formula.indexOf("p[") == -1) {
        //	//    formula = String(formula).replace(/\[/ig, "\"");
        //	//    formula = String(formula).replace(/\]/ig, "\"");
        //	//}
        //}
        ////while (true) {
        ////    var idx = formula.indexOf("Clientfuncs_");
        ////    if (idx < 0) break;
        ////    if (formula.substr(idx, 7) == "GetIndex") {
        ////        formula = formula.replace("Clientfuncs_GetIndex()", "cf_GetIndex(scopeId,tIndex,form)");
        ////    }
        ////todo: replace q[], qm[] with getJQueryValueString(string fieldName)
        ////lc will be valid
    }

    public string GetLogicController(CommonFormStructure structure, string eventType)
    {
        UIRuleEvent.eEventType evtyp = GetEventType(eventType, out bool bHasSource);
        StringBuilder sb = new();
        if (evtyp != UIRuleEvent.eEventType.onPageLoad)
        {
            _ = sb.Append(NewLine1T + "if(scope==null || scope==''){");
        }
        else
        {
            _ = sb.Append(NewLine1T + "var scope=null;");
        }

        GetLogicControlerOfLogics(structure.Logic, structure.NamespaceId, structure.EntityId,
            structure.Form_ReportId,
            eventType, bHasSource, evtyp, sb);
        if (evtyp != UIRuleEvent.eEventType.onPageLoad)
        {
            _ = sb.Append(NewLine1T + "}");
        }

        if (structure.Tables != null && structure.Tables.Count > 0)
        {
            if (evtyp != UIRuleEvent.eEventType.onPageLoad)
            {
                _ = sb.Append(NewLine1T + "else {");
                _ = sb.Append($"{NewLine2T}var tableName = scope.split(';')[0];");
                _ = sb.Append($"{NewLine2T}switch(tableName){NewLine3T}" + "{");
            }

            foreach (TableDefinition tbl in structure.Tables)
            {
                if (evtyp != UIRuleEvent.eEventType.onPageLoad)
                {
                    _ = sb.Append($"{NewLine2T}case '" + tbl.FieldName + "':");
                }
                else
                {
                    _ = sb.Append(NewLine1T + "scope='" + tbl.FieldName + "';");
                }

                GetLogicControlerOfLogics(tbl.Logic, tbl.NamespaceId, tbl.EntityId, tbl.FormId,
                    eventType, bHasSource, evtyp, sb);
                if (evtyp != UIRuleEvent.eEventType.onPageLoad)
                {
                    _ = sb.Append($"{NewLine2T}break;");
                }
            }

            if (evtyp != UIRuleEvent.eEventType.onPageLoad)
            {
                _ = sb.Append(NewLine3T + "}");
                _ = sb.Append(NewLine1T + "}");
            }
        }

        _ = sb.Append(NewLine);
        return sb.ToString();
    }

    private void GetLogicControlerOfLogics(FormLogicDefinition formLogicDefinition, string namespaceId,
        string entityId,
        string formReportId, string eventType, bool bHasSource, UIRuleEvent.eEventType evtyp, StringBuilder sb)
    {
        if (formLogicDefinition?.Logics == null)
        {
            return;
        }

        if (bHasSource)
        {
            _ = sb.Append($"{NewLine1T}switch(source){NewLine2T}" + "{");
        }
        else
        {
            _ = sb.Append($"{NewLine1T}var source = null;");
        }

        bool bHasAjax = false;
        Dictionary<string, bool> bHasAjaxDic = [];
        string lastSource = null;
        foreach (IGrouping<string, FormLogicDefinitionItem> logic in formLogicDefinition.Logics.Where(l =>
                l.TriggeringEvent.EventType == evtyp)
            .GroupBy(l => l.TriggeringEvent.Source))
        {
            GetLogicControlerOfLogic(namespaceId, entityId, formReportId, eventType, bHasSource,
                sb, ref bHasAjax, bHasAjaxDic, ref lastSource, logic);
        }

        if (lastSource != null)
        {
            _ = sb.Append($"{NewLine3T}break;");
        }

        if (bHasSource)
        {
            _ = sb.Append(NewLine2T + "}");
        }
    }

    private void GetLogicControlerOfLogic(string namespaceId, string entityId,
        string formReportId, string eventType, bool bHasSource,
        StringBuilder sb, ref bool bHasAjax, Dictionary<string, bool> bHasAjaxDic,
        ref string lastSource, IGrouping<string, FormLogicDefinitionItem> logicsOfSource)
    {
        if (bHasSource)
        {
            if (logicsOfSource.Key != lastSource)
            {
                if (lastSource != null)
                {
                    _ = sb.Append($"{NewLine3T}break;");
                }

                _ = sb.Append($"{NewLine2T}case \"" + logicsOfSource.Key + "\":");
            }
        }

        foreach (FormLogicDefinitionItem logic in logicsOfSource)
        {
            foreach (FormLogicOperationDefinition op in logic.Operations)
            {
                GetLogicControlerOfOperation(namespaceId, entityId, formReportId, eventType, bHasSource,
                    sb, ref bHasAjax, bHasAjaxDic, logic, op);
            }
        }

        if (bHasSource)
        {
            lastSource = logicsOfSource.Key;
        }
    }

    private void GetLogicControlerOfOperation(string namespaceId, string entityId,
        string formReportId, string eventType, bool bHasSource, StringBuilder sb,
        ref bool bHasAjax, Dictionary<string, bool> bHasAjaxDic, FormLogicDefinitionItem logic,
        FormLogicOperationDefinition op)
    {
        LogicBeginOfLoop(sb, op);
        //						if (rop.Type == eOperationType.ChangeCSSAttribute)
        //							rop.Type = ChangeOperationTypeByAttribute(op.SpecificAttribute, value);
        switch (op.Type)
        {
            case eOperationType.None:
                break;
            case eOperationType.SetValue:
                _ = sb.Append(NewLine3T +
                             GetJQuerySetValueString(op.TargetId, JQueryPreProcessFormula(op.Value)));
                break;
            case eOperationType.ToggleClass:
                _ = sb.Append(NewLine3T +
                             GetJQueryToggleClassString(op.TargetId, op.TargetArea, op.Attr,
                                 JQueryPreProcessFormula(op.Value)));
                break;
            case eOperationType.SetClass:
                _ = sb.Append(NewLine3T +
                             GetJQuerySetClassString(op.TargetId, op.TargetArea, op.Attr,
                                 JQueryPreProcessFormula(op.Value)));
                break;
            case eOperationType.RemoveClass:
                _ = sb.Append(NewLine3T +
                             GetJQueryRemoveClassString(op.TargetId, op.TargetArea, op.Attr,
                                 JQueryPreProcessFormula(op.Value)));
                break;
            case eOperationType.ChangeCSSAttribute:
                _ = sb.Append(NewLine3T +
                             GetJQuerySetCssAttrString(op.TargetId, op.TargetArea, op.Attr,
                                 JQueryPreProcessFormula(op.Value)));
                break;
            case eOperationType.SetLocalParameterValue:
                _ = sb.Append(NewLine3T +
                             GetJQuerySetLcString(op.TargetId, JQueryPreProcessFormula(op.Value)));
                break;
            case eOperationType.ShowWarning:
                _ = sb.Append($"{NewLine4T}alert(" + JQueryPreProcessFormula(op.Value) + ");");
                break;
            case eOperationType.ShowError:
                _ = sb.Append($"{NewLine4T}alert(" + JQueryPreProcessFormula(op.Value) + ");");
                _ = sb.Append($"{NewLine4T}return;");
                break;
            case eOperationType.ShowPrompt:
                _ = sb.Append($"{NewLine4T}alert(" + JQueryPreProcessFormula(op.Value) + ");");
                break;
            case eOperationType.ShowModal:
                //todo:
                break;
            case eOperationType.ShowWindow:
                //todo:
                break;
            case eOperationType.ShowAutoHideWindow:
                //todo:
                break;
            case eOperationType.CallController:
                //todo:
                break;
            case eOperationType.ServerOperation:
                LogicOfServerOperation(namespaceId, entityId, formReportId, eventType,
                    bHasSource, sb, ref bHasAjax, bHasAjaxDic, logic);
                break;
        }

        LogicEndOfLoop(sb, op);
    }

    private void LogicOfServerOperation(string namespaceId, string entityId,
        string formReportId, string eventType, bool bHasSource, StringBuilder sb,
        ref bool bHasAjax, IDictionary<string, bool> bHasAjaxDic, FormLogicDefinitionItem logic)
    {
        if (bHasSource)
        {
            if (bHasAjaxDic.ContainsKey(logic.TriggeringEvent.Source))
            {
                return;
            }

            bHasAjaxDic[logic.TriggeringEvent.Source] = true;
        }
        else
        {
            if (bHasAjax)
            {
                return;
            }

            bHasAjax = true;
        }

        Dictionary<string, List<ExpressionNode>> dic = [];
        //FetchNodes(dic, op);
        foreach (FormLogicOperationDefinition operation in logic.Operations)
        {
            FetchNodes(dic, operation);
        }

        List<string> paramList = dic.Where(itm => itm.Key.StartsWith("q["))
            .Select(nodes =>
            {
                VariableNameExpressionNode l = (VariableNameExpressionNode)nodes.Value.OfType<IndexExpressionNode>()
                    .FirstOrDefault()
                    ?.IndexExpression;
                return l?.Name;
            })
            .ToList();
        _ = sb.Append(NewLine3T +
                     $"runAjax(\"{namespaceId}\",\"{entityId}\",\"{formReportId}\", source, \"{eventType}\", runOperations, scope, \"{(eventType == "onPageLoad" ? string.Empty : string.Join(",", paramList))}\");");
    }

    private void FetchNodes(Dictionary<string, List<ExpressionNode>> dic,
        FormLogicOperationDefinition operation)
    {
        if (!string.IsNullOrEmpty(operation.Formula))
        {
            _ = Parser.FetchNodes<IndexExpressionNode>(operation.Formula, dic);
        }

        if (!string.IsNullOrEmpty(operation.Condition))
        {
            _ = Parser.FetchNodes<IndexExpressionNode>(operation.Condition, dic);
        }

        if (operation.FormulaList != null)
        {
            foreach (string formula in operation.FormulaList)
            {
                if (!string.IsNullOrEmpty(formula))
                {
                    _ = Parser.FetchNodes<IndexExpressionNode>(formula, dic);
                }
            }
        }
    }

    private void LogicBeginOfLoop(StringBuilder sb, FormLogicOperationDefinition op)
    {
        switch (op.Loop_ConditionType)
        {
            case UIRuleTask.eControlType.NoCondition:
                break;
            case UIRuleTask.eControlType.IfCondition:
                _ = sb.Append(NewLine3T + "if(" + JQueryPreProcessFormula(op.Condition) + ") {");
                break;
            case UIRuleTask.eControlType.DoWhileCondition:
                _ = sb.Append(NewLine3T + "do {");
                break;
            case UIRuleTask.eControlType.WhileDoCondition:
                _ = sb.Append(NewLine3T + "while (" + JQueryPreProcessFormula(op.Condition) + ") {");
                break;
        }
    }

    private void LogicEndOfLoop(StringBuilder sb, FormLogicOperationDefinition op)
    {
        switch (op.Loop_ConditionType)
        {
            case UIRuleTask.eControlType.NoCondition:
                break;
            case UIRuleTask.eControlType.IfCondition:
                _ = sb.Append(NewLine3T + "}");
                switch (op.Type)
                {
                    case eOperationType.SetClass:
                        _ = sb.Append(NewLine3T + "else {");
                        _ = sb.Append(NewLine3T +
                                     GetJQueryRemoveClassString(op.TargetId, op.TargetArea, op.Attr,
                                         JQueryPreProcessFormula(op.Value)));
                        _ = sb.Append(NewLine3T + "}");
                        break;
                    case eOperationType.RemoveClass:
                        _ = sb.Append(NewLine3T + "else {");
                        _ = sb.Append(NewLine3T +
                                     GetJQuerySetClassString(op.TargetId, op.TargetArea, op.Attr,
                                         JQueryPreProcessFormula(op.Value)));
                        _ = sb.Append(NewLine3T + "}");
                        break;
                    case eOperationType.ChangeCSSAttribute:
                        _ = sb.Append(NewLine3T + "else {");
                        _ = sb.Append(NewLine3T +
                                     GetJQuerySetCssAttrString(op.TargetId, op.TargetArea, op.Attr, ""));
                        _ = sb.Append(NewLine3T + "}");
                        break;
                }

                break;
            case UIRuleTask.eControlType.DoWhileCondition:
                _ = sb.Append(NewLine3T + "} while (" + JQueryPreProcessFormula(op.Condition) + ");");
                break;
            case UIRuleTask.eControlType.WhileDoCondition:
                _ = sb.Append(NewLine3T + "}");
                break;
        }
    }

    private bool CheckEvent(string source, string eventName, UIRule rule)
    {
        bool related = false;
        foreach (UIRuleEvent ev in rule.Events)
        {
            if (ev.type.ToString() != eventName)
            {
                continue;
            }

            if (!(string.IsNullOrWhiteSpace(source) || source == "body"))
            {
                if (ev.exporterFieldId != source)
                {
                    continue;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(ev.exporterFieldId))
                {
                    continue;
                }
            }

            related = true;
            break;
        }

        return related;
    }

    private void AddOperationToLogic(string namespaceId, string entityId,
        IdentityUser user, List<QField> qs, List<LCField> lCs, ServerOperationResult result,
        UiEntity uientity, string ruleName, UIRuleTask op, List<FormField> formFields,
        string culture)
    {
        if (op.conditionCalcLocation == UIRuleTask.eCalcLocation.Server &&
             !string.IsNullOrEmpty(op.conditionStr))
        {
            if (!ExpressionNode.CheckIfTrue(ServerEval(user, namespaceId, entityId, op.conditionStr, qs, lCs)))
            {
                return;
            }
        }

        FormLogicOperationDefinition rop = new(op, this)
        {
            Name = string.IsNullOrEmpty(op.operationId) ? ruleName : op.operationId
        };
        object ev;
        switch (rop.Type)
        {
            case eOperationType.SetValue:
                rop.TargetId = op.assignToFieldId;
                ev = ServerEval(user, namespaceId, entityId, op.formulaStr, qs, lCs);
                rop.Value = ev?.ToString() ?? "";
                break;
            case eOperationType.SetLocalParameterValue:
                rop.TargetId = "lc_" + op.localParamId;
                ev = ServerEval(user, namespaceId, entityId, op.formulaStr, qs, lCs);
                rop.Value = ev?.ToString() ?? "";
                lCs.Add(new LCField { Field = rop.TargetId, Value = ev });
                break;
            case eOperationType.ToggleClass:
            case eOperationType.SetClass:
            case eOperationType.RemoveClass:
            case eOperationType.ChangeCSSAttribute:
            case eOperationType.SetProperty:
                if (op.TaskType == UIRuleTask.eTaskType.SetProperty &&
                     op.SpecificAttribute == eControlPropertyId.FilterFormula)
                {
                    SetFilterFormula(op, ref rop, uientity, namespaceId, entityId, user, qs, lCs, formFields, culture);
                }
                else
                {
                    SetAttributeOperation(op, ref rop, result, namespaceId, entityId, user, qs, lCs);
                }

                break;
            case eOperationType.ShowWarning:
                //todo: op.exceptionInfo???????????

                //if( culture == "fa")
                rop.Value = string.IsNullOrEmpty(op.Validation_ConstraintWarningText)
                    ? op.Validation_WarningText
                    : ServerEval(user, namespaceId, entityId, op.Validation_ConstraintWarningText, qs, lCs).ToString();
                //else
                //rop.Value = string.IsNullOrEmpty(op.Validation_EnConstraintWarningText)?op.Validation_EnWarningText:op.Validation_EnConstraintWarningText;
                break;
            case eOperationType.ShowError:
                //todo: op.exceptionInfo???????????
                //if( culture == "fa")
                rop.Value = string.IsNullOrEmpty(op.Validation_ConstraintWarningText)
                    ? op.Validation_WarningText
                    : ServerEval(user, namespaceId, entityId, op.Validation_ConstraintWarningText, qs, lCs).ToString();
                //else
                //rop.Value = string.IsNullOrEmpty(op.Validation_EnConstraintWarningText)?op.Validation_EnWarningText:op.Validation_EnConstraintWarningText;
                break;
            case eOperationType.ShowPrompt:

                //if( culture == "fa")
                rop.Value = string.IsNullOrEmpty(op.Validation_ConstraintWarningText)
                    ? op.Validation_WarningText
                    : ServerEval(user, namespaceId, entityId, op.Validation_ConstraintWarningText, qs, lCs).ToString();
                //else
                //rop.Value = string.IsNullOrEmpty(op.Validation_EnConstraintWarningText)?op.Validation_EnWarningText:op.Validation_EnConstraintWarningText;
                break;
            case eOperationType.ShowModal:
                rop.Value = "/Form/" + "Form?NamespaceId=\"" + op.modelId + "\"&EntityId=\"" + op.entityId +
                                "\"&FormId=\"" + op.form_rerportId + "\"";
                break;
            case eOperationType.ShowWindow:
                rop.Value = "/Form/" + "Form?NamespaceId=\"" + op.modelId + "\"&EntityId=\"" + op.entityId +
                                "\"&FormId=\"" + op.form_rerportId + "\"";
                break;
            case eOperationType.ShowAutoHideWindow:
                rop.Value = "/Form/" + "Form?NamespaceId=\"" + op.modelId + "\"&EntityId=\"" + op.entityId +
                                "\"&FormId=\"" + op.form_rerportId + "\"";
                break;
            case eOperationType.CallController:
                rop.Value = op.controlerId;
                break;
        }

        if (rop != null)
        {
            result.Items.Add(rop);
        }
    }

    private void SetAttributeOperation(UIRuleTask op, ref FormLogicOperationDefinition rop,
        ServerOperationResult result, string namespaceId, string entityId,
        IdentityUser user, List<QField> qs, List<LCField> lCs)
    {
        object value = op.calcFormulaLocation == UIRuleTask.eCalcLocation.Server
            ? ServerEval(user, namespaceId, entityId, op.formulaStr, qs, lCs)
            : op.formulaStr;
        if (rop.Type == eOperationType.ChangeCSSAttribute)
        {
            rop.Type = ChangeOperationTypeByAttribute(op.SpecificAttribute, value);
        }

        if (string.IsNullOrEmpty(op?.assignToFieldId) && op?.Params != null)
        {
            foreach (UIRuleTask.TaskParam prm in op.Params)
            {
                FormLogicOperationDefinition rop1 = new(op, this)
                {
                    Attr = op.SpecificAttribute.ToString(),
                    TargetId = prm.field_controlId
                };
                eControlPropertyTarget part = op.ControlPart;
                if (op.SpecificAttribute == eControlPropertyId.ShowHide)
                {
                    part = eControlPropertyTarget.Control;
                }

                rop1.TargetArea = part switch
                {
                    eControlPropertyTarget.Content => eTargetArea.Input,
                    eControlPropertyTarget.Label => eTargetArea.Label,
                    //case eControlPropertyTarget.Control:
                    //	break;
                    _ => eTargetArea.Container,
                };
                if (rop.Type is eOperationType.ChangeCSSAttribute or eOperationType.SetProperty)
                {
                    rop1.Value = value.ToString();
                }
                if (rop1.Type == eOperationType.ChangeCSSAttribute || rop.Type == eOperationType.SetProperty)
                {
                    rop1.Type = rop.Type;
                }
                result.Items.Add(rop1);
            }

            rop = null;
        }
        else
        {
            rop.Attr = op.SpecificAttribute.ToString();
            rop.TargetId = op.assignToFieldId;
            rop.TargetArea = op.ControlPart switch
            {
                eControlPropertyTarget.Content => eTargetArea.Input,
                eControlPropertyTarget.Label => eTargetArea.Label,
                //case eControlPropertyTarget.Control:
                //	break;
                _ => eTargetArea.Container,
            };
            if (rop.Type == eOperationType.ChangeCSSAttribute)
            {
                rop.Value = value.ToString();
            }
        }
    }

    private void SetFilterFormula(UIRuleTask op, ref FormLogicOperationDefinition rop, UiEntity uientity,
        string namespaceId, string entityId, IdentityUser user, List<QField> qs, List<LCField> lCs,
        List<FormField> formFields, string culture)
    {
        UIRuleTask.TaskParam param = op.Params.FirstOrDefault();
        if (param == null)
        {
            return;
        }

        FetchOperationParamField(uientity, namespaceId, param, out EntityField field, out Entity referEntity);
        if (referEntity != null)
        {
            SetAttributeOperationByQuery(op, rop, uientity, namespaceId, entityId, user, qs, lCs, formFields, culture,
                param, field, referEntity);
        }
        else
        {
            rop = null;
        }
    }

    private void FetchOperationParamField(UiEntity uientity, string namespaceId, UIRuleTask.TaskParam param,
        out EntityField field, out Entity referEntity)
    {
        string[] fieldIds = param.field_controlId.Split('.');
        field = uientity.GetField(fieldIds[0]);
        if (fieldIds.Length > 1)
        {
            for (int ii = 1; ii < fieldIds.Length; ii++)
            {
                Entity associationEntity = field?.AssociationEntity?.Entity();
                if (associationEntity == null)
                {
                    break;
                }

                field = field.AssociationEntity?.Entity()?.GetField(fieldIds[ii]);
            }
        }

        referEntity = field?.AssociationEntity?.Entity();
        if (field == null)
        {
            string[] obj = param.field_controlId.Split('_');
            if (obj.Length == 3)
            {
                referEntity = ProjectDefinition.Project.GetEntityByEntityId(obj[0], namespaceId);
                field = referEntity?.GetField(obj[2]);
                referEntity = field?.AssociationEntity?.Entity();
            }
        }
    }

    private void SetAttributeOperationByQuery(UIRuleTask op, FormLogicOperationDefinition rop,
        UiEntity uientity, string namespaceId, string entityId, IdentityUser user, List<QField> qs, List<LCField> lCs,
        List<FormField> formFields, string culture, UIRuleTask.TaskParam param, EntityField field, Entity referEntity)
    {
        ElasticObject q = new();
        ElasticObject lc = new();
        LocalParameters filterValues = new()
        {
            {"q", q},
            {"lc", lc},
            {"NamespaceId", namespaceId},
            {"EntityId", entityId}
        };
        if (user != null)
        {
            _ = filterValues.Add("user", user);
        }

        foreach (QField item in qs ?? Enumerable.Empty<QField>())
        {
            _ = q.SetField(item.Field, item.Value);
        }

        foreach (LCField item in lCs ?? Enumerable.Empty<LCField>())
        {
            _ = lc.SetField(item.Field, item.Value);
        }

        FormField formField = formFields?.FirstOrDefault(f => f.Id == param.field_controlId);
        string displayFields = formField?.Property(eControlPropertyId.DisplayFields);
        if (string.IsNullOrEmpty(displayFields))
        {
            displayFields = field.Property(EntityFieldPropertyId.DisplayFields);
        }

        string formFieldConstraint = formField?.Property(eControlPropertyId.FilterFormula);
        if (string.IsNullOrEmpty(formFieldConstraint))
        {
            formFieldConstraint = field.AssociationEntity.Constraint;
        }
        else if (!string.IsNullOrEmpty(field.AssociationEntity.Constraint))
        {
            formFieldConstraint = $"({formFieldConstraint}) And ({field.AssociationEntity.Constraint})";
        }

        ComboData cd = formField?.CheckProperty(eControlPropertyId.RemoteData) ?? true
            ? ComboDataRoutines.InitComboData(uientity, referEntity,
                false, culture, op.formulaStr, displayFields,
                formField?.GetProperties(), 1, 1)
            : ComboDataRoutines.GetRecords(uientity, referEntity,
                false, culture, op.formulaStr, 1,
                formFieldConstraint, displayFields, filterValues,
                formField.GetProperty(eControlPropertyId.OrderBy)?.ToString(), 5000,
                true, formField.GetProperties());
        rop.Type = eOperationType.ChangeList;
        rop.Rows = cd.Rows;
        rop.Attr = op.SpecificAttribute.ToString();
        rop.TargetId = param.field_controlId;
        rop.Formula = op.formulaStr;
    }

    public string ConvertToClientCode(string formula)
    {
        if (string.IsNullOrEmpty(formula))
        {
            return formula;
        }
        //string PPattern = @"p\[[0-9]*\]";
        //string QSubPattern = @"(q\[*\]\[*\])|(qm\[*\]\[*\])|(qpkv\[*\]\[*\])|(qmpkv\[*\]\[*\])";
        //Regex regex0 = new Regex(QSubPattern);
        //MatchCollection QConditionMatches0 = regex0.Matches(formula);
        //foreach (Match mMatch0 in QConditionMatches0)
        //{
        //	bool SelectByPKVFlag = mMatch0.Value.IndexOf(@"pkv") >= 0;
        //	int i = mMatch0.Value.IndexOf('[');
        //	int j = mMatch0.Value.IndexOf(']', i);
        //	string fieldName = mMatch0.Value.Substring(i, j - i);
        //	i = mMatch0.Value.IndexOf('[', j);
        //	j = mMatch0.Value.IndexOf(']', i);
        //	var subFieldName = mMatch0.Value.Substring(i, j - i);
        //}
        MatchCollection k = new Regex(@"(q\[\w*\])").Matches(formula);
        foreach (Match mMatch in k)
        {
            int i = mMatch.Value.IndexOf('[');
            int j = mMatch.Value.IndexOf(']', i);
            string fieldName = mMatch.Value.Substring(i + 1, j - i - 1);
            formula = formula.Replace(mMatch.Value, "getFieldValue(\"" + fieldName + "\",This,scope)");
        }

        /*var kt = new Regex(@"IF(*;*;*)", RegexOptions.IgnoreCase);
			if (kt!=null)
			{
				var ktd = kt.Matches(formula);
				if (ktd != null)
				{
					foreach (Match mMatch in ktd)
			{
				int i = mMatch.Value.IndexOf('(');
				int j = mMatch.Value.IndexOf(')', i);
				var parts = mMatch.Value.Substring(i, j - i).Split(';');
				var newstr = "((" + parts[0] + ")?(" + parts[1] + "):(" + parts[2] + "))";
				formula.Replace(mMatch.Value, newstr);
			}
				}
			}*/
        return formula;
    }

    public eOperationType GetClientOperationType(UIRuleTask operation)
    {
        switch (operation.TaskType)
        {
            case UIRuleTask.eTaskType.SetContent:
                return eOperationType.SetValue;
            case UIRuleTask.eTaskType.SetProperty:
                if (operation.SpecificAttribute == eControlPropertyId.ReadOnly)
                {
                    return eOperationType.SetProperty;
                }

                return eOperationType.ChangeCSSAttribute;
            case UIRuleTask.eTaskType.Validation:
                return operation.validationType switch
                {
                    UIRuleTask.eValidationType.Warning => eOperationType.ShowWarning,
                    _ => eOperationType.ShowError,
                };
            case UIRuleTask.eTaskType.SetLocalParameterValue:
                return eOperationType.SetLocalParameterValue;
            case UIRuleTask.eTaskType.OpenForm:
                return eOperationType.ShowModal;
            case UIRuleTask.eTaskType.OpenReport:
                return eOperationType.ShowWindow;
            case UIRuleTask.eTaskType.CallController:
                return eOperationType.CallController;
            case UIRuleTask.eTaskType.CallDataOperation:
            case UIRuleTask.eTaskType.OneRowQuery:
                return eOperationType.ServerOperation;
        }

        return eOperationType.None;
    }

    public object ServerEval(IdentityUser user, string namespaceId, string entityId,
        string formula, IList<QField> qs, IList<LCField> lCs)
    {
        //if (Qs != null)
        //{
        //	foreach (var item in Qs)
        //	{
        //		formula.Replace("q[" + item.Field + "]", item.Value);
        //		formula.Replace("qm[" + item.Field + "]", item.Value);
        //	}
        //}
        //if (LCs != null)
        //{
        //	foreach (var item in LCs)
        //	{
        //		formula.Replace("lc[" + item.Field + "]", item.Value);
        //	}
        //}
        try
        {
            ElasticObject q = new();
            ElasticObject lc = new();
            if (qs != null)
            {
                foreach (QField item in qs)
                {
                    _ = q.SetField(item.Field, item.Value);
                }
            }

            if (lCs != null)
            {
                foreach (LCField item in lCs)
                {
                    _ = lc.SetField(item.Field, item.Value);
                }
            }

            LocalParameters el = FormDataRoutines.GetLocalParamValues(user, namespaceId, entityId, "", q);
            _ = el.AddOrUpdate("lc", lc);
            return ServerEval(formula, el);
        }
        catch
        {
            // ignored
        }

        return null;
    }

    private eOperationType ChangeOperationTypeByAttribute(eControlPropertyId eControlPropertyId, object value)
    {
        if ((int)eControlPropertyId is >= 3000 and < 10000)
        {
            return eOperationType.ChangeCSSAttribute; //it is css attribute
        }

        switch (eControlPropertyId)
        {
            case eControlPropertyId.ShowHide:
                bool b = value is int && (int)value != 0 || value is double && (double)value > 0.000001 ||
                          value is bool && (bool)value || value is string && (string)value == "true";
                return b ? eOperationType.RemoveClass : eOperationType.SetClass;
            case eControlPropertyId.DoubleWidth:
            case eControlPropertyId.IsPassword:
            case eControlPropertyId.ReadOnly:
            case eControlPropertyId.Required:
            case eControlPropertyId.NoDetailsIcon:
            case eControlPropertyId.NoEditIcon:
                bool b1 = value is int && (int)value != 0 || value is double && (double)value > 0.000001 ||
                            value is bool && (bool)value ||
                            value is string && (string)value == "true";
                return b1 ? eOperationType.SetClass : eOperationType.RemoveClass;
                //			case eControlPropertyId.ClassStyle:
                //				break;
        }

        return eOperationType.ChangeCSSAttribute;
    }

    private bool AllOperationsAreClientSide(UIRule ctrl)
    {
        foreach (UIRuleTask op in ctrl.Operations)
        {
            if (op.calcFormulaLocation == UIRuleTask.eCalcLocation.Server ||
                 op.conditionCalcLocation == UIRuleTask.eCalcLocation.Server)
            {
                return false;
            }

            switch (op.TaskType)
            {
                case UIRuleTask.eTaskType.CallDataOperation:
                case UIRuleTask.eTaskType.OneRowQuery:
                    return false;
            }
        }

        return true;
    }
}
