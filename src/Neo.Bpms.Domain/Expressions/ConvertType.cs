using System.Collections;
using System.Net;

namespace Neo.Bpms.Domain.Expressions;

public static class ConvertType
{
    public static void SetValue(object obj, string name, object value)
    {
        if (obj is IExpressionValue eObj)
        {
            if (value is IExpressionValue expressionValue)
            {
                value = expressionValue.InternalValue;
                if (value is IExpressionValue expressionValue2)
                    value = expressionValue2.InternalValue;
            }

            eObj.SetField(name, value);
        }
        else
        {
            Type type = obj.GetType();
            MemberInfo member = ReflectionField.FetchMember(type, name);
            switch (member)
            {
                case FieldInfo fieldInfo:
                    if (fieldInfo.IsInitOnly)
                        return;
                    break;
                case PropertyInfo propertyInfo:
                    if (!propertyInfo.CanWrite)
                        return;
                    break;
                default:
                    return;
            }

            object convertedValue = ConvertValue(value, ReflectionField.FetchMemberType(member));
            ReflectionField.SetValue(obj, member, convertedValue);
        }
    }

    public static object ConvertValue(object value, Type propertyType)
    {
        Type nullType = Nullable.GetUnderlyingType(propertyType);
        if (value is IExpressionValue expressionValue)
            value = expressionValue.InternalValue;
        if (propertyType == typeof(object))
            return value;
        object convertedValue = value;
        if (propertyType.IsEnum || nullType != null && nullType.IsEnum)
        {
            if (value == null || value is string && string.IsNullOrEmpty(value.ToString()))
                convertedValue = nullType != null ? null : GetEnumDefaultValue(propertyType);
            else
                convertedValue = Enum.Parse(nullType ?? propertyType, convertedValue.ToString(), true);
            return convertedValue;
        }

        if (propertyType == typeof(IPAddress))
        {
            convertedValue = convertedValue is long
                ? GetIpAddress(Convert.ToInt64(convertedValue))
                : GetIpAddress(convertedValue?.ToString());
        }

        if (propertyType == typeof(string) && value is IPAddress)
        {
            convertedValue = convertedValue?.ToString();
        }
        else if (propertyType == typeof(TimeSpan) || propertyType == typeof(TimeSpan?))
        {
            if (propertyType == typeof(TimeSpan?) && value is string && string.IsNullOrEmpty(value.ToString()))
                convertedValue = null;
            else if (convertedValue is not TimeSpan)
                convertedValue = TimeSpan.FromTicks(Convert.ToInt64(convertedValue));
        }
        else if (propertyType == typeof(bool?) || propertyType == typeof(bool))
        {
            if (propertyType == typeof(bool?) &&
                (value == null || value is string && string.IsNullOrEmpty(value.ToString())))
                convertedValue = null;
            else if (convertedValue is not bool)
                convertedValue = ConvUtill.ToBoolean(convertedValue);
        }
        else if (propertyType == typeof(DateTime?))
        {
            if (value is string && string.IsNullOrEmpty(value.ToString()) || value == null)
                convertedValue = null;
            else if (convertedValue is not TimeSpan && nullType != null)
                convertedValue = Convert.ChangeType(value, nullType);
        }
        else if (propertyType == typeof(long?) ||
                 propertyType == typeof(ulong?) ||
                 propertyType == typeof(bool?) ||
                 propertyType == typeof(double?) ||
                 propertyType == typeof(int?) ||
                 propertyType == typeof(uint?) ||
                 propertyType == typeof(short?) ||
                 propertyType == typeof(ushort?) ||
                 propertyType == typeof(char?))
        {
            if (value is string && string.IsNullOrEmpty(value.ToString()) || value == null)
                convertedValue = null;
            else
            {
                if (nullType != null)
                    convertedValue = Convert.ChangeType(value, nullType);
            }
        }
        else if (ReflectionTools.IsGenericList(propertyType) && ReflectionTools.IsGenericList(value.GetType()))
        {
            List<LocalParameters> listValue = value as List<LocalParameters>;
            Type t = propertyType.GetGenericArguments().First();
            Type genericList = typeof(List<>);
            IList result = (IList)Activator.CreateInstance(genericList.MakeGenericType(t));
            List<object> objects = listValue?.Select(l => l.To(t)).ToList();
            if (objects != null)
            {
                foreach (object o in objects)
                {
                    result.Add(o);
                }

                return result;
            }
        }

        if (convertedValue != null && propertyType != convertedValue.GetType())
        {
            if (convertedValue is IConvertible)
            {
                if (propertyType == typeof(long) ||
                    propertyType == typeof(ulong) ||
                    propertyType == typeof(bool) ||
                    propertyType == typeof(double) ||
                    propertyType == typeof(int) ||
                    propertyType == typeof(uint) ||
                    propertyType == typeof(short) ||
                    propertyType == typeof(ushort) ||
                    propertyType == typeof(char)
                )
                {
                    if (value is string && string.IsNullOrEmpty(value.ToString()))
                        convertedValue = 0;
                    else
                        convertedValue = Convert.ChangeType(value, propertyType);
                }
                else if (propertyType == typeof(List<string>) && value is string val)
                {
                    convertedValue = val.Split(',').ToList();
                }
                else
                {
                    try
                    {
                        convertedValue = Convert.ChangeType(value, nullType ?? propertyType);
                    }
                    catch //(Exception e)
                    {
                        // ignored
                    }
                }
            }
            else
            {
                if (nullType != convertedValue.GetType())
                {
                    throw new Exception($"{value} is not convertible to {propertyType}");
                }
            }
        }

        return convertedValue;
    }

    public static void Copy(object dest, object source)
    {
        List<MemberInfo> sourceMembers = [.. ReflectionField.Members(source.GetType())];
        foreach (MemberInfo member in ReflectionField.Members(dest.GetType()))
        {
            if (sourceMembers.FirstOrDefault(m => m.Name == member.Name) == null)
                continue;
            ReflectionField.GetValue(source, source.GetType(), member.Name, out object value);
            SetValue(dest, member.Name, value);
        }
    }

    public static IPAddress GetIpAddress(string ip)
    {
        return IPAddress.TryParse(ip, out IPAddress ipAddress) ? ipAddress : null;
    }

    public static IPAddress GetIpAddress(long? ip)
    {
        return !ip.HasValue || ip == 0
            ? IPAddress.Loopback
            : !IPAddress.TryParse(ip.ToString(), out IPAddress ipAddress) ? IPAddress.Loopback : ipAddress;
    }

    private static object GetEnumDefaultValue(Type type)
    {
        Array values = type.GetEnumValues();
        int min = 0;
        foreach (object v in values)
        {
            int itemValue = Convert.ToInt32(v);
            min = Math.Min(min, itemValue);
            if (itemValue == 0)
                return v;
        }

        return min;
    }
}