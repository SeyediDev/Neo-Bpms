using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace Neo.Bpms.Domain.Features.Dynamic;

/// <summary>
/// See http://amazedsaint.blogspot.com/2010/02/introducing-elasticobject-for-net-40.html for details
/// </summary>
public partial class ElasticObject : DynamicObject, IElasticHierarchyWrapper, INotifyPropertyChanged,
    IExpressionValue
{
    #region Private

    private readonly IElasticHierarchyWrapper _elasticProvider = new SimpleHierarchyWrapper();
    //		private NodeType _nodeType = NodeType.Element;

    #endregion

    #region Ctor

    public ElasticObject()
    {
        InternalName = "id" + Guid.NewGuid().ToString();
    }

    public ElasticObject(string name)
    {
        InternalName = name;
    }

    public ElasticObject(string name, object value)
        : this(name)
    {
        if (value is IDictionary<string, object> objects)
        {
            foreach (KeyValuePair<string, object> val in objects)
            {
                object v = val.Value;
                if (v is ElasticObject e)
                    v = e.InternalValue;
                SetField(val.Key, v);
            }
        }
        else
            InternalValue = value;
    }

    #endregion

    #region obj with reflection

    public void CallConstructor(Type objType, Type[] paramTypes, object[] paramValues)
    {
        ConstructorInfo ctor = objType.GetConstructor(paramTypes);
        if (ctor == null)
        {
            throw new ApplicationException("Proxy not found");
        }

        InternalValue = ctor.Invoke(paramValues);
    }

    public bool CallMethod(string method, params object[] parameters)
    {
        return CallMethod(method, out _, parameters);
    }

    public bool CallMethod(string method, out object result, params object[] parameters)
    {
        object internalValue = InternalValue;
        if (internalValue != null)
        {
            Type objType = internalValue.GetType();
            MethodInfo mi = objType.GetMethod(method);
            if (mi != null)
            {
                result = mi.Invoke(internalValue, parameters);
                return true;
            }
        }

        result = null;
        return false;
    }

    public bool CallMethod(string method, out object result, Type[] types, object[] parameters)
    {
        object internalValue = InternalValue;
        if (internalValue != null)
        {
            Type objType = internalValue.GetType();
            if (types.Length != parameters.Length)
                throw new ArgumentException("Parameter count mismatch.");

            MethodInfo mi = objType.GetMethod(method, types);
            if (mi == null)
                throw new ApplicationException("method " + method + " not found.");

            result = mi.Invoke(internalValue, parameters);
            return true;
        }

        result = null;
        return false;
    }

    public Type? ObjectType
    {
        get
        {
            object internalValue = InternalValue;
            return internalValue?.GetType();
        }
    }

    public object? ObjectInstance => InternalValue;

    #endregion obj with reflection

    #region Methods

    /// <summary>
    /// Add a member to this element, with the specified value
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    internal ElasticObject CreateOrGetAttribute(string memberName, object value)
    {
        //var _obj = InternalValue;
        if (!HasAttribute(memberName)
        ) //&& (_obj == null || (_obj.GetType().GetField(memberName) == null && _obj.GetType().GetProperty(memberName) == null))
            AddAttribute(memberName, new ElasticObject(memberName, value));
        return Attribute(memberName);
    }

    /// <summary>
    /// Fully qualified name
    /// </summary>
    public string InternalFullName
    {
        get
        {
            string path = InternalName;
            ElasticObject parent = InternalParent;

            while (parent != null)
            {
                path = parent.InternalName + "_" + path;
                parent = parent.InternalParent;
            }

            return path;
        }
    }

    /// <summary>
    /// Interpret the invocation of a binary operation
    /// </summary>
    public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
    {
        //if (binder.Operation == ExpressionType.LeftShiftAssign && _nodeType == NodeType.Element)
        //{
        //	InternalContent = arg;
        //	result = this;
        //	return true;
        //}
        if (binder.Operation == ExpressionType.LeftShiftAssign) //&& _nodeType == NodeType.Attribute)
        {
            InternalValue = arg;
            result = this;
            return true;
        }

        if (binder.Operation == ExpressionType.LeftShift)
        {
            if (arg is string)
            {
                ElasticObject exp = new(arg as string, null); // { _nodeType = NodeType.Element };
                AddElement(exp);
                result = exp;
                return true;
            }

            if (arg is ElasticObject)
            {
                ElasticObject eobj = arg as ElasticObject;
                //if (!Elements.Contains(eobj))
                AddElement(eobj);
                result = eobj;
                return true;
            }

            if (arg is XElement element)
            {
                ElasticObject o = element.ToElastic();
                AddElement(o);
                result = o;
                return true;
            }

            if (arg.GetType().IsClass)
            {
                ElasticObject exp = new(arg.GetType().Name, null); // { _nodeType = NodeType.Element };
                exp.AddClassAttributes(arg);
                AddElement(exp);
                result = exp;
            }
            else
            {
                ElasticObject exp = new(arg.GetType().Name, arg); // { _nodeType = NodeType.Element };
                AddElement(exp);
                result = exp;
            }

            return true;
        }
        else if ((binder.Operation == ExpressionType.LessThan || binder.Operation == ExpressionType.LessThanOrEqual ||
                  binder.Operation == ExpressionType.GreaterThan ||
                  binder.Operation == ExpressionType.GreaterThanOrEqual ||
                  binder.Operation == ExpressionType.Equal || binder.Operation == ExpressionType.NotEqual))
        {
            if (arg is ElasticObject)
            {
                ElasticObject bArg = arg as ElasticObject;
                result = !(binder.Operation == ExpressionType.LessThan ||
                           binder.Operation == ExpressionType.GreaterThan ||
                           binder.Operation == ExpressionType.NotEqual);
                foreach (KeyValuePair<string, ElasticObject> item in Attributes)
                {
                    if (!GetField(item.Key, out object a)) a = null;
                    if (!bArg.GetField(item.Key, out object b)) b = null;
                    if (a == null)
                    {
                        return (binder.Operation == ExpressionType.LessThan ||
                                binder.Operation == ExpressionType.LessThanOrEqual ||
                                binder.Operation == ExpressionType.NotEqual);
                    }

                    if (b == null)
                    {
                        return (binder.Operation == ExpressionType.GreaterThan ||
                                binder.Operation == ExpressionType.GreaterThanOrEqual ||
                                binder.Operation == ExpressionType.NotEqual);
                    }

                    if ((dynamic)a != b)
                    {
                        if (binder.Operation == ExpressionType.LessThan ||
                            binder.Operation == ExpressionType.LessThanOrEqual)
                        {
                            result = (dynamic)a < b;
                            break;
                        }

                        if (binder.Operation == ExpressionType.GreaterThan ||
                            binder.Operation == ExpressionType.GreaterThanOrEqual)
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

            //if (this.InternalValue.GetType() == arg.GetType() )
            {
                switch (binder.Operation)
                {
                    case ExpressionType.NotEqual:
                        result = (dynamic)InternalValue != arg;
                        break;
                    case ExpressionType.Equal:
                        result = (dynamic)InternalValue == arg;
                        break;
                    case ExpressionType.LessThan:
                        result = (dynamic)InternalValue < arg;
                        break;
                    case ExpressionType.LessThanOrEqual:
                        result = (dynamic)InternalValue <= arg;
                        break;
                    case ExpressionType.GreaterThan:
                        result = (dynamic)InternalValue > arg;
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        result = (dynamic)InternalValue >= arg;
                        break;
                    default:
                        result = null;
                        return false;
                }

                return true;
            }
        }

        if (binder.Operation == ExpressionType.ExclusiveOrAssign)
        {
            Namespace = arg;
            result = this;
            return true;
        }

        return base.TryBinaryOperation(binder, arg, out result);
    }

    private void AddClassAttributes(object classArg)
    {
        foreach (FieldInfo item in classArg.GetType().GetFields())
        {
            object obj = item.GetValue(classArg);
            if (obj.GetType().IsArray)
            {
                if (obj is Array objs)
                {
                    foreach (object item2 in objs)
                    {
                        ElasticObject exp = new(obj.GetType().Name, null); // { _nodeType = NodeType.Element };
                        exp.AddClassAttributes(item2);
                        AddElement(exp);
                    }
                }
            }
            else if (obj.GetType().IsClass)
            {
                if (obj is Dictionary<object, object> rs)
                {
                    foreach (KeyValuePair<object, object> item3 in rs)
                    {
                        ElasticObject exp = new(obj.GetType().Name,
                            null); //item3.Key// { _nodeType = NodeType.Element };
                        exp.AddClassAttributes(item3.Value);
                        AddElement(exp);
                    }
                }
                else
                {
                    if (obj is SortedList<object, object> rs1)
                    {
                        foreach (KeyValuePair<object, object> item3 in rs1)
                        {
                            ElasticObject exp = new(obj.GetType().Name,
                                null); //item3.Key// { _nodeType = NodeType.Element };
                            exp.AddClassAttributes(item3.Value);
                            AddElement(exp);
                        }
                    }
                    else
                    {
                        if (obj is SortedDictionary<object, object> rs2)
                        {
                            foreach (KeyValuePair<object, object> item3 in rs2)
                            {
                                ElasticObject exp = new(obj.GetType().Name,
                                    null); //item3.Key// { _nodeType = NodeType.Element };
                                exp.AddClassAttributes(item3.Value);
                                AddElement(exp);
                            }
                        }
                        else
                        {
                            if (obj is IEnumerable<object> rs3)
                            {
                                foreach (object item3 in rs3)
                                {
                                    ElasticObject exp = new(obj.GetType().Name,
                                        null); // { _nodeType = NodeType.Element };
                                    exp.AddClassAttributes(item3);
                                    AddElement(exp);
                                }
                            }
                            else
                            {
                                ElasticObject exp = new(obj.GetType().Name, null); // { _nodeType = NodeType.Element };
                                exp.AddClassAttributes(obj);
                                AddElement(exp);
                            }
                        }
                    }
                }
            }
            else
                AddAttribute(item.Name, obj);
        }

        foreach (PropertyInfo item in classArg.GetType().GetProperties())
        {
            object obj = item.GetValue(classArg);
            if (obj.GetType().IsClass)
            {
                ElasticObject exp = new(obj.GetType().Name, null); // { _nodeType = NodeType.Element };
                exp.AddClassAttributes(obj);
                AddElement(exp);
            }
            else
                AddAttribute(item.Name, obj);
        }
    }

    /// <summary>
    /// Try the unary operation.
    /// </summary>
    public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
    {
        if (binder.Operation == ExpressionType.OnesComplement)
        {
            result = InternalValue; // (_nodeType == NodeType.Element) ? InternalContent : InternalValue;
            return true;
        }

        return base.TryUnaryOperation(binder, out result);
    }

    /// <summary>
    /// Handle the indexer operations
    /// </summary>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    {
        //result = null;
        if ((indexes.Length == 1) && indexes[0] == null)
        {
            result = _elasticProvider.Elements.ToList();
        }
        else if ((indexes.Length == 1) && indexes[0] is int)
        {
            result = Elements.ElementAt((int)indexes[0]);
        }
        else if ((indexes.Length == 1) && indexes[0] is string)
        {
            return GetField(indexes[0] as string, out result);
        }
        else if ((indexes.Length == 1) && indexes[0] is Func<dynamic, bool>)
        {
            Func<dynamic, bool> filter = indexes[0] as Func<dynamic, bool>;
            result = Elements.Where(c => filter != null && filter(c)).ToList();
        }
        else
        {
            IEnumerable<string> list = indexes.Cast<string>();
            result = Elements.Where(c => list.Contains(c.InternalName)).ToList();
        }

        return true;
    }

    public override bool TrySetIndex(SetIndexBinder binder, Object[] indexes, Object value)
    {
        if (binder.CallInfo.ArgumentCount == 1)
        {
            if (indexes[0] is string)
            {
                SetField(indexes[0] as string, value);
                return true;
            }

            if (indexes[0] is int)
            {
                ElasticObject e = Elements.ElementAt((int)indexes[0]);
                e.InternalValue = value;
                return true;
            }
        }

        return false;
    }

    public override bool TryDeleteIndex(DeleteIndexBinder binder, Object[] indexes)
    {
        if (binder.CallInfo.ArgumentCount == 1)
        {
            if (indexes[0] is string)
            {
                return TryDeleteField(indexes[0] as string);
            }

            if (indexes[0] is int)
            {
                ElasticObject e = Elements.ElementAt((int)indexes[0]);
                return TryDeleteField(e.InternalName);
            }
        }

        return false;
    }

    /// <summary>
    /// Catch a get member invocation
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        return GetField(binder.Name, out result);
    }

    private bool TryDeleteField(string s)
    {
        if (_elasticProvider.HasAttribute(s))
        {
            _elasticProvider.RemoveAttribute(s);
            return true;
        }

        ElasticObject obj = _elasticProvider.Element(s);
        if (obj != null)
        {
            _elasticProvider.RemoveElement(obj);
            return true;
        }

        return false;
    }

    public override bool TryCreateInstance(CreateInstanceBinder binder, Object[] args, out Object result)
    {
        ElasticObject r = new();
        for (int i = 0; i < binder.CallInfo.ArgumentCount; i++)
        {
            r.SetField(binder.CallInfo.ArgumentNames[i], args[i]);
        }

        result = r;
        return true;
    }

    public override bool TryDeleteMember(DeleteMemberBinder binder)
    {
        return TryDeleteField(binder.Name);
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
        try
        {
            Type[] types = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = args[i].GetType();
            }

            bool b = CallMethod(binder.Name, out result, types, args);
            if (!b)
            {
                ElasticObject obj = new(binder.Name, null);

                foreach (object a in args)
                {
#if NETFX_CORE
				foreach(var p in a.GetType().GetTypeInfo().DeclaredProperties)
					AddAttribute(p.Name,p.GetValue(a,null));
#else
                    foreach (PropertyInfo p in a.GetType().GetProperties())
                        AddAttribute(p.Name, p.GetValue(a, null));
#endif
                }

                AddElement(obj);
                result = obj;
            }

            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        string memberName = binder.Name;
        AddAttribute(memberName, value);
        return true;
    }

    private void AddAttribute(string memberName, object value)
    {
        object obj = InternalValue;
        bool set = false;
        if (obj != null)
        {
            Type objType = obj.GetType();
            FieldInfo fi = objType.GetField(memberName);
            if (fi != null)
            {
                fi.SetValue(obj, value);
                set = true;
            }
            else
            {
                PropertyInfo pi = objType.GetProperty(memberName);
                if (pi != null)
                {
                    pi.SetValue(obj, value);
                    set = true;
                }
            }
        }

        if (!set)
        {
            if (!_elasticProvider.HasAttribute(memberName))
            {
                _elasticProvider.AddAttribute(memberName, new ElasticObject(memberName, value));
            }
            else
            {
                _elasticProvider.SetAttributeValue(memberName, value);
            }
        }

        OnPropertyChanged(memberName);
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    #endregion

    #region IElasticHierarchyWrapper<ElasticObject> Members

    public IEnumerable<KeyValuePair<string, ElasticObject>> Attributes => _elasticProvider.Attributes;

    public bool HasAttribute(string name)
    {
        return _elasticProvider.HasAttribute(name);
    }


    public void SetAttributeValue(string name, object obj)
    {
        _elasticProvider.SetAttributeValue(name, obj);
    }

    public object GetAttributeValue(string name)
    {
        return _elasticProvider.GetAttributeValue(name);
    }

    public ElasticObject Attribute(string name)
    {
        return _elasticProvider.Attribute(name);
    }


    public void AddAttribute(string key, ElasticObject value)
    {
        //			value._nodeType = NodeType.Attribute;
        value.InternalParent = this;
        _elasticProvider.AddAttribute(key, value);
    }

    public void RemoveAttribute(string key)
    {
        _elasticProvider.RemoveAttribute(key);
    }


    public object? InternalValue
    {
        get => _elasticProvider.InternalValue;
        set => _elasticProvider.InternalValue = value;
    }

    public string? InternalName
    {
        get => _elasticProvider.InternalName;
        set => _elasticProvider.InternalName = value;
    }

    public ElasticObject? InternalParent
    {
        get => _elasticProvider.InternalParent;
        set => _elasticProvider.InternalParent = value;
    }

    #endregion

    public object? Namespace { get; set; }

    public override string ToString()
    {
        return ToString2(0);
    }

    private string ToString2(int level)
    {
        if (level > 4) return $"[{InternalName}]";
        bool first = true;
        if (Attributes?.Any() ?? false)
        {
            StringBuilder s = new();
            s.Append("{");
            foreach (KeyValuePair<string, ElasticObject> attribute in Attributes)
            {
                if (!first)
                    s.Append(",");
                s.Append("\"" + attribute.Key + "\":");
                if (attribute.Value?.InternalValue is List<object> valueAsList)
                {
                    s.Append("[");
                    bool firstItem = true;
                    foreach (object item in valueAsList)
                    {
                        if (!firstItem)
                            s.Append(",");
                        s.Append((item as ElasticObject)?.ToString2(level + 1) ?? item.ToString());
                        firstItem = false;
                    }

                    s.Append("]");
                }
                else
                    s.Append("\"" + attribute.Value?.ToString2(level + 1) + "\"");

                first = false;
            }

            s.Append("}");
            return s.ToString();
        }

        if (Elements?.Any() ?? false)
        {
            StringBuilder s = new();
            s.Append("[");
            foreach (ElasticObject element in Elements)
            {
                if (!first)
                    s.Append(",");
                s.Append("{" + element?.ToString2(level + 1) + "}");
                first = false;
            }

            s.Append("]");
            return s.ToString();
        }

        if (InternalValue is Dictionary<string, object> attributesAsDic)
        {
            StringBuilder s = new();
            s.Append("{");
            foreach (KeyValuePair<string, object> attributeAsDic in attributesAsDic)
            {
                if (!first)
                    s.Append(",");
                s.Append("\"" + attributeAsDic.Key + "\":\"" +
                         ((attributeAsDic.Value as ElasticObject)?.ToString2(level + 1) ??
                          attributeAsDic.Value.ToString())
                         + "\"");
                first = false;
            }

            s.Append("}");
            return s.ToString();
        }

        if (InternalValue is List<object> attributesAsList)
        {
            StringBuilder s = new();
            s.Append("[");
            foreach (object attribute in attributesAsList)
            {
                if (!first)
                    s.Append(",");
                s.Append((attribute as ElasticObject)?.ToString2(level + 1) ?? attribute.ToString());
                first = false;
            }

            s.Append("]");
            return s.ToString();
        }

        return (InternalValue as ElasticObject)?.ToString2(level + 1) ?? InternalValue?.ToString() ?? "";
    }
}

public enum NodeType
{
    Element,
    Attribute
}

public enum FormatType
{
    Xml,
}
