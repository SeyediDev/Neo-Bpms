namespace Neo.Bpms.Domain.Features.Cmmn.CodeFirst;

public static class TypeExtension
{
    public static bool TryGetInterfaceGenericParameters(this Type type, Type @interface, out Type typeParameter)
    {
        typeParameter = null;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == @interface)
        {
            typeParameter = type.GetGenericArguments().FirstOrDefault();
            return typeParameter != null;
        }

        Type implements = type.FindInterfaces((ty, obj) => ty.IsGenericType && ty.GetGenericTypeDefinition() == @interface, null).FirstOrDefault();
        if (implements == null)
        {
            return false;
        }

        typeParameter = implements.GetGenericArguments().FirstOrDefault();
        return typeParameter != null;
    }
}
