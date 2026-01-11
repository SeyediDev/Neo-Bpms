using Neo.Bpms.Domain.Entities.ProcessModel;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Infrastructure.Features.BpmnConversion;
using Neo.Bpms.Infrastructure.Features.Bpms.Engine;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class ProcessController
{
    [HttpGet]
    public JsonResult DownloadProcess(string processId, string versionId)
    {
        IdentityUser user = GetUser();
        if (!CheckProcessAccess(user, processId))
            CheckProcessesDesignAccess(user, false);
        BpmnDefinitions bpmnDefinition = ProjectDefinition.Project.GetBpmnDefinition(processId, versionId) ?? throw new Exception($"فرآیند {processId} از پیش تعریف نشده است. DownloadProcess");
        versionId = bpmnDefinition.exporterVersion;
        ElasticObject diagramBpmnDefinition = ProjectProcess.GetDiagramBpmnDefinition(processId, versionId);
        string result = BpmnExporter.Export(bpmnDefinition, diagramBpmnDefinition,
             BpmnExporter.ExportType.Bpmn);
        return Json(new { xml = result, errors = bpmnDefinition.ErrorInfos, processName = bpmnDefinition.Name });
    }

    [HttpGet]
    public JsonResult DownloadProcessVersion(long? processId, long? versionId)
    {
        IdentityUser user = GetUser();
        ProcessRunTime process = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.GetProcessRuntimeByDbId(processId);
        ProcessVersionRuntime processVersion = ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.GetProcessVersionRuntimeByDbId(versionId);
        if (process == null && processVersion == null)
            return Json(new { processName = processId });
        process ??= ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.GetProcessRuntime(processVersion.ProcessId);
        processVersion ??= process.ActiveVersion;
        if (!CheckProcessAccess(user, processVersion.ProcessId))
            CheckProcessesDesignAccess(user, false);
        ElasticObject diagramBpmnDefinition = ProjectProcess.GetDiagramBpmnDefinition(processVersion.ProcessId, processVersion.VersionNo);
        BpmnDefinitions bpmnDefinition = processVersion.BusinessProcessVersion.BpmnDefinitions;
        string result = BpmnExporter.Export(bpmnDefinition, diagramBpmnDefinition,
             BpmnExporter.ExportType.Bpmn);
        return Json(new { xml = result, errors = bpmnDefinition.ErrorInfos, processName = bpmnDefinition.Name });
    }


    [HttpGet]
    public ActionResult Dmn()
    {
        IdentityUser user = GetUser();
        CheckProcessesDesignAccess(user, false);
        return View();
    }
    
    [HttpGet]
    public ActionResult BusinessRuleDesign(string businessRuleCode, string versionId, string nodeId)
    {
        IdentityUser user = GetUser();
        CheckProcessesDesignAccess(user, false);
        QueryUtility q = QueryUtility<BusinessRuleVersion>
            .Where($"(BusinessRule.Code)=='{businessRuleCode}'")
            .Where($"{nameof(BusinessRuleVersion.Code)} == '{versionId}'");
        q.Include("BusinessRule").SelectField(nameof(BusinessRule.BusinessRuleTypeId));
        ElasticObject businesRule = q.FirstOrDefault();
        return businesRule.GetEnumText(nameof(BusinessRule.BusinessRuleTypeId), BusinessRuleTypeId.DataFlow) == BusinessRuleTypeId.Dmn
            ? RedirectToAction(nameof(DmnDesign), new { BusinessRuleCode = businessRuleCode, VersionId = versionId, nodeId })
            : RedirectToAction(nameof(FormController.Edit), "Form",
            new { NamespaceId = "MetaModel", EntityId = nameof(BusinessRuleVersion), Ids = businesRule.Id });
    }

    [HttpGet]
    public ActionResult DmnDesign()
    {
        IdentityUser user = GetUser();
        CheckProcessesDesignAccess(user, false);
        SetPagePackId("Process_BpmnDesign");
        ViewBag.user = user;
        ViewBag.ContainerClass = "container-fluid";
        return View();
    }

    public ActionResult Design()
    {
        IdentityUser user = GetUser();
        CheckProcessesDesignAccess(user, false);
        return View();
    }

    public ActionResult BpmnDesign()
    {
        IdentityUser user = GetUser();
        CheckProcessesDesignAccess(user, false);
        SetPagePackId("Process_BpmnDesign");
        ViewBag.user = user;
        ViewBag.ContainerClass = "container-fluid";
        return View();
    }

    [HttpPost]
    public JsonResult SaveProcess(string processId, string versionId, string strXml)
    {
        CheckProcessesDesignAccess(GetUser(), true);

        BusinessProcess process = null;
        BusinessProcessVersion processVersion = null;
        ProjectProcess.FetchBusinessProcessIfIsNull(ref process, ref processVersion, processId, versionId);

        ErrorInformationList fatalError = [];
        bool result = false;
        if (string.IsNullOrEmpty(strXml))
            fatalError.AddFatal("Invalid(empty) process definition.", "SaveProcess", "15.0.0", "");
        if (process == null)
            fatalError.AddFatal($"Invalid process {processId}.", "SaveProcess", "15.0.1", "");
        else if (processVersion == null)
            fatalError.AddFatal($"Invalid process {processId} version {versionId}.", "SaveProcess", "15.0.2", "");
        else if (processVersion.BpmnDefinitions == null)
            fatalError.AddFatal($"Can not load BpmnDefinitions of process {processId} version {versionId}.",
                 "SaveProcess", "15.0.3", "");
        if (fatalError.Count == 0)
        {
            ProjectProcess.ImportBpmnDefinitions(process, processVersion, strXml);
            result = (!processVersion.BpmnDefinitions?.ErrorInfos.Any(e => e.Type <= eErrorLevel.Error) 
                ?? false) && 
                ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository
                .LoadBpmnDefinitions(process, processVersion, true);
        }
        ErrorInformationList errorInfo = processVersion?.BpmnDefinitions?.ErrorInfos;
        if ((errorInfo?.Count ?? 0) == 0)
            errorInfo = fatalError;
        return Json(new { success = result, errors = errorInfo });
    }

    [HttpGet]
    public JsonResult ProcessAdministratorLock(string processId, string versionId)
    {
        CheckProcessesDesignAccess(GetUser(), true);

        BusinessProcess process = null;
        BusinessProcessVersion processVersion = null;
        ProjectProcess.FetchBusinessProcessIfIsNull(ref process, ref processVersion, processId, versionId);

        ErrorInformationList fatalError = [];
        bool result = false;
        bool isLock = false;
        if (process == null)
            fatalError.AddFatal($"Invalid process {processId}.", "SaveProcess", "15.0.1", "");
        else if (processVersion == null)
            fatalError.AddFatal($"Invalid process {processId} version {versionId}.", "SaveProcess", "15.0.2", "");
        else
        {
            isLock = processVersion.AdministratorLock;
            result = true;
        }
        return Json(new { success = result, isLock, errors = fatalError });
    }
    
    [HttpPost]
    public JsonResult ProcessAdministratorLock(string processId, string versionId, bool isLock)
    {
        //todo check security add to system features
        CheckProcessesDesignAccess(GetUser(), true);

        BusinessProcess process = null;
        BusinessProcessVersion processVersion = null;
        ProjectProcess.FetchBusinessProcessIfIsNull(ref process, ref processVersion, processId, versionId);

        ErrorInformationList fatalError = [];
        bool result = false;
        if (process == null)
            fatalError.AddFatal($"Invalid process {processId}.", "SaveProcess", "15.0.1", "");
        else if (processVersion == null)
            fatalError.AddFatal($"Invalid process {processId} version {versionId}.", "SaveProcess", "15.0.2", "");
        else
        {
            processVersion.AdministratorLock = isLock;
            ApplyUtility<BPMNProcessVersion>.Update(new ElasticObject
            {
                [nameof(BPMNProcessVersion.Id)] = processVersion.DbId,
                [nameof(BPMNProcessVersion.AdministratorLock)] = processVersion.AdministratorLock
            });
            result = true;
        }
        return Json(new { success = result, isLock, errors = fatalError });
    }
    
    [HttpGet]
    public JsonResult ProcessCheckedIn(string processId, string versionId)
    {
        CheckProcessesDesignAccess(GetUser(), true);

        BusinessProcess process = null;
        BusinessProcessVersion processVersion = null;
        ProjectProcess.FetchBusinessProcessIfIsNull(ref process, ref processVersion, processId, versionId);

        ErrorInformationList fatalError = [];
        bool result = false;
        bool checkedIn = false;
        if (process == null)
            fatalError.AddFatal($"Invalid process {processId}.", "SaveProcess", "15.0.1", "");
        else if (processVersion == null)
            fatalError.AddFatal($"Invalid process {processId} version {versionId}.", "SaveProcess", "15.0.2", "");
        else
        {
            checkedIn = processVersion.CheckInUserId != null;
            result = true;
        }
        return Json(new { success = result, checkedIn, errors = fatalError });
    }
    
    [HttpPost]
    public JsonResult ProcessCheckedIn(string processId, string versionId, bool check)
    {
        CheckProcessesDesignAccess(GetUser(), true);

        BusinessProcess process = null;
        BusinessProcessVersion processVersion = null;
        ProjectProcess.FetchBusinessProcessIfIsNull(ref process, ref processVersion, processId, versionId);

        ErrorInformationList fatalError = [];
        bool result = false;
        bool checkedIn = false;
        if (process == null)
            fatalError.AddFatal($"Invalid process {processId}.", "SaveProcess", "15.0.1", "");
        else if (processVersion == null)
            fatalError.AddFatal($"Invalid process {processId} version {versionId}.", "SaveProcess", "15.0.2", "");
        else
        {
            if (check)
            {
                processVersion.CheckInUserId = GetUser().Id;
                ApplyUtility<BPMNProcessVersion>.Update(new ElasticObject
                {
                    [nameof(BPMNProcessVersion.Id)] = processVersion.DbId,
                    [nameof(BPMNProcessVersion.CheckInUserId)] = processVersion.CheckInUserId
                });
            }
            else
            {
                processVersion.CheckInUserId = null;
                ApplyUtility<BPMNProcessVersion>.Update(new ElasticObject
                {
                    [nameof(BPMNProcessVersion.Id)] = processVersion.DbId,
                    [nameof(BPMNProcessVersion.CheckInUserId)] = null
                });
            }
            checkedIn = processVersion.CheckInUserId != null;
            result = true;
        }
        return Json(new { success = result, CheckedIn = checkedIn, errors = fatalError });
    }

    [HttpGet]
    public JsonResult AllProcessesList()
    {
        CheckProcessesDesignAccess(GetUser(), false);
        var processes = ProjectDefinition.Project.BusinessProcesses?.Values.SelectMany(p => p.Versions).Select(pv =>
             new { id = pv.Value?.ProcessIdVersionId, name = pv.Value?.Name });
        return Json(processes);
    }
    
    [HttpGet]
    public JsonResult AllBusinessRuleList()
    {
        CheckProcessesDesignAccess(GetUser(), false);
        var businessRules = QueryUtility<BusinessRuleVersion>.ToList<BusinessRuleVersion>("StateId == 1").
        Select(b => new { id = b?.Id, name = b?.Name });
        return Json(businessRules);
    }
}
