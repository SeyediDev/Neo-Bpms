namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;

public class Gateway(IFlowElementsContainer flowElementsContainer, string id, string name, Gateway.eGatewayType gatewayType) : GatewayBase(flowElementsContainer, id, name)
{
    public eGatewayType gatewayType = gatewayType;

    public enum eGatewayType
    {
        /// <summary>
        /// Converging or Diverging
        /// </summary>
        Exclusive = 1,

        /// <summary>
        /// Converging or Diverging
        /// </summary>
        Inclusive,

        /// <summary>
        /// Converging or Diverging
        /// </summary>
        Parallel,

        /// <summary>
        /// Converging or Diverging
        /// </summary>
        Complex,

        /// <summary>
        /// Diverging
        /// </summary>
        EventBased
    }
}