using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.FEEL;

public class FunctionDefinition(string id, IEnumerable<InformationItem> formalParameter, DMNExpression body) : DMNExpression(id, formalParameter, body.itemDefinition)
{
    /// <summary>
    /// the instances of InformationItem that are the parameters of this Context
    /// </summary>
    public List<InformationItem> formalParameter = [.. formalParameter];
    /// <summary>
    /// The instance of Expression that is the body in this FunctionDefinition
    /// </summary>
    public DMNExpression body;
}
