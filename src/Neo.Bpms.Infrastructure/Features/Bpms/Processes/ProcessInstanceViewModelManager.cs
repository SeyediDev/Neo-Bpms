using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes;

public static class ProcessInstanceViewModelManager
{
    public static IEnumerable<ProcessInstanceViewModel> GetProcessInstancesViewModels(long? specificPiId)
    {
        IEnumerable<ProcessInstanceViewModel> res = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.processesRunTimes?.Values.SelectMany(ProcessVersionSelector(specificPiId))
            .Take(1000);
        return res;
    }

    private static Func<ProcessRunTime, IEnumerable<ProcessInstanceViewModel>> ProcessVersionSelector(long? specificPiId)
    {
        return pr =>
            pr.Versions?.Values.SelectMany(ProcessInstanceSelector(specificPiId));
    }

    private static Func<ProcessVersionRuntime, IEnumerable<ProcessInstanceViewModel>> ProcessInstanceSelector(long? specificPiId)
    {
        AuditTrail auditTrail = new(TriggerTypeId.View, "GetProcessInstancesViewModels");
        ExecutionInstance execution = new(auditTrail, null);
        return prv => (specificPiId.HasValue && specificPiId.Value > 0
                ? [DataStorage.FetchPi(prv, specificPiId.Value, execution)]
                : DataStorage.LoadProcessInstances(prv, execution).Values.ToList())
        .Select(pi => new ProcessInstanceViewModel(pi));
    }
}
