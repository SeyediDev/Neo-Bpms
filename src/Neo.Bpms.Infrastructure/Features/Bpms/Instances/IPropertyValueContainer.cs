namespace Neo.Bpms.Infrastructure.Features.Bpms.Instances;

public interface IPropertyValueContainer
{
    ElasticObject Data { get; }
    bool SetData(string name, object value);
    bool GetData(string name, out object value, IList<string> fields = null);
    string FetchData(string name);
}
