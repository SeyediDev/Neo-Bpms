namespace Neo.Bpms.Domain.Models.Cmmn.Entities;

public class MethodParam
{
    private ParameterInfo _paramInfo;
    private string _name;
    private bool _isOutput;
    private bool isOptional, isRetVal;
    private Type type;

    public MethodParam(ParameterInfo paramInfo)//Type type, 
    {
        _paramInfo = paramInfo;
    }
    public MethodParam()
    {
    }
    public MethodParam(string name, Type type, bool isOut, bool isOptional, bool isRepeatable)
    {
        _paramInfo = null;
        _name = name;
        _isOutput = isOut;
        this.isOptional = isOptional;
        IsRepeatable = isRepeatable;
        this.type = type;
    }
    //		public string FaName { get; set; }
    public string Name
    {
        get => _paramInfo == null ? _name : _paramInfo.Name;
        set => _name = value;
    }
    public bool IsOut
    {
        get => _paramInfo?.IsOut ?? _isOutput;
        set => _isOutput = value;
    }
    public bool IsOptional
    {
        get => _paramInfo?.IsOptional ?? isOptional;
        set => isOptional = value;
    }
    public bool IsRetVal
    {
        get => _paramInfo?.IsRetval ?? isRetVal;
        set => isRetVal = value;
    }
    public bool IsRepeatable { get; set; }
    public Type Type
    {
        get => _paramInfo == null ? type : _paramInfo.ParameterType;
        set => type = value;
    }
    public bool HasDefaultValue;
    public object DefaultValue;
}