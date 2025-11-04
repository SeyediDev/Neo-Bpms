using Neo.Bpms.Domain.Entities.Cmmn.Entities;

namespace Neo.Bpms.Domain.Entities.Cmmn;

public class DataEvent : BaseModelClass
{
    public DataEvent()
    {
    }
    public DataEvent(Entity entity, string id, string name, ExpressionNode condition) ://, Entity structure
        base(entity, id, name)
    {
        this.condition = condition;
    }
    public Entity Entity => Parent as Entity;
    public ExpressionNode condition;
}