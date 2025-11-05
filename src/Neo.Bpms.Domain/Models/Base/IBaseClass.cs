namespace Neo.Bpms.Domain.Models.Base;

public interface IBaseClass : IBaseClassId<string>
{
    //string Id { get; set; }
    string Name { get; set; }
}