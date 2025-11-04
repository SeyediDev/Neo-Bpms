namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormStructures;

public class ServiceOperationFormData
{
    public ServiceOperationFormData()
    {

    }
    public ServiceOperationFormData(ElasticObject filterValues)
    {
        FilterValues = filterValues;
    }
    public ElasticObject Response = new();
    public ElasticObject FilterValues;
    public string AlreadySelectedsJson { get; set; }
}