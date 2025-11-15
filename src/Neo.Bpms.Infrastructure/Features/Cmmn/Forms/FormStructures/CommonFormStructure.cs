using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Security.Authorization;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Common;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Logic;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormStructures;

public class CommonFormStructure
{
    public CommonFormStructure()
    {
    }
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string FormSubjectId { get; set; }
    public string Form_ReportId { get; set; }
    public string Name { get; set; }
    public string ConfigId { get; set; }
    public List<InputFieldDefinition> Fields { get; set; } = [];
    public List<string> KeyFields { get; set; } = [];
    public FormLogicDefinition Logic { get; set; } = new();
    public IDictionary<string, ComboData> CombosData { get; set; } = new Dictionary<string, ComboData>();
    public List<TableDefinition> Tables => [.. Fields.OfType<TableDefinition>()];
    public ICollection<IdentityRole> UserGroups { get; set; }

    public ElasticObject FilterValues { get; set; }
    public List<ColumnFieldDefinition> ColumnInfos { get; set; } = [];
    public List<IndexFormSubjectId> Subjects { get; set; } = [];
    public List<FormLinkId> BulkEdits { get; set; }
    public List<ProcessCreateFormLinkId> BulkProcessCreates { get; set; }
    public string EditFormId { get; set; }
    public string EditAction { get; set; } = "Edit";
    public string DeleteFormId { get; set; }
    public string DetailFormId { get; set; }
    public string DetailAction { get; set; } = "Details";
    public string CreateFormId { get; set; }

    public bool HasEdit => !string.IsNullOrEmpty(EditFormId);
    public bool HasDelete => !string.IsNullOrEmpty(DeleteFormId);
    public bool HasDetails => !string.IsNullOrEmpty(DetailFormId);
    public bool HasCreate => !string.IsNullOrEmpty(CreateFormId);
    public bool HasServiceOperation { get; set; }
    public string ParentReportIds { get; set; }
    public bool hasMandatoryFilter { get; set; }
    public Form.eFormType FormType { get; set; }

    public string FormAction
    {
        get
        {
            return FormType switch
            {
                Form.eFormType.Index => Form.eFormType.Index.ToString(),
                Form.eFormType.Create => Form.eFormType.Create.ToString(),
                Form.eFormType.Edit => Form.eFormType.Edit.ToString(),
                Form.eFormType.Delete => Form.eFormType.Delete.ToString(),
                Form.eFormType.VirtualDelete => "Delete",
                Form.eFormType.Detail => Form.eFormType.Detail.ToString(),
                Form.eFormType.BulkEdit => Form.eFormType.BulkEdit.ToString(),
                //                    case Form.eFormType.SpecificURLForRecord:
                //						return Form.eFormType.BulkEdit.ToString();						
                //					case Form.eFormType.SpecificURL:
                //						break;
                Form.eFormType.ProcessCreate => "Create",
                Form.eFormType.WorkItem or Form.eFormType.ActiveProcessInstance or Form.eFormType.ProcessInstance => "Edit",
                Form.eFormType.ProcessCreateOnExistingRecord => "BulkCreateProcess",
                Form.eFormType.Report => "Index",
                _ => "",
            };
        }
    }

    public List<ConfiguredFilter> ConfiguredFilters { get; set; }
    public List<ConfiguredFolder> ConfiguredFolders { get; set; }

    public List<ConfigTreeItem> FiltersTree
    {
        get
        {
            List<ConfigTreeItem> result = ConfiguredFolders?.Where(f => !f.IsForConfig)
                .OrderBy(f => f.IsPublic.ToString() + string.Join(",", f.Roles ?? []) + f.Name)
                .Select(f =>
                {
                    ConfiguredFolder folder = ConfiguredFolders?.FirstOrDefault(ff => ff.Id == f.FolderId);
                    return new ConfigTreeItem
                    {
                        id = $"folder-{f.Id}",
                        data = new
                        {
                            isPublic = f.IsPublic,
                            userGroupId = string.Join(",", f.Roles ?? []),
                            folderId = f.Id.ToString(),
                            isForConfig = false
                        },
                        parent = folder == null ? "#" : $"folder-{folder.Id}",
                        text = f.Name,
                        type = "default"
                    };
                })
                .ToList();
            result?.AddRange(ConfiguredFilters
                .OrderBy(f => f.IsPublic.ToString() + string.Join(",", f.Roles ?? []) + f.Name)
                .Select(filter =>
                {
                    ConfiguredFolder folder = ConfiguredFolders?.FirstOrDefault(ff => ff.Id == filter.FolderId);
                    return new ConfigTreeItem
                    {
                        id = $"filter-{filter.Id}",
                        data = new
                        {
                            isPublic = filter.IsPublic,
                            isDefault = filter.IsDefault,
                            filterId = filter.Id.ToString(),
                            configId = filter.ConfigId
                        },
                        parent = folder == null ? "#" : $"folder-{folder.Id}",
                        text = filter.Name,
                        type = "filter"
                    };
                }));
            return result;
        }
    }

    public bool IsBulk => Form.IsBulk(FormType);
    public List<string> PassingParameters { get; set; }
    public bool HasTemplate { get; set; }
    public FormSortType FormSortType { get; set; }
}
