namespace Neo.Bpms.Domain.Entities.ProcessModel;

public class BusinessRuleType : BaseProcessModelStateBasedEntity
{
}
public enum BusinessRuleTypeId
{
    [DisplayNameAndEnName("DataFlow")] DataFlow = 1,
    [DisplayNameAndEnName("DMN")] Dmn = 2,
    [DisplayNameAndEnName("PRR")] Prr = 3,
    [DisplayNameAndEnName("PMML - Future")] Pmml = 4,
    [DisplayNameAndEnName("Solving Model - Future")] SolvingModel = 5,

    [DisplayNameAndEnName("NonExecutable")] NonExecutable = 101,
}
