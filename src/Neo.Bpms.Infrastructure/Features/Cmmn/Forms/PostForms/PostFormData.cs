using Neo.Bpms.Domain.Models.Bpmn.Extensions;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

public class PostFormData(string culture, Form inputForm, ElasticObject formData)
{
    public Form InputForm { get; set; } = inputForm;
    public ElasticObject FormData { get; set; } = formData;
    public string Culture { get; set; } = culture;
    public LocalParameters InputData => FormData?.ToLocalParameters();
    public Form form { get; set; } = inputForm;
    public CommonFormStructure structure { get; set; }
    public ExceptionInfos errors { get; set; } = [];
    public bool IsValid => errors == null || !errors.Any();
    public bool RedirectToWorkItems { get; set; }
    public string parentNamespaceId { get; set; }
    public string parentEntityId { get; set; }
    public string parentFormSubjectId { get; set; }
    public string subTableAssociationFeildId { get; set; }
    public string parentIds { get; set; }
    public long? WorkItemId { get; set; }
    public string EntityPkv { get; set; }
    public string WorkDescription { get; set; } = formData?.GetString(RenderingForm.WorkDescription);
    public void AddError(Exception exception)
    {
        errors ??= [];
        errors.Add(new ExceptionInfo { Exception = exception });
    }

    public void FetchParentParams(ElasticObject record)
    {
        parentNamespaceId = record.GetString("__parentNamespaceId");
        parentEntityId = record.GetString("__parentEntityId");
        parentFormSubjectId = record.GetString("__parentFormSubjectId");
        subTableAssociationFeildId = record.GetString("__subTableAssociationFieldId");
        parentIds = record.GetString("__parentIds");
    }
}
