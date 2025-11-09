using Neo.Bpms.Domain.Models.Security.Authorization;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities;

public partial class ApplyUtility
{
    private string _transactionId ;
    private DataOperation.eOperationType _operationType;
    private DataOperation _dataOperation;
    private UserSecurityAccessFlags _access;
    private string _subjectId;

    private bool RunDataOperations<T>(ElasticObject record, T oRecord,
        DataOperation dataOperation, DataOperation.eOperationType operationType,
        UserSecurityAccessFlags access, string subjectId)
    {
        if (InnerRunDataOperations(record, oRecord,
            dataOperation, operationType, access, subjectId))
        {
            if (OpenConnection())
            {
                InitSubQueryDataSource();
                return true;
            }
        }

        Release();
        return false;
    }

    private bool InnerRunDataOperations<T>(ElasticObject record, T oRecord,
        DataOperation dataOperation, DataOperation.eOperationType operationType,
        UserSecurityAccessFlags access, string subjectId)
    {
        RecordsAffected = 0;
        if (MaxRows != null)
            DataSource.TopRows = MaxRows.Value;
        _transactionId = Guid.NewGuid().ToString();
        LocalParameters ??= new LocalParameters(AuditTrail?.User);
        LocalParameters?.AddOrUpdate("user", AuditTrail?.User);

        _dataOperation = dataOperation;
        _operationType = operationType;
        _access = access;
        _subjectId = subjectId;

        PreProcessAudit(record);

        SetOutputState(record);

        if (!CheckConformance(record))
            return false;

        if (dataOperation != null &&
            !AddAutoCalcs(record, oRecord, dataOperation.AutoCalcs, AutoCalc.AutoCalcLocation.BeforeValidation))
            return false;
        if (!AddAutoCalcs(record, oRecord, Entity.AutoCalcs, AutoCalc.AutoCalcLocation.BeforeValidation))
            return false;

        if (dataOperation != null &&
            !CheckValidations(record, oRecord, dataOperation.Validations))
            return false;
        if (!CheckValidations(record, oRecord, Entity.Validations))
            return false;

        if (dataOperation != null &&
            !AddAutoCalcs(record, oRecord, dataOperation.AutoCalcs, AutoCalc.AutoCalcLocation.AfterValidation))
            return false;
        if (!AddAutoCalcs(record, oRecord, Entity.AutoCalcs, AutoCalc.AutoCalcLocation.AfterValidation))
            return false;
        return true;
    }

    private void InitQueryDataSource(QueryUtility query)
    {
        query?.PreProcessQuery(LocalParameters);
    }

    private void InitSubQueryDataSource()
    {
        if (DataSource == null) return;
        if (_subQueries == null) return;
        foreach (var subQuery in _subQueries)
        {
            if (subQuery?.Query == null) continue;
            subQuery.Query.PreProcessQuery(LocalParameters);
            if (DataSource.SubTables == null)
                DataSource.SubTables = [];
            if (!DataSource.SubTables.ContainsKey(subQuery.Key))
                DataSource.SubTables.Add(subQuery.Key,
                    QueryUtility.NewSubDataSource(this, LocalParameters, subQuery));
            subQuery.Query.InitSubQueries(DataSource);
        }
    }

    private void PreProcessAudit(ElasticObject record)
    {
        if (Entity.Auditable == AuditableVersion.V1)
        {
            if (_operationType == DataOperation.eOperationType.Constructor)
            {
                AddUserToAudit(record, "AutoAudit_CreatedByUsr");//V1
                AddDateToAudit(record, "AutoAudit_CreatedDate");//V1
                AddField("AutoAudit_RowVersion", "1");//V1
                AddField("AutoAudit_CreatedBy", Parser.Parse("Suser_SName()"));//V1
            }
            AddUserToAudit(record, "AutoAudit_ModifiedByUsr");//V1
            AddDateToAudit(record, "AutoAudit_ModifiedDate");//V1
            AddField("AutoAudit_RowVersion", Parser.Parse("ISNULL(AutoAudit_RowVersion,1)+1"));//V1
            AddField("AutoAudit_ModifiedBy", Parser.Parse("Suser_SName()"));//V1
        }
        else if (Entity.Auditable == AuditableVersion.V2)
        {
            if (_operationType == DataOperation.eOperationType.Constructor)
            {
                AddUserToAudit(record, nameof(IBaseAuditableEntity.CreatedById));
                AddDateToAudit(record, nameof(IBaseAuditableEntity.CreateDate));
            }
            AddUserToAudit(record, nameof(IBaseAuditableEntity.LastModifiedById));
            AddDateToAudit(record, nameof(IBaseAuditableEntity.LastModified));
        }

    }

    private void AddUserToAudit(ElasticObject record, string userAuditFieldName)
    {
        if (AuditTrail?.User == null) return;
        var userAuditField = Entity.GetField(userAuditFieldName);
        if (userAuditField == null) return;
        var userId = AuditTrail.User.Id;
        if (record != null)
        {
            record[userAuditField.DbFieldName] = userId;
            record[userAuditFieldName] = userId;
        }

        AddField(userAuditField, userId);
    }

    private void AddDateToAudit(ElasticObject record, string dateAuditFieldName)
    {
        var auditField = Entity.GetField(dateAuditFieldName);
        if (auditField == null) return;
        var d = DateTime.Now;
        if (record != null)
        {
            record[auditField.DbFieldName] = d;
            record[dateAuditFieldName] = d;
        }

        AddField(auditField, d);
    }

    private void SetOutputState(ElasticObject record)
    {
        if (_dataOperation == null || _dataOperation.outputStateId == 0) return;
        record?.SetField("StateId", _dataOperation.outputStateId);
        //todo else if( oRecord!=null)
        //	oRecord.SetField("StateId", dataOperation.outputStateId);
        AddField("StateId", _dataOperation.outputStateId);
    }

    private bool AddAutoCalcs<T>(ElasticObject record, T oRecord,
        AutoCalcList autoCalcList, AutoCalc.AutoCalcLocation loaction)
    {
        if (autoCalcList?.Calculations == null)
            return true;
        foreach (var autoCalc in autoCalcList.Calculations)
        {
            if (autoCalc.Loaction <= 0) autoCalc.Loaction = AutoCalc.AutoCalcLocation.BeforeValidation;
            if (autoCalc.Loaction != loaction) continue;
            if (autoCalc.GenerationType == AutoCalc.eGenerationType.DBInsert) continue;
            if (!autoCalc.RecalcOnAnyChange && _operationType != DataOperation.eOperationType.Constructor) continue;
            if (autoCalc.Condition?.Root != null)
            {
                var res = autoCalc.Condition.Root.Eval(record, LocalParameters);
                if (res == null || !ConvUtill.ToBoolean(res))
                    continue;
            }

            LocalParameters.AddOrUpdate("FieldId", autoCalc.FieldId);
            AddField(autoCalc.FieldId, autoCalc.Formula.Root, autoCalc.FieldId);
        }

        return true;
    }

    private bool CheckValidations<T>(ElasticObject record, T oRecord,
        List<Validation> validations)
    {
        if (validations == null) return true;
        var lp = LocalParameters.Clone();
        lp.Set(record);
        var hasError = false;
        foreach (var item in validations)
        {
            if (item.Condition?.Root != null)
            {
                var formula = item.Condition.Root.clone();
                formula = formula.replace(lp);
                formula = formula.processAndReplace(EntityConversions.ConvertFormulaFields, this); //todo this
                formula = formula.processAndReplace(EntityConversions.ConvertNames, this); //todo this
                var res = formula.Eval(record, lp);
                if (res == null || !ExpressionNode.CheckIfTrue(res))
                    continue;
            }

            if (item.Statement?.Root != null)
            {
                var formula = item.Statement.Root.clone();
                formula = formula.replace(lp);
                formula = formula.processAndReplace(EntityConversions.ConvertFormulaFields, this); //todo this
                formula = formula.processAndReplace(EntityConversions.ConvertNames, this); //todo this
                var res = formula.Eval(record, lp);
                if (ExpressionNode.CheckIfTrue(res))
                    continue;
                AddException(item.Exception, item.FieldId);
                hasError = true;
            }
        }

        return !hasError;
    }

    private bool CheckConformance(ElasticObject record)
    {
        if (Entity.Conformances == null) return true;
        foreach (var item in Entity.Conformances)
        {
            var conf = item as Conformance;
            if (conf == null) continue;
            if ((string.IsNullOrEmpty(conf.subject) && _subjectId != conf.subject))
                continue;
            if ((conf.access & _access) == 0) continue;
            if (conf.level == ConformanceLevel.Full) return true;
            if (conf.level == ConformanceLevel.None)
            {
                AddException(conf.exception, null);
                return false;
            }

            var res = conf.expression.Eval(record, LocalParameters);
            if (res != null && ExpressionNode.CheckIfTrue(res))
                continue;
            AddException(conf.exception, null);
        }

        return true;
    }

    private bool ThrowTrig<T>(ElasticObject record, T oRecord,
        DataOperation dataOperation, DataOperation.eOperationType operationType)
    {
        return (dataOperation == null ||
            Trig(dataOperation.triggers, record, oRecord, operationType)) && Trig(Entity.triggers, record, oRecord, operationType);
    }

    private bool Trig<T>(List<Trigger> triggers, ElasticObject record, T oRecord,
        DataOperation.eOperationType operationType)
    {
        if (triggers == null) return true;
        foreach (var item in triggers)
        {
            //todo
            throw new NotImplementedException();
        }

        return true;
    }
}
