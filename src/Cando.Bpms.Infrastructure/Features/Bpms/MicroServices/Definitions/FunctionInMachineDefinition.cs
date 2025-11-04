namespace Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.Definitions
{
    public class FunctionInMachineDefinition
    {
        public string FuncName { get; set; }
        public int ResourceCount { get; set; }
        public string Url { get; set; }
        public long RefreshTime { get; set; }//milliseconds
        public string PollingAddress { get; set; }
        public List<ServiceResource> Resources { get; }
        public bool Immediate { get; set; }
        internal ServiceMachineDefinition Machine { get; set; }

        public FunctionInMachineDefinition()
        {
            Resources = [];
        }

        public void InstantiateStateForEachResource()
        {
            for (var i = 0; i < ResourceCount; i++)
            {
                Resources.Add(new ServiceResource(this, i));
            }
        }

        public ServiceResource GetResource(int resourceIndex)
        {
            if (resourceIndex < 1 || resourceIndex > Resources.Count)
                return null;
            return Resources[resourceIndex - 1];
        }

        public ServiceResource GetFirstReadyResource()
        {
            return Resources.FirstOrDefault(rs => rs.State == ResourceStates.Idle);
        }

        public IEnumerable<ServiceResource> GetWorkingResources()
        {
            return Resources.Where(r => r.IsWorking());
        }

        public int IdleResourcesCount()
        {
            return Resources.Count(r => !r.IsWorking());
        }

        internal IEnumerable<SingleResourceInfo> GetResourcesInfo(ServiceMachineDefinition machine)
        {
            return Resources.Select(r => new SingleResourceInfo(machine, r));
        }
    }
}
