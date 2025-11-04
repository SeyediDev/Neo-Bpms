using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.ProjectDefinitions.Extensions;

public static class EntityFieldExtensions
{
    public static EntityField GetDotAssociatedFieldWithCompleteId(this EntityField field, string[] fieldIds,
        out string fieldId)
    {
        fieldId = fieldIds[0];
        for (var ii = 1; ii < fieldIds.Length; ii++)
        {
            field = (field?.AssociationEntity).DestEntity?.GetField(fieldIds[ii]);
            if (field == null) break;
            fieldId += ".";
            fieldId += ii == fieldIds.Length - 1
                ? field.AssociationEntity?.Maps?[0].SourceField ?? field.Id
                : field.Id;
        }
        return field;
    }
}