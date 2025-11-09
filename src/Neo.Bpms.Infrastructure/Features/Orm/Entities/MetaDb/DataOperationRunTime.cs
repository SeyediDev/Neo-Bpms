namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.MetaDb;

public static class DataOperationRuntime
{
    public static void ReConfig(DataOperation dataOperation)
    {
        if (dataOperation?.AutoCalcs?.Calculations != null)
        {
            foreach (AutoCalc autoCalc in dataOperation.AutoCalcs.Calculations)
            {
                if (autoCalc.Condition != null)
                {
                    Parser.ParseTree(autoCalc.Condition);
                }

                if (autoCalc.Formula != null)
                {
                    Parser.ParseTree(autoCalc.Formula);
                }
            }
        }
    }
}
