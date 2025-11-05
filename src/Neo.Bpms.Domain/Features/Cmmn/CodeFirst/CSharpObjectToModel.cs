using FieldProperty = Neo.Bpms.Domain.Models.Attributes.FieldAttributes.FieldProperty;

namespace Neo.Bpms.Domain.Features.Cmmn.CodeFirst;

public class CSharpObjectToModel
{
    /// <summary>
    /// Extracts the ids from attributes.
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="attributes">The attributes.</param>
    internal static void ExtractIds(EFAttr_Id ids, IEnumerable<object> attributes)
    {
        EFAttr_Id ida = GetAttribute<EFAttr_Id>(attributes);
        if (ida == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(ida.Name))
        {
            ids.Name = ida.Name;
        }

        if (!string.IsNullOrEmpty(ida.EnName))
        {
            ids.EnName = ida.EnName;
        }

        if (!string.IsNullOrEmpty(ida.DBName))
        {
            ids.DBName = ida.DBName;
        }

        if (!string.IsNullOrEmpty(ida.OldDbName))
        {
            ids.OldDbName = ida.OldDbName;
        }
    }

    protected static void CheckFieldAttributes(EntityField field, Entity entity, object attr)
    {
        switch (attr)
        {
            case FieldValidation _:
                field.AddFieldValidation(new EntityFieldValidation());
                break;
            case EFAttr_Conformance at:
                entity.AddConformance(new Conformance(entity, field, at.Name, at.Name, at.Access, at.Level,
                    at.Subject,
                    Parser.Parse(at.ConformanceExpression), new ExceptionInformation(at.Error.DataEngine, at.Error.Name)
                    {
                        ErrorText = at.Error.ErrorText,
                        ErrorFormula = at.Error.ErrorFormula,
                        EnErrorText = at.Error.EnErrorText,
                        EnErrorFormula = at.Error.EnErrorFormula
                    }));
                break;
            case DefaultValueAttribute attrDefaultValue:
                field.DefaultValue = attrDefaultValue.Value;
                break;
            case DisplayNameAttribute at:
                field.Name = at.DisplayName;
                break;
            case InDisplayStringAttribute inDisplayString:
                _ = entity.AddBasicField(new BasicField
                {
                    FieldId = field.Id,
                    OtherFieldThatIgnoreMe = inDisplayString.OtherFieldThatIgnoreMe,
                    IgnoreIfNull = inDisplayString.IgnoreIfNull,
                    WithTitle = inDisplayString.WithTitle,
                    Culture = inDisplayString.Culture
                });
                break;

            case FAttr_Index indexAttr:
                EntityIndex idx = indexAttr.IsUnique
                    ? entity.AddUniqueIndex(field.Id, field.EnName, field.Id, indexAttr.Clustered)
                    : entity.AddIndex(field.Id, field.EnName, field.Id, indexAttr.Clustered);
                idx?.Fields.Add(new IndexField { FieldName = field.Id, IsDescending = indexAttr.IsDescending });
                break;
            case FieldProperty property:
                field.AddProperty(property.PropertyId, property.Value);
                break;
            case SBVRAttribute sbvr:
                field.AddSBVR(sbvr);
                break;
        }
    }

    protected static T GetAttribute<T>(IEnumerable<object> attributes) where T : Attribute
    {
        foreach (object attr in attributes)
        {
            if (attr is T attribute)
            {
                return attribute;
            }
        }

        return default;
    }
}
