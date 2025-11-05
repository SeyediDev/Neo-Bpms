namespace Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.MonitoringInfo
{
    public class SingleResourceInfo
    {
        public SingleResourceInfo()
        {
        }
        internal SingleResourceInfo(ServiceMachineDefinition machine, ServiceResource resource)
        {
            CurrentRequestId = resource.CurrentRequestId;
            Progress = resource.Progress;
            IsWorking = resource.IsWorking();
            IsImmediate = resource.Function.Immediate;
            FunctionName = resource.Function.FuncName;
            Url = resource.Function.Url;
            MachineId = machine.MachineId;
            IsAlive = machine.IsAlive();
        }

        public string MachineId { get; set; }
        public bool IsAlive { get; set; }
        public string FunctionName { get; set; }
        public string Url { get; set; }
        public long CurrentRequestId { get; set; }
        public bool IsImmediate { get; set; }
        public bool IsWorking { get; set; }
        public int Progress { get; set; }
    }
}