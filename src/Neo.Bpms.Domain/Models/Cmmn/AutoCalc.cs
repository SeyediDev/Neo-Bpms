namespace Neo.Bpms.Domain.Models.Cmmn;

public class AutoCalcList
{
    //			public int Trip;
    //		public int tableId;
    //		public ExpressionNode condition = null;
    //		public QueryDefinition query = null;
    public List<AutoCalc> Calculations = [];
    public void AddAutoCalc(AutoCalc ac)
    {
        Calculations.Add(ac);
    }
}
public class AutoCalc
{
    public enum eGenerationType
    {
        //			Never,
        Always = 0,
        Insert = 1,
        IfNull = 2,
        DBInsert = 3
    }

    public enum AutoCalcLocation
    {
        None,
        ///// <summary>
        ///// this type of auto calc will be done before change control.
        ///// change control: for update data operations, update operation will be called only if there is a change,
        ///// </summary>
        //BeforeCheckForChanges = 1,
        //BSave = 2,
        /// <summary>
        /// this type of auto calc will be done before validation control. 
        /// validation control: if there is some validation rules at this data operation, operation will be done only if all of validations passed.
        /// </summary>
        BeforeValidation = 1,
        AfterValidation = 2,
        ///// <summary>
        ///// this type of auto calc will be done before triggers. 
        ///// triggers: if there is some data operatoion triggers, triggers will be done after operation done successfully.
        ///// </summary>
        //BeforeTrgiggers = 4,
        ///// this type of auto calc will be done after the first operation iteration and triggers. 
        ///// after doing these calculations, the operation will be done for the second time(without changing the state).
        //AfterTriggers = 5,
    }
    public AutoCalcLocation Loaction;
    public enum eAutoCalcType
    {
        SimpleFieldCalculation,
        TableReplaceFromQuery,
        TableInsertFromQuery,
        TableUpsertFromQuery,
        FieldCalculationForEachTableRow,
    }
    public string FieldId;
    public ExpressionTree Condition { get; set; }
    public ExpressionTree Formula { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the field must be recalced on any change.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the field must be recalced on any change; otherwise, <c>false</c>.
    /// </value>
    public bool RecalcOnAnyChange { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the field must be calced only if it is null.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the field must be calced only if it is null; otherwise, <c>false</c>.
    /// </value>
    public bool IfNull { get; set; }
    public eGenerationType GenerationType { get; set; }

    public AutoCalc Clone()
    {
        return new AutoCalc
        {
            FieldId = FieldId,
            Formula = Formula,//todo
            Condition = Condition,//todo
            GenerationType = GenerationType,
            IfNull = IfNull,
            Loaction = Loaction,
            RecalcOnAnyChange = RecalcOnAnyChange
        };
    }
}