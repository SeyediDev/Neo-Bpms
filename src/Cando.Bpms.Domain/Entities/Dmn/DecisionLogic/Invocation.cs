namespace Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

/// <summary>
/// Invocation is a mechanism that permits the evaluation of one value expression – the invoked expression – inside another value expression – the invoking expression – 
/// by binding locally the input variables of the invoked expression to values inside the invoking expression. 
/// In an invocation, the input variables of the invoked expression are usually called: parameters. Invocation permits the same value expression to be re-used in 
/// multiple expressions, without having to duplicate it as a sub-expression in all the using expressions.
/// 
/// In DMN, the class Invocation is used to model invocations as a kind of Expression
/// 
/// An instance of Invocation is made of zero or more binding, which are instances of Binding, and model how the parameters of the invoked expression are bound to the inputVariables of the invoking instance of Expression.
/// An instance of Invocation references a calledFunction, which is the instance of Expression to be invoked.
/// The value of an instance of Invocation is the value of the associated calledFunction, with its inputVariables 
/// assigned values at runtime per the bindings in the Invocation.
/// Invocation MAY be used to model invocations in decision models, when a Decision element has exactly one knowledgeRequirement element, 
/// and when the decisionLogic in the Decision element consists only in invoking the BusinessKnowledgeModel element that is referenced by that requiredKnowledge 
/// and a more complex value expression is not required.
/// Using Invocation instances as the decisionLogic in Decision elements permits the re-use of the body of an instance of BusinessKnowledgeModel 
/// as the logic for any instance of Decision that requires that BusinessKnowledgeModel, where each requiring Decision element specifies its own bindings 
/// for the BusinessKnowledgeModel element’s parameters.
/// 
/// 
/// </summary>
public class Invocation(string id, IEnumerable<InformationItem> inputVariable, ItemDefinition itemDefinition, 
    DMNExpression calledFunction) : DMNExpression(id, inputVariable, itemDefinition)
{
    /// <summary>
    /// The Expression that is invoked by this Invocation. It MUST BE the body of the BusinessKnowledgeModel element that is required by the instance of 
    /// Decision that contains this Invocation. This is a derived attribute.
    /// </summary>
    public DMNExpression calledFunction= calledFunction;
    /// <summary>
    /// This attribute lists the instances of Binding used to bind the inputVariables of the calledFunction in this Invocation.
    /// </summary>
    public List<Binding> binding = [];
    public void addBinding(Binding binding)
    {
        this.binding.Add(binding);
    }
}
