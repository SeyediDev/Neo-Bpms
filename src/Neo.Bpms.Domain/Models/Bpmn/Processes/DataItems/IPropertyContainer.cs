namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;

public interface IPropertyContainer : IDataElementContainer
{
    List<Property> properties { get; set; }
}
