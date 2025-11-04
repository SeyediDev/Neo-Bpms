using Neo.Bpms.Domain.Expressions;

namespace Neo.Bpms.Domain.Utility;

public partial class LocalParameters
{
    public T To<T>()
        where T : new()
    {
        Type type = typeof(T);
        return (T)To(type);
    }

    public object To(Type type)
    {
        ConstructorInfo ctr = type.GetConstructor(new Type[0]);
        object obj = ctr?.Invoke(new object[0]);
        MergeToObject(obj);
        return obj;
    }

    public void MergeToObject(object obj)
    {
        Type type = obj.GetType();
        foreach (KeyValuePair<string, object> item in this)
        {
            PropertyInfo propertyInfo = null;
            FieldInfo fieldInfo = Reflect(type, item.Key, ref propertyInfo);
            Type fieldType = fieldInfo?.FieldType ?? propertyInfo?.PropertyType;
            try
            {
                if (fieldInfo != null)
                    fieldInfo.SetValue(obj, ConvertType.ConvertValue(item.Value, fieldType));
                else if (propertyInfo != null)
                    propertyInfo.SetValue(obj, ConvertType.ConvertValue(item.Value, fieldType));
            }
            catch
            {
                if (!fieldType.IsClass)
                    throw;
            }
        }
    }

    //todo use ReflectionTools
    private static FieldInfo Reflect(Type type, string key, ref PropertyInfo propertyInfo)
    {
        FieldInfo fieldInfo = type.GetField(key);
        if (fieldInfo == null)
            propertyInfo = type.GetProperty(key);
        return fieldInfo;
    }
}
