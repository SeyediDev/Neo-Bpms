namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    private InputOutputSpecification _curInputOutputSpecification;

    protected void AddIoSpecification(string id)
    {
        _curInputOutputSpecification = new InputOutputSpecification(process, id);
        process.ioSpecification = _curInputOutputSpecification;
    }

    protected void AddDataInput(string id, string name, bool isCollection = false)
    {
        _curInputOutputSpecification.dataInputs ??= [];
        DataInput curDataInput = new(_curInputOutputSpecification, id, name, null, isCollection);
        _curInputOutputSpecification.dataInputs.Add(curDataInput);
    }
}
