namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

/// <summary>
/// Conformance Attribute
/// 
/// Conformance checks the access control of user(role) to modify fields or entity
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class EFAttr_Conformance : Attribute
{
    public EFAttr_Conformance(string name, UserSecurityAccessFlags access, ConformanceLevel level,
        string conformanceExpression = null, string errorText = null, string errorFormula = null,
        string enErrorText = null, string enErrorFormula = null)
    {
        Name = name;
        Access = access;
        Level = level;
        ConformanceExpression = conformanceExpression;

        Error = new ExceptionInformationData(ExceptionArea.DataEngine, name)
        {
            ErrorText = errorText,
            ErrorFormula = errorFormula,
            EnErrorText = enErrorText,
            EnErrorFormula = enErrorFormula,
            Name = name,
        };
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="EFAttr_Conformance"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="access">The access.</param>
    /// <param name="level">The level.</param>
    /// <param name="error"></param>
    /// <param name="conformanceExpression">The conformance expression.</param>
    public EFAttr_Conformance(string name, UserSecurityAccessFlags access, ConformanceLevel level,
    ExceptionInformationData error, string conformanceExpression = null)
    {
        Name = name;
        Access = access;
        Level = level;
        ConformanceExpression = conformanceExpression;
        Error = error;
    }
    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    /// <value>
    /// The subject of entity modification (edit subject).
    /// </value>
    public string Subject { get; set; }

    public string Name { get; set; }

    public UserSecurityAccessFlags Access { get; set; }

    public ConformanceLevel Level { get; set; }

    public string ConformanceExpression { get; set; }
    public ExceptionInformationData Error { get; set; }
}
public class ExceptionInformationData(ExceptionArea dataEngine, string name)
{
    public ExceptionArea DataEngine { get; internal set; } = dataEngine;
    public string ErrorText { get; internal set; }
    public string ErrorFormula { get; internal set; }
    public string EnErrorText { get; internal set; }
    public string EnErrorFormula { get; internal set; }
    public string Name { get; internal set; } = name;
}
