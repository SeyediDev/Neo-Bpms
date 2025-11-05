namespace Neo.Bpms.Infrastructure.Features.Bpms.MicroServices
{
    internal class MicroservicesStatusChecker(MicroservicesDefinitions microservices)
    {
        public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;

        public void CheckStatus(object stateInfo)
        {
            try
            {
                foreach (var resource in microservices.GetWorkingResources())
                {
                    resource.QueueIfTimedOut();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
            }
        }
    }
}