namespace Neo.Bpms.Domain.Entities.Cmmn;

public class Trigger
{
    public Trigger() { }
    public ExpressionTree condition;
    public DataOperation triggeredOperation;
    public List<Parameter> parameters = [];
    public void addParameter(Parameter p)
    {
        parameters.Add(p);
    }
    public class Parameter
    {
        public Parameter() { }
        public Parameter(string trigFieldId, string sourceFieldId)
        {
            this.trigFieldId = trigFieldId;
            this.sourceFieldId = sourceFieldId;
            formula = null;
        }
        public Parameter(string trigFieldId, ExpressionTree formula)
        {
            this.trigFieldId = trigFieldId;
            sourceFieldId = null;
            this.formula = formula;
        }
        public string trigFieldId;
        public string sourceFieldId;
        public ExpressionTree formula;
    }
}
