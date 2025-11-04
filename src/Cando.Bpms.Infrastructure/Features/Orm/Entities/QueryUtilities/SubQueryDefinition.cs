namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    public class SubQueryDefinition : JoinDefinition
    {
        public QueryUtility Query { get; set; }
        public string Key => EntityDbNameManager.GetTableDbFullName(Query.Entity) + joinType;

        public void SetReverseMapping(EntityField leftefld)
        {
            SetMapping(leftefld, true);
        }

        public void SetMapping(EntityField leftefld, bool reverse = false)
        {
            if (fieldMappings.Count > 0)
                return;
            foreach (var map in leftefld.AssociationEntity.Maps)
            {
                var srcField = leftefld.Entity.GetField(map.SourceField);
                if (srcField == null) continue;
                var destField = leftefld.AssociationEntity.Entity().GetField(map.DestField);
                if (destField == null) continue;
                if (reverse)
                {
                    fieldMappings.Add(new JoinFieldMapping(
                        map.DestField, destField.Formula?.FormulaBody,
                        map.SourceField, srcField.Formula?.FormulaBody));
                }
                else
                {
                    fieldMappings.Add(new JoinFieldMapping(
                        map.SourceField, srcField.Formula?.FormulaBody,
                        map.DestField, destField.Formula?.FormulaBody));
                }
            }
        }
    }

    internal void InitSubQueries(DataSource dataSource)
    {
        foreach (var subTable in DataSource.SubTables ??
                                 Enumerable.Empty<KeyValuePair<string, SubDataSource>>())
        {
            if (!dataSource.SubTables.ContainsKey(subTable.Key))
                dataSource.SubTables.Add(subTable.Key, subTable.Value);
        }
    }

    public void GenerateQuery_WriteJoins(ref CandoStringBuilder joins, ref Dictionary<string, string> tables)
    {
        DataSource.GenerateQuery_WriteJoins(DataSource, ref joins, ref tables);
    }
}
