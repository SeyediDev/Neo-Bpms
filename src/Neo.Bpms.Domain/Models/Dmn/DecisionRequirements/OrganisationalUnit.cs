namespace Neo.Bpms.Domain.Models.Dmn.DecisionRequirements;

public class OrganisationalUnit : BusinessContextElement
{
    public OrganisationalUnit(string id, string URI) : base(id, URI) { }
    public OrganisationalUnit(string id, string name, string URI) : base(id, name, URI) { }
    public OrganisationalUnit(string id, string name, string description, string URI) : base(id, name, description, URI) { }
    /// <summary>
    /// the instances of Decision that are made by this OrganisationalUnit
    /// </summary>
    public Dictionary<string, Decision> decisionMade = [];
    public void addDecisionMade(Decision decision)
    {
        if (!decisionMade.ContainsKey(decision.id))
            decisionMade.Add(decision.id, decision);
    }
    /// <summary>
    /// the instances of Decision that are owned by this OrganisationalUnit.
    /// </summary>
    public Dictionary<string, Decision> decisionOwned = [];
    public void addDecisionOwned(Decision decision)
    {
        if (!decisionOwned.ContainsKey(decision.id))
            decisionOwned.Add(decision.id, decision);
    }
}
