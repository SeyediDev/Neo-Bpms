namespace Neo.Bpms.Domain.Models.Cmmn;

public enum EntityFieldPropertyId
{
    DisplayFields = 50,
    Multiple = 100,
    UseFileServer,
    ShowDocumentInPage,
    DocumentType
}

public class FieldProperty
{
    public EntityFieldPropertyId PropertyId { get; set; }
    public object Value { get; set; }

    public FieldProperty Clone()
    {
        return new FieldProperty
        {
            PropertyId = PropertyId,
            Value = Value
        };
    }
}
