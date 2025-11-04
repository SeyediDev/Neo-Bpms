namespace Neo.Bpms.Domain.Entities.Base;

public interface IBaseClass : IBaseClassId<string>
{
    //string Id { get; set; }
    string Name { get; set; }
}