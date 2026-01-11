using Neo.Bpms.Domain.Entities.CmmnConfig;
using Neo.Bpms.Domain.Entities.ProcessModel;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Infrastructure.Features.Bpms.Engine;

namespace Neo.Bpms.UI.MVC.Controllers;

public class TreeController : BpmsController
{
    [HttpGet]
    public JsonResult ProcessFramework()
    {
        CheckProcessesDesignAccess(GetUser(), false);
        List<TreeViewModel> tree = CreateProcessTree();
        return Json(tree);
    }

    [HttpPost]
    public JsonResult Save(long? id, TreeNodeTypeId nodeTypeId, string name,
        string standardCode, long? parentId, double? orderId, string purpose = null, string description = null, BusinessRuleTypeId? businessRuleTypeId = null)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        TreeConfig treeConfig = (id.HasValue ? TreeConfigManager.Fetch(nodeTypeId, id.Value) : null)
            ?? new TreeConfig();

        if (string.IsNullOrEmpty(standardCode))
            throw new ValidationException("Please set code.");
        if (string.IsNullOrEmpty(name))
            throw new ValidationException("Please set name.");
        if (standardCode.Contains(' '))
            throw new ValidationException("Code can't contain space character.");
        if (char.IsDigit(standardCode[0]))
            throw new ValidationException("Code can't start with numbers.");

        treeConfig.NodeTypeId = nodeTypeId;
        treeConfig.Name = name;
        treeConfig.ParentId = parentId;
        treeConfig.StandardCode = standardCode;
        if (!string.IsNullOrEmpty(purpose) && purpose != "undefined")
            treeConfig.Purpose = purpose;
        if (!string.IsNullOrEmpty(description) && description != "undefined")
            treeConfig.Description = description;
        treeConfig.BusinessRuleTypeId = businessRuleTypeId;
        if (orderId.HasValue)
            treeConfig.OrderId = orderId.Value;
        SaveTreeNode(treeConfig);
        return Json(treeConfig.Id);
    }

    [HttpPost]
    public JsonResult Drag(TreeNodeTypeId nodeTypeId, long id, long? parentId, double? orderId)
    {
        CheckProcessesDesignAccess(GetUser(), true);
        TreeConfig treeConfig = TreeConfigManager.Fetch(nodeTypeId, id);
        treeConfig.ParentId = parentId;
        if (orderId.HasValue)
            treeConfig.OrderId = orderId.Value;
        SaveTreeNode(treeConfig);
        return Json(treeConfig.Id);
    }

    [HttpPost]
    public JsonResult Delete(long id, TreeNodeTypeId? nodeTypeId)
    {
        IdentityUser user = GetUser();
        CheckProcessesDesignAccess(user, true);

        TreeConfig treeConfig = TreeConfigManager.Delete(id, nodeTypeId, user);
        switch (treeConfig?.NodeTypeId)
        {
            case TreeNodeTypeId.ProcessCategory:
            case TreeNodeTypeId.ProcessGroup:
                break;
            case TreeNodeTypeId.Process:
                ProjectProcess.RemoveProcess(treeConfig.StandardCode);
                break;
            case TreeNodeTypeId.ProcessVersion:
                {
                    TreeConfig processTree = TreeConfigManager.Fetch(TreeNodeTypeId.Process, treeConfig.ParentId ?? 0);
                    if (processTree != null)
                        ProjectProcess.RemoveProcessVersion(processTree.StandardCode, treeConfig.StandardCode);
                    break;
                }
        }
        return Json(treeConfig?.Id);
    }

    private void SaveTreeNode(TreeConfig treeConfig)
    {
        IdentityUser user = GetUser(User);
        switch (treeConfig.NodeTypeId)
        {
            case TreeNodeTypeId.ProcessCategory:
            case TreeNodeTypeId.ProcessGroup:
            case TreeNodeTypeId.MainProcess:
            case TreeNodeTypeId.SubProcess:
            case TreeNodeTypeId.System:
            case TreeNodeTypeId.SubSystem:
            case TreeNodeTypeId.BusinessSegment:
            case TreeNodeTypeId.BusinessUnit:
            case TreeNodeTypeId.Department:
            case TreeNodeTypeId.OrganizationUnit:
            case TreeNodeTypeId.BusinessDomain:
            case TreeNodeTypeId.BusinessPolicy:
            case TreeNodeTypeId.BusinessPolicyGroup:
            case TreeNodeTypeId.RoleGroup:
                TreeConfigManager.Save(treeConfig, user);
                break;
            case TreeNodeTypeId.Process:
                SaveProcessNode(treeConfig, user);
                break;
            case TreeNodeTypeId.ProcessVersion:
                SaveProcessVersionNode(treeConfig);
                break;
            case TreeNodeTypeId.BusinessRule:
                TreeConfigManager.Save(treeConfig, user);
                TreeConfig bsversion = new()
                {
                    Id = 0,
                    NodeTypeId = TreeNodeTypeId.BusinessRuleVersion,
                    Name = "version 1.0",
                    ParentId = treeConfig.Id,
                    Description = null,
                    StandardCode = "1.0"
                };
                TreeConfigManager.Save(bsversion, user);
                SaveNewBusinessRule(treeConfig, user);
                break;
            case TreeNodeTypeId.BusinessRuleVersion:
                TreeConfigManager.Save(treeConfig, user);
                BusinessRule b = QueryUtility<BusinessRule>
                    .Where($"{nameof(BusinessRule.Name)}=='{treeConfig.Name}'")
                    .FirstOrDefault<BusinessRule>();//todo ??!!
                ApplyUtility<BusinessRuleVersion>.Insert(new BusinessRuleVersion { BusinessRuleId = b.Id, StateId = 1, Code = treeConfig.StandardCode, Name = treeConfig.Name }, false, user);
                break;

        }
    }

    private static void SaveNewBusinessRule(TreeConfig treeConfig, IdentityUser user)
    {
        BusinessRule businessRule = new()
        {
            Name = treeConfig.Name,
            BusinessRuleTypeId = treeConfig.BusinessRuleTypeId ?? 0,
            Code = treeConfig.StandardCode,
            Description = treeConfig.Description,
            Purpose = treeConfig.Purpose,
            StateId = 1
        };
        ApplyUtility<BusinessRule>.Insert(businessRule, false, user);
        ApplyUtility<BusinessRuleVersion>.Insert(new BusinessRuleVersion { BusinessRuleId = businessRule.Id, StateId = 1, Code = "1.0", Name = treeConfig.Name }, false, user);
    }

    private static void SaveProcessNode(TreeConfig treeConfig, IdentityUser user)
    {
        TreeConfigManager.Save(treeConfig, user);
        BusinessProcess businessProcess = null;
        BusinessProcessVersion businessProcessVersion = null;
        ProjectProcess.AddBusinessProcessIfIsNull(
            ref businessProcess, ref businessProcessVersion,
            treeConfig.StandardCode, treeConfig.Name, null, "1.0");

        treeConfig.Id = AppendProcessToTree(businessProcess);
    }

    private static void SaveProcessVersionNode(TreeConfig treeConfig)
    {
        TreeConfig processTree = TreeConfigManager.Fetch(TreeNodeTypeId.Process, treeConfig.ParentId ?? 0);
        if (processTree == null) return;
        BusinessProcess businessProcess = null;
        BusinessProcessVersion businessProcessVersion = null;
        ProjectProcess.AddBusinessProcessIfIsNull(
        ref businessProcess, ref businessProcessVersion,
        processTree.StandardCode, treeConfig.Name, null, treeConfig.StandardCode);
        treeConfig.Id = AppendProcessVersionToTree(processTree, businessProcess, businessProcessVersion);
    }

    private static List<TreeViewModel> CreateProcessTree()
    {
        List<TreeViewModel> tree = [];
        lock ("DoSetTreeParams")
        {
            if (!BusinessProcess.DoSetTreeParams)
            {
                AppendProcessesToTree();
                BusinessProcess.DoSetTreeParams = true;
            }
        }
        foreach (TreeTypeItem treeTypeItem in TreeConfigManager.TreeTypes.Values.ToList())
        {
            foreach (TreeConfig treeConfig in treeTypeItem.Items.Values.OrderBy(i => i.OrderId).ToList())
            {
                CreateProcessTreeNode(tree, treeConfig);
            }
        }
        return tree;
    }

    private static void CreateProcessTreeNode(ICollection<TreeViewModel> tree, TreeConfig treeConfig)
    {
        switch (treeConfig.NodeTypeId)
        {
            case TreeNodeTypeId.ProcessCategory:
            case TreeNodeTypeId.ProcessGroup:
            case TreeNodeTypeId.MainProcess:
            case TreeNodeTypeId.SubProcess:
            case TreeNodeTypeId.System:
            case TreeNodeTypeId.SubSystem:
            case TreeNodeTypeId.BusinessSegment:
            case TreeNodeTypeId.BusinessUnit:
            case TreeNodeTypeId.Department:
            case TreeNodeTypeId.OrganizationUnit:
            case TreeNodeTypeId.BusinessRule:
            case TreeNodeTypeId.BusinessRuleVersion:
            case TreeNodeTypeId.BusinessDomain:
            case TreeNodeTypeId.BusinessPolicy:
            case TreeNodeTypeId.BusinessPolicyGroup:
            case TreeNodeTypeId.RoleGroup:
                tree.Add(new TreeViewModel(treeConfig, 0));
                break;
            case TreeNodeTypeId.Process:
                if (ProjectDefinition.Project.ProcessExists(treeConfig.StandardCode))
                {
                    ProcessRunTime processRuntime = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine)
                        .Repository.GetProcessRuntime(treeConfig.StandardCode);
                    tree.Add(new TreeViewModel(treeConfig, processRuntime?.DbId ?? 0));
                }
                break;
            case TreeNodeTypeId.ProcessVersion:
                TreeConfig processTree = TreeConfigManager.Fetch(TreeNodeTypeId.Process, treeConfig.ParentId ?? 0);
                if (processTree != null)
                {
                    BusinessProcess businessProcess = ProjectDefinition.Project.GetBusinessProcess(processTree.StandardCode);
                    BusinessProcessVersion version = businessProcess?.GetVersion(treeConfig.StandardCode);
                    ProcessVersionRuntime processVersionRuntime = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.GetProcessVersionRuntime(processTree.StandardCode,
                        treeConfig.StandardCode);
                    if (version?.BpmnDefinitions != null)
                        tree.Add(new TreeViewModel(treeConfig, processVersionRuntime?.DbId ?? 0) { data = new { version.IsActive } });
                }
                break;
        }
    }

    private static void AppendProcessesToTree()
    {
        foreach (BusinessProcess businessProcess in ProjectDefinition.Project.BusinessProcesses.Values)
            AppendProcessToTree(businessProcess);
    }

    private static long AppendProcessToTree(BusinessProcess businessProcess)
    {
        TreeConfig treeConfig = TreeConfigManager.Fetch(TreeNodeTypeId.Process, businessProcess.Id, 0, null,
            businessProcess.Name);
        if (!treeConfig.ParentId.HasValue || treeConfig.ParentId.Value == 0)
        {
            TreeConfig processCategory = TreeConfigManager.Fetch(TreeNodeTypeId.ProcessCategory, "NotCategorizedCategory", 0, null, "طبقه‌بندی نشده");
            TreeConfig processGroup = TreeConfigManager.Fetch(TreeNodeTypeId.ProcessGroup, "NotCategorizedGroup", 0, processCategory.Id, "گروه بندی نشده");
            treeConfig.ParentId = processGroup.Id;
        }
        foreach (BusinessProcessVersion processVersion in businessProcess.Versions.Values)
            AppendProcessVersionToTree(treeConfig, businessProcess, processVersion);
        return treeConfig.Id;
    }

    private static long AppendProcessVersionToTree(TreeConfig processTreeConfig,
        BusinessProcess businessProcess, BusinessProcessVersion processVersion)
    {
        TreeConfig treeConfig = TreeConfigManager.Fetch(TreeNodeTypeId.ProcessVersion,
            processVersion.Id, 0, processTreeConfig.Id,
            $"{businessProcess.Name} Version {processVersion.Id}");
        return treeConfig.Id;
    }
}
