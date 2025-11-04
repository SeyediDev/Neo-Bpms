using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;

/// <summary>
/// The Expression class is used to specify an Expression using natural-language text. 
/// These Expressions are not executable. 
/// The natural language text is captured using the documentation attribute, inherited from BaseElement.
/// 
/// Expressions are used in many places within BPMN to extract information from the different elements, normally data elements. 
/// The most common usage is when modeling decisions, where conditional Expressions are used to direct the flow along specific paths based on some criteria.
/// 
/// BPMN supports underspecified Expressions, where the logic is captured as natural-language descriptive text. 
/// It also supports formal Expressions, where the logic is captured in an executable form using a specified Expression language.
/// 
/// 
/// The definition of an Expression can be done in two ways: it can be contained where it is used, or it can be defined at the Process level and then referenced where it is used.
/// </summary>
public abstract class BpmnExpression(string id) : BaseElementWithMixedContent(id) //abstract not in BPMN.2
{
    public virtual string Body()
    {
        return "";//todo
    }
}
