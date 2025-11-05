namespace Neo.Bpms.Domain.Features.Definitions.Entities;
public abstract partial class ModelDefinition
{
    /// <summary>
    /// Defines Enumeration
    /// </summary>
    /// <returns></returns>
    public virtual bool DefineEnumerations()
    {
        return true;
    }
    /// <summary>
    /// Defines Enumeration
    /// </summary>
    /// <returns></returns>
    protected virtual void Enumerations()
    {
    }

    protected Enumeration currentEnumeration;

    /// <summary>
    /// Defines the enumeration.
    /// </summary>
    /// <param name="enName">Name of the en.</param>
    /// <param name="name">The name.</param>
    /// <param name="deleteExtraItems">if set to  <c>true</c> will delete extra items</param>
    /// <returns></returns>
    protected Enumeration DefineEnumeration(string enName, string name, bool deleteExtraItems = false)
    {
        currentEnumeration = new Enumeration(model, name, enName) { DeleteExtraItems = deleteExtraItems };
        currentBaseElement = currentEnumeration;
        currentEnumeration.EnName = enName;
        model.AddEnumeration(currentEnumeration);
        return currentEnumeration;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Enumeration</typeparam>
    /// <param name="deleteExtraItems">if set to  <c>true</c> will delete extra items</param>
    /// <param name="faName">The name</param>
    /// <param name="enName">The english name</param>
    protected void DefineEnumeration<T>(bool deleteExtraItems = false, string faName = null, string enName = null)
    {
        Type t = typeof(T);
        DefineEnumeration(t, deleteExtraItems, faName, enName);
    }
    protected void DefineEnumeration(Type t, bool deleteExtraItems = false, string faName = null, string enName = null)
    {
        if (!t.IsEnum)
        {
            throw new Exception($"the type given {t.Name} is not enum");
        }

        faName = FetchFaName(faName, t);
        enName = FetchEnName(enName, t);
        if (string.IsNullOrEmpty(enName))
        {
            enName = t.Name;
            if (enName.StartsWith("e"))
            {
                enName = enName[1..];
            }

            if (enName.EndsWith("Id"))
            {
                enName = enName[..^2];
            }
        }
        _ = DefineEnumeration(enName, faName, deleteExtraItems);
        AddEnumItems(t);
    }
    /// <summary>
    /// Adds Enum Items
    /// </summary>
    /// <typeparam name="T">Enumeration</typeparam>
    protected void AddEnumItems(Type t)
    {
        Array values = Enum.GetValues(t);
        foreach (object value in values)
        {
            Enum @enum = (Enum)value;
            EAttr_State stateAttr = @enum.GetAttribute<EAttr_State>();
            string faName = FetchFaName(@enum);
            string enName = FetchEnName(@enum);
            _ = AddEnumItem(value, faName ?? enName, enName, "", "", stateAttr?.Category ?? EntityStateCategory.ActiveNode);
        }
    }

    private static string FetchEnName(string enName, Type t)
    {
        if (string.IsNullOrEmpty(enName))
        {
            enName = AttributeReader.GetAttribute<DisplayNameAndEnName>(t)?.EnName;
        }

        if (string.IsNullOrEmpty(enName))
        {
            enName = AttributeReader.GetAttribute<EnumDescriptionAttribute>(t)?.EnName;
        }
        //if(string.IsNullOrEmpty(enName))
        //	enName=AttributeReader.GetAttribute<DisplayName>(t)?.EnName;
        if (string.IsNullOrEmpty(enName))
        {
            enName = AttributeReader.GetAttribute<DisplayNameAndEnName>(t)?.EnName;
        }

        if (string.IsNullOrEmpty(enName))
        {
            enName = AttributeReader.GetAttribute<EFAttr_Id>(t)?.EnName;
        }

        return enName;
    }

    private static string FetchFaName(string faName, Type t)
    {
        if (string.IsNullOrEmpty(faName))
        {
            faName = AttributeReader.GetAttribute<DisplayNameAndEnName>(t)?.Name;
        }
        //if (string.IsNullOrEmpty(faName))
        //	faName = AttributeReader.GetAttribute<DisplayName>(t)?.Name;
        if (string.IsNullOrEmpty(faName))
        {
            faName = AttributeReader.GetAttribute<DisplayNameAndEnName>(t)?.Name;
        }

        if (string.IsNullOrEmpty(faName))
        {
            faName = AttributeReader.GetAttribute<EFAttr_Id>(t)?.Name;
        }

        if (string.IsNullOrEmpty(faName))
        {
            faName = AttributeReader.GetAttribute<DisplayNameAttribute>(t)?.DisplayName;
        }

        if (string.IsNullOrEmpty(faName))
        {
            faName = AttributeReader.GetAttribute<DescriptionAttribute>(t)?.Description;
        }

        if (string.IsNullOrEmpty(faName))
        {
            faName = AttributeReader.GetAttribute<EnumDescriptionAttribute>(t)?.Name;
        }

        return faName;
    }

    private static string FetchFaName(Enum @enum)
    {
        string faName = @enum.GetAttribute<DisplayNameAndEnName>()?.Name;
        //if (string.IsNullOrEmpty(faName))
        //	faName = @enum.GetAttribute<DisplayName>()?.Name;
        if (string.IsNullOrEmpty(faName))
        {
            faName = @enum.GetAttribute<DisplayNameAndEnName>()?.Name;
        }

        if (string.IsNullOrEmpty(faName))
        {
            faName = @enum.GetAttribute<EFAttr_Id>()?.Name;
        }

        if (string.IsNullOrEmpty(faName))
        {
            faName = @enum.GetAttribute<DisplayNameAttribute>()?.DisplayName;
        }

        if (string.IsNullOrEmpty(faName))
        {
            faName = @enum.GetAttribute<DescriptionAttribute>()?.Description;
        }

        if (string.IsNullOrEmpty(faName))
        {
            faName = @enum.GetAttribute<EnumDescriptionAttribute>()?.Name;
        }

        return faName;
    }

    private static string FetchEnName(Enum @enum)
    {
        string enName = @enum.GetAttribute<DisplayNameAndEnName>()?.EnName;
        if (string.IsNullOrEmpty(enName))
        {
            enName = @enum.GetAttribute<EnumDescriptionAttribute>()?.EnName;
        }
        //if (string.IsNullOrEmpty(enName))
        //	enName = @enum.GetAttribute<DisplayName>()?.EnName;
        if (string.IsNullOrEmpty(enName))
        {
            enName = @enum.GetAttribute<DisplayNameAndEnName>()?.EnName;
        }

        if (string.IsNullOrEmpty(enName))
        {
            enName = @enum.GetAttribute<EFAttr_Id>()?.EnName;
        }

        if (string.IsNullOrEmpty(enName))
        {
            enName = @enum.ToString();
        }

        return enName;
    }


    protected Enumeration SelectEnumeration(string namespaceId, string entityId)
    {
        currentEnumeration = new Enumeration(namespaceId, entityId);
        currentBaseElement = currentEnumeration;
        model.AddEnumeration(currentEnumeration);
        return currentEnumeration;
    }

    protected EnumerationItem currentEnumerationItem;

    /// <summary>
    /// Adds the enum item.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="enName">Name of the en.</param>
    /// <param name="description">The tooltip.</param>
    /// <param name="enDescription">The en tooltip.</param>
    /// <param name="stateCategory"></param>
    /// <returns></returns>
    protected EnumerationItem AddEnumItem(object id, string name, string enName, string description = null,
        string enDescription = null, EntityStateCategory stateCategory = EntityStateCategory.ActiveNode)
    {
        return AddEnumItem(Convert.ToInt32(id), name, enName, description, enDescription, stateCategory);
    }
    /// <summary>
    /// Adds the enum item.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="enName">Name of the en.</param>
    /// <param name="description">The tooltip.</param>
    /// <param name="enDescription">The en tooltip.</param>
    /// <param name="stateCategory"></param>
    /// <returns></returns>
    protected EnumerationItem AddEnumItem(int id, string name, string enName, string description = null,
        string enDescription = null, EntityStateCategory stateCategory = EntityStateCategory.ActiveNode)
    {
        if (currentEnumeration == null)
        {
            return null;
        }

        currentEnumerationItem = new EnumerationItem(currentEnumeration, id, name, stateCategory);
        currentBaseElement = currentEnumerationItem;
        _ = currentEnumeration.addItem(currentEnumerationItem);
        currentEnumerationItem.EnName = enName;
        currentEnumerationItem.Description = description;
        currentEnumerationItem.EnDescription = enDescription;

        return currentEnumerationItem;
    }

}
