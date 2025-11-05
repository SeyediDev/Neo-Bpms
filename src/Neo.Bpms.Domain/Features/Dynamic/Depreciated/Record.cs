using System.Collections.Specialized;
using System.Dynamic;
using System.Linq.Expressions;

namespace Neo.Bpms.Domain.Features.Dynamic.Depreciated;

public class Record : DynamicObject
{
    #region obj with reflection

    private readonly Type _objType;
    private object _obj;

    private BindingFlags _commonBindingFlags =
        BindingFlags.Instance |
        BindingFlags.Public;

    public Record(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");

        _obj = obj;
        _objType = obj.GetType();
    }

    public Record(Type objType)
    {
        if (objType == null)
            throw new ArgumentNullException("objType");

        _objType = objType;
    }

    public Record()
    {
        _obj = null;
    }

    public void CallConstructor()
    {
        CallConstructor(new Type[0], new object[0]);
    }

    public void CallConstructor(Type[] paramTypes, object[] paramValues)
    {
        ConstructorInfo ctor = _objType.GetConstructor(paramTypes);
        if (ctor == null)
        {
            throw new ApplicationException(Constants.ErrorMessages.ProxyCtorNotFound);
        }

        _obj = ctor.Invoke(paramValues);
    }

    public object GetProperty(string property)
    {
        object retval = _objType.InvokeMember(
            property,
            BindingFlags.GetProperty | _commonBindingFlags,
            null /* Binder */,
            _obj,
            null /* args */);

        return retval;
    }

    public object SetProperty(string property, object value)
    {
        object retval = _objType.InvokeMember(
            property,
            BindingFlags.SetProperty | _commonBindingFlags,
            null /* Binder */,
            _obj,
            new[] { value });

        return retval;
    }

    public object GetField(string field)
    {
        if (!_fieldValues.TryGetValue(field, out object retval) && _obj != null)
            retval = _objType.InvokeMember(
            field,
            BindingFlags.GetField | _commonBindingFlags,
            null /* Binder */,
            _obj,
            null /* args */);
        return retval;
    }
    public object SetField(string field, object value)
    {
        object retval;
        if (_obj != null && _objType.GetField(field) != null)
        {
            retval = _objType.InvokeMember(
                field,
                BindingFlags.SetField | _commonBindingFlags,
                null /* Binder */,
                _obj,
                new[] { value });
        }
        else
        {
            if (!_fieldValues.ContainsKey(field))
                _fieldValues.Add(field, value);
            else
                _fieldValues[field] = value;
            retval = value;
        }
        return retval;
    }

    public object CallMethod(string method, params object[] parameters)
    {
        object retval = _objType.InvokeMember(
            method,
            BindingFlags.InvokeMethod | _commonBindingFlags,
            null /* Binder */,
            _obj,
            parameters /* args */);

        return retval;
    }

    public object CallMethod(string method, Type[] types,
                             object[] parameters)
    {
        if (types.Length != parameters.Length)
            throw new ArgumentException(
                Constants.ErrorMessages.ParameterValueMistmatch);

        MethodInfo mi = _objType.GetMethod(method, types);
        if (mi == null)
            throw new ApplicationException(string.Format(
                Constants.ErrorMessages.MethodNotFound, method));

        object retval = mi.Invoke(_obj, _commonBindingFlags, null,
                                  parameters, null);

        return retval;
    }

    public Type ObjectType
    {
        get { return _objType; }
    }

    public object ObjectInstance
    {
        get { return _obj; }
    }

    public BindingFlags BindingFlags
    {
        get { return _commonBindingFlags; }

        set { _commonBindingFlags = value; }
    }

    #endregion obj with reflection
    #region Fields Memory Dictionary
    private readonly Dictionary<string, object> _fieldValues = [];
    #endregion Fields Memory Dictionary

    #region dynamic language interfaces

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        try
        {
            SetField(binder.Name, value);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        //			return FieldValues.TryGetValue(binder.Name, out result);
        try
        {
            result = GetField(binder.Name);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }
    public override bool TryDeleteMember(DeleteMemberBinder binder)
    {
        return TryDeleteField(binder.Name);
    }
    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
        try
        {
            result = CallMethod(binder.Name, args);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
    {
        bool bresult = true;
        result = null;
        try
        {
            int i = 0;

            foreach (string argumentName in binder.CallInfo.ArgumentNames)
            {
                if (binder.ReturnType != typeof(void) && i >= binder.CallInfo.ArgumentCount - 1)
                {
                    result = GetField(argumentName);
                    if (result.GetType() != binder.ReturnType)
                        return false;
                }
                else
                {

                    if (_obj != null && _objType.GetField(argumentName) != null)
                    {
                        _objType.InvokeMember(argumentName, BindingFlags.SetField | _commonBindingFlags, null /* Binder */, _obj,
                                              new[] { args[i] });
                    }
                    else
                    {
                        if (!_fieldValues.ContainsKey(argumentName))
                        {
                            bresult = false;
                            break;
                        }
                        _fieldValues[argumentName] = args[i];
                    }
                }
                i++;
            }
        }
        catch (Exception)
        {
            bresult = false;
        }
        return bresult;
    }
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    {
        result = null;
        if (binder.CallInfo.ArgumentCount == 1)
        {
            object index = indexes[0];
            string s = index as string;
            if (s != null)
            {
                result = GetField(s);
                if (result.GetType() == binder.ReturnType)
                    return true;
            }
        }
        return false;
    }
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
    {
        if (binder.CallInfo.ArgumentCount == 1)
        {
            object index = indexes[0];
            string s = index as string;
            if (s != null)
            {
                SetField(s, value);
                return true;
            }
        }
        return false;
    }
    public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
    {
        if (binder.CallInfo.ArgumentCount == 1)
        {
            object index = indexes[0];
            string s = index as string;
            if (s != null)
            {
                return TryDeleteField(s);
            }
        }
        return false;
    }

    private bool TryDeleteField(string s)
    {
        if (_fieldValues.ContainsKey(s))
        {
            _fieldValues.Remove(s);
            return true;
        }
        return false;
    }

    public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
    {
        Record r = new(binder.ReturnType);
        for (int i = 0; i < binder.CallInfo.ArgumentCount; i++)
        {
            r.SetField(binder.CallInfo.ArgumentNames[i], args[i]);
        }
        result = r;
        return true;
    }
    //public static object getPropertyValue(object obj, string name)
    //{
    //	var type = obj.GetType();
    //	if (type.IsClass)
    //	{
    //		if (name == "this")
    //			return obj;
    //		var property = type.GetProperty(name);
    //		if (property != null)
    //		{
    //			return property.GetValue(obj);
    //		}
    //		var field = type.GetField(name);
    //		if (field != null)
    //		{
    //			return field.GetValue(obj);
    //		}
    //	}
    //	if (type.IsEnum)
    //	{
    //		var names = type.GetEnumNames();
    //		var values = type.GetEnumValues();

    //		var i = 0;
    //		foreach (var v in values)
    //		{
    //			if (names[i] == name)
    //			{
    //				return v;
    //			}
    //			i++;
    //		}
    //	}
    //	return "undefined";
    //}

    public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
    {
        if (binder.Operation == ExpressionType.LeftShiftAssign)
        {
            Append(arg);
            result = this;
            return true;
        }

        if (binder.Operation == ExpressionType.LeftShift)
        {
            Record r = new();
            r.Append(this);
            r.Append(arg);
            result = r;
            return true;
        }

        else if ((
            binder.Operation == ExpressionType.LessThan ||
            binder.Operation == ExpressionType.LessThanOrEqual ||
            binder.Operation == ExpressionType.GreaterThan ||
            binder.Operation == ExpressionType.GreaterThanOrEqual ||
            binder.Operation == ExpressionType.Equal ||
            binder.Operation == ExpressionType.NotEqual
            ) && arg is Record)
        {
            Record bArg = arg as Record;
            result = !(binder.Operation == ExpressionType.LessThan || binder.Operation == ExpressionType.GreaterThan || binder.Operation == ExpressionType.NotEqual);
            foreach (KeyValuePair<string, object> item in _fieldValues)
            {
                object a = GetField(item.Key);
                object b = bArg.GetField(item.Key);
                if (a == null)
                {
                    return binder.Operation == ExpressionType.LessThan || binder.Operation == ExpressionType.LessThanOrEqual || binder.Operation == ExpressionType.NotEqual;
                }
                if (b == null)
                {
                    return binder.Operation == ExpressionType.GreaterThan || binder.Operation == ExpressionType.GreaterThanOrEqual || binder.Operation == ExpressionType.NotEqual;
                }
                if ((dynamic)a != b)
                {
                    if (binder.Operation == ExpressionType.LessThan || binder.Operation == ExpressionType.LessThanOrEqual)
                    {
                        result = (dynamic)a < b;
                        break;
                    }
                    if (binder.Operation == ExpressionType.GreaterThan || binder.Operation == ExpressionType.GreaterThanOrEqual)
                    {
                        result = (dynamic)a < b;
                        break;
                    }
                    if (binder.Operation == ExpressionType.Equal)
                    {
                        result = false;
                        break;
                    }
                    if (binder.Operation == ExpressionType.NotEqual)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return true;
        }
        return base.TryBinaryOperation(binder, arg, out result);
    }

    private void Append(object arg)
    {
        Type type = arg.GetType();
        if (type.IsClass)
        {
            foreach (FieldInfo item in type.GetFields())
            {
                SetField(item.Name, item.GetValue(arg));
            }
            foreach (PropertyInfo item in type.GetProperties())
            {
                SetField(item.Name, item.GetValue(arg));
            }
        }
        else
        {
            SetField(type.Name, arg);
        }
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
        IEnumerable<string> retVal = _fieldValues.Select(fv => fv.Key);
        return _obj != null ? retVal.Union(_objType.GetMembers().Select(memberInfo => memberInfo.Name)) : retVal;
    }
    public override string ToString()
    {
        NameValueCollection nvs = [];
        foreach (KeyValuePair<string, object> fieldValue in _fieldValues)
        {
            nvs.Add(fieldValue.Key, fieldValue.Value.ToString());
        }
        if (_obj != null)
        {
            foreach (FieldInfo fieldInfo in _objType.GetFields())
            {
                nvs.Add(fieldInfo.Name, GetField(fieldInfo.Name).ToString());
            }
            foreach (PropertyInfo propertyInfo in _objType.GetProperties())
            {
                nvs.Add(propertyInfo.Name, GetProperty(propertyInfo.Name).ToString());
            }
        }
        return nvs.ToString();
    }


    #endregion dynamic language interfaces

    #region Fields State
    protected Dictionary<string, EFieldState> FieldStates = [];
    public EFieldState GetFieldState(string name)
    {
        return FieldStates.TryGetValue(name, out EFieldState value) ? value : EFieldState.NotInitialized;
    }
    public bool ChangeState(string name, EFieldState newState)
    {
        if (FieldStates.ContainsKey(name))
            FieldStates.Add(name, newState);
        else
            FieldStates[name] = newState;
        return true;
    }
    #endregion Fields State

    #region Record Identification
    public object Id;
    #endregion

    #region Record State
    public object StateId;
    #endregion
}
