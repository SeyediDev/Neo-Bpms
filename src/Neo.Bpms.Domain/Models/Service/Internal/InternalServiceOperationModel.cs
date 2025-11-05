namespace Neo.Bpms.Domain.Models.Service.Internal;

public abstract class InternalServiceOperationModel<TIn, TOut>(string name) 
    : ServiceOperationDefinition(null, name, name, null, null), IInternalServiceOperation
    where TIn : new()
    where TOut : new()
{
    private Type _runtimeType { get; set; }

    public Type RunTimeType
    {
        get => _runtimeType;
        set => _runtimeType = value;
    }

    //public abstract TOut RunOperation(AuditTrail auditTrail, TIn input);
    //public override void RunOperation(AuditTrail auditTrail, LocalParameters inData, out LocalParameters outData)
    //{
    //	var inputParams = inData.To<TIn>();
    //	var outputParams = RunOperation(auditTrail, inputParams);
    //	outData = LocalParameters.ToLocalParameters(outputParams);
    //}
    public Type InputStructure => typeof(TIn);
    public Type OutputStructure => typeof(TOut);
}
