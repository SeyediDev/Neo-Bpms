namespace Neo.Bpms.Domain.Features.MetaDefinitions.Entities;

public abstract class DocumentControlDefinitions: BaseModelingDefinition
{
    public abstract string Name { get; }
    public virtual bool Multiple { get; } = false;
    public virtual bool NeedToConfirm { get; } = false;
    public virtual bool UseFileServer { get; } = false;
    public virtual bool ShowDocumentInPage { get; } = false;
    public virtual int? DocumentType { get; }
}
public abstract class FormulaDefinitions : BaseModelingDefinition
{
    public abstract string Name { get; }
    public abstract string Formula { get; }
    public abstract Type Type { get; }
}
