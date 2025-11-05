namespace Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.Definitions
{
    public class ServiceMachineDefinition(string machineId)
    {
        public string MachineId { get; private set; } = machineId;
        private DateTime _latestAliveTime = DateTime.Now;
        private bool _hadUnsuccessfulConnection;
        public ConcurrentDictionary<string, FunctionInMachineDefinition> Functions { get; } = new ConcurrentDictionary<string, FunctionInMachineDefinition>();

        public void UpdateAlivedTime()
        {
            _latestAliveTime = DateTime.Now;
            YouAreConnected();
        }

        public void YouAreConnected()
        {
            _hadUnsuccessfulConnection = false;
        }
        public void HadUnsuccessfulConnection()
        {
            _hadUnsuccessfulConnection = true;
        }

        public FunctionInMachineDefinition GetFunction(string functionName)
        {
            return !Functions.TryGetValue(functionName, out FunctionInMachineDefinition value) ? null : value;
        }

        public bool IsAlive()
        {
            //todo Decide on 125 seconds criteria for a bad machine and don't hardcode it.
            return !_hadUnsuccessfulConnection && DateTime.Now.Subtract(_latestAliveTime) < TimeSpan.FromSeconds(125);
        }

        public bool HasReadyResourceFor(string functionName)
        {
            return Functions.Values
                            .Any(f => f.FuncName == functionName &&
                                      f.Resources.Any(rs => rs.State == ResourceStates.Idle));
        }
        public bool IsReadyFor(string functionName)
        {
            return IsAlive() && HasReadyResourceFor(functionName);
        }

        public IEnumerable<ServiceResource> GetWorkingResources()
        {
            return Functions.Values
                            .Where(f => !f.Immediate)
                            .SelectMany(f => f.GetWorkingResources());
        }

        public IEnumerable<SingleResourceInfo> GetResourcesInfo()
        {
            return Functions.Values.SelectMany(f => f.GetResourcesInfo(this));
        }
    }
}