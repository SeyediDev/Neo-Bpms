using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramInterchange;

namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.UMLDiagram;

public class BusinessRuleShape(BusinessRuleShape.Type type, string id, string name, Bounds bounds, 
    BaseClass rule, DiagramElement owningElement) : Shape(id, name, bounds, null, owningElement)
{
    public Type type = type;
    public enum Type
    {
        BusinessRule,
        BusinessRuleGroup,
    }

    public List<BaseClass> terms = null;
    public List<BaseClass> facts;
    public BaseClass AddTerm(BaseClass term)
    {
        terms ??= [];
        terms.Add(term);
        return term;
    }
    public BaseClass AddFact(BaseClass fact)
    {
        facts ??= [];
        facts.Add(fact);
        return fact;
    }

    public BaseClass Rule => rule ?? ModelElement as BaseClass;
}
public class KnowledgeTerm: BaseClass { }

