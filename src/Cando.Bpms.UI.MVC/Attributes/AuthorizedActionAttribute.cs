namespace Neo.Bpms.UI.MVC.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizedActionAttribute : Attribute
{
    public AuthorizedActionAttribute()
    {
    }
}
