namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement dml functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    private bool SetKeyValue<T>(T obj)
    {
        if (DataReader == null)
        {
            return false;
        }

        do
        {
            if (DataReader == null || !ReadFromDataReader())
            {
                break;
            }

            object[] values = new object[DataReader.FieldCount];
            _ = DataReader.GetValues(values);
            for (int i = 0; i < DataReader.FieldCount; i++)
            {
                object value = values[i];
                if (value is not DBNull && (value as string) != "NULL")
                {
                    string name = DataReader.GetName(i);
                    EntityField fieldMeta = DataSrcDefinition.Entity.GetFieldByDbName(name);
                    SetField(obj, fieldMeta?.Id ?? name, value);
                }
            }
        } while (DataReader.NextResult());

        SetParentPkv(obj);
        return true;
    }

    private void SetParentPkv<T>(T obj)
    {
        Type type = obj.GetType();
        foreach (EntityField parentField in DataSrcDefinition.Entity?.ParentEntities ?? [])
        {
            if (!ReflectionField.GetValue(obj, parentField.Id, out object parentObject))
            {
                //todo loop in fields to find parent field. may be parent 2 parent occurred.
            }
            SetParentPkv(obj, type, parentField, parentObject);
            SetParentBoolField(obj, parentField);
        }
    }

    private static void SetParentBoolField<T>(T obj, EntityField parentField)
    {
        string parentBoolFieldId = parentField.ParentEntity.BooleanFieldIdInParentThatPresentMe;
        if (string.IsNullOrEmpty(parentBoolFieldId))
        {
            return;
        }

        object v = GetField(obj, parentBoolFieldId);
        if (v != null && !v.Equals(true))
        {
            SetField(obj, parentBoolFieldId, true);
        }
    }

    protected virtual string GetFieldType(Type type)
    {
        return "VARCHAR";
    }

    private string FetchKeyFilterValue<T>(T obj, T keyObj, out string keyValue)
    {
        string value = "";
        string keyFilterValues = string.Join(" AND ", DataSrcDefinition.Entity.KeyFields.Select(key =>
        {
            object o = GetField(keyObj, key.Id) ?? GetField(obj, key.Id);
            value = GetSqlValue(o, key, key.CSharpType);
            return "[" + key.DbFieldName + "]=" + value;
        }));
        keyValue = value;
        return keyFilterValues;
    }

    protected object GetFieldFromObject<T>(ElasticObject elasticObject, T obj, string fieldName)
    {
        if (obj == null && elasticObject != null)
        {
            return elasticObject[fieldName];
        }

        return obj != null ? GetField(obj, fieldName) : null;
    }

    private IEnumerable<ReferenceField> Parents(Entity entity)
    {
        List<ReferenceField> r = entity.entityFields.Values.Where(f => f.ReferenceFields != null)
            .Where(f => f.IsForParentEntity(entity)).SelectMany(f =>
                f.ReferenceFields.Where(rf => rf.Relationship is ParentEntity)
                    ).GroupBy(g => $"{g.Relationship.DestEntity.NamespaceId}.{g.Relationship.DestEntity.Id}")
            .Select(parent => parent.First()).ToList();
        return r;
    }

    private void SetException(string errorCode)
    {
        if (Exceptions == null)
        {
            SetExceptionIfExists(logger, errorCode, Connection.Exceptions,
                new DataLayerException(DataEngineException.DefaultQueryFailed, "error in command " + SqlCommand));
        }
    }

    private void SetException(string errorCode, Exception e)
    {
        SetExceptionIfExists(logger, errorCode, null,
            new DataLayerException(DataEngineException.DefaultQueryFailed, e.Message)
            {
                Source = e.Source
            });
    }

    public class SqlValueField(EntityField field)
    {
        public EntityField Field { get; set; } = field;
        public string FieldId { get; set; }
        public object Value { get; set; }
        public string SqlValue { get; set; }
    }
}
