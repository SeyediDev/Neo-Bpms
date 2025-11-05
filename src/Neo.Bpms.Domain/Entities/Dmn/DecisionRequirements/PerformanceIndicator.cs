namespace Neo.Bpms.Domain.Entities.Dmn.DecisionRequirements;

public class PerformanceIndicator : BusinessContextElement
{
    public PerformanceIndicator(string id, string URI) : base(id, URI) { }
    public PerformanceIndicator(string id, string name, string URI) : base(id, name, URI) { }
    public PerformanceIndicator(string id, string name, string description, string URI) : base(id, name, description, URI) { }
    /// <summary>
    /// the instances of Decision that impact this PerformanceIndicator
    /// </summary>
    public Dictionary<string, Decision> impactingDecision = [];
    public void addImpactingDecision(Decision impactingDecision)
    {
        this.impactingDecision.Add(impactingDecision.id, impactingDecision);
    }
}
