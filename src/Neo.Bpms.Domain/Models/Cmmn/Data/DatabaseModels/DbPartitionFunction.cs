namespace Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;

public class DbPartitionFunction
{
    public long Id;
    public string Name;
    public string Type;
    public string BoundaryValueOnRight;
    public Dictionary<long, Parameter> Parameters = [];
    public List<RangeValue> RangeValues = [];

    public class Parameter
    {
        public long Id { get; set; }
        public string Type { get; set; }
    }

    public class RangeValue
    {
        public string BoundaryId { get; set; }
        public Parameter Parameter;
        public string Value { get; set; }
    }
}
