namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems;

public interface IPropertyContainer : IDataElementContainer
{
    List<Property> properties { get; set; }
}
