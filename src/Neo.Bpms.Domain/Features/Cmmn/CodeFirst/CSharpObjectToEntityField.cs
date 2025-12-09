using static Neo.Bpms.Domain.Models.Cmmn.AutoCalc;

namespace Neo.Bpms.Domain.Features.Cmmn.CodeFirst;

public record MemberItem(MemberInfo MemberInfo, Type MemberType, int Order);

public class CSharpObjectToEntityField(INameSpaceRepository nameSpaceRepository, Type type, Entity entity, ILogger logger) 
    : CSharpObjectToModel
{
    /// <summary>
    /// Defines the entity structure from code.
    /// </summary>
    /// <returns></returns>
    public void DefineEntityFields()
    {
        int order = 1;
        List<MemberItem> members = [];
        foreach (MemberInfo member in ReflectionField.Members(type))
        {
            Type itemType = ReflectionField.FetchMemberType(member);
            if (ReflectionField.IsStatic(member) ||
                itemType.FullName.StartsWith("System.Collections.Generic.ICollection") ||
                itemType.FullName != "System.Byte[]" && itemType.TryGetInterfaceGenericParameters(typeof(ICollection<>), out _) ||
                member.Name == nameof(IDomainEventEntity.DomainEvents))
            {
                continue;
            }
            members.Add(new MemberItem(member, itemType, order++));
        }
        AddMembers(members, false);
        AddMembers(members, true);
        
        //if (entity.DisplayStrings.Count > 0)
        //{
        //    var findStringType = entity.entityFields.Values?.FirstOrDefault(f => f.fieldType == TVariableTypes.varString);
        //    if (findStringType != null)
        //        entity.AddBasicField(new BasicField { FieldId = findStringType.Id, IgnoreIfNull = true });
        //}
        if (entity.EntityType.IsInBaseInterface<IDomainEventEntity>())
        {
            if (entity.entityFields is not null &&
                !entity.entityFields.Values.Any(f => f.IncludeInPkv))
            {
                var idField = entity.GetField("Id");
                if (idField != null) 
                {
                    idField.Flags |= EntityFieldFlags.IncludeInPKV;
                    entity.InitAutoCalcs();
                    ExpressionTree formulaEx = Parser.ParseTree("AutoIncrement()");
                    entity.AutoCalcs.AddAutoCalc(new AutoCalc
                    {
                        FieldId = idField.Id,
                        GenerationType = AutoCalc.eGenerationType.DBInsert,
                        Formula = formulaEx,
                        Condition = null,
                        Loaction = AutoCalcLocation.BeforeValidation,
                        IfNull = false,
                        RecalcOnAnyChange = false,
                    });
                }
            }
        }
        if (entity.EntityType.IsInBaseInterface<IBaseAuditableEntity>())
        {
            SetAuditField(nameof(IBaseAuditableEntity.CreateDate));
            SetAuditField(nameof(IBaseAuditableEntity.CreatedById));
            //SetAuditField(nameof(IBaseAuditableEntity.ExpireDate));
            //SetAuditField(nameof(IBaseAuditableEntity.IsDeleted));
            SetAuditField(nameof(IBaseAuditableEntity.LastModified));
            SetAuditField(nameof(IBaseAuditableEntity.LastModifiedById));
            void SetAuditField(string fieldId) 
            { 
                var idField = entity.GetField(fieldId);
                if (idField != null)
                {
                    idField.Flags |= EntityFieldFlags.AuditField;
                }
            }
        }
    }

    private void AddMembers(IEnumerable<MemberItem> members, bool isRelationship)
    {
        foreach (MemberItem memberItem in members)
        {
            if (IsRelationship(memberItem.MemberType) == isRelationship)
            {
                AddMember(memberItem);
            }
        }
    }

    private void AddMember(MemberItem memberItem)
    {
        string id = memberItem.MemberInfo.Name;
        EFAttr_Id ids = new()
        {
            Name = memberItem.MemberInfo.Name,
            EnName = memberItem.MemberInfo.GetDisplayableName(),
            DBName = memberItem.MemberInfo.Name
        };
        ExtractIds(ids, memberItem.MemberInfo.GetCustomAttributes(true));
        TVariableTypes fieldType = EntityField.GetFieldType(memberItem.MemberType, logger);
        EntityField field = new(entity, id, ids.Name, ids.EnName, memberItem.MemberType, fieldType)
        {
            Order = memberItem.Order,
            IsRealMember = type == memberItem.MemberInfo.DeclaringType
        };
        field.SetDbFieldNameMap(ids.DBName, ids.OldDbName);
        _ = entity.AddField(field);
        if (memberItem.MemberInfo.IsFieldRequired(memberItem.MemberType))
        {
            field.Required = true;
        }
        
        if (IsRelationship(memberItem.MemberType))
        {
            field.Flags |= EntityFieldFlags.NotMap;
            CSharpObjectToRelationship.DefineRelationship(entity, field,
                id, ids.Name, ids.EnName, memberItem.MemberType, memberItem.MemberInfo.GetCustomAttributes(true), nameSpaceRepository);
        }
        else
        {
            SetFieldMetaFromAttributes(entity, field, memberItem.MemberInfo.GetCustomAttributes(true));
        }
    }

    private static bool IsRelationship(Type itemType)
    {
        return ReflectionTools.IsClass(itemType) &&
               !itemType.FullName.StartsWith("System.Collections.Generic.ICollection") &&
               !itemType.TryGetInterfaceGenericParameters(typeof(ICollection<>), out _);
    }

    /// <summary>
    /// Gets the field meta from attributes.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="field">The field.</param>
    /// <param name="attributes">The attributes.</param>
    private static void SetFieldMetaFromAttributes(Entity entity, EntityField field, object[] attributes)
    {
        foreach (object attr in attributes)
        {
            CheckFieldAttributes(field, entity, attr);
            switch (attr)
            {
                case FAttr_AutomaticCalculation ac:
                    {
                        entity.InitAutoCalcs();
                        ExpressionTree formulaEx = Parser.ParseTree(ac.Formula),
                            conditionEx = ac.Condition == null
                                ? null
                                : Parser.ParseTree(ac.Condition);
                        entity.AutoCalcs.AddAutoCalc(new AutoCalc
                        {
                            Condition = conditionEx,
                            Formula = formulaEx,
                            Loaction = (AutoCalcLocation)ac.Location,
                            IfNull = ac.IfNull,
                            RecalcOnAnyChange = ac.RecalcOnAnyChange,
                            FieldId = field.Id,
                            GenerationType = (AutoCalc.eGenerationType)ac.GenerationType
                        });
                        break;
                    }
                case RequiredAttribute _:
                    field.Required = true;
                    break;
                case KeyAttribute _:
                    {
                        field.Flags |= EntityFieldFlags.IncludeInPKV;
                        break;
                    }

                case FAttr_Boolean boolean:
                    {
                        FAttr_Boolean at = boolean;
                        field.Boolean = new BooleanEntityField
                        {
                            TrueTitle = at.TrueTitle,
                            FalseTitle = at.FalseTitle,
                            AllTitle = at.AllTitle,
                            NullTitle = at.NullTitle
                        };
                        break;
                    }
                case System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute _:
                    field.Flags |= EntityFieldFlags.NotMap;
                    break;

                case FAttr_IsFormula formula:
                    {
                        FAttr_IsFormula at = formula;
                        field.Formula = new EntityFieldFormula
                        {
                            UsedForAggregationOnly = at.UsedForAggregationOnly,
                            FormulaText = at.Formula
                        };
                        if (string.IsNullOrEmpty(at.FormulaId))
                        {
                            field.Formula.FormulaBody = Parser.Parse(at.Formula);
                        }
                        else
                        {
                            field.Formula.FormulaMethodId = at.FormulaId;
                        }

                        field.Flags |= EntityFieldFlags.NotMap;
                        break;
                    }
                case DbMapAttribute at:
                    field.DbFieldName = at.DBName;
                    field.OldDbFieldName = at.OldDbName;
                    break;
                case OldDbMapAttribute at:
                    field.OldDbFieldName = at.OldDbName;
                    break;
                case MaxLengthAttribute length:
                    {
                        field.MaxLen = length.Length;
                        break;
                    }
                case MinLengthAttribute length:
                    {
                        field.MinLen = length.Length;
                        break;
                    }
                case RangeAttribute range:
                    {
                        field.Maximum = range.Maximum;
                        field.Minimum = range.Minimum;
                        break;
                    }
            }
        }
        if (field.FieldType == TVariableTypes.String && field.MaxLen == 0)
        {
            if (field.Id == "Title")
                field.MaxLen = 41;
            if (field.Id == "Description")
                field.MaxLen = 512;
        }
        if (field.Id == "Title" && field.Name == "Title")
        {
            field.Name = "عنوان";
        }
        else if (field.Id == "Description" && field.Name == "Description")
        {
            field.Name = "توضیحات";
        }
    }
}
