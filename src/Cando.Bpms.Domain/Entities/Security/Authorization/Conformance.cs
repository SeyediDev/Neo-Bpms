using Neo.Bpms.Domain.Entities.Base.Exceptions;
using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Security.Authorization;

public class Conformance(Entity entity, EntityField field, string id, string name, UserSecurityAccessFlags access, ConformanceLevel level, string subject,
    ExpressionNode expression, ExceptionInformation exception) : BaseModelClass(entity, id, name)
{
    public string roleId;
    public Entity entity = entity;
    public EntityField field = field;
    public ExceptionInformation exception = exception;
    public ExpressionNode expression = expression;
    public ConformanceLevel level = level;
    public UserSecurityAccessFlags access = access;
    public string subject = subject;
}
