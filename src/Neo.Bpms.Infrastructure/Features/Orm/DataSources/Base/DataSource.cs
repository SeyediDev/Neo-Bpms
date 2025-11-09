using System.Data.SqlTypes;
using Neo.Bpms.Domain.Models.Cmmn.Data.Provider;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Base;

public abstract class DataSource(DataSourceDefinition definition, Connection connection, AuditTrail auditTrail) 
    : IDataSource
{
    public readonly DataSourceDefinition DataSrcDefinition = definition;
    public AuditTrail AuditTrail { get; set; } = auditTrail;
    public string Alias { get; set; }
    public virtual string Name => DataSrcDefinition.name;
    public string SqlCommand { get; set; }
    public bool Distinct { get; set; }
    public int StartIndex { get; set; }
    public int TopRows { get; set; }
    public bool OnlyRecordCount { get; set; }
    public bool HasPaging => TopRows > 0 || StartIndex > 0;
    public List<FilterDefinition> Filters { get; set; }
    public List<FilterDefinition> HavingFilters { get; set; }
    public Dictionary<string, ColumnDefinition> Fields { get; set; } = [];
    public Dictionary<string, FieldDefinition> Outfields { get; set; } = [];
    public List<FormulaDefinition> Formulas { get; set; }
    public List<AggregateDefinition> Aggregates { get; set; }
    public List<OrderByDefinition> OrderBys { get; set; }
    public Dictionary<string, SubDataSource> SubTables { get; set; }
    public LocalParameters FilterValues { get; set; }
    public List<Exception> Exceptions { get; set; }

    public Dictionary<string, ExtractedParentEntity> ExtractedParentEntities;
    public static string GetInnerInjectionKey()
    {
        return "#@!$SJVS:";
    }

    public int RecordsAffected { get; set; }
    public Connection Connection { get; set; } = connection;

    public virtual List<(EntityField entityField, object value)> ParentAssociationFields { get; set; } =
        [];
    protected static readonly PageingVersion PagingVersion = PageingVersion.OffsetRows;

    public void AddOutField(FieldDefinition columnDefinition)
    {
        Outfields.Add(columnDefinition.overFieldName ?? columnDefinition.fieldName, columnDefinition);
    }

    protected bool IsOpen { get; set; }

    protected ExpressionNode objectFilter { get; set; }
    protected ExpressionNode objectHavingFilter { get; set; }
    public CancellationToken CancellationToken { get; set; } = default;

    public void AddFilter(ExpressionNode expFilter, LocalParameters localParameters, string orGroupId)
    {
        ExpressionNode outExp;
        if (Connection.Definition.SupportsFilter())
        {
            if (!ConvertExpressionToScript(true, expFilter, out string filter, out outExp, localParameters))
            {
                outExp = expFilter;
            }
            else
            {
                if (filter is "0" or "false")
                {
                    filter = "(0=1)";
                }
                else if (filter is "1" or "true")
                {
                    filter = "(1=1)";
                }

                AddFilter(filter, localParameters, orGroupId);
            }
        }
        else
        {
            outExp = expFilter;
        }

        if (outExp != null)
        {
            objectFilter = objectFilter != null
                ? new LogicalExpressionNode(LogicalExpressionNode.eLogicalExpressionType.And, objectFilter, outExp, 0)
                : expFilter;
        }
    }

    public void AddFilter(string filterParam, LocalParameters localParameters, string orGroupId)
    {
        if (Connection.Definition.SupportsFilter())
        {
            Filters ??= [];
            Filters.Add(new FilterDefinition
            {
                Filter = filterParam,
                OrGroupId = orGroupId
            });
        }
        else
        {
            ExpressionNode expFilter = Parser.Parse(filterParam);
            AddFilter(expFilter, localParameters, orGroupId);
        }
    }

    public void AddHaving(ExpressionNode expHavingFilter, LocalParameters localParameters, string orGroupId)
    {
        ExpressionNode outExp;
        if (Connection.Definition.SupportsFilter())
        {
            if (!ConvertExpressionToScript(true, expHavingFilter, out string sHavingFilter, out outExp, localParameters))
            {
                outExp = expHavingFilter;
            }
            else
            {
                AddHaving(sHavingFilter, localParameters, orGroupId);
            }
        }
        else
        {
            outExp = expHavingFilter;
        }

        if (outExp != null)
        {
            objectHavingFilter = objectHavingFilter != null
                ? new LogicalExpressionNode(LogicalExpressionNode.eLogicalExpressionType.And, objectHavingFilter, outExp,
                    0)
                : expHavingFilter;
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public void AddHaving(string havingFilter, LocalParameters localParameters, string orGroupId)
    {
        if (Connection.Definition.SupportsFilter())
        {
            HavingFilters ??= [];
            HavingFilters.Add(new FilterDefinition
            {
                Filter = havingFilter,
                OrGroupId = orGroupId
            });
        }
        else
        {
            ExpressionNode expTree = Parser.Parse(havingFilter);
            AddHaving(expTree, localParameters, orGroupId);
        }
    }

    public bool AddGroupBy(EntityField field, string fieldName, ExpressionNode formula,
        eAggregationFunctions func, eAggregateScope scope, string over)
    {
        if (DataSrcDefinition.Entity == null || !string.IsNullOrEmpty(fieldName) &&
            DataSrcDefinition.Entity.GetFieldByDbName(fieldName) == null && fieldName != "*")
        {
            return false;
        }

        Aggregates ??= [];
        if (over != null)
        {
            if (Aggregates.Any(a => a.overFieldName == over && a.function == func && a.aggregateScope == scope))
            {
                return false;
            }
        }

        Aggregates.Add(new AggregateDefinition(field)
        {
            fieldName = fieldName,
            formula = formula,
            function = func,
            aggregateScope = scope,
            overFieldName = over
        });
        return true;
    }

    public void AddFormula(ExpressionNode formula, string asFieldName)
    {
        Formulas ??= [];
        Formulas.Add(new FormulaDefinition { asFieldName = asFieldName, formula = formula });
    }

    public bool AddOrderBy(EntityField field, int orderIndex, string fieldName, SortType order,
        bool orderById = false)
    {
        if (DataSrcDefinition.Entity == null)
        {
            return false;
        }

        OrderBys ??= [];
        if (OrderBys.Any(o => (o.fieldName ?? "") + o.formula?.toText() == fieldName))
        {
            return false;
        }

        OrderBys.Add(new OrderByDefinition(field)
        {
            orderIndex = orderIndex,
            fieldName = fieldName,
            order = order,
            orderById = orderById
        });
        return true;
    }

    public bool AddOrderBy(EntityField field, int orderIndex, ExpressionNode formula, SortType order,
        bool orderById = false)
    {
        if (DataSrcDefinition.Entity == null)
        {
            return false;
        }

        OrderBys ??= [];
        if (OrderBys.Any(o => (o.fieldName ?? "") + o.formula?.toText() == formula?.toText()))
        {
            return false;
        }

        OrderBys.Add(new OrderByDefinition(field)
        {
            orderIndex = orderIndex,
            formula = formula,
            order = order,
            orderById = orderById
        });
        return true;
    }

    public virtual string GetFieldType(TVariableTypes fieldType)
    {
        return "string";
    }

    public virtual string GetFullFieldType(TVariableTypes fieldType, int maxLen)
    {
        return "string";
    }

    public virtual string GetFullFieldType(string fieldType, int maxLen)
    {
        return "string";
    }

    public void SetQueryCommand(string command)
    {
        Exceptions = null;
        SqlCommand = command;
    }

    public virtual string GenerateQuery()
    {
        return SqlCommand;
    }

    // ReSharper disable once UnusedMember.Global
    public virtual bool InsertXML(string commandText, string xmlValueDef, SqlXml xmlData)
    {
        return false;
    }

    public virtual void GenerateQuery_WriteJoins(DataSource dataSource, ref NeoStringBuilder sql,
        ref Dictionary<string, string> tables)
    {
    }

    public bool Select(string sql)
    {
        return openForRead(sql);
    }

    protected virtual bool openForRead(string sql)
    {
        SetQueryCommand(sql);
        return openForRead();
    }

    protected virtual bool openForUpdate(string selectCommand, bool runCommand = false, List<object> parameters = null)
    {
        SetQueryCommand(selectCommand);
        return OpenForWrite();
    }

    public virtual bool openForRead()
    {
        if (IsOpen)
        {
            return false;
        }

        IsOpen = true;
        return true;
    }

    public virtual async Task<bool> OpenForReadAsync()
    {
        if (IsOpen)
        {
            return false;
        }

        IsOpen = true;
        await Task.CompletedTask; 
        return true;
    }

    public virtual bool OpenForWrite()
    {
        if (IsOpen)
        {
            return false;
        }

        IsOpen = true;
        return true;
    }

    public virtual bool Close()
    {
        SqlCommand = null;
        Connection?.Try(() => Connection?.Close());
        Connection = null;
        bool b = IsOpen;
        IsOpen = false;
        return b;
    }

    public virtual bool Insert(ElasticObject record, bool dontGiveOutput)
    {
        return false;
    }

    public virtual bool Update(ElasticObject record, ElasticObject keyObj, bool compareWithPrevious)
    {
        return false;
    }

    public virtual bool SetAsync(Func<object, bool> callback, object callbackParams,
        ElasticObject elasticObject, ElasticObject keyObj, bool generateAuditRecord)
    {
        return false;
    }

    public virtual bool SetAsync<T>(Func<object, bool> callback, object callbackParams,
        T obj, T keyObj, bool generateAuditRecord)
    {
        return false;
    }

    public virtual bool UpdateGroup(ElasticObject elasticObject)
    {
        return false;
    }

    public virtual bool Upset(ElasticObject elasticObject, ElasticObject keyObj, bool dontGiveOutput)
    {
        return false;
    }

    public virtual bool Delete(ElasticObject elasticObject)
    {
        return false;
    }

    public virtual bool DeleteGroup(ElasticObject elasticObject)
    {
        return false;
    }

    public bool ReadNext(out ElasticObject obj)
    {
        if (Aggregates != null)
        {
            if (objectFilter != null)
            {
                //objectFilter = null;
                //throw new DataLayerException(eDataEngineException.CanNotReadAggregatedQueryWithInMemoryFilter, "فیلتر زمان اجرا بر روی کوئری تجمیعی امکان پذیر نیست. بجای آن از فیلتر having استفاده شود.");
            }

            if (objectHavingFilter == null)
            {
                return read(out obj);
            }

            do
            {
                if (!read(out obj))
                {
                    break;
                }

                if (OnlyRecordCount)
                {
                    return true;
                }

                if (ExpressionNode.CheckIfTrue(objectHavingFilter.Eval(obj, FilterValues)))
                {
                    return true;
                }

                if (CancellationToken.IsCancellationRequested)
                {
                    break;
                }
            } while (true);
        }
        else
        {
            if (objectFilter == null)
            {
                return read(out obj);
            }

            do
            {
                if (!read(out obj))
                {
                    break;
                }

                if (OnlyRecordCount)
                {
                    return true;
                }

                if (ExpressionNode.CheckIfTrue(objectFilter.Eval(obj, FilterValues)))
                {
                    return true;
                }

                if (CancellationToken.IsCancellationRequested)
                {
                    break;
                }
            } while (true);
        }

        return false;
    }

    public bool ReadNext<T>(out T obj)
        where T : new()
    {
        obj = default;
        if (Aggregates != null)
        {
            if (objectFilter != null)
            {
                throw new DataLayerException(DataEngineException.CanNotReadAggregatedQueryWithInMemoryFilter,
                    "فیلتر زمان اجرا بر روی کوئری تجمیعی امکان پذیر نیست. بجای آن از فیلتر هوینگ استفاده شود.");
            }

            if (objectHavingFilter == null)
            {
                return read(out obj);
            }

            do
            {
                if (!read(out obj))
                {
                    break;
                }

                if (OnlyRecordCount)
                {
                    return true;
                }

                if (ExpressionNode.CheckIfTrue(objectHavingFilter.Eval(obj, FilterValues)))
                {
                    return true;
                }

                if (CancellationToken.IsCancellationRequested)
                {
                    break;
                }
            } while (true);
        }
        else
        {
            if (objectFilter == null)
            {
                return read(out obj);
            }

            do
            {
                if (!read(out obj))
                {
                    break;
                }

                if (OnlyRecordCount)
                {
                    return true;
                }

                if (ExpressionNode.CheckIfTrue(objectFilter.Eval(obj, FilterValues)))
                {
                    return true;
                }

                if (CancellationToken.IsCancellationRequested)
                {
                    break;
                }
            } while (true);
        }

        return false;
    }

    public virtual bool read(out ElasticObject obj)
    {
        obj = null;
        return false;
    }

    public virtual bool read<T>(out T obj) where T : new()
    {
        obj = default;
        return false;
    }

    // ReSharper disable once UnusedMemberInSuper.Global
    public virtual bool read(out object[] values)
    {
        values = null;
        return false;
    }

    public virtual bool Test<T>(T obj, bool checkAllExists)
    {
        return obj != null;
    }

    public virtual bool Test(ElasticObject obj)
    {
        return obj != null;
    }

    public virtual bool insert<T>(T obj, bool dontGiveOutput)
    {
        return false;
    }

    public virtual bool BulkInsert<T>(IEnumerable<T> obj)
    {
        return false;
    }

    public virtual bool InsertFromQuery(DataSource queryDataSource)
    {
        throw new NotImplementedException();
    }

    public virtual bool update<T>(T obj, T keyObj, bool compareWithPrevious)
    {
        return false;
    }

    public virtual bool upset<T>(T obj, T keyObj, bool dontGiveOutput)
    {
        return false;
    }

    public virtual bool Delete<T>(T obj)
    {
        return false;
    }

    public virtual bool find<T>(ExpressionNode filterExpressionNode, out T newRecord)
        where T : new()
    {
        if (filterExpressionNode != null)
        {
            AddFilter(filterExpressionNode, FilterValues, "");
        }

        if (openForRead())
        {
            return read(out newRecord);
        }

        newRecord = default;
        return false;
    }

    public virtual ElasticObject find(ExpressionNode filterExpressionNode)
    {
        if (filterExpressionNode != null)
        {
            AddFilter(filterExpressionNode, FilterValues, "");
        }

        return !openForRead() ? null : read(out ElasticObject outVal) ? outVal : null;
    }

    public static object GetField<T>(T obj, string key)
    {
        return ExpressionNode.GetPropertyValue(obj, key);
    }

    protected static void SetField<T>(T obj, string key, object value)
    {
        ExpressionNode.SetPropertyValue(obj, key, value);
    }

    protected virtual object GetValue(string key)
    {
        return null;
    }

    private static object GetField(ElasticObject obj, string key)
    {
        if (!obj.GetField(key, out object outObj))
        {
            outObj = null;
        }

        return outObj;
    }

    // ReSharper disable once UnusedMember.Global
    protected static void SetField(ElasticObject obj, string key, object value)
    {
        _ = obj.SetField(key, value);
    }

    // ReSharper disable once UnusedMemberInSuper.Global
    public virtual Entity extractEntityMetaData(ILogger logger)
    {
        return null;
    }

    public virtual bool toList<T>(ref List<T> records,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            records.Add(record);
            _ = (func?.Invoke(record, param));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public bool ToList(ref List<ElasticObject> records,
        Func<ElasticObject, object, bool> func = null, object param = null)
    {
        if (!openForRead())
        {
            return false;
        }

        while (true)
        {
            if (!ReadNext(out ElasticObject record))
            {
                break;
            }

            records.Add(record);
            _ = (func?.Invoke(record, param));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public async Task<(bool, List<ElasticObject> records)> ToListAsync(Func<ElasticObject, object, bool> func = null,
        object param = null)
    {
        List<ElasticObject> records = [];
        if (!await OpenForReadAsync())
        {
            return (false, records);
        }

        while (true)
        {
            if (!ReadNext(out ElasticObject record))
            {
                break;
            }

            records.Add(record);
            _ = (func?.Invoke(record, param));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return (true, records);
    }

    public virtual void ForEach<T>(Action<T> action)
        where T : new()
    {
        if (!openForRead())
        {
            return;
        }

        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            action(record);
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    public virtual void ParallelForEach<T>(Action<T> action)
        where T : new()
    {
        if (!openForRead())
        {
            return;
        }

        _ = Parallel.ForEach(GetRecords<T>(), record =>
        {
            if (record != null)
            {
                action(record);
            }
        });
    }

    public virtual void ForEach(Action<ElasticObject> action)
    {
        if (!openForRead())
        {
            return;
        }

        while (true)
        {
            if (!ReadNext(out ElasticObject record))
            {
                break;
            }

            action(record);
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    public virtual void ParallelForEach(Action<ElasticObject> action)
    {
        if (!openForRead())
        {
            return;
        }

        _ = Parallel.ForEach(GetRecords(), record =>
        {
            if (record != null)
            {
                action(record);
            }
        });
    }

    public IEnumerable<ElasticObject> GetRecords()
    {
        do
        {
            _ = ReadNext(out ElasticObject record);
            if (record == null)
            {
                break;
            }

            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }

            yield return record;
        } while (true);
    }

    public IEnumerable<T> GetRecords<T>() where T : new()
    {
        do
        {
            _ = ReadNext(out T record);
            if (record == null)
            {
                break;
            }

            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }

            yield return record;
        } while (true);
    }

    public virtual List<TSelect> Select<T, TSelect>(Func<T, TSelect> selector)
        where T : new()
    {
        List<TSelect> list = [];
        if (!openForRead())
        {
            return list;
        }

        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            list.Add(selector(record));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return list;
    }

    public virtual bool Any<T>(Func<T, bool> predicate)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            if (predicate == null || predicate(record))
            {
                return true;
            }

            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return false;
    }

    public virtual bool All<T>(Func<T, bool> predicate)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            if (!predicate(record))
            {
                return false;
            }

            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public virtual List<TSelect> Select<TSelect>(Func<ElasticObject, TSelect> selector)
        where TSelect : new()
    {
        List<TSelect> list = [];
        if (!openForRead())
        {
            return list;
        }

        while (true)
        {
            if (!ReadNext(out ElasticObject record))
            {
                break;
            }

            list.Add(selector(record));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return list;
    }

    public virtual bool Any(Func<ElasticObject, bool> predicate)
    {
        if (!openForRead())
        {
            return false;
        }

        while (true)
        {
            if (!ReadNext(out ElasticObject record))
            {
                break;
            }

            if (predicate == null || predicate(record))
            {
                return true;
            }

            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return false;
    }

    public virtual bool All(Func<ElasticObject, bool> predicate)
    {
        if (!openForRead())
        {
            return false;
        }

        while (true)
        {
            if (!ReadNext(out ElasticObject record))
            {
                break;
            }

            if (!predicate(record))
            {
                return false;
            }

            if (CancellationToken.IsCancellationRequested)
            {
                return false;
            }
        }

        return true;
    }

    public bool toDictionary<TKey, T>(ref Dictionary<TKey, T> records,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            if (DataSrcDefinition.Entity.KeyFields.Count() == 1)
            {
                object keyValue = GetField(record, DataSrcDefinition.Entity.KeyFields.FirstOrDefault()?.DbFieldName);
                if (keyValue is TKey key)
                {
                    if (records.ContainsKey(key))
                    {
                        continue;
                    }

                    records.Add(key, record);
                }
            }
            else
            {
                TKey keyValue = default;
                foreach (EntityField key in DataSrcDefinition.Entity.KeyFields)
                {
                    SetField(keyValue, key.DbFieldName, GetField(record, key.DbFieldName));
                }

                if (keyValue == null || records.ContainsKey(keyValue))
                {
                    continue;
                }

                records.Add(keyValue, record);
            }

            _ = (func?.Invoke(record, param));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public bool ToConcurrentDictionary<TKey, T>(ref ConcurrentDictionary<TKey, T> records,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            if (DataSrcDefinition.Entity.KeyFields.Count() == 1)
            {
                object keyValue = GetField(record, DataSrcDefinition.Entity.KeyFields.FirstOrDefault()?.DbFieldName);
                if (keyValue is TKey key)
                {
                    if (records.ContainsKey(key))
                    {
                        continue;
                    }

                    _ = records.TryAdd(key, record);
                }
            }
            else
            {
                TKey keyValue = default;
                foreach (EntityField key in DataSrcDefinition.Entity.KeyFields)
                {
                    SetField(keyValue, key.DbFieldName, GetField(record, key.DbFieldName));
                }

                if (keyValue == null || records.ContainsKey(keyValue))
                {
                    continue;
                }

                _ = records.TryAdd(keyValue, record);
            }

            _ = (func?.Invoke(record, param));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public bool ToDictionaryById<T>(ref Dictionary<long, T> records,
        Func<T, object, bool> func = null, object param = null)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        bool hasKeyInType = HasKeyInType<T>("Id");
        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            long id = GetKeyValueId("Id", hasKeyInType, record);
            records.Add(id, record);
            _ = (func?.Invoke(record, param));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public bool ToDictionaryById(ref Dictionary<long, ElasticObject> records,
        Func<ElasticObject, object, bool> func = null, object param = null)
    {
        if (!openForRead())
        {
            return false;
        }

        while (true)
        {
            if (!ReadNext(out ElasticObject record))
            {
                break;
            }

            object keyValue = GetField(record, "Id");
            _ = long.TryParse(keyValue?.ToString() ?? "0", out long id);
            records.Add(id, record);
            _ = (func?.Invoke(record, param));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public bool ToDictionaryClassById<DicClass, T>(string dbRecordFieldName, ref Dictionary<long, DicClass> records,
        Func<T, object, bool> func = null, object param = null)
        where T : new() where DicClass : new()
    {
        if (!openForRead())
        {
            return false;
        }

        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            object keyValue = GetField(record, "Id");
            _ = long.TryParse(keyValue?.ToString() ?? "0", out long id);
            DicClass dicItem = new();
            SetField(dicItem, dbRecordFieldName, record);
            records.Add(id, dicItem);
            _ = (func?.Invoke(record, param));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public bool ToDictionaryOfList<T>(string keyFieldId,
        ref Dictionary<long, List<T>> dicOfList)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        bool hasKeyInType = HasKeyInType<T>(keyFieldId);
        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            long id = GetKeyValueId(keyFieldId, hasKeyInType, record);
            List<T> items = dicOfList.AddOrGetItem(id);
            items.Add(record);
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public bool ToStringDictionaryOfList<T>(string keyFieldId,
        ref Dictionary<string, List<T>> dicOfList)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        bool hasKeyInType = HasKeyInType<T>(keyFieldId);
        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            string id = GetKeyValue(keyFieldId, hasKeyInType, record);
            List<T> items = dicOfList.AddOrGetItem(id);
            items.Add(record);
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    private string GetKeyValue<T>(string keyFieldId, bool hasKeyInType, T record) where T : new()
    {
        object keyValue = hasKeyInType ? GetField(record, keyFieldId) : GetValue(keyFieldId);
        string id = keyValue?.ToString() ?? "";
        return id;
    }

    private long GetKeyValueId<T>(string keyFieldId, bool hasKeyInType, T record) where T : new()
    {
        string keyValue = GetKeyValue(keyFieldId, hasKeyInType, record);
        _ = long.TryParse(!string.IsNullOrEmpty(keyValue) ? keyValue : "0", out long id);
        return id;
    }

    private static bool HasKeyInType<T>(string keyFieldId) where T : new()
    {
        Type t = typeof(T);
        bool hasKeyInType = t.GetField(keyFieldId) != null || t.GetProperty(keyFieldId) != null;
        return hasKeyInType;
    }

    public bool ToDictionaryOfDictionary<T>(string keyFieldId1, string keyFieldId2,
        ref Dictionary<long, Dictionary<string, T>> dicOfDic)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        bool hasKey1InType = HasKeyInType<T>(keyFieldId1);
        bool hasKey2InType = HasKeyInType<T>(keyFieldId2);
        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            long id = GetKeyValueId(keyFieldId1, hasKey1InType, record);
            string keyValue2 = GetKeyValue(keyFieldId2, hasKey2InType, record);
            Dictionary<string, T> items = dicOfDic.AddOrGetItem(id);
            _ = items.AddOrUpdateItem(keyValue2, record);
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public bool ToLongDictionaryOfLongDictionary<T>(string keyFieldId1, string keyFieldId2,
        ref Dictionary<long, Dictionary<long, T>> dicOfDic)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        bool hasKey1InType = HasKeyInType<T>(keyFieldId1);
        bool hasKey2InType = HasKeyInType<T>(keyFieldId2);
        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            long id = GetKeyValueId(keyFieldId1, hasKey1InType, record);
            long keyValue2 = GetKeyValueId(keyFieldId2, hasKey2InType, record);
            Dictionary<long, T> items = dicOfDic.AddOrGetItem(id);
            _ = items.AddOrUpdateItem(keyValue2, record);
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public bool ToDictionaryByUnique<T>(string keyFieldId,
        ref Dictionary<string, T> dicOfUnique)
        where T : new()
    {
        if (!openForRead())
        {
            return false;
        }

        bool hasKeyInType = HasKeyInType<T>(keyFieldId);
        while (true)
        {
            if (!ReadNext(out T record))
            {
                break;
            }

            string keyValue = GetKeyValue(keyFieldId, hasKeyInType, record);
            _ = dicOfUnique.AddOrUpdateItem(keyValue, record);
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    // ReSharper disable once UnusedMember.Global
    public bool ToDictionary<TKey>(ref Dictionary<TKey, ElasticObject> records,
        Func<ElasticObject, object, bool> func = null, object param = null)
    {
        if (!openForRead())
        {
            return false;
        }

        string keyId = DataSrcDefinition.Entity.KeyFields.FirstOrDefault()?.DbFieldName;
        while (true)
        {
            if (!ReadNext(out ElasticObject record))
            {
                break;
            }

            if (DataSrcDefinition.Entity.KeyFields.Count() == 1)
            {
                object keyValue = GetField(record, keyId);
                if (keyValue is TKey key)
                {
                    records.Add(key, record);
                }
            }
            else
            {
                TKey keyValue = default;
                foreach (EntityField key in DataSrcDefinition.Entity.KeyFields)
                {
                    SetField(keyValue, key.DbFieldName, GetField(record, key.DbFieldName));
                }

                if (keyValue != null)
                {
                    records.Add(keyValue, record);
                }
            }

            _ = (func?.Invoke(record, param));
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return true;
    }

    public bool Command(string command, List<object> parameters = null)
    {
        return openForUpdate(command, true, parameters);
    }

    public virtual bool ConvertExpressionToScript(bool forWhere, ExpressionNode input,
        out string formula, out ExpressionNode output,
        LocalParameters localParameters, bool exactValue = false)
    {
        formula = "";
        output = null;
        return false;
    }

    protected void AddException(ILogger logger, string errorCode, EngineException e)
    {
        Exceptions ??= [];
        Exceptions.Add(e);
        logger.LogError("Error {errorCode} : In command\r\n{SqlCommand}", errorCode, SqlCommand);
        logger.LogError(e, "{Message}", e.Message);
    }

    protected void AddException(ILogger logger, string errorCode, Exception exception)
    {
        Exceptions ??= [];
        Exceptions.Add(exception);
        logger.LogError($"Error {errorCode} : \r\n{exception.Message}");
    }

    protected void AddException(ILogger logger, string errorCode, string message)
    {
        AddException(logger, errorCode, new Exception(message));
    }

    protected void SetExceptionIfExists(ILogger logger, string errorCode, List<DataLayerException> dataLayerExceptions,
        EngineException defaultException = null)
    {
        if (Exceptions != null && Exceptions.Count != 0)
        {
            return;
        }

        if ((dataLayerExceptions == null || dataLayerExceptions.Count == 0) &&
            defaultException == null)
        {
            return;
        }

        if (dataLayerExceptions != null)
        {
            foreach (DataLayerException dataLayerException in dataLayerExceptions)
            {
                AddException(logger, errorCode, dataLayerException);
            }
        }

        if (defaultException != null)
        {
            AddException(logger, errorCode, defaultException);
        }
    }

    public void AddField(EntityField field, string fieldId, object value)
    {
        Fields.Add(fieldId, new ColumnDefinition(field) { fieldName = fieldId, formula_value = value });
        AddOutField(new FieldDefinition(field) { fieldName = fieldId, overFieldName = fieldId });
    }

    public virtual bool SaveFile(string guid, byte[] fileInfoData)
    {
        return false;
    }

    public void Release()
    {
        Fields.Clear();
        FilterValues?.Clear();
        objectFilter = null;
        objectHavingFilter = null;
        SqlCommand = null;
        StartIndex = 1;
        Filters = null;
        HavingFilters = null;

        Outfields.Clear();
        Formulas?.Clear();
        Aggregates?.Clear();
        OrderBys?.Clear();
        SubTables?.Clear();
        Exceptions = null;
        foreach (SubDataSource subDataSource in SubTables?.Values ?? Enumerable.Empty<SubDataSource>())
        {
            subDataSource.dataSource?.Release();
            _ = (subDataSource.dataSource?.Close());
        }

        SubTables?.Clear();
    }

    // ReSharper disable once UnusedMember.Global
    public virtual SqlXml GetSqlXml(int i)
    {
        return null;
    }

    protected enum PageingVersion
    {
        TopRows = 1,
        OffsetRows = 2
    }

    public class ExtractedParentEntity
    {
        public ParentEntity ParentEntityRelation { get; set; }
        public string TableName { get; set; }
    }
}
