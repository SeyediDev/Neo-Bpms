namespace Neo.Bpms.Domain.Models.Cmmn.Entities;

public class Method : BaseModelClass
{
    public Method(Entity entity, string id, string name,
        MethodImplementationType implementationType, string script)
        : base(entity, id, name)
    {
        ImplementationType = implementationType;
        Script = script;
    }

    public Method(Entity entity, string id, ExpressionTree body,
        MethodType methodType, string name = null) //, string enName = null
        : base(entity, id, name)
    {
        Body = body;
        MethodType = methodType;
    }

    public Method(Entity entity, string name, Type returnType, IEnumerable<MethodParam> parameters, MethodType type)
        //string className, string scriptName, 
        : base(entity, name, name)
    {
        //_className = className;
        //_scriptName = scriptName;
        Params = parameters?.ToList();
        MethodType = type;
        Name = name;
        this.returnType = returnType;
    }

    public Type returnType { get; set; }

    public List<MethodParam> Params { get; set; }

    public Method()
    {
    }

    public MethodImplementationType ImplementationType { get; set; }
    public MethodType MethodType;
    public ExpressionTree Body;
    public string Script;
}

public enum MethodImplementationType
{
    CSharp = 1,
    DBMS = 2
}

public enum MethodType
{
    Constructor = 1,
    Destructor = 2,
    Static = 3,
    Method = 4,

    Get = 10,
    Put = 11,
    Insert = 12,
    Delete = 13,

    Updater = 20,
    Reader = 21,
    UpdateAll = 22,
}
