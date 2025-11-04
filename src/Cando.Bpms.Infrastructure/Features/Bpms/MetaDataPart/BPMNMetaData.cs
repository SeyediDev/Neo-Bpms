using Neo.Bpms.Domain.Modeling.Entities.ProcessModel;

namespace Neo.Bpms.Infrastructure.Features.Bpms.MetaDataPart;

public class BPMNMetaData : MetaData
{
    protected override void Init()
    {
        Interfaces = ToDictionaryByUnique<BPMNInterface>("InterfaceId");
        Operations = ToDictionaryOfDictionary<BPMNOperation>("InterfaceId", "OperationId");
        Processes = ToDictionaryByUnique<BPMNProcess>("ProcessId");
        ProcessVersions = ToDictionaryOfDictionary<BPMNProcessVersion>("ProcessId", "Version");
        FlowNodes = ToDictionaryOfDictionary<BPMNFlowNode>("ProcessVersionId", "FlowNodeId");
    }

    public Dictionary<string, BPMNInterface> Interfaces { get; set; }
    public Dictionary<long, Dictionary<string, BPMNOperation>> Operations { get; set; }
    public Dictionary<string, BPMNProcess> Processes { get; set; }
    public Dictionary<long, Dictionary<string, BPMNProcessVersion>> ProcessVersions { get; set; }
    public Dictionary<long, Dictionary<string, BPMNFlowNode>> FlowNodes { get; set; }
}
