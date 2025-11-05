namespace Neo.Bpms.Domain.Models.Dmn.DecisionLogic;

/// <summary>
/// In DMN 1.0, the class Binding is used to model, in an Invocation element, the binding of the free variables – or parameters – 
/// in the invoked expression to the input variables of the invoking expression.
/// An instance of Binding is made of one bindingFormula, which is an instance of Expression, and of one reference to a parameter, which is an instance of InformationItem.
/// The inputVariables of the bindingFormula in a Binding element MUST be a subset of the inputVariables in the owning instance of Invocation.
/// The parameter referenced by a Binding element MUST be one of the parameters of the BusinessKnowledgeModel element that contains the 
/// calledFunction element that is invoked by the containing instance of Invocation.
/// When the Invocation element is executed, each InformationItem element that is referenced as a parameter by a binding in the Invocation element is assigned, 
/// at runtime, the value of the bindingFormula.
/// </summary>
public class Binding
{
    /// <summary>
    /// The InformationItem on which the calledFunction of the owning instance of Invocation depends that is bound by this Binding.
    /// </summary>
    public InformationItem parameter;
    /// <summary>
    /// The instance of Expression to which the parameter in this Binding is bound when the owning instance of Invocation is evaluated.
    /// </summary>
    public DMNExpression bindingFormula;
    public Binding(InformationItem parameter)
    {
        this.parameter = parameter;
        bindingFormula = null;
    }
    public Binding(InformationItem parameter, DMNExpression bindingFormula)
    {
        this.parameter = parameter;
        this.bindingFormula = bindingFormula;
    }
}
