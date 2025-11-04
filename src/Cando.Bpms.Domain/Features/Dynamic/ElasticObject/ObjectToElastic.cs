namespace Neo.Bpms.Domain.Features.Dynamic;

public partial class ElasticObject
{
    public static ElasticObject ToElastic(object obj)
    {
        ElasticObject elastic = new();
        elastic.MergeObject(obj);
        return elastic;
    }

    public void MergeObject(object obj)
    {
        Type type = obj.GetType();
        if (!type.IsClass || ReflectionTools.IsGenericList(type))
            return;
        IEnumerable<MemberInfo> members = ReflectionField.Members(type);
        foreach (MemberInfo member in members)
        {
            object fieldValue = ReflectionField.GetValue(obj, member);
            if (fieldValue == null) continue;
            AddAttribute(member.Name, fieldValue);
        }
    }
}
