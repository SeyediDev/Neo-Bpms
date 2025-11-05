using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.DecisionTable;

/// <remark>
/// A decision table is a tabular representation of a set of related input and output expressions, organized into rules indicating which output entry applies to a 
/// specific set of input entries. The decision table contains all (and only) the inputs required to determine the output. Moreover, a complete table contains all 
/// possible combinations of input values (all the rules).
/// 
/// Input expressions are usually simple, for example, a name (e.g. Customer Status) or a test (e.g. Age<25). 
/// The expression can be any text (e.g., natural language text) but SHOULD NOT conflict with FEEL syntax.
/// 
/// Input expressions may be expected to result in a limited number or a limited range of values. It is important to model these expected input values, 
/// because a decision table may be considered complete if its rules cover all combinations of expected input values.
/// Regardless of how the expected input values are modeled, input values should be exclusive and complete. Exclusive means that input values do not overlap. 
/// Complete means that all relevant input values from the domain are covered.
/// For example, the following two input value ranges overlap: <5, <10. The following two ranges are incomplete: <5, >5.
/// The list of input values is optional. If provided, it is a list of unary tests restricting the corresponding inputs to values that test true. 
/// The list can be any text (e.g., natural language text) but SHOULD NOT conflict with FEEL syntax.
/// 
/// 
/// The table name or the output name MUST be specified. If the table name is specified, the output name MUST be the same as the table name, 
/// or left unspecified (empty box for single-output tables, or omitted box for multiple-output tables). If the table name is not specified, 
/// the output name MUST be specified.
/// 
/// The output entries of a decision table are often drawn from a list of output values. 
/// The ordering of the list of output values can be used to specify the priority when multiple rules match but only one hit should be returned. 
/// The ordering is also used when the hit policy is output order.
/// The list of output values is optional. If provided, it is a list restricting the value of the output entries to the given list of values. 
/// The list can be any text (e.g., natural language text) but SHOULD NOT conflict with FEEL syntax.
/// 
/// The decision table can show a compound output
/// 
/// 
/// Rule input entries are expressions. The expression can be any text (e.g., natural language text) but SHOULD NOT conflict with FEEL syntax. 
/// A dash symbol (‘-‘) can be used to mean any input value, i.e., the input is irrelevant for the containing rule.
/// The values in a unary test should be '-' or a subset of the input values specified. For example, if the input values for input 'Age' are specified 
/// as [0..120], then an input entry of <0 SHOULD be reported as invalid.
/// Tables containing a ‘-‘are called contracted tables. The others are called expanded.
/// Tables where every input entry is true, false, or '-' are historically called limited-entry tables, but there is no need to maintain this restriction.
/// Evaluation of the expressions in a decision table does not produce side-effects. The order of input entries is not related to any execution order in implementation
/// 
/// A rule output entry is an expression. The expression can be any text (e.g., natural language text) but SHOULD NOT conflict with FEEL syntax.
/// In vertical (rules as columns) tables with a single output name which is identical to the table name, a shorthand notation may be used to indicate: 
/// output applies (X) or does not apply (-), as is common practice in decision tables.
/// 
/// A decision table may have several rules, and in general more than one rule may be matched for a given set of inputs. 
/// The hit policy specifies what the result of the decision table is in such cases, and also contains additional information that can be used to check correctness 
/// at design-time. For clarity, the hit policy is summarized using a single character in a particular decision table cell. Tools may support only a subset of hit 
/// policies, but the table type must be clear and therefore the hit policy indication is mandatory.
/// The hit policy MUST default to Unique. Decision tables with the Unique hit policy do not contain rules with overlapping input entries.
/// If rules are allowed to contain overlapping input entries, the hit policy indicates how these overlapping rules have to be interpreted. A single hit table returns the output of one rule only; a multiple hit table may return the output of multiple rules (or a function of the outputs, e.g. sum of values).
/// A single hit table returns the output of one rule only. It may or may not contain overlapping rules. 
/// In case of overlapping rules, the hit policy has to indicate which of the matching rules to select.
/// 
/// Single hit policies for single output decision tables are:
/// 1. Unique: no overlap is possible and all rules are exclusive. Only a single rule can be matched. This is the default.
/// 2. Any: there may be overlap, but all of the matching rules show the same output, so any match can be used.
/// 3. Priority: multiple rules can match, with different outputs. This policy returns the matching rule with the highest output priority. 
/// Output priorities are specified in an ordered list of values, for example, the list of expected output values.
/// 4. First: multiple (overlapping) rules can match, with different output entries. The first hit by rule order is returned (and evaluation can halt). 
/// This is a common usage, because it resolves inconsistencies by forcing the first hit. It is important to distinguish this type of table from others 
/// because the meaning depends on the order of the rules. The last rule is often the catch-remainder. Because of this order, the table is hard to validate 
/// manually and therefore has to be used with care.
/// 
/// A multiple hit table may return output entries from multiple rules. The result will be a list of rule outputs or a function of the outputs.
/// Multiple hit policies for single output decision tables can be:
/// 5. No order: returns all hits in a unique list in arbitrary order.
/// 6. Output order: returns all hits in decreasing priority order. Output priorities are specified in an ordered list of values.
/// 7. Rule order: returns all hits in rule order. Note: the meaning may depend on the order of the rules.
/// Other policies, such as more complex manipulations on the outputs, can be performed by post-processing the output list (outside the decision table).
/// The single letter for hit policy also identifies if a table is single or multiple hit.
/// To reduce complexity, decision tables with compound outputs support only the following hit policies: Unique, Any, First, No order, and Rule order
/// 
/// Multiple hits must be aggregated into a single result. DMN 1.0 specifies six aggregation indicators, namely:collect, sum, min, max, count, average. 
/// Optionally, the aggregation indicator may be included in the table. The default is collect.
/// Aggregation indicators have no incidence on decision tables with single hit policies.
/// In decision tables with multiple hit policies, the semantics of the aggregation indicators are:


/// </remark>
/// <summary>
/// In DMN 1.0, the class DecisionTable is used to model a decision table.
/// As a kind of Expression, an instance of DecisionTable has a value, which depends on the conclusions of the associated rules, 
/// the associated hitPolicy and the associated aggregration, if any. 
/// </summary>
public class DecisionTable : DMNExpression
{
    /// <summary>
    /// the instances of Clause that compose this DecisionTable.
    /// </summary>
    public List<Clause> clause = [];
    /// <summary>
    /// the instances of DecisionRule that compose this DecisionTable.
    /// </summary>
    public List<DecisionRule> rule = [];
    public enum eHitPolicy
    {
        /// <summary>
        /// Single hit policies for single output decision tables are:
        /// 1. Unique: no overlap is possible and all rules are exclusive. Only a single rule can be matched. This is the default.
        /// . if the associated hitPolicy is UNIQUE, the value of an instance of DecisionTable is the value of the conclusion of the only applicable rule (see section 7.3.3, Decision Rule, for the definition of rule applicability);
        /// </summary>
        UNIQUE = 1,
        /// <summary>
        /// 2. Any: there may be overlap, but all of the matching rules show the same output, so any match can be used.
        /// . if the associated hitPolicy is FIRST, the value of an instance of DecisionTable is the value of the conclusion of the first applicable rule, according to the rule ordering;
        /// </summary>
        FIRST = 2,
        /// <summary>
        /// 3. Priority: multiple rules can match, with different outputs. This policy returns the matching rule with the highest output priority. 
        /// Output priorities are specified in an ordered list of values, for example, the list of expected output values.
        /// . if the associated hitPolicy is PRIORITY, the value of an instance of DecisionTable is the value of the conclusion of the of the first applicable rule, according to the ordering of the outputEntry in the clause of its conclusion (see section 7.3.2, Decision Table Clause);
        /// </summary>
        PRIORITY = 3,
        /// <summary>
        /// 4. First: multiple (overlapping) rules can match, with different output entries. The first hit by rule order is returned (and evaluation can halt). 
        /// This is a common usage, because it resolves inconsistencies by forcing the first hit. It is important to distinguish this type of table from others 
        /// because the meaning depends on the order of the rules. The last rule is often the catch-remainder. Because of this order, the table is hard to validate 
        /// manually and therefore has to be used with care.
        /// 
        /// . if the associated hitPolicy is ANY, the value of an instance of DecisionTable is the value of any of the applicable rules;
        /// </summary>
        ANY = 4,
        /// <summary>
        /// A multiple hit table may return output entries from multiple rules. The result will be a list of rule outputs or a function of the outputs.
        /// Multiple hit policies for single output decision tables can be:
        /// 5. No order: returns all hits in a unique list in arbitrary order.
        /// . if the associated hitPolicy is UNORDERED, the value of an instance of DecisionTable is the result of applying the aggregation function specified by the aggregation attribute of the DecisionTable to the unordered set of the values of the conclusions of all the applicable rules;
        /// </summary>
        UNORDERED = 5,
        /// <summary>
        /// 6. Output order: returns all hits in decreasing priority order. Output priorities are specified in an ordered list of values.
        /// . if the associated hitPolicy is RULE ORDER, the value of an instance of DecisionTable is the result of applying the aggregation function specified by the aggregation attribute of the DecisionTable to the set of the values of the conclusions of all the applicable rules, ordered according to the rule ordering;
        /// </summary>
        RULE_ORDER = 6,
        /// <summary>
        /// 7. Rule order: returns all hits in rule order. Note: the meaning may depend on the order of the rules.
        /// . if the associated hitPolicy is OUTPUT ORDER, the value of an instance of DecisionTable is the result of applying the aggregation function specified by the aggregation attribute of the DecisionTable to the set of the values of the conclusions of all the applicable rules, ordered according to the ordering of the outputEntry in the clause of its conclusion.
        /// </summary>
        OUTPUT_ORDER = 7
    }
    /// <summary>
    /// The hit policy that determines the semantics of this DecisionTable. Default is: UNIQUE.
    /// If the hitPolicy associated with an instance of DecisionTable is FIRST or RULE ORDER, the rules that are associated with the DecisionTable MUST be ordered. 
    /// The ordering is represented by the explicit numbering of the rules in the diagrammatic representation of the DecisionTable.
    /// If the hitPolicy associated with an instance of DecisionTable is PRIORITY or OUTPUT ORDER, the outputEntry of one of the 
    /// clauses in the DecisionTable MUST be ordered, and these outputEntries MUST be associated as conclusions to the rules in the DecisionTable. 
    /// </summary>
    public eHitPolicy hitPolicy = eHitPolicy.UNIQUE;
    public enum eBuiltinAggregator
    {
        /// <summary>
        /// 1. collect. The result of the decision table is the list of all the outputs, ordered or unordered per the hit policy.
        /// </summary>
        COLLECT = 1,
        /// <summary>
        /// 2. sum. The result of the decision table is the sum of all the outputs.
        /// </summary>
        SUM = 2,
        /// <summary>
        /// 3. min. The results of the decision table is the smallest value of all the outputs.
        /// </summary>
        MIN = 3,
        /// <summary>
        /// 4. max. The results of the decision table is the largest value of all the outputs.
        /// </summary>
        MAX = 4,
        /// <summary>
        /// 5. count. The results of the decision table is the number of outputs.
        /// </summary>
        COUNT = 5,
        /// <summary>
        /// 6. average. The results of the decision table is the average value of all the outputs, defined as the sum divided by the count, where the semantics of sum and count are as specified above.
        /// </summary>
        AVERAGE = 6
    }
    /// <summary>
    /// The aggregation function to be applied to the values of the applicable rules when there are more than one, to determine the value of this DecisionTable. Default is: COLLECT.
    /// </summary>
    public eBuiltinAggregator aggregation = eBuiltinAggregator.COLLECT;
    /// <summary>
    /// If present, this attribute MUST be false unless this DecisionTable is complete
    /// An instance of DecisionTable is said to be complete if and only if, for any valid binding of the DecisionTable inputVariables, at least one of the DecisionTable rules is applicable.
    /// </summary>
    public bool isComplete = false;
    /// <summary>
    /// If present, this attribute MUST be false unless this DecisionTable is consistent. Default is: false.
    /// An instance of DecisionTable is said to be consistent if and only if, for any valid binding of the DecisionTable inputVariables, all the applicable rules have the same value.
    /// </summary>
    public bool isConsistent = false;
    public enum eDecisionTableOrientation { RuleAsRow, RuleAsColumn, CrossTable }
    /// <summary>
    /// The preferred orientation for the diagrammatic representation of this DecisionTable. This DecisionTable SHOULD BE represented as specified by this attribute.
    /// </summary>
    public eDecisionTableOrientation preferedOrientation;
    public DecisionTable(string id, string name, IEnumerable<InformationItem> inputVariable, ItemDefinition itemDefinition)
        : base(id, inputVariable, itemDefinition)
    {
        this.name = name;
    }
}
