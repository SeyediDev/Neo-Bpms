namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Expressions;

/// <summary>
/// The FormalExpression class is used to specify an executable Expression using a specified Expression language. 
/// A natural-language description of the Expression can also be specified, in addition to the formal specification.
/// 
/// The default Expression language for all Expressions is specified in the Definitions element, using the expressionLanguage attribute. 
/// It can also be overridden on each individual FormalExpression using the same attribute.
/// </summary>
public class FormalExpression(string id, string language, ExpressionTree body, ItemDefinition evaluatesToTypeRef) : BpmnExpression(id)
{
    /// <summary>
    /// Overrides the Expression language specified in the Definitions. The language MUST be specified in a URI format.
    /// </summary>
    public string language = language;

    /// <summary>
    /// The body of the Expression. 
    /// Note that this attribute is not relevant when the XML Schema is used for interchange. 
    /// Instead, the FormalExpression complex type supports mixed content. The body of the Expression would be specified as element content.
    /// <example>
    /// <!--<formalExpression id=“ID_2">count(../dataObject[id="CustomerRecord_1"]/emailAddress) > 0-->
    /// <!--<evaluatesToType id="ID_3" typeRef=“xsd:boolean"/>-->
    /// <!--</formalExpression>-->
    /// </example>
    /// </summary>
    public ExpressionTree body = body;

    /// <summary>
    /// The type of object that this Expression returns when evaluated. For example, conditional Expressions evaluate to a boolean
    /// </summary>
    public ItemDefinition evaluatesToTypeRef = evaluatesToTypeRef;

    public FormalExpression(string id, ExpressionTree formula, ItemDefinition evaluatesToTypeRef) :
        this(id, null, formula, evaluatesToTypeRef)
    {
    }

    public FormalExpression(string id, ExpressionTree formula) :
        this(id, formula, null)
    {
    }

    public ExpressionTree Expression => body;
    public override string Body()
    {
        return Expression?.ExpressionString;
    }
}
