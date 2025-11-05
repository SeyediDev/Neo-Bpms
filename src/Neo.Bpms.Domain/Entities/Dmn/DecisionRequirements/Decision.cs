using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.DecisionRequirements;

/// <summary>
/// A Decision element denotes the act of determining an output from a number of inputs, using decision logic which may reference one or more Business Knowledge Models.
/// 
/// Decision is used to model a decision.
/// It may have a question and allowedAnswers, which are all Strings. 
/// The optional description attribute is meant to contain a brief description of the decision-making embodied in the Decision. 
/// The optional question attribute is meant to contain a natural language question that characterizes the Decision such that the output of the Decision is an answer 
/// to the question. The optional allowedAnswers attribute is meant to contain a natural language description of the answers allowed for the question such as Yes/No, 
/// a list of allowed values, a range of numeric values etc.
/// </summary>
public class Decision : DRGElement
{
    public Decision(string id, string name, string description, string question) : base(id, name, description) { this.question = question; }
    public Decision(string id, string name, string description) : base(id, name, description) { question = null; }
    public Decision(string id, string name) : base(id, name) { question = null; }
    public Decision(string id) : base(id) { question = null; }

    /// <summary>
    /// A natural language question that characterizes the Decision such that the output of the Decision is an answer to the question.
    /// </summary>
    public string question;
    /// <summary>
    /// A natural language description of the answers allowed for the question such as Yes/No, a list of allowed values, a range of numeric values etc.
    /// </summary>
    public string allowedAnswers = null;
    /// <summary>
    /// The instance of Expression that represents the decision logic for this Decision
    /// </summary>
    public DMNExpression decisionLogic;
    /// <summary>
    /// the instances of InformationRequirement that compose this Decision.
    /// </summary>
    public List<InformationRequirement> informationRequirement = [];
    public void addInformationRequirement(InformationItem variable)
    {
        InformationRequirement ir = new(this, variable);
        informationRequirement.Add(ir);
    }
    /// <summary>
    /// the instances of KnowledgeRequirement that compose this Decision.
    /// </summary>
    public List<KnowledgeRequirement> knowledgeRequirement = [];
    public void addKnowledgeRequirement(BusinessKnowledgeModel businessKnowledgeModel)
    {
        KnowledgeRequirement kr = new(businessKnowledgeModel);
        knowledgeRequirement.Add(kr);
    }
    /// <summary>
    /// the instances of AuthorityRequirement that compose this Decision.
    /// </summary>
    public List<AuthorityRequirement> authorityRequirement = [];
    public void addAuthorityRequirement(KnowledgeSource requiredAuthority)
    {
        AuthorityRequirement ar = new(this, requiredAuthority);
        authorityRequirement.Add(ar);
    }
    /// <summary>
    /// The instances of BMM::Objective that are supported by this Decision.
    /// </summary>
    public List<Objective> supportedObjective = [];
    public void addSupportedObjective(Objective supportedObjective)
    {
        this.supportedObjective.Add(supportedObjective);
    }
    /// <summary>
    /// the instances of PerformanceIndicator that are impacted by this Decision.
    /// </summary>
    public Dictionary<string, PerformanceIndicator> impactedPerformanceIndicator = [];
    public void addImpactedPerformanceIndicator(PerformanceIndicator pi)
    {
        if (!impactedPerformanceIndicator.ContainsKey(pi.id))
        {
            impactedPerformanceIndicator.Add(pi.id, pi);
            pi.addImpactingDecision(this);
        }
    }
    /// <summary>
    /// The instances of OrganisationalUnit that make this Decision.
    /// </summary>
    public Dictionary<string, OrganisationalUnit> decisionMaker = [];
    public void addDecisionMaker(OrganisationalUnit decisionMaker)
    {
        if (!this.decisionMaker.ContainsKey(decisionMaker.id))
        {
            this.decisionMaker.Add(decisionMaker.id, decisionMaker);
            decisionMaker.addDecisionMade(this);
        }
    }
    /// <summary>
    /// The instances of OrganisationalUnit that own this Decision.
    /// </summary>
    public Dictionary<string, OrganisationalUnit> decisionOwner = [];
    /// <summary>
    /// lists the instances of BPMN::processthat require this Decision to be made.
    /// </summary>
    public void addDecisionOwner(OrganisationalUnit decisionOwner)
    {
        if (!this.decisionOwner.ContainsKey(decisionOwner.id))
        {
            this.decisionOwner.Add(decisionOwner.id, decisionOwner);
            decisionOwner.addDecisionOwned(this);
        }
    }
    //public List<Process> usingProcesses = new List<Process>();
    ///// <summary>
    ///// the instances of BPMN::task that make this Decision.
    ///// </summary>
    //public List<BPMN.Processes.Task> usingTasks =new List<BPMN.Processes.ProcessTask>();
    //public void addBPMNTask(Process process, BPMN.Processes.Task task)
    //{
    //	this.usingProcesses.Add(process);
    //	this.usingTasks.Add(task);
    //}

}
