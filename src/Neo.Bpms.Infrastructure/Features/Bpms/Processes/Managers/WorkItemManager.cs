using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.FilterModels;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers;

public class WorkItemManager
{
    public List<WorkItemViewModel> GetWorkItems(out long recordCount, int recordsPerPage,
        WorkItemsQueryType workItemsQueryType, IdentityUser user, string culture, WorkItemsFilter filter,
         EntityFilterInfo entityFilter)
    {
        LocalParameters lp = new(user);
        if (entityFilter?.FilterValues != null)
            entityFilter.FilterValues.SetField("user", user);
        QueryUtility q = CreateQuery(workItemsQueryType, filter, entityFilter, lp, true,
             out _, out _);
        recordCount = q.GetRecordCount(entityFilter?.FilterValues, lp);
        q.ReleaseQuery();

        lp = new LocalParameters(user);
        JoinQueriesData joinQueries;
        Dictionary<string, FormField> referFormFields;
        q = CreateQuery(workItemsQueryType, filter, entityFilter, lp, false,
             out joinQueries, out referFormFields);
        q.SetPage(filter.Page, recordsPerPage);

        List<WorkItemViewModel> ret = AddWorkItems(q, entityFilter?.FilterValues, joinQueries, referFormFields, user, culture, lp);
        q.ReleaseQuery();
        return ret;
    }

    public List<ProcessItemsViewModel> GetMyProcessesWorkItems(IdentityUser user)
    {
        WorkItemsFilter workItemsFilter = WorkItemsFilter.GetDefaultWorkItemsFilter(null, null, user.Id, 1);
        LocalParameters lp = [];
        QueryUtility qAi = QueryUtility<ActivityInstanceRecord>
            .Where($"({nameof(ActivityInstanceRecord.ActualOwnerId)}=='{user.Id}')")
            .Where(GetUserTaskStateFilter(workItemsFilter));
        AddActiveStatesFilter(qAi);
        //var qPi = qAi.Include("ProcessInstance");
        QueryUtility qFlowNode = qAi.Include("BPMNFlowNode").Where("StateId=1");
        AddFlowNodeTypeFilter(qFlowNode);
        QueryUtility qProcessVersion = qFlowNode.Include("ProcessVersion").Where("StateId=1");
        QueryUtility qProcess = qProcessVersion.Include("Process").Where("StateId=1");
        qProcess.GroupBy("ProcessId", true);
        qAi.Count("*", "Count");
        qAi.SumFormula("IF((StartTime==null),1,0)", "NotStartedCount");
        List<ElasticObject> list = qAi.ToList(lp);
        return [.. list.Select(li => new ProcessItemsViewModel(li)).Where(li => li.ProcessName != null)];
    }

    private List<WorkItemViewModel> AddWorkItems(QueryUtility q, ElasticObject filterValues,
         JoinQueriesData joinQueries,
         Dictionary<string, FormField> referFormFields, IdentityUser user, string culture,
         LocalParameters lp)
    {
        List<WorkItemViewModel> workItems = [];
        if (!q.GetDocuments(filterValues, lp)) return workItems;
        ProcessVersionRuntime process = null;
        FlowNodeRunTime flowNodeRunTime = null;
        foreach (ElasticObject record in q.GetRecords())
        {
            if (filterValues != null && joinQueries != null && referFormFields != null)
                FetchJoinQuery.SetJoinQueryReference(culture, joinQueries, q, referFormFields, record, true, user);
            WorkItemViewModel workItem = new(record);
            if (process == null || process.definition?.Id != workItem.ProcessId || process.VersionNo != workItem.ProcessVersion)
                process = GetProcessVersion(workItem.ProcessId, workItem.ProcessVersion);
            if (process != null && (flowNodeRunTime == null || flowNodeRunTime.flowNode?.Id != workItem.TaskId))
                flowNodeRunTime = GetActivity(process, workItem.TaskId);
            ActivityRuntime activityRuntime = flowNodeRunTime as ActivityRuntime;
            workItems.Add(workItem);
            workItem.NamespaceId = process?.definition?.EntityNamespaceId;
            workItem.EntityId = process?.definition?.EntityId;
            workItem.ProcessName = process?.definition?.Name;
            workItem.ActivityName = flowNodeRunTime?.flowNode?.Name;
            workItem.FlowNodeType = flowNodeRunTime?.FlowNodeTypeId;
            workItem.ActivityType = activityRuntime?.Activity?.ActivityType;
            workItem.EventType = (flowNodeRunTime?.flowNode as Event)?.TriggerType;
            workItem.DisplayFields = process?.definition?.DisplayFields;

            if (workItem.UserTaskState >= UserTaskInstanceStateId.Completed) continue;
            if (activityRuntime?.flowNode is not UserTask userTask) continue;
            UiEntity uiEntity = (UiEntity)process?.definition?.Entity ??
                 ProjectDefinition.Project.GetUiEntity(process.definition.EntityNamespaceId, process.definition.EntityId);
            AddActivityInstanceForm(uiEntity, workItem, process, userTask);
            AddProcessInstanceForms(uiEntity, workItem, userTask);
        }
        if (filterValues == null)
        {
            Parallel.ForEach(workItems.GroupBy(w => new { w.NamespaceId, w.EntityId, w.DisplayFields }), entities =>
            {
                if (string.IsNullOrEmpty(entities.Key.EntityId)) return;
                Entity entity = ProjectDefinition.Project.GetEntity(entities.Key.NamespaceId, entities.Key.EntityId);
                if (entity == null) return;
                List<string> pkvList = entities.GroupBy(g => g.EntityPkv).Select(e => e.Key).ToList();
                string filter = $"Id In ({string.Join(",", pkvList)})";
                ComboData comboData = ComboDataRoutines.GetRecords(entity, entity, true, "", filter, 1, "",
                     entities.Key.DisplayFields, null, "", pkvList.Count, false);
                foreach (WorkItemViewModel workItem in entities)
                {
                    FormDataRow entityRecord = comboData.GetRow(workItem.EntityPkv);
                    workItem.EntityDescription = entityRecord?.DisplayValue;
                    if (string.IsNullOrEmpty(workItem.EntityDescription))
                    {
                        if (string.IsNullOrEmpty(workItem.EntityPkv))
                            workItem.EntityDescription =
                                 $"{workItem.Description}<<<لطفا این پرونده را به مدیر سامانه گزارش کنید>>>{workItem.ActivityInstanceId}";
                        else
                            workItem.EntityDescription =
                                 "پرونده " + workItem.EntityPkv + " " + workItem.Description;
                    }
                }
            });
        }
        else
        {
            FetchJoinQuery.FetchJoinQueriesData(culture, lp, joinQueries);
        }
        return workItems;
    }

    private void AddActivityInstanceForm(UiEntity uiEntity, WorkItemViewModel workItem,
        ProcessVersionRuntime process, UserTask userTask)
    {
        Form form = uiEntity?.getForm(userTask.FormId);
        if (form != null)
        {
            workItem.Forms.Add(new WorkItemFormInfo
            {
                FormId = userTask.FormId,
                FormType = form.FormType,
                NamespaceId = process.definition.EntityNamespaceId,
                EntityId = process.definition.EntityId,
                FormSubjectId = form.FormSubjectId,
                Name = form.Name,
                TaskId = userTask.Id
            });
        }
    }

    private void AddProcessInstanceForms(UiEntity uiEntity, WorkItemViewModel workItem, UserTask userTask)
    {
        List<Form> processInstanceForms = uiEntity?.getForms()?.Where(f => f.FormType == Form.eFormType.ProcessInstance).ToList();
        foreach (Form processInstanceForm in processInstanceForms ?? Enumerable.Empty<Form>())
        {
            //todo check access
            workItem.Forms.Add(new WorkItemFormInfo
            {
                FormId = processInstanceForm.Id,
                FormType = processInstanceForm.FormType,
                NamespaceId = uiEntity?.NamespaceId,
                EntityId = uiEntity?.Id,
                FormSubjectId = processInstanceForm.FormSubjectId,
                Name = processInstanceForm.Name,
                TaskId = userTask.Id
            });
        }
    }

    private QueryUtility CreateQuery(WorkItemsQueryType workItemsQueryType,
            WorkItemsFilter workItemsFilter, EntityFilterInfo entityFilter, LocalParameters lp,
            bool forCount, out JoinQueriesData joinQueries,
            out Dictionary<string, FormField> referFormFields)
    {
        joinQueries = null;
        referFormFields = null;
        QueryUtility qAi = QueryUtility.New<ActivityInstanceRecord>();
        qAi.Where(GetUserTaskStateFilter(workItemsFilter));
        //DisabledFilters();			
        QueryUtility qPi = qAi.Include("ProcessInstance")?
            .SelectField("EntityPKV", "__EntityPKV");
        if (qPi == null)
            return null;

        ProcessVersionRuntime processVersion = GetProcessVersion(workItemsFilter.ProcessId, workItemsFilter.ProcessVersionId);
        QueryUtility qFlowNode = qAi.Include("BPMNFlowNode").Where("StateId=1");
        QueryUtility qProcessVersion = qFlowNode.Include("ProcessVersion").Where("StateId=1");
        QueryUtility qProcess = qProcessVersion.Include("Process").Where("StateId=1");
        if (!forCount)
        {
            if (string.IsNullOrEmpty(entityFilter?.SortFields))
            {
                qAi.OrderBy(nameof(ActivityInstanceRecord.Priority), SortType.Descending);
                qAi.OrderBy(nameof(ActivityInstanceRecord.Id), SortType.Descending);
            }
        }
        if (!string.IsNullOrEmpty(workItemsFilter.ProcessId))
        {
            if (processVersion == null)
                return null;
            if (!string.IsNullOrEmpty(workItemsFilter.EntityPkv) || entityFilter != null)
                qPi.Where($"ProcessVersionId=={processVersion.DbId}");
            else
                qAi.Where($"((BPMNFlowNode.ProcessVersionId)=={processVersion.DbId}) Or (IsNull(MainProcessId,(BPMNProcess.Id))=={processVersion.ProcessDbId})");

            if (!string.IsNullOrEmpty(workItemsFilter.Activity))
            {
                qFlowNode.Where($"FlowNodeId=='{workItemsFilter.Activity}'");
            }
        }
        if (workItemsFilter.JustRestorableTasks)
        {
            qAi.Where("0==Select('ActivityInstanceRecordView',Count((ActivityInstanceRecordView.Id))," +
                      "((ActivityInstanceRecordView.ProcessInstanceId)==(ActivityInstanceRecord.ProcessInstanceId)) And " +
                      //		                "((ActivityInstanceRecordView.Closed) == true) And " +
                      "((ActivityInstanceRecordView.CompletionTime) > (ActivityInstanceRecord.CompletionTime))" +
                      ")");
        }
        //q.Where($"1==Select('ActivityInstanceRecordView',Count((ActivityInstanceRecordView.Id)),((ActivityInstanceRecordView.ProcessInstanceId)==(ActivityInstanceRecord.ProcessInstanceId)) && ((ActivityInstanceRecordView.FlowNodeId) in ({GetExactNextActivities(process, workItemsFilter.Activity)})) && ((ActivityInstanceRecordView.userTaskStateId) < 30))");
        if (workItemsQueryType == WorkItemsQueryType.MyWorkItems)
            qAi.Where($"{nameof(ActivityInstanceRecord.ActualOwnerId)}='{workItemsFilter.UserId}'");
        if (workItemsFilter.UserGroupId > 0)
            qAi.Where($"{nameof(ActivityInstanceRecord.UserGroupId)}='{workItemsFilter.UserGroupId}'");
        if (workItemsFilter.JustRestorableTasks)
        {
            qFlowNode.Where($"FlowNodeTypeId={(int)FlowNodeTypeId.UserTask}");
            qAi.Where($"StateId={(int)ActivityInstanceStateId.Completed}");
        }
        else if (!workItemsFilter.Finished)
            AddActiveStatesFilter(qAi);

        AddDateFilters(qAi, lp, workItemsFilter);
        SelectActivityInstanceFields(qAi, qFlowNode, qProcessVersion, qProcess);
        if (workItemsQueryType != WorkItemsQueryType.MyWorkItems)
        {
            qAi.LeftOuterJoin<SystemUser>("Id", nameof(ActivityInstanceRecord.ActualOwnerId))?.Query
                 .SelectFormulaField("__UserName", "Concat((FirstName),(' '),(LastName))");
            qAi.LeftOuterJoin<SystemUserGroup>("Id", nameof(ActivityInstanceRecord.UserGroupId))?.Query
                 .SelectFormulaField("__UserGroupName", "Name");
        }

        if (!string.IsNullOrEmpty(workItemsFilter.NamespaceId) ||
            !string.IsNullOrEmpty(workItemsFilter.EntityId))
        {
            Entity entityFilterDef =
                ProjectDefinition.Project.GetEntity(workItemsFilter.NamespaceId, workItemsFilter.EntityId);
            if (entityFilterDef != null)
                qProcessVersion.Where($"MetaEntityId=={entityFilterDef.DbId}");
        }

        string namespaceId = workItemsFilter.NamespaceId;
        if (string.IsNullOrEmpty(namespaceId))
            namespaceId = processVersion?.definition.EntityNamespaceId;
        string entityId = workItemsFilter.EntityId;
        if (string.IsNullOrEmpty(entityId))
            entityId = processVersion?.definition.EntityId;
        if (!string.IsNullOrEmpty(workItemsFilter.EntityPkv))
            qPi.Where($"EntityPKV='{workItemsFilter.EntityPkv}'");
        else if (!string.IsNullOrEmpty(namespaceId) && !string.IsNullOrEmpty(entityId)
                 && !string.IsNullOrEmpty(workItemsFilter.Description) || entityFilter != null)
        {
            QueryUtility qEntity = qPi.LeftOuterJoin(namespaceId, entityId, "Id", "EntityPKV").Query;
            if (entityFilter != null)
            {
                joinQueries = new JoinQueriesData(Guid.NewGuid().ToString());
                FormQuery formQ = new(entityFilter.Form, entityFilter.Structure,
                    new CancellationToken(false) /*todo*/)
                { q = qEntity };
                referFormFields = formQ.EstablishEntityQueryForIndex(entityFilter.FilterValues,
                    entityFilter.SortFields, null, forCount, joinQueries, true, true);
            }
            if (!string.IsNullOrEmpty(workItemsFilter.Description))
            {
                string[] descriptions = workItemsFilter.Description.Split(' ');
                List<string> processFields = processVersion?.definition.DisplayFields?.Split(',').ToList();
                if (processFields == null || processFields.Count == 0)
                    processFields = [.. qEntity.Entity.DisplayStrings.Select(bf => bf.FieldId)];
                foreach (string processField in processFields)
                {
                    QueryUtility qFilter = qEntity;
                    string[] fieldIds = processField.Split('.');
                    for (int iFieldId = 0; iFieldId < fieldIds.Length - 1; iFieldId++)
                        qFilter = qFilter.Include(fieldIds[iFieldId]);
                    string fieldId = fieldIds[fieldIds.Length - 1];
                    EntityField field = qFilter.Entity.GetField(fieldId);
                    if (field == null) continue;
                    foreach (string description in descriptions)
                        AddDescriptionFilter(qFilter, field, description, 0);
                }
            }
        }
        if (workItemsFilter.JustRestorableTasks)
            qPi.Where($"StateId={(int)ProcessInstanceStateId.Activated}");
        //qPi.Where("IsNull(EntityPKV,'')!=''");

        if (!forCount)
            qAi.AddKeyAsOrderBy(SortType.Descending);

        if (workItemsQueryType == WorkItemsQueryType.MyWorkItems)
            AddFlowNodeTypeFilter(qFlowNode);
        return qAi;
    }

    private void AddFlowNodeTypeFilter(QueryUtility q)
    {
        q.Where(
             $"FlowNodeTypeId In ({(int)FlowNodeTypeId.UserTask},{(int)FlowNodeTypeId.ServiceTask})");
    }

    private void AddActiveStatesFilter(QueryUtility q)
    {
        q.Where(
             $"StateId In({(int)ActivityInstanceStateId.Ready},{(int)ActivityInstanceStateId.Active},{(int)ActivityInstanceStateId.Terminating},{(int)ActivityInstanceStateId.Failing},{(int)ActivityInstanceStateId.Compensating})");
    }

    private string GetUserTaskStateFilter(WorkItemsFilter workItemsFilter)
    {
        if (workItemsFilter.JustRestorableTasks)
            return $"userTaskStateId=={(int)UserTaskInstanceStateId.Completed}";
        List<int> desiredStates = [(int)UserTaskInstanceStateId.None];
        if (workItemsFilter.Created)
            desiredStates.Add((int)UserTaskInstanceStateId.Created);
        if (workItemsFilter.Offered)
        {
            desiredStates.Add((int)UserTaskInstanceStateId.OfferedToASingleResource);
            desiredStates.Add((int)UserTaskInstanceStateId.OfferedToMultipleResources);
        }
        if (workItemsFilter.Allocated)
        {
            desiredStates.Add((int)UserTaskInstanceStateId.AllocatedToASingleResource);
            desiredStates.Add((int)UserTaskInstanceStateId.Started);
        }
        if (workItemsFilter.Suspended)
            desiredStates.Add((int)UserTaskInstanceStateId.Suspended);
        if (workItemsFilter.Finished)
        {
            desiredStates.Add((int)UserTaskInstanceStateId.Completed);
            desiredStates.Add((int)UserTaskInstanceStateId.Failed);
        }
        return $"ISNULL(userTaskStateId,0) In({string.Join(",", desiredStates)})";
    }

    private void SelectActivityInstanceFields(QueryUtility qAi, QueryUtility qFlowNode,
        QueryUtility qProcessVersion, QueryUtility qProcess)
    {
        qAi.SelectField("Id", "__ActivityInstanceId")
        .SelectField("ProcessInstanceId", "__ProcessInstanceId")
        .SelectField(nameof(ActivityInstanceRecord.ActualOwnerId), "__ActualOwnerId")
        .SelectField(nameof(ActivityInstanceRecord.UserGroupId), "__UserGroupId")
        .SelectField("CreationTime", "__CreationTime")
        .SelectField("StartTime", "__StartTime")
        .SelectField("CompletionTime", "__CompletionTime")
        .SelectField("userTaskStateId", "__userTaskStateId")
        .SelectField("Description", "__Description");
        qProcess.SelectField("ProcessId", "__WorkflowId");
        qProcessVersion.SelectField("Version", "__WFVersion");
        qFlowNode.SelectField("FlowNodeId", "__FlowNodeId");
    }

    private void AddDateFilters(QueryUtility qAi, LocalParameters lp, WorkItemsFilter workItemsFilter)
    {
        if (workItemsFilter.FromCreationDate != null)
        {
            lp.AddOrUpdate("fct", workItemsFilter.FromCreationDate);
            qAi.Where("CreationTime>=fct");
        }
        if (workItemsFilter.ToCreationDate != null)
        {
            lp.AddOrUpdate("tct", workItemsFilter.ToCreationDate);
            qAi.Where("CreationTime<=tct");
        }
    }

    private void AddDescriptionFilter(QueryUtility qFilter, EntityField field, string description, int level)
    {
        if (field.AssociationEntity == null)
            qFilter.Where($"strany({field.Id},'{description}')", "Description");
        else
        {
            if (level >= 3) return;
            QueryUtility qIncludeFilter = qFilter.Include(field.Id);
            foreach (BasicField basicFieldId in field.AssociationEntity.Entity().DisplayStrings)
                AddDescriptionFilter(qIncludeFilter, qIncludeFilter.Entity.GetField(basicFieldId.FieldId), description, level + 1);
        }
    }

    private FlowNodeRunTime GetActivity(ProcessVersionRuntime processVersion, string activityId)
    {
        FlowNodeRunTime activity = null;
        if (string.IsNullOrEmpty(activityId))
            activity = processVersion.nodes.Values.FirstOrDefault();
        else
            processVersion?.nodes?.TryGetValue(activityId, out activity);
        return activity;
    }

    private ProcessVersionRuntime GetProcessVersion(string processId, string processVersion)
    {
        ProcessRunTime process;
        if (((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.processesRunTimes == null)
            return null;
        if (string.IsNullOrEmpty(processId))
            process = null;
        else if (!((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).Repository.processesRunTimes.TryGetValue(processId, out process) || process == null)
            return null;
        return process?.Versions.FirstOrDefault(
            v => v.Key == processVersion
                  || string.IsNullOrEmpty(processVersion)).Value;
    }
}
