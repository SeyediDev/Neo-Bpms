namespace Neo.Bpms.Domain.Models.Cmmn.UI.Forms.UIRules;

/// <summary>
/// A rule Task step
/// </summary>
public class UIRuleTask
{
    public class QueryFilter
    {
        public QueryFilter() { }
        public QueryFilter(QueryFilter f)
        {
            //				this.scopeId = f.scopeId;
            filter = f.filter;
        }
        public QueryFilter(string filter)
        {
            //				this.scopeId = scopeId;
            this.filter = filter;
        }
        //			public int scopeId;
        public string filter;
    }
    public class TaskParam
    {
        public string field_controlId;
        public string query_operationFieldId;
        public eLocalParameterIds localParamId;

        public TaskParam() { }
        public TaskParam(TaskParam p)
        {
            field_controlId = p.field_controlId;
            query_operationFieldId = p.query_operationFieldId;
            localParamId = p.localParamId;
        }
        public TaskParam(string fieldId)
        {
            field_controlId = fieldId;
            query_operationFieldId = string.Empty;
            localParamId = eLocalParameterIds.None;
        }
        public TaskParam(string query_operationFieldId, string viewModelFieldId)
        {
            this.query_operationFieldId = query_operationFieldId;
            field_controlId = viewModelFieldId;
            localParamId = eLocalParameterIds.None;
        }
        public TaskParam(string query_operationFieldId, eLocalParameterIds localParamId)
        {
            this.query_operationFieldId = query_operationFieldId;
            field_controlId = string.Empty;
            this.localParamId = localParamId;
        }
        //			public int ParamType;
    }
    public enum eTaskType
    {
        //ChangeSpecificFeature = 1,
        SetContent = 3,
        SetProperty = 4,
        CallDataOperation = 5,
        //			ChangeWebControlStyle = 6,
        //			CreateViewableControl = 7,
        Validation = 8,
        //			InvokeWebService = 9,
        //			ChangeConstraintCondition = 10,
        SetLocalParameterValue = 11,
        OneRowQuery = 12,
        CallController = 13,
        OpenForm = 14,
        OpenReport = 15,
    }
    public enum eValidationType
    {
        invalid = 0,
        Warning = 1,
        Error = 2,
    }
    public enum eControlType
    {
        NoCondition = 1,
        IfCondition = 2,
        DoWhileCondition = 3,
        WhileDoCondition = 4,
    };
    public enum eCalcLocation
    {
        //None = 0,
        Server = 1,
        Client = 2,
    };

    public eControlType ControlType;
    public eTaskType TaskType;
    public string modelId;
    public string entityId;
    public string operationId;
    public string form_rerportId;

    public string conditionStr;
    public eCalcLocation conditionCalcLocation;

    public string formulaStr { get; set; }
    public eCalcLocation calcFormulaLocation;
    public string assignToFieldId;
    public int localParamId { get; set; }
    public List<QueryFilter> queryFilters;
    /// <summary>
    /// add Query Filter
    /// </summary>
    /// <param name="queryFilter">query Filter</param>
    /// <returns></returns>
    public void addQueryFilter(QueryFilter queryFilter)
    {
        queryFilters ??= [];
        queryFilters.Add(queryFilter);
    }
    public List<TaskParam> Params;
    /// <summary>
    /// add Parameter
    /// </summary>
    /// <param name="param">param</param>
    /// <returns></returns>
    public void addParameter(TaskParam param)
    {
        Params ??= [];
        Params.Add(param);
    }

    public eValidationType validationType;
    public ExceptionInformation exceptionInfo;
    public string Validation_WarningText;
    public string Validation_EnWarningText;
    public string Validation_ConstraintWarningText;
    public string Validation_EnConstraintWarningText;

    public eControlPropertyId SpecificAttribute;
    public eControlPropertyTarget ControlPart;
    public string controlerId;

    /// <summary>
    /// Controller Task
    /// </summary>
    /// <returns></returns>
    public UIRuleTask()
    {
    }
}
