using Neo.Bpms.Domain.Models.Bpmn.Extensions;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

public class PostFormData(string culture, Form inputForm, ElasticObject formData)
{
    public Form InputForm { get; set; } = inputForm;
    public ElasticObject FormData { get; set; } = formData;
    public string Culture { get; set; } = culture;
    public LocalParameters InputData => FormData?.ToLocalParameters();
    public Form Form { get; set; } = inputForm;
    public CommonFormStructure Structure { get; set; }
    public ExceptionInfos Errors { get; set; } = [];
    public bool IsValid => Errors == null || !Errors.Any();
    public bool RedirectToWorkItems { get; set; }
    public string ParentNamespaceId { get; set; }
    public string ParentEntityId { get; set; }
    public string ParentFormSubjectId { get; set; }
    public string SubTableAssociationFeildId { get; set; }
    public string ParentIds { get; set; }
    public long? WorkItemId { get; set; }
    public string EntityPkv { get; set; }
    public string WorkDescription { get; set; } = formData?.GetString(RenderingForm.WorkDescription);
    
    public void AddError(Exception exception)
    {
        Errors ??= [];
        Errors.Add(new ExceptionInfo { Exception = exception });
    }

    public void FetchParentParams(ElasticObject record)
    {
        ParentNamespaceId = record.GetString("__parentNamespaceId");
        ParentEntityId = record.GetString("__parentEntityId");
        ParentFormSubjectId = record.GetString("__parentFormSubjectId");
        SubTableAssociationFeildId = record.GetString("__subTableAssociationFieldId");
        ParentIds = record.GetString("__parentIds");
    }
}
