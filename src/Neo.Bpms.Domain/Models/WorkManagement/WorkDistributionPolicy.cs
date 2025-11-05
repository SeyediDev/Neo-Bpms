namespace Neo.Bpms.Domain.Models.WorkManagement;

public class WorkDistributionPolicy(
    eWorkAllocationPolicy policy = eWorkAllocationPolicy.AllocateToUser,
    eAllocationOrOfferingAlgorithm algorithm = eAllocationOrOfferingAlgorithm.Rotational,
    string rankingFormula = null,
    bool needsAllocationBeforePerform = false,
    int maxGroupOfferedUsers = 5)
{
    public eWorkAllocationPolicy policy = policy;
    public eAllocationOrOfferingAlgorithm algorithm = algorithm;
    public string rankingFormula = rankingFormula;
    public bool needsAllocationBeforePerform = needsAllocationBeforePerform;
    public int maxGroupOfferedUsers = maxGroupOfferedUsers;
}