using Neo.Bpms.Infrastructure.Features.Bpms.Loader.Dto;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeGlobalTask;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Loader;

public partial class Repository(ILogger<Repository> Logger) : IBpmsRepository
{
    public ConcurrentDictionary<string, ProcessRunTime> processesRunTimes { get; set; } =
        new ConcurrentDictionary<string, ProcessRunTime>();

    public ConcurrentDictionary<long, FlowNodeRunTime> FlowNodeRunTimes { get; set; } =
        new ConcurrentDictionary<long, FlowNodeRunTime>();
    public ConcurrentDictionary<long, ProcessVersionRuntime> ProcessVersionRunTimes { get; set; } =
        new ConcurrentDictionary<long, ProcessVersionRuntime>();

    public Dictionary<string, GlobalTaskRunTime> globalTasks { get; set; } = [];

    /// <summary>
    /// collaborations and choreographic
    /// </summary>
    internal Dictionary<string, MessageCatches> MessagesCatches { get; set; } = [];
    internal Dictionary<string, List<SignalCatchRuntime>> signalCatches = [];
}
