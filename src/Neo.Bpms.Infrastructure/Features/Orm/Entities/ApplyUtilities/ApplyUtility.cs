using Neo.Bpms.Domain.Entities.Cmmn.DataSynchronization;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.ApplyUtilities;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities;

public static partial class ApplyUtility<T> where T : new()
{
    public static ApplyUtility New() //todo check usages
    {
        return new ApplyUtility(ProjectDefinition.Project.GetEntity<T>());
    }

    public static ApplyUtility NewInTrail(AuditTrail auditTrail, LocalParameters localParameters = null)
    {
        return new ApplyUtility(ProjectDefinition.Project.GetEntity<T>(), auditTrail, localParameters);
    }

    public static ApplyUtility New(IdentityUser user, LocalParameters localParameters = null)
    {
        return new ApplyUtility(ProjectDefinition.Project.GetEntity<T>(), user, localParameters);
    }
}

public partial class ApplyUtility : EntityConnection
{
    #region ctor
    public static ApplyUtility Apply<T>(AuditTrail auditTrail = null, LocalParameters localParameters = null)
        where T : new()
    {
        return new ApplyUtility(ProjectDefinition.Project.GetEntity<T>(), auditTrail, localParameters);
    }

    public ApplyUtility(string namespaceId, string entityId, IdentityUser user = null,
        LocalParameters localParameters = null)
        : this(ProjectDefinition.Project.GetEntity(namespaceId, entityId),
            new AuditTrail(null, null,
                TriggerTypeId.ApplyUtilityConstructor, $"Apply Utility Constructor {entityId}", user, 0/*todo*/),
            localParameters)
    {
    }

    public ApplyUtility(string namespaceId, string entityId, AuditTrail auditTrail,
        LocalParameters localParameters)
        : this(ProjectDefinition.Project.GetEntity(namespaceId, entityId),
            auditTrail, localParameters)
    {
    }

    public ApplyUtility(string namespaceId, string entityId, IAuditTrail auditTrail,
        LocalParameters localParameters)
        : this(ProjectDefinition.Project.GetEntity(namespaceId, entityId),
            (AuditTrail)auditTrail, localParameters)
    {
    }

    public ApplyUtility(Entity entity, IdentityUser user = null, LocalParameters localParameters = null)
        : this(entity,
            new AuditTrail(null, null, TriggerTypeId.ApplyUtilityConstructor, $"Apply Utility Constructor {entity?.Id}",
                user, 0/*todo*/), localParameters)
    {
    }

    public ApplyUtility(Entity entity, AuditTrail auditTrail, LocalParameters localParameters = null)
        : base(entity, auditTrail, $"ApplyUtility.{entity?.Id}")
    {
        LocalParameters = localParameters ?? new LocalParameters(auditTrail?.User);
        _reporterConfig = new EntityChangedReporterConfig
        {
            EntityAddress = Entity.EntityAddress,
            CallCaller = false
        };
    }

    private readonly EntityChangedReporterConfig _reporterConfig;

    ~ApplyUtility()
    {
        if (_dataSource != null)
        {
            Logger.LogError("~ApplyUtility({0}, {1}) data source dont release by code.", Entity?.Id, Name);
            Release();
        }
    }

    private void LogCommandTxtError(string errorCode)
    {
        Logger.LogError(errorCode + ": " + CommandTxt);
    }

    public override void Release()
    {
        if (_subQueries != null)
        {
            foreach (var subQuery in _subQueries)
                subQuery.Query?.ReleaseQuery();
        }

        _subQueries = null;
        base.Release();
    }
    #endregion

    #region parameters
    //public int RecordsAffected => DataSource.RecordsAffected;
    public IdentityUser User => AuditTrail?.User;

    public LocalParameters LocalParameters { get; set; }
    public int RecordsAffected { get; set; }
    public int? MaxRows { private get; set; }

    private List<QueryUtility.SubQueryDefinition> _subQueries;
    #endregion

    #region AddField
    public QueryUtility AddSubQuery(eJoinType joinType, QueryUtility query,
        string mainFieldId, string joinFieldId)
    {
        var subQuery = new QueryUtility.SubQueryDefinition
        {
            joinType = eJoinType.InnerJoin,
            Query = query,
            fieldMappings =
            [
                new JoinDefinition.JoinFieldMapping(mainFieldId, joinFieldId)
            ]
        };
        subQuery.Query.Parent = this;
        _subQueries ??= [];
        _subQueries.Add(subQuery);
        return subQuery.Query;
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public ApplyUtility AddFormulaField(string fieldId, string formula, string overFieldName = null)
    {
        return AddField(fieldId, Parser.Parse(formula), overFieldName);
    }

    public ApplyUtility AddField(string fieldId, ExpressionNode formula, string overFieldName = null)
    {
        var ef = Entity.GetField(fieldId);
        if (ef == null) return this;
        if (formula == null)
        {
            return AddField(fieldId, "", overFieldName);
        }

        formula = formula.clone();
        formula = formula.replace(LocalParameters);
        formula = formula.processAndReplace(EntityConversions.ConvertFormulaFields, this);
        formula = formula.processAndReplace(EntityConversions.ConvertNames, this);
        if (formula is ConstantExpressionNode node)
        {
            return AddField(ef, node.Value, overFieldName);
        }

        LocalParameters.AddOrUpdate("__DBNamespaceId", Entity.NamespaceId);
        LocalParameters.AddOrUpdate("__DBEntityId", Entity.Id);
        LocalParameters.AddOrUpdate("__DBFieldName", ef.Id);
        var parentKeys = LocalParameters.Get("__parentKeys");
        if (parentKeys != null)
        {
            var pKeys = parentKeys.ToString().Split(',');
            var parentIds = LocalParameters.Get("__parentIds");
            if (parentIds != null)
            {
                var pIds = parentIds.ToString().Split('#');
                var iIds = 0;
                var dbWhereClause = "";
                foreach (var k in pKeys)
                {
                    var efk = Entity.GetField(k);
                    if (efk == null) continue;
                    dbWhereClause += efk.DbFieldName + "=" + pIds[iIds];
                    iIds++;
                    if (iIds >= pIds.Length) break;
                }

                LocalParameters.AddOrUpdate("__parentKeyFilter", dbWhereClause);
            }
        }

        if (!DataSource.ConvertExpressionToScript(false, formula, out var sFormula, out _, LocalParameters)
        ) return this;
        return AddField(ef, sFormula, overFieldName);
    }

    public ApplyUtility AddField(string fieldId, object value, string overFieldName = null)
    {
        var ef = Entity.GetField(fieldId);
        return ef != null ? AddField(ef, value, overFieldName) : this;
    }

    private ApplyUtility AddField(EntityField field, object value, string overFieldName = null)
    {
        if (field.AssociationEntity?.Maps != null)
        {
            if (field.AssociationEntity.Maps.Count == 1)
            {
                var srcField = Entity.GetField(field.AssociationEntity.Maps.FirstOrDefault()?.SourceField);
                if (srcField != null)
                    AddField(srcField, value, srcField.Id);
            }
            else
            {
                var ids = value?.ToString().Split('#') ?? new string[1];
                var iIds = 0;
                foreach (var map in field.AssociationEntity.Maps)
                {
                    var ef = Entity.GetField(map.SourceField);
                    if (ef != null)
                    {
                        var v = iIds < ids.Length ? ids[iIds] : null;
                        AddField(ef, v, ef.Id);
                    }

                    iIds++;
                }
            }
        }
        else if (!field.NotMap)
        {
            var dbFieldName = field.DbFieldName;
            var col = new ColumnDefinition(field)
            {
                fieldName = dbFieldName,
                overFieldName = overFieldName,
                formula_value = value
            };
            if (!DataSource.Fields.ContainsKey(dbFieldName))
                DataSource.Fields.Add(dbFieldName, col);
            else
                DataSource.Fields[dbFieldName] = col;
        }

        return this;
    }

    private void AddFieldsIfEmpty(ElasticObject record)
    {
        if (Entity == null || DataSource == null ||
            DataSource.Fields != null && DataSource.Fields.Count != 0) return;
        foreach (var eFld in Entity.entityFields.Values)
        {
            if (eFld.NotMap || eFld.AuditField) continue;
            if (eFld.AssociationEntity != null) continue;
            if (!record.GetField(eFld.Id, out var obj)) continue;
            if (eFld.IsAutoIncrement())
                continue;
            AddField(eFld, obj);
        }

        SetStateIdIfNeeded();
    }

    private void AddFieldsIfEmpty()
    {
        if (DataSource.Fields != null && DataSource.Fields.Count != 0) return;
        foreach (var eFld in Entity.entityFields.Values)
        {
            if (eFld.NotMap || eFld.AuditField) continue;
            if (eFld.AssociationEntity == null)
            {
                //not needed in bulk insert
                AddField(eFld, null, eFld.Id);
            }
        }

        SetStateIdIfNeeded();
    }

    private void AddFieldsIfEmpty<T>(T oRecord, bool bUpdate)
    {
        if (DataSource.Fields != null && DataSource.Fields.Count != 0) return;
        AddFieldsOfRecord(oRecord, bUpdate);
        SetStateIdIfNeeded();
    }

    private void AddFieldsOfRecord<T>(T oRecord, bool bUpdate)
    {
        foreach (var eFld in Entity.entityFields.Values)
        {
            if ((eFld.NotMap && eFld.ParentEntity == null) || eFld.AuditField)
                continue;
            if (eFld.AssociationEntity != null && eFld.ParentEntity == null)
                continue;
            var memberInfo = ReflectionField.FetchMember(oRecord.GetType(), eFld.Id);
            if (memberInfo == null)
                continue;
            ReflectionField.GetMemberValue(oRecord, memberInfo, out var obj);
            if (obj == null && !bUpdate) continue;
            if (eFld.IsAutoIncrement())
                continue;
            if (!eFld.Required && obj != null)
            {
                var o = obj.ToString();
                if (string.IsNullOrEmpty(o) || o == "0")
                    obj = null;
            }

            if (eFld.ParentEntity != null)
                AddFieldsOfRecord(obj, bUpdate);
            else
                AddField(eFld, obj, eFld.Id);
        }
    }

    private void SetStateIdIfNeeded()
    {
        if (Entity.IsStateBase)
        {
            var stateIdFieldId = Entity.StateIdFieldId;
            if (!DataSource.Fields.ContainsKey(stateIdFieldId))
            {
                var activeStateId = Entity.FirstActiveStateId;
                AddField(stateIdFieldId, activeStateId, stateIdFieldId);
            }
        }
    }
    #endregion

    #region total private function
    private void AddException(ExceptionInformation exceptionInformation, string fieldName)
    {
        Errors ??= [];
        Errors.Add(new ExceptionInfo
        {
            ForField = fieldName,
            Exception = new Exception(exceptionInformation.EnErrorText + " " + exceptionInformation.ErrorText)
        });
    }

    private void Rollback()
    {
        //todo Rollback transaction
    }
    #endregion

    #region ApplyExpression
    public bool ApplyExpression(ExpressionNode exp,
        UserSecurityAccessFlags access = UserSecurityAccessFlags.Update, string subjectId = null)
    {
        exp = exp.clone();
        exp = exp.replace(LocalParameters);
        exp = exp.processAndReplace(EntityConversions.ConvertFormulaFields, this);
        exp = exp.processAndReplace(EntityConversions.ConvertNames, this);
        if (exp is ConstantExpressionNode node)
        {
            if (!ExpressionNode.CheckIfTrue(node.Value)) return true;
            exp = null;
        }

        if (!DataSource.ConvertExpressionToScript(true, exp, out var sExp, out var outExp, LocalParameters))
            return false;
        if (outExp != null) return false;
        if (!RunDataOperations<ElasticObject>(null, null, null, DataOperation.eOperationType.Destructor,
            access,
            subjectId))
            return false;
        var b = DataSource.Command(sExp);
        RecordsAffected = DataSource.RecordsAffected;
        CommandTxt = DataSource.SqlCommand;
        if (b)
            b = ThrowTrig<ElasticObject>(null, null, null,
                DataOperation.eOperationType.Update /*todo*/);
        if (!b) Rollback();
        Release();
        return b;
    }
    #endregion

    public void SetEntityChangedReporterConfig(bool callCaller, string callerName)
    {
        _reporterConfig.CallCaller = callCaller;
        _reporterConfig.CallerName = callerName;
    }

    public void BeginBatch(int batchCount = 0)
    {
        AuditTrail ??= new AuditTrail(TriggerTypeId.BeginBatchInApplyUtility,
                "BeginBatch In Apply Utility");
        if (AuditTrail.Batch == null)
            AuditTrail.Batch = EntityBatchDataManipulation.New(Entity, LocalParameters, AuditTrail);
        AuditTrail?.Batch.BeginBatch(batchCount);
    }

    public void EndBatch()
    {
        if (AuditTrail?.Batch == null)
            return;
        AuditTrail.Batch.EndBatch();
        AuditTrail.Batch = null;
    }

    private bool OpenConnection()
    {
        var tableName = EntityDbNameManager.GetDbTableName(Entity);
        return DataSource?.Connection?.Open(tableName, AuditTrail?.DataTransaction) ?? false;
    }

    public bool RunSqlCommand(string command)
    {
        var b = DataSource.Command(command);
        Release();
        return b;
    }
}
