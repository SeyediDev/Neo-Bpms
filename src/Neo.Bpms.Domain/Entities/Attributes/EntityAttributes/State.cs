namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

/// <summary>
/// وضعیت رکورد
/// </summary>
public class State : EAttr_State
{
    /// <summary>
    /// وضعیت رکورد
    /// </summary>
    /// <param name="id">شناسه</param>
    /// <param name="name">نام</param>
    /// <param name="category">نوع</param>
    public State(int id, string name, EntityStateCategory category = EntityStateCategory.ActiveNode)
        : base(id, "S" + id, name, category)
    {
    }

    public State(string name, EntityStateCategory category = EntityStateCategory.ActiveNode)
        : base(0, "S", name, category)
    {
    }
}