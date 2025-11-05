using Neo.Bpms.Domain.Entities.Cmmn;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.Domain.Expressions.Parsers;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;

public class FieldViewModel
{
    public FieldViewModel()
    {
    }
    public FieldViewModel(EntityField field)
    {
        persianName = field.Name;
        englishName = field.Id;
        prevId = field.Id;
        dbName = field.DbFieldName;
        oldDbName = field.OldDbFieldName;
        type = field.FieldType;
        BasicField bf = field.Entity.SelfDisplayStrings.FirstOrDefault(f => f.FieldId == field.Id);
        displayString = bf != null;
        otherFieldThatIgnoreMe = bf?.OtherFieldThatIgnoreMe;
        ignoreIfNull = bf?.IgnoreIfNull ?? false;
        withTitle = bf?.WithTitle ?? false;
        defaultValue = field.DefaultValue?.ToString();
        maxLength = field.MaxLen;
        limitedLenght = field.MaxLen > 0;
        notNull = field.Required;
        notMap = field.NotMap;
        includeInPkv = field.IncludeInPkv;
        autoIncrement = field.IsAutoIncrement();
        auditField = field.AuditField;

        if (field.AssociationEntity != null)
            throw new Exception("AssociationEntity In Entity Field");
        if (field.Boolean != null || field.FieldType == TVariableTypes.BOOL)
            booleanFields = new BooleanFieldViewModel(field.Boolean);
        formulaFields = new FormulaFieldViewModel(field.Formula ?? new EntityFieldFormula());
        //validation = field.fieldValidations; todo
        //linkedEntity = field.entity.Id;//todo what?!!
    }

    public string persianName { get; set; }
    public string id => englishName;
    public string englishName { get; set; }
    public string prevId { get; set; }
    public string dbName { get; set; }
    public string oldDbName { get; set; }
    public TVariableTypes type { get; set; }
    public bool displayString { get; set; } //todo name!
    public string otherFieldThatIgnoreMe { get; set; } //todo not implemented in client
    public bool ignoreIfNull { get; set; }
    public bool withTitle { get; set; }
    public string defaultValue { get; set; }
    public long? maxLength { get; set; }
    public bool limitedLenght { get; set; }
    public bool notNull { get; set; }
    public bool notMap { get; set; }
    public bool includeInPkv { get; set; }
    public bool autoIncrement { get; set; } // todo
    public bool auditField { get; set; }

    //public AssociationViewModel association { get; set; }
    public BooleanFieldViewModel booleanFields { get; set; }
    public FormulaFieldViewModel formulaFields { get; set; }

    public string validation { get; set; }

    public void ModifyEntityField(EntityField entityField)
    {
        entityField.Id = englishName;
        entityField.EnName = englishName;
        entityField.Name = persianName;
        entityField.SetDbFieldNameMap(dbName, oldDbName);
        entityField.FieldType = type;
        if (maxLength != null)
            entityField.MaxLen = (int)maxLength;
        if (autoIncrement && !entityField.IsAutoIncrement())
        {
            entityField.Entity.InitAutoCalcs();
            ExpressionTree formulaEx = Parser.ParseTree("AutoIncrement()");
            entityField.Entity.AutoCalcs.AddAutoCalc(new AutoCalc
            {
                Condition = null,
                Formula = formulaEx,
                Loaction = AutoCalc.AutoCalcLocation.BeforeValidation,
                IfNull = false,
                RecalcOnAnyChange = false,
                FieldId = entityField.Id,
                GenerationType = AutoCalc.eGenerationType.DBInsert
            });
        }
        else if (!autoIncrement && entityField.IsAutoIncrement())
        {
            entityField.DisableAutoIncrement();
        }
        entityField.DefaultValue = defaultValue;
        BasicField basicField = entityField.Entity.SelfDisplayStrings.FirstOrDefault(bf => bf.FieldId == entityField.Id);
        if (basicField == null && displayString)
            entityField.Entity.AddBasicField(new BasicField
            {
                FieldId = id,
                OtherFieldThatIgnoreMe = otherFieldThatIgnoreMe,
                WithTitle = withTitle,
                IgnoreIfNull = ignoreIfNull
            });
        else if (basicField != null && !displayString)
            entityField.Entity.RemoveBasicField(basicField);
        else if (basicField != null && displayString)
        {
            basicField.OtherFieldThatIgnoreMe = otherFieldThatIgnoreMe;
            basicField.IgnoreIfNull = ignoreIfNull;
            basicField.WithTitle = withTitle;
        }

        switch (type)
        {
            case TVariableTypes.BOOL:
                entityField.Boolean = booleanFields?.ToBoolean();
                break;
            case TVariableTypes.Association:
                //todo entityField.AssociationEntity = association?.ToAssociation(entityField.entity, this);
                notMap = true;
                notNull = false;
                break;
        }
        if (!string.IsNullOrEmpty(formulaFields?.ToFormula()?.ToString()))
        {
            notMap = true;
            entityField.Formula = formulaFields?.ToFormula();
        }
        else
        {
            entityField.Formula = null;
        }
        SetFieldFlags(entityField, notNull, notMap, includeInPkv, auditField);
    }

    private static void SetFieldFlags(EntityField entityField, bool notNull, bool notMapp, bool includeInPkv, bool auditField)
    {
        entityField.Flags = EntityFieldFlags.None;
        if (notNull)
            entityField.Required = true;
        if (notMapp)
            entityField.AddFieldFlags(EntityFieldFlags.NotMap);
        if (includeInPkv)
            entityField.AddFieldFlags(EntityFieldFlags.IncludeInPKV);
        if (auditField)
            entityField.AddFieldFlags(EntityFieldFlags.AuditField);
    }
}
