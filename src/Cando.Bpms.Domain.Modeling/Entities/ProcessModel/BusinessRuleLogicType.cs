namespace Neo.Bpms.Domain.Modeling.Entities.ProcessModel;

public class BusinessRuleLogicType : BaseStringListProcessModelEntity
{
}

public enum BusinessRuleLogicTypeId
{
    [DisplayNameAndEnName("Formula")] Formula = 1,
    [DisplayNameAndEnName("Operation")] Operation = 2,
    [DisplayNameAndEnName("Repeatable Formula")] RepeatableFormula = 3,
    [DisplayNameAndEnName("Repeatable Operation")] RepeatableOperation = 4
}
