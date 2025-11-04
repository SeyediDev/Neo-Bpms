namespace Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.Definitions
{
    public class ServiceFunctionDefinition(string functionName)
    {
        public string FuncName { get; set; } = functionName;
        public Dictionary<string, FunctionInMachineDefinition> ProviderMachines { get; } = [];
        public int IdleResourcesCount => ProviderMachines.Values.Where(m => m.Machine.IsAlive()).Sum(m => m.IdleResourcesCount());

        public void UpdateOrAddProviderMachineFunction(string machineId, FunctionInMachineDefinition function)
        {
            if (ProviderMachines.ContainsKey(machineId))
                ProviderMachines[machineId] = function;
            else
                ProviderMachines.Add(machineId, function);
        }
    }
}