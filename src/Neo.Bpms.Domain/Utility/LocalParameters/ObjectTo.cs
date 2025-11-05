using System.Collections;

namespace Neo.Bpms.Domain.Utility;

public partial class LocalParameters
{
    public static LocalParameters DeepToLocalParameters(object obj)
    {
        LocalParameters localParameters = [];
        localParameters.DeepMergeObject(obj);
        return localParameters;
    }

    public void MergeObject(object obj)
    {
        void AddThings(MemberInfo[] memberInfos)
        {
            foreach (MemberInfo memberInfo in memberInfos)
            {
                object fieldValue = null;
                if (memberInfo.MemberType == MemberTypes.Field)
                    fieldValue = (memberInfo as FieldInfo)?.GetValue(obj);
                if (memberInfo.MemberType == MemberTypes.Property)
                    fieldValue = (memberInfo as PropertyInfo)?.GetValue(obj);
                if (fieldValue == null) continue;
                AddOrUpdate(memberInfo.Name, fieldValue);
            }
        }

        if (obj == null) return;
        Type type = obj.GetType();
        if (!type.IsClass)
            return;
        AddThings(type.GetFields());
        AddThings(type.GetProperties());
    }
    public void DeepMergeObject(object obj)
    {
        void AddThings(MemberInfo[] memberInfos)
        {
            foreach (MemberInfo memberInfo in memberInfos)
            {
                object fieldValue = null;
                if (memberInfo.MemberType == MemberTypes.Field)
                    fieldValue = (memberInfo as FieldInfo)?.GetValue(obj);
                if (memberInfo.MemberType == MemberTypes.Property)
                    fieldValue = (memberInfo as PropertyInfo)?.GetValue(obj);
                if (fieldValue == null) continue;
                if (ReflectionTools.IsGenericList(fieldValue.GetType()))
                {
                    List<LocalParameters> list = [.. (from object listValue in fieldValue as IEnumerable select DeepToLocalParameters(listValue))];
                    fieldValue = list;
                }
                AddOrUpdate(memberInfo.Name, fieldValue);
            }
        }

        if (obj == null) return;
        Type type = obj.GetType();
        if (!type.IsClass)
            return;
        AddThings(type.GetFields());
        AddThings(type.GetProperties());
    }
}
