using Neo.Bpms.Domain.Expressions;

namespace Neo.Bpms.Domain.Features.Dynamic;

public partial class ElasticObject
{
    public T To<T>(string prefix = null)
        where T : new()
    {
        Type type = typeof(T);
        return (T)To(type, prefix);
    }

    public object To(Type type, string prefix = null)
    {
        ElasticObject el = this;
        ConstructorInfo ctr = type.GetConstructor([]);
        object obj = ctr?.Invoke([]);

        if (!string.IsNullOrEmpty(prefix))
            el = GetSubElasticObject(prefix);
        Dictionary<string, FieldItem> subs = [];
        foreach (KeyValuePair<string, ElasticObject> item in el.Attributes ?? [])
        {
            string[] keyIds = item.Key.Split('.');
            string key = keyIds[0];
            object value = item.Value?.InternalValue;
            if (value is IExpressionValue expressionValue)
            {
                value = expressionValue.InternalValue;
                if (value is IExpressionValue expressionValue2)
                    value = expressionValue2.InternalValue;
            }
            if (keyIds.Length == 1)
            {
                MemberInfo member = ReflectionField.FetchMember(type, key);
                if (member == null)
                    continue;
                if (ReflectionField.IsClass(member) is true)
                {
                    if (value != null && ReflectionField.IsClass(value.GetType()) is false)
                        continue;
                }

                ConvertType.SetValue(obj, key, value);
            }
            else
            {
                if (!subs.TryGetValue(key, out FieldItem fieldItem))
                {
                    fieldItem = new FieldItem { Items = [] };
                    fieldItem.FieldInfo = Reflect(type, key, ref fieldItem.PropertyInfo);
                    subs.Add(key, fieldItem);
                }

                fieldItem.Items.Add(item);
            }
        }

        FillSubObjects(subs, obj);

        return obj;
    }

    public ElasticObject GetSubElasticObject(string subRecordName)
    {
        ElasticObject subElasticObject = null;
        foreach (KeyValuePair<string, ElasticObject> item in Attributes)
        {
            if (item.Key.Length < subRecordName.Length) continue;
            if (item.Key[..subRecordName.Length] != subRecordName) continue;
            subElasticObject ??= new ElasticObject(subRecordName);
            string key = item.Key[subRecordName.Length..];
            subElasticObject.SetField(key, item.Value);
        }

        return subElasticObject;
    }

    public static FieldInfo Reflect(Type type, string key, ref PropertyInfo propertyInfo)
    {
        FieldInfo fieldInfo = type.GetField(key);
        if (fieldInfo == null)
            propertyInfo = type.GetProperty(key);
        return fieldInfo;
    }

    private static void FillSubObjects(Dictionary<string, FieldItem> subs, object obj)
    {
        foreach (KeyValuePair<string, FieldItem> fieldItem in subs)
        {
            if (fieldItem.Value.FieldInfo == null && fieldItem.Value.PropertyInfo == null)
                continue;
            ElasticObject subElastic = new();
            foreach (KeyValuePair<string, ElasticObject> item in fieldItem.Value.Items)
                subElastic.AddAttribute(item.Key[(fieldItem.Key.Length + 1)..], item.Value?.InternalValue);
            var fieldType = fieldItem.Value.FieldInfo?.FieldType ?? fieldItem.Value.PropertyInfo?.PropertyType;
            var value = subElastic.To(fieldType);
            ConvertType.SetValue(obj, fieldItem.Key, value);
        }
    }

    private class FieldItem
    {
        public PropertyInfo PropertyInfo;
        public FieldInfo FieldInfo;
        public List<KeyValuePair<string, ElasticObject>> Items;
    }
}
