using Neo.Domain.Entities.Common;

namespace Neo.Bpms.Domain.Models.Cmmn.Fields;

public class EntityField : BaseModelClass, ISBVRContainer
{
    public EntityFieldVisibility Visibility = EntityFieldVisibility.Public;

    [XmlIgnore] public long DbId { get; set; }

    /// <summary>
    /// eEntityFieldFlags bit Masks
    /// </summary>
    public EntityFieldFlags Flags { get; set; }

    public bool IsStatic { get; set; }

    public EntityFieldAnalyticType AnalyticType { get; set; }

    public List<EntityFieldValidation> FieldValidations { get; set; }

    private string DBFieldNameMap { get; set; }

    private string _oldDbFieldNameMap;

    public int MaxLen { get; set; }
    public int MinLen { get; set; }

    public object DefaultValue { get; set; }
    public TVariableTypes FieldType { get; set; }

    [XmlIgnore]
    public Type CSharpType
    {
        get => _cSharpType ?? GetFieldType(FieldType);
        set => _cSharpType = value;
    }

    [XmlIgnore] //todo
    public FieldValueClassificationNode ValueClassification;

    public Entity Entity => Parent as Entity;

    public bool IsRealMember { get; set; }

    [XmlIgnore] //todo
    public List<ReferenceField> ReferenceFields { get; set; } //todo fill in loadXMLMeta

    public bool IsReferenceFieldModification => ReferenceFields != null && ReferenceFields.Count != 0 && IsRealMember;

    [XmlIgnore]
    public Association AssociationEntity
    {
        get => Association ?? WeakEntityAssociation ?? (Association)ParentEntity;
        set
        {
            switch (value)
            {
                case WeakEntityAssociation w:
                    WeakEntityAssociation = w;
                    break;
                case ParentEntity p:
                    _parentEntity = p;
                    break;
                default:
                    Association = value;
                    break;
            }
        }
    }

    public CompositionEntity CompositionEntity { get; set; }
    public BaseEntity BaseEntity { get; set; }
    private ParentEntity _parentEntity;
    public ParentEntity ParentEntity => _parentEntity;
    public WeakEntityAssociation WeakEntityAssociation { get; set; }
    public Association Association { get; set; }

    public EntityFieldFormula Formula;
    public BooleanEntityField Boolean;

    [XmlIgnore] private Type _cSharpType;

    public EntityField Clone(Entity parentEntity)
    {
        EntityField f = new(parentEntity, Id, Name, EnName, CSharpType, FieldType, Flags)
        {
            Visibility = Visibility,
            IsStatic = IsStatic,
            AnalyticType = AnalyticType,
            Formula = Formula,
            DefaultValue = DefaultValue,
            bExistInDBMS = bExistInDBMS,
            DBFieldNameMap = DBFieldNameMap,
            _oldDbFieldNameMap = _oldDbFieldNameMap,
            Conformance = Conformance?.ToList(),
            FieldValidations = FieldValidations?.ToList(),
            MaxLen = MaxLen,
            ValueClassification = ValueClassification,
            Properties = Properties?.Select(p => p.Clone()).ToList(),
            ReferenceFields = ReferenceFields?.Select(p => p.Clone()).ToList(),
            DbFieldName = DbFieldName,
            DbId = 0,
            MinLen = MinLen,
            IsRealMember = IsRealMember,
            OldDbFieldName = OldDbFieldName,
            Order = Order,
            Boolean = Boolean?.Clone(),

            BaseEntity = BaseEntity,
            AssociationEntity = AssociationEntity?.Clone(),
            CompositionEntity = CompositionEntity?.Clone(),
        };
        f.CloneBase(this);
        return f;
    }

    public EntityField(Entity entity, string fieldId, string name, string enName, Type cSharpType,
        TVariableTypes fieldType, EntityFieldFlags flg = EntityFieldFlags.None)
        : base(entity, fieldId, name)
    {
        EnName = enName;
        FieldType = fieldType;
        Flags = flg;
        CSharpType = cSharpType ?? GetFieldType(fieldType);
    }

    public EntityField()
    {
    }

    public bool CheckFlag(EntityFieldFlags flg)
    {
        return ((int)Flags & (int)flg) != 0;
    }

    #region conformance

    /// <summary>
    /// conformance will be defined in the security models as a function checking conformance of entity for a user
    /// </summary>
    public List<BaseModelClass> Conformance { get; set; }

    public bool bExistInDBMS { get; set; }

    public void addConformance(BaseModelClass conformance)
    {
        Conformance ??= [];
        Conformance.Add(conformance);
    }

    #endregion

    public bool IncludeInPkv => CheckFlag(EntityFieldFlags.IncludeInPKV) && !IsForParent;

    public bool NotMap => CheckFlag(EntityFieldFlags.NotMap);
    public bool DonSync => AssociationEntity != null || Formula != null || NotMap;

    public bool NotMapped
    {
        get
        {
            if (AssociationEntity?.Maps == null) return NotMap;
            foreach (EntityRelationMap map in AssociationEntity.Maps)
            {
                var srcField = Entity.GetField(map.SourceField);
                if (srcField == null)
                    return false;
                if (srcField.NotMap)
                    return true;
            }

            return false;
        }
    }

    public bool AuditField => CheckFlag(EntityFieldFlags.AuditField);

    //public bool Required => NotNull;
    public bool Required
    {
        get
        {
            return CheckFlag(EntityFieldFlags.NotNull | EntityFieldFlags.IncludeInPKV) ||
                (AssociationEntity?.Maps?.Any(m => Entity.GetField(m.SourceField)?.Required ?? false) ?? false);
        }
        set
        {
            if (value)
                Flags = EntityFieldFlags.NotNull;
            else
                Flags &= ~EntityFieldFlags.NotNull;
        }
    }

    public string DbFieldName
    {
        get => NotMap ? "" : !string.IsNullOrEmpty(DBFieldNameMap) ? DBFieldNameMap : Id;
        set => DBFieldNameMap = value;
    }
    public string GetSetDBFieldNameMap() => DBFieldNameMap;
    public string OldDbFieldName
    {
        get => NotMap ? "" : _oldDbFieldNameMap;
        set => _oldDbFieldNameMap = value;
    }

    public void SetDbFieldNameMap(string dbFieldNameMap, string oldDbFieldNameMap)
    {
        DBFieldNameMap = dbFieldNameMap;
        _oldDbFieldNameMap = oldDbFieldNameMap;
    }

    public string GetTypeName()
    {
        switch (FieldType)
        {
            case TVariableTypes.DoubleMinuteSecond:
            case TVariableTypes.HourMinute:
            case TVariableTypes.DayHourMinute:
            case TVariableTypes.DurHourMinute:
                return "TimeSpan";
            case TVariableTypes.DateTime:
            case TVariableTypes.Date:
                return "Date";
            case TVariableTypes.StringListItem:
                return "enum";
            case TVariableTypes.StringListBitMask:
                return "enums";
            case TVariableTypes.DateStr:
                return "string";
            default:
                if (IsEnum)
                    return "enum";
                string typeName = FieldType.ToString().ToLower();
                if (typeName.StartsWith("var"))
                    typeName = typeName[2..];
                return typeName;
        }
    }

    public bool IsLongParam()
    {
        return FieldType switch
        {
            TVariableTypes.Char or TVariableTypes.UChar or TVariableTypes.Short or TVariableTypes.UShort or
            TVariableTypes.Int or TVariableTypes.Long or TVariableTypes.ULong or TVariableTypes.Decimal => true,
            _ => false,
        };
    }

    public bool IsTimeSpan()
    {
        return FieldType switch
        {
            TVariableTypes.DurHourMinute or TVariableTypes.DayHourMinute or
            TVariableTypes.DoubleMinuteSecond or TVariableTypes.HourMinute => true,
            _ => false,
        };
    }

    public bool IsDoubleParam()
    {
        return FieldType switch
        {
            TVariableTypes.Double => true,
            _ => false,
        };
    }

    public bool IsBoolParam()
    {
        return FieldType switch
        {
            TVariableTypes.BOOL => true,
            _ => false,
        };
    }

    public bool IsTextParam()
    {
        return FieldType switch
        {
            TVariableTypes.String => true,
            _ => false,
        };
    }

    public bool IsDateTime()
    {
        return FieldType switch
        {
            TVariableTypes.Date or TVariableTypes.DateTime or TVariableTypes.DateStr => true,
            _ => false,
        };
    }

    public bool IsForeignParam()
    {
        return FieldType switch
        {
            TVariableTypes.Association or TVariableTypes.Composition or TVariableTypes.ParentEntity or TVariableTypes.BaseEntity => true,
            _ => false,
        };
    }

    public static TVariableTypes GetFieldType(Type reflectionType, ILogger logger)
    {
        TVariableTypes typ;
        Type underlyingType = Nullable.GetUnderlyingType(reflectionType);
        if (underlyingType != null)
            reflectionType = underlyingType;
        if (reflectionType.IsEnum)
        {
            typ = TVariableTypes.StringListItem;
        }
        else if (reflectionType == typeof(bool) || reflectionType == typeof(bool?))
        {
            typ = TVariableTypes.BOOL;
        }
        else if (reflectionType == typeof(double) || reflectionType == typeof(double?))
        {
            typ = TVariableTypes.Double;
        }
        else if (reflectionType == typeof(byte) || reflectionType == typeof(byte?))
        {
            typ = TVariableTypes.Short;
        }
        else if (reflectionType == typeof(char) || reflectionType == typeof(char?) ||
                 reflectionType == typeof(ushort) || reflectionType == typeof(ushort?) ||
                 reflectionType == typeof(short) || reflectionType == typeof(short?) ||
                 reflectionType == typeof(int) || reflectionType == typeof(int?) ||
                 reflectionType == typeof(uint) || reflectionType == typeof(uint?) ||
                 reflectionType == typeof(IStronglyTypedId<int>)
                 )
        {
            typ = TVariableTypes.Int;
        }
        else if (reflectionType == typeof(byte[]) || reflectionType == typeof(byte?[]))
        {
            typ = TVariableTypes.ByteArray;
        }
        else if (reflectionType == typeof(long) || reflectionType == typeof(long?) ||
            reflectionType == typeof(ulong) || reflectionType == typeof(ulong?))
        {
            typ = TVariableTypes.Long;
        }
        else if (reflectionType == typeof(ulong) || reflectionType == typeof(ulong?))
        {
            typ = TVariableTypes.ULong;
        }
        else if (reflectionType == typeof(decimal) || reflectionType == typeof(decimal?))
        {
            typ = TVariableTypes.Decimal;
        }
        else if (reflectionType == typeof(DateTime) || reflectionType == typeof(DateTime?) ||
                reflectionType == typeof(DateOnly) || reflectionType == typeof(DateOnly?))
        {
            typ = TVariableTypes.Date;
        }
        else if (reflectionType == typeof(TimeSpan) || reflectionType == typeof(TimeSpan?) ||
            reflectionType == typeof(TimeOnly) || reflectionType == typeof(TimeOnly?))//TODO declare new var type
        {
            typ = TVariableTypes.DayHourMinute;
        }
        else if (ReflectionTools.IsGenericList(reflectionType))
        {
            typ = TVariableTypes.List;
        }
        else if (reflectionType.IsClass && reflectionType != typeof(string) && reflectionType != typeof(object))
        {
            typ = reflectionType.IsInBaseInterface<IDocument>() ? TVariableTypes.File : TVariableTypes.Association;
        }
        else if (reflectionType.IsClass && (reflectionType == typeof(string) || reflectionType == typeof(object)))
        {
            typ = TVariableTypes.String;
        }
        else if (reflectionType == typeof(Guid))
        {
            typ = TVariableTypes.String;
        }
        else if (reflectionType.IsInBaseInterface<IStronglyTypedId<int>>())
        {
            typ = TVariableTypes.Int;
        }
        else if (reflectionType.IsInBaseInterface<IStronglyTypedId<long>>())
        {
            typ = TVariableTypes.Long;
        }
        else if (reflectionType.IsInBaseInterface<IStronglyTypedId<Guid>>())
        {
            typ = TVariableTypes.String;
        }
        else
        {
            typ = TVariableTypes.String;
            logger.LogError("Unknown type {FullName}", reflectionType.FullName);
        }
        return typ;
    }

    public static Type GetFieldType(TVariableTypes fieldType)
    {
        Type typ;
        switch (fieldType)
        {
            case TVariableTypes.BOOL:
                typ = typeof(bool);
                break;
            case TVariableTypes.Char:
                typ = typeof(char);
                break;
            case TVariableTypes.Short:
                typ = typeof(short);
                break;
            case TVariableTypes.Int:
                typ = typeof(int);
                break;
            case TVariableTypes.ByteArray:
                typ = typeof(byte?[]);
                break;
            case TVariableTypes.Long:
                typ = typeof(long);
                break;
            case TVariableTypes.Decimal:
                typ = typeof(decimal);
                break;
            case TVariableTypes.UChar:
                typ = typeof(short);
                break;
            case TVariableTypes.UShort:
                typ = typeof(int);
                break;
            case TVariableTypes.ULong:
                typ = typeof(ulong);
                break;
            case TVariableTypes.Double:
            case TVariableTypes.DoubleMinuteSecond:
                typ = typeof(double);
                break;
            case TVariableTypes.String:
                typ = typeof(string);
                break;

            case TVariableTypes.DateTime:
            case TVariableTypes.Date:
            case TVariableTypes.DateStr:
                typ = typeof(DateTime);
                break;
            case TVariableTypes.HourMinute:
            case TVariableTypes.DayHourMinute:
            case TVariableTypes.DurHourMinute:
                typ = typeof(TimeSpan);
                break;
            case TVariableTypes.StringListItem:
                typ = typeof(long);
                //typ = typeof (enum);
                break;
            case TVariableTypes.Association:
                typ = typeof(long);
                break;
            case TVariableTypes.StringListBitMask:
                typ = typeof(long);
                break;
            default:
                return null;
        }

        return typ;
    }

    public void AddFieldValidation(EntityFieldValidation fv)
    {
        /*if (fieldValidations == null)
            fieldValidations = new List<FieldValidation>();
        fieldValidations.Add(fv);*/
    }

    public void AddFieldFlags(params EntityFieldFlags[] fieldFlags)
    {
        foreach (EntityFieldFlags flag in fieldFlags)
        {
            Flags |= flag;
        }
    }

    public void RemoveFieldFlags(EntityFieldFlags fieldFlag)
    {
        Flags &= ~fieldFlag;
    }

    public List<FieldProperty> Properties { get; set; }
    public int Order { get; set; }

    public void AddProperty(EntityFieldPropertyId propertyId, object value)
    {
        Properties ??= [];
        Properties.Add(new FieldProperty { PropertyId = propertyId, Value = value });
    }

    public bool HasProperty(EntityFieldPropertyId propertyId) =>
        Properties?.FirstOrDefault(p => (int)p.PropertyId == (int)propertyId) != null;

    public string Property(EntityFieldPropertyId propertyId) =>
        Properties?.FirstOrDefault(p => (int)p.PropertyId == (int)propertyId)?.Value?.ToString();
    public object GetPropertyValue(EntityFieldPropertyId propertyId) =>
        Properties?.FirstOrDefault(p => (int)p.PropertyId == (int)propertyId)?.Value;

    //public enum eMultiplicity
    //{
    //	Zero,
    //	ZeroToOne,
    //	ZeroToN,
    //	One,
    //	OneToN,
    //	N
    //}
    public bool IsAutoIncrement()
    {
        return Entity?.AutoCalcs?.Calculations?.Any(ac => IsAutoIncrement(ac, Id))
               ?? false;
    }

    private bool IsAutoIncrement(AutoCalc ac, string fieldId)
    {
        return ac.FieldId == fieldId &&
               !ac.RecalcOnAnyChange &&
               ac.GenerationType == AutoCalc.eGenerationType.DBInsert;
    }

    public void DisableAutoIncrement()
    {
        Entity?.AutoCalcs?.Calculations?.RemoveAll(ac => IsAutoIncrement(ac, Id));
    }

    public void AddReferenceField(EntityRelationship relationship, EntityField referenceField, EntityField parentAssociationField)
    {
        ReferenceFields ??= [];
        ReferenceFields.Add(
            new ReferenceField(this)
            {
                ReferencedField = referenceField,
                ParentAssociationField = parentAssociationField,
                Relationship = relationship
            });
    }

    public bool MappedToDataInThisEntity => !DonSync && !IsForParent;

    public bool IsForParent =>
        ReferenceFields != null && ReferenceFields.Any(r => r.Relationship is ParentEntity);

    public bool IsForParentField(EntityField parentEntityField, Entity parentEntity) =>
        ReferenceFields != null && ReferenceFields.Any(r =>
            r.Relationship is ParentEntity &&
            r.ParentAssociationField.Id == parentEntityField.Id &&
            r.Relationship.DestEntity.Equals(parentEntity));

    public bool IsForParentEntity(Entity childEntity) =>
        ReferenceFields != null &&
        ReferenceFields.Any(r => r.Relationship is ParentEntity p && Equals(p.SourceEntity, childEntity));

    public bool IsEnum
    {
        get
        {
            return CSharpType.IsEnum ||
                (Nullable.GetUnderlyingType(CSharpType)?.IsEnum ?? FieldType is TVariableTypes.StringListItem or TVariableTypes.StringListBitMask);
        }
    }


    public EntityField ParentAssociationThatMappingWithMe => Entity.entityFields.Values.FirstOrDefault(f =>
        f.ParentEntity?.Maps != null && f.ParentEntity.Maps.Any(m => m.SourceField == Id));

    public bool IsRelationShipField => AssociationEntity != null || BaseEntity != null || CompositionEntity != null;
    public List<SBVR> SBVRs { get; set; } = [];
    public object? Maximum { get; internal set; }
    public object? Minimum { get; internal set; }

    public void AddSBVR(SBVRAttribute sbvr)
    {
        SBVRs.Add(new SBVR()
        {
            Modality = sbvr.Modality,
            Subject = sbvr.Subject ?? Name ?? Id ?? "فیلد",
            VerbPhrase = sbvr.VerbPhrase,
            Condition = sbvr.Condition
        });
    }
}
