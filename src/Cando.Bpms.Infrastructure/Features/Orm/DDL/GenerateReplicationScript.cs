namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    public void GenerateReplicationScript(string SourceDB, string DestDB, string namespaceId,
        Func<string, string, string, bool> func)
    {
        _messageFunction = func;
        var models = ProjectDefinition.Project.Namespaces;
        ModelNamespace model = null;
        if (!models.TryGetValue(namespaceId, out model)) return;
        AddMessage(null, null, namespaceId);
        var path = Directory.GetCurrentDirectory() + "\\Scripts"; // + "\\"+namespaceId;
        Directory.CreateDirectory(path);
        var entities = model.GetEntities();

        var sql = new StringWriter();
        foreach (var entity in entities.Values.Where(e => e.BaseExtension == null))
        {
            //var sql = new StringWriter();
            var tableName = EntityDbNameManager.GetTableDbFullName(entity);
            var fromSrcSql = " FROM [" + SourceDB + "]." + tableName + " G where ";
            var bFirst = true;
            foreach (var field in entity.MappedEntityFields)
                if (field.IncludeInPkv)
                {
                    var fieldName = field.DbFieldName;
                    fromSrcSql += (bFirst ? "" : " And ") +
                                  "(G." + fieldName + " =[" + DestDB + "]." + tableName + "." + fieldName + ")";
                    bFirst = false;
                }

            var auto = entity.AutoCalcs?.Calculations.FirstOrDefault(ac =>
                !ac.RecalcOnAnyChange && ac.GenerationType == AutoCalc.eGenerationType.DBInsert);
            var identityField = auto?.FieldId;

            sql.WriteLine("USE [" + DestDB + "]");
            sql.WriteLine("GO");
            sql.WriteLine("SET ANSI_NULLS ON");
            sql.WriteLine("GO");
            sql.WriteLine("SET QUOTED_IDENTIFIER ON");
            sql.WriteLine("GO");
            sql.WriteLine("");
            sql.WriteLine($"DELETE FROM [{DestDB}].[{DbNameManager.GetDbSchemaName(entity)}].[{tableName}]");
            sql.WriteLine("\tWHERE NOT EXISTS(SELECT 1 " + fromSrcSql + ")");
            sql.WriteLine("GO");
            sql.WriteLine();

            sql.WriteLine($"UPDATE[{DestDB}].[{DbNameManager.GetDbSchemaName(entity)}].[{tableName}] SET ");
            bFirst = true;
            foreach (var field in entity.MappedEntityFields)
            {
                if (field.AssociationEntity != null) continue;
                if (field.Id == identityField) continue;
                var fieldName = field.DbFieldName;
                sql.WriteLine("\t" + (bFirst ? "" : ",") +
                              "[" + fieldName + "] = (SELECT G.[" + fieldName + "] " + fromSrcSql + ")");
                bFirst = false;
            }

            sql.WriteLine("GO");
            sql.WriteLine();
            sql.WriteLine($"INSERT INTO [{DestDB}].[{DbNameManager.GetDbSchemaName(entity)}].[{tableName}]");
            bFirst = true;
            foreach (var field in entity.MappedEntityFields)
            {
                if (field.AssociationEntity != null) continue;
                if (field.Id == identityField) continue;
                var fieldName = field.DbFieldName;
                sql.WriteLine("\t" + (bFirst ? "" : ",") + "[" + fieldName + "]");
                bFirst = false;
            }

            sql.WriteLine(") ( SELECT ");
            bFirst = true;
            foreach (var field in entity.MappedEntityFields)
            {
                if (field.AssociationEntity != null) continue;
                if (field.Id == identityField) continue;
                var fieldName = field.DbFieldName;
                sql.WriteLine("\t" + (bFirst ? "" : ",") + "[" + fieldName + "]");
                bFirst = false;
            }

            sql.WriteLine($"FROM [{SourceDB}].[{DbNameManager.GetDbSchemaName(entity)}].[{tableName}] F");
            sql.WriteLine($"WHERE NOT EXISTS(SELECT 1 FROM [{DestDB}].[{DbNameManager.GetDbSchemaName(entity)}].[{tableName}]");
            bFirst = true;
            foreach (var field in entity.MappedEntityFields)
                if (field.IncludeInPkv)
                {
                    var fieldName = field.DbFieldName;
                    sql.Write((bFirst ? "" : " And ") + "(G.[" + fieldName + "]=F.[" + fieldName + "])");
                    bFirst = false;
                }

            sql.Write(")");
            sql.WriteLine(")");
            sql.WriteLine("GO");

            //var file = File.CreateText(path+"\\"+entity.Id+(entity.Id==tableName?"":" - "+ tableName)+".sql");
            //file.Write(sql.ToString());
            //file.Close();
        }

        var file = File.CreateText(path + "\\" + namespaceId + ".sql");
        file.Write(sql.ToString());
        file.Close();
    }
}