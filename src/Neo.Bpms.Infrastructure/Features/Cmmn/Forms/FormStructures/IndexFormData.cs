namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormStructures;

public class IndexFormData
{
    public IndexFormData()
    {

    }
    public IndexFormData(ElasticObject filterValues)
    {
        FilterValues = filterValues;
        recordCount = 0;
    }
    public List<ElasticObject> Rows { get; set; } = [];
    public ElasticObject FilterValues;
    public long recordCount;
    public string AlreadySelectedsJson { get; set; }
}
