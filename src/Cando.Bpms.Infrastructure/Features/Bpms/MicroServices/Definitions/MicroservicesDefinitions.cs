namespace Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.Definitions
{
    public class MicroservicesDefinitions
    {
        public ConcurrentDictionary<string, ServiceFunctionDefinition> FunctionDefinitions { get; }
        public ConcurrentDictionary<string, ServiceMachineDefinition> MachineDefinitions { get; }
        public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
        public MicroservicesDefinitions()
        {
            FunctionDefinitions = new ConcurrentDictionary<string, ServiceFunctionDefinition>();
            MachineDefinitions = new ConcurrentDictionary<string, ServiceMachineDefinition>();
        }

        public void Register(string machineId, IEnumerable<FunctionInMachineDefinition> functions)
        {
            var machine = ReplaceOrAddNewMachineDefinition(machineId);
            RegisterFunctionsOfMachine(functions, machine);
        }

        public IEnumerable<FunctionInMachineDefinition> GetFunctionsByMachineId(string machineId)
        {
            try
            {
                return MachineDefinitions[machineId].Functions.Values;
            }
            catch
            {
                return [];
            }
        }

        public ServiceResource GetSpecificResource(string machineId, string functionName, int resourceIndex)
        {
            if (MachineDefinitions == null || !MachineDefinitions.TryGetValue(machineId, out ServiceMachineDefinition value))
                return null;

            FunctionInMachineDefinition function;
            if (!value.Functions.TryGetValue(functionName, out function))
                return null;
            return function.GetResource(resourceIndex);
        }

        public IEnumerable<ServiceResource> GetWorkingResources()
        {
            return MachineDefinitions.Values
                //                .Where(m => m.IsAlive())
                .SelectMany(m => m.GetWorkingResources());
        }

        public int IdleResourcesCount(string functionName)
        {
            if (!FunctionDefinitions.TryGetValue(functionName, out ServiceFunctionDefinition value))
                return 0;
            return value.IdleResourcesCount;
        }

        //        public ServiceResource GetFirstReadyResource(string functionName) //no one needs it?
        //        {
        //            var machine =
        //                GetFunction(functionName)
        //                    .ProviderMachines.Values.FirstOrDefault(m => m.Resources.Any(rs => rs.State == ResourceStates.Idle));
        //            return machine?.Resources?.FirstOrDefault(rs => rs.State == ResourceStates.Idle);
        //        }

        //        public ServiceFunctionDefinition GetFunction(string functionName)
        //        {
        //            if (FunctionDefinitions == null || !FunctionDefinitions.ContainsKey(functionName))
        //                return null;
        //            return FunctionDefinitions[functionName];
        //        }

        public ServiceMachineDefinition GetFirstReadyMachine(string functionName, string preferredMachineId)
        {
            ServiceMachineDefinition preferredMachine;
            if (preferredMachineId != null && MachineDefinitions.TryGetValue(preferredMachineId, out preferredMachine) &&
                preferredMachine.IsReadyFor(functionName))
            {
                Logger.LogTrace("preferred {0} {1}", preferredMachineId, functionName);
                return preferredMachine;
            }
            return MachineDefinitions.Values
                .FirstOrDefault(machine => machine.IsReadyFor(functionName));
        }

        public IEnumerable<SingleResourceInfo> GetResourcesInfo()
        {
            return MachineDefinitions.Values.SelectMany(md => md.GetResourcesInfo());
        }

        public bool HasMachine(string machineId)
        {
            return machineId != null && MachineDefinitions.ContainsKey(machineId);
        }

        private ServiceMachineDefinition ReplaceOrAddNewMachineDefinition(string machineId)
        {
            if (MachineDefinitions.TryGetValue(machineId, out ServiceMachineDefinition value))
            {
                foreach (var workingResource in value.GetWorkingResources())
                {
                    Logger.LogTrace("Replacing resource {0} {1}",
                        workingResource.Function?.Machine?.MachineId, workingResource.Function?.FuncName);
                    workingResource.TakeBackToQueue();
                }
            }
            return MachineDefinitions[machineId] = new ServiceMachineDefinition(machineId);
        }

        private void RegisterFunctionsOfMachine(IEnumerable<FunctionInMachineDefinition> functions,
            ServiceMachineDefinition machine)
        {
            foreach (var registeringFunction in functions)
            {
                registeringFunction.InstantiateStateForEachResource();
                registeringFunction.Machine = machine;
                var functionDefinition = GetOrAddFunctionDefinition(registeringFunction.FuncName);
                functionDefinition.UpdateOrAddProviderMachineFunction(machine.MachineId, registeringFunction);
                machine.Functions.TryAdd(registeringFunction.FuncName, registeringFunction);
            }
        }

        private ServiceFunctionDefinition GetOrAddFunctionDefinition(string functionName)
        {
            ServiceFunctionDefinition functionDefinition;
            if (FunctionDefinitions.TryGetValue(functionName, out functionDefinition))
                return functionDefinition;

            functionDefinition = new ServiceFunctionDefinition(functionName);
            FunctionDefinitions.TryAdd(functionName, functionDefinition);

            return functionDefinition;
        }
    }
}