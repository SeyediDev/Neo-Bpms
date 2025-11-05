namespace Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

/// <summary>
/// In DMN 1.0, the class LiteralExpression is used to model a value expression whose value is specified by text in some specified expression language.
/// 
/// An instance of LiteralExpression has an optional text, which is a String, and an optional expressionLanguage, which is a String that identifies 
/// the expression language of the text. If no expressionLanguage is specified, the expression language of the text is the expressionLanguage that is 
/// associated with the containing instance of Definitions. The expressionLanguage MUST be specified in a URI format. The default expression language is FEEL.
/// 
/// As a subclass of Expression, each instance of LiteralExpression has a value. The text in an instance of LiteralExpression determines its value, 
/// according to the semantics of the LiteralExpression’s expressionLanguage. The semantics of DMN 1.0 decision models as described in this specification 
/// applies only if the text of all the instances of LiteralExpression in the model are valid expressions in their associated expression language.
/// An instance of LiteralExpression may include an import, which is an instance of Import that identifies where the text of the LiteralExpression is located. 
/// An instance of LiteralExpression MUST NOT have both a text and an import. The importType of the import MUST be the same as the expressionLanguage of the 
/// LiteralExpression element.
/// </summary>
public class LiteralExpression : DMNExpression
{
    /// <summary>
    /// The text of this LiteralExpression. It MUST be a valid expression in the expressionLanguage.
    /// </summary>
    public string text;
    /// <summary>
    /// This attribute identifies the expression language used in this LiteralExpression. 
    /// This value overrides the expression language specified for the containing instance of DecisionRequirementDiagram. 
    /// The language MUST be specified in a URI format.
    /// </summary>
    public string expressionLanguage = null;
    /// <summary>
    /// The instance of Import that specifies where the text of this LiteralExpression is located.
    /// </summary>
    public Core.Import import;
    public LiteralExpression(string id, IEnumerable<InformationItem> inputVariable, ItemDefinition itemDefinition, string text)
        : base(id, inputVariable, itemDefinition)
    {
        this.text = text;
        import = null;
    }
    public LiteralExpression(string id, IEnumerable<InformationItem> inputVariable, ItemDefinition itemDefinition, Core.Import import)
        : base(id, inputVariable, itemDefinition)
    {
        text = null;
        this.import = import;
    }
}
