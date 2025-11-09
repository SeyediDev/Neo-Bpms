using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Engine.Data.ADODotNet;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadTablesFields()
    {
        var sql = DDLGenerator.GetTablesFields(Options.SpecificEntity);
        var pRsField = Select(sql, "10.1.1.14");
        if (pRsField == null) return;
        DbTable dbTable = null;
        DbView dbView = null;
        foreach (var item in pRsField) // (int idx = 0; idx < pRsField.Rows.Count; idx++)
        {
            var schemaName = item.GetString("TABLE_SCHEMA");
            var tableName = item.GetString("TABLE_NAME");
            var tableFullName = $"[{schemaName}].[{tableName}]";
            if (dbView == null || (tableFullName != dbView.FullName))
                _model.Views.TryGetValue(tableFullName, out dbView);
            if (dbView != null)
                dbTable = null;
            else if (dbTable == null || (tableFullName != dbTable.FullName))
                _model.Tables.TryGetValue(tableFullName, out dbTable);
            if (dbTable != null || dbView != null)
            {
                var fieldName = item.GetString("COLUMN_NAME");
                var bDeleted = CheckColumnDeleted(fieldName);
                item.GetField("DATA_TYPE", out var cv);
                var cDataType = cv;
                item.GetField("FIELDLEN", out cv);
                var cFieldlen = cv;
                var dbField = new DbField
                {
                    //m_FieldId = FieldId;
                    Name = fieldName,
                    Deleted = bDeleted,
                    Type = cDataType?.ToString() ?? "string",
                    Len = !string.IsNullOrEmpty(cFieldlen?.ToString()) ? Convert.ToInt32(cFieldlen.ToString()) : 0,
                    Collation = item.GetString("COLLATIONNAME") //Persian_100_CI_AI
                };
                SetFieldLen(dbField, item);

                dbField.IsIdentity = FieldIsIdentity(item);

                var nullable = item.GetString("NULLABLE");
                dbField.NotNull = nullable[..1] == "N";
                var bAddField = dbView?.AddField(dbField) ?? dbTable.AddField(dbField);
                if (!bAddField)
                    AddLog("10.1.1.15", Log.Error, "فیلد تکراری در پایگاه داده" + tableName + " : " + dbField.Name);
            }
            else
            {
                AddLog("10.1.1.16", Log.Error, "پایگاه داده خراب است " + tableName);
            }
        }
    }

    protected virtual bool FieldIsIdentity(ElasticObject item)
    {
        return false;
    }

    protected virtual void SetFieldLen(DbField dbField, ElasticObject item)
    {
    }

    private void SyncTableFields(Entity entity, DbTable dbTable)
    {
        var collation = DbNameManager.GetDbTableCollation(entity);
        foreach (var field in entity.MappedEntityFields)
        {
            if (_pleaseStop) return;
            SyncEntityField(field, dbTable);
        }

        var fields = dbTable.Fields.Values.Select(f => f.Name).ToList();
        foreach (var fieldName in fields)
        {
            if (_pleaseStop) return;
            SyncDatabaseField(dbTable, fieldName, entity, collation);
        }
    }

    private void SyncEntityField(EntityField field, DbTable dbTable)
    {
        if (dbTable.CheckField(field.DbFieldName)) return;
        var ignoreName = GetIgnoreDbName(field.DbFieldName);
        var mainDbField = dbTable.Fields.Values.FirstOrDefault(f => GetIgnoreDbName(f.Name) == ignoreName);
        var entity = field.Entity;
        if (mainDbField == null)
        {
            var dbField = dbTable.GetField(field.OldDbFieldName)
                          ?? dbTable.GetField("del_" + field.DbFieldName);
            if (dbField != null)
            {
                RenameDbField(entity, dbTable, dbField, dbField.Name, field.DbFieldName);
                dbField.Deleted = false;
                return;
            }

            AddFieldToDbTable(entity, dbTable, field);
        }
        else
        {
            var ignoreNameField = GetFieldViaDbName(entity, ignoreName);
            if (ignoreNameField != null)
                AddFieldToDbTable(entity, dbTable, field);
            else
                RenameDbField(entity, dbTable, mainDbField, mainDbField.Name, field.DbFieldName);
        }
    }

    private void AddFieldToDbTable(Entity entity, DbTable dbTable, EntityField pField)
    {
        var dbField = dbTable.GetField(pField.DbFieldName);
        if (dbField != null)
        {
            if (!dbField.Deleted)
            {
                AddLog("10.0.67", Log.Info, entity.Name);
                return;
            }
        }

        dbField ??= dbTable.GetField("del_" + pField.DbFieldName);
        var fieldType = GetSqlFullFieldType(pField);
        bool bAdd = false, added = false;
        if (dbField == null)
            bAdd = true;
        else
        {
            added = RenameDbField(entity, dbTable, dbField, dbField.Name, pField.DbFieldName);
            if (added)
            {
                AddLog("10.0.70", Log.Info,
                    $"Table {dbTable.FullName} Column {dbField.Name} renamed with {pField.DbFieldName}");
                dbField.Deleted = false;
            }
        }

        if (bAdd)
        {
            var bNotNull = pField.Required;
            var isIdentity = false;
            var auto = entity.AutoCalcs?.Calculations.FirstOrDefault(ac => ac.FieldId == pField.Id);
            if (auto != null && !auto.RecalcOnAnyChange && auto.GenerationType == AutoCalc.eGenerationType.DBInsert)
                isIdentity = true;
            added = AddDbField(entity, dbTable, pField.DbFieldName, fieldType, bNotNull, isIdentity);
            if (added)
            {
                dbField = new DbField
                {
                    Name = pField.DbFieldName,
                    Len = pField.MaxLen,
                    Type = GetSqlFieldType(pField.FieldType),
                    Deleted = false
                };
                dbTable.AddField(dbField);
            }
        }

        if (added)
            pField.bExistInDBMS = true;
    }

    private bool AddDbField(Entity entity, DbTable dbTable, string fieldName, string fullFieldType, bool bNotNull,
        bool isIdentity)
    {
        DropTriggers(entity, dbTable);
        var ddl = DDLGenerator.AddField(dbTable.FullName, fieldName, fullFieldType, bNotNull, isIdentity);
        //ddl += "ALTER TABLE "+ dbTable.FullName + " SET (LOCK_ESCALATION = TABLE);";
        if (dbTable.GetField(fieldName) != null)
        {
            AddLog("10.0.150", Log.Error, ddl);
            return false;
        }

        AddToCommandList(entity, ddl, true);
        return DoSqlCommand(entity, ddl, "", "10.0.9");
    }

    private void SyncDatabaseField(DbTable dbTable, string fieldName, Entity entity, string collation)
    {
        var dbField = dbTable.GetField(fieldName);
        if (dbField == null)
            return;
        var field = GetFieldViaDbName(entity, dbField.Name);
        if (!IsMappedInThisEntity(field))
        {
            var formedName = GetFormedDbName(dbField.Name);
            field = GetFieldViaDbName(entity, formedName);
            if (IsMappedInThisEntity(field))
                RenameDbField(entity, dbTable, dbField, dbField.Name, formedName);
            else if (Options.RenameUndefinedField && !entity.DerivedEntitiesExtendedMe)
                DeleteDbField(entity, dbTable, dbField);
        }

        if (!IsMappedInThisEntity(field))
        {
            if (Options.RenameUndefinedField && !entity.DerivedEntitiesExtendedMe)
                DeleteDbField(entity, dbTable, dbField);
        }
        else if (!dbField.Deleted)
        {
            if (HasDifference(dbField, field, collation))
                ChangeDbField(entity, dbTable, dbField, field, collation);
            else
            {
                var defaultValue = AdoDotNetDatabaseDataSource.GetSqlValue(field.DefaultValue, field, field.CSharpType, false);
                var dbDefaultValue = dbField.Default?.Value ?? "";
                if (!defaultValue.Equals("null", StringComparison.CurrentCultureIgnoreCase))
                {
                    if(dbDefaultValue.ToString().StartsWith("((") && dbDefaultValue.ToString().EndsWith("))"))
                    {
                        dbDefaultValue = dbDefaultValue.ToString()[2..^2];
                    }
                    if (dbDefaultValue != defaultValue && !defaultValue.Equals("null", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(dbField.Default?.Name))
                            DropConstraint(entity, dbTable, dbField.Default.Name);
                        if (!string.IsNullOrEmpty(defaultValue))
                        {
                            DropTriggers(entity, dbTable);
                            var ddl =
                                $@"UPDATE {dbTable.FullName} SET [{dbField.Name}]={defaultValue} WHERE [{dbField.Name}] is null;
ALTER TABLE {dbTable.FullName} ADD DEFAULT {defaultValue} FOR [{dbField.Name}]";
                            AddToCommandList(entity, ddl, true);
                            DoSqlCommand(entity, ddl, "", "10.5.1.1");
                        }
                    }
                }

                if (field.Required == dbField.NotNull)
                    return;
                if (field.Required)
                    SetDbFieldNotNull(entity, dbTable, dbField, field, collation);
                else
                    SetDbFieldNull(entity, dbTable, dbField, field, collation);
            }
        }
    }

    private static string GetIgnoreDbName(string dbFieldName)
    {
        return dbFieldName.Replace("_", "").ToLower();
    }

    private static string GetFormedDbName(string oldName)
    {
        var nameObjects = oldName.Split('_');
        var formedName = "";
        foreach (var nameObject in nameObjects)
        {
            if (string.IsNullOrEmpty(nameObject))
                continue;
            if (nameObject[..1] == nameObject[..1].ToLower())
            {
                formedName += nameObject[..1].ToUpper();
                if (nameObject.Length > 1)
                    formedName += nameObject[1..];
            }
            else
                formedName += nameObject;
        }

        return formedName;
    }

    private bool DropDbField(Entity entity, DbTable dbTable, DbField dropField)
    {
        DropTriggers(entity, dbTable);
        var ddl = DDLGenerator.DropField(dbTable.FullName, dropField.Name);
        AddToCommandList(entity, ddl, true);
        var b = DoSqlCommand(entity, ddl, "", "10.0.89");
        if (b)
        {
            dbTable.DeleteField(dropField.Name);
        }

        return b;
    }

    private bool RenameDbField(Entity entity, DbTable dbTable, DbField dbField, string oldName, string newName)
    {
        DropTriggers(entity, dbTable);
        var ddl = DDLGenerator.RenameField(dbTable.FullName, oldName, newName);
        AddToCommandList(entity, ddl, true);
        var b = DoSqlCommand(entity, ddl, "", "10.0.72");
        if (!b) return false;
        dbTable.DeleteField(oldName);
        dbField.Name = newName;
        dbTable.AddField(dbField);
        return true;
    }

    private void DeleteDbField(Entity entity, DbTable dbTable, DbField dbField)
    {
        if (Options.DropUndefinedField || CheckColumnDeleted(dbField.Name))
        {
            RemoveAllFieldDependency(dbTable, entity, dbField);
            if (Options.DropUndefinedField || (dbField.Name.Length > 8 && dbField.Name[..8] == "del_del_"))
                DropDbField(entity, dbTable, dbField);
        }
        else
        {
            var newName = "del_" + dbField.Name;
            var dbNewSqlField = dbTable.GetField(newName);
            if (dbNewSqlField != null)
            {
                RemoveAllFieldDependency(dbTable, entity, dbNewSqlField);
                RenameDbField(entity, dbTable, dbNewSqlField, dbNewSqlField.Name, "del_" + dbNewSqlField.Name);
            }

            if (RenameDbField(entity, dbTable, dbField, dbField.Name, newName))
                RemoveAllFieldDependency(dbTable, entity, dbField);
        }
    }

    private void SetDbFieldNull(Entity entity, DbTable dbTable, DbField dbField, EntityField pField,
        string collation)
    {
        var fieldTypeStr = GetSqlFullFieldType(pField);
        if (pField.IsTextParam())
            fieldTypeStr += " COLLATE " + collation;
        SetColumnNullable(entity, dbTable, dbField, fieldTypeStr, true);
        dbField.NotNull = false;
    }

    private void SetDbFieldNotNull(Entity entity, DbTable dbTable,
        DbField dbField, EntityField pField, string collation)
    {
        var fieldTypeStr = GetSqlFullFieldType(pField);
        if (pField.IsTextParam())
            fieldTypeStr += " COLLATE " + collation;
        SetColumnNotNull(entity, dbTable, dbField, fieldTypeStr);
        dbField.NotNull = true;
    }

    private void SetColumnNullable(Entity entity, DbTable dbTable,
        DbField dbField, string fullFieldType, bool removeAllTableDependency)
    {
        if (removeAllTableDependency)
        {
            DropTriggers(entity, dbTable);
            RemoveAllFieldDependency(dbTable, entity, dbField);
        }

        var ddl = DDLGenerator.SetColumnNullable(dbTable.FullName, dbField.Name, fullFieldType);
        AddToCommandList(entity, ddl, true);
        DoSqlCommand(entity, ddl, "", "10.0.112.2");
    }

    private void SetColumnNotNull(Entity entity, DbTable dbTable,
        DbField dbField, string fullFieldType)
    {
        DropTriggers(entity, dbTable);
        RemoveAllFieldDependency(dbTable, entity, dbField);
        var ddl = DDLGenerator.SetColumnNotNull(dbTable.FullName, dbField.Name, fullFieldType);
        AddToCommandList(entity, ddl, true);
        if( !DoSqlCommand(entity, ddl, "", "10.0.112") )
        {
            ddl = DDLGenerator.SetNotNullColumnValue(dbTable.FullName, dbField.Name, "''");
            if (DoSqlCommand(entity, ddl, "", "10.0.112.3"))
            {
                ddl = DDLGenerator.SetColumnNotNull(dbTable.FullName, dbField.Name, fullFieldType);
                AddToCommandList(entity, ddl, true);
                DoSqlCommand(entity, ddl, "", "10.0.112.4");
            }
        }
    }

    private bool HasDifference(DbField dbField, EntityField field, string collation)
    {
        var fieldType = GetSqlFullFieldType(field);
        var sqlFieldType = GetSqlFieldType(field.FieldType);
        if (!string.Equals(dbField.Type, fieldType, StringComparison.CurrentCultureIgnoreCase) &&
            !string.Equals(dbField.Type, sqlFieldType, StringComparison.CurrentCultureIgnoreCase))
            return true;
        if (!IsStringType(sqlFieldType)) return false;
        if (Options.CheckCollation)
        {
            if (dbField.Collation != collation)
                return true;
        }

        var mylen = field.MaxLen;
        if (mylen <= 0) mylen = 4000;
        var sqllen = dbField.Len;
        if (sqllen <= 0) sqllen = 4000;
        return mylen != sqllen;
    }

    private void ChangeDbField(Entity entity, DbTable dbTable, DbField dbField, EntityField pField,
        string collation)
    {
        var fieldTypeStr = GetSqlFullFieldType(pField);
        var castFieldType = fieldTypeStr;
        if (pField.IsTextParam())
            fieldTypeStr += " COLLATE " + collation;
        var tempColumnName = "F_TMP_" + pField.DbFieldName;
        var tmpField = dbTable.GetField(tempColumnName);
        if (tmpField != null)
            if (!DropDbField(entity, dbTable, tmpField))
                tempColumnName += DateTime.Now.Ticks;
        if (!AddDbField(entity, dbTable, tempColumnName, fieldTypeStr, false, false))
            return;
        var ddl = DDLGenerator.ChangeField(dbTable.FullName, tempColumnName, pField.DbFieldName, castFieldType);
        AddToCommandList(entity, ddl, false);
        if (!DoSqlCommand(entity, ddl, "", "10.0.86"))
        {
            var castColumnName = "F_CAST_" + pField.DbFieldName;
            var castField = dbTable.GetField(castColumnName);
            if (castField != null)
                if (!DropDbField(entity, dbTable, castField))
                    castColumnName += DateTime.Now.Ticks;
            if (!RenameDbField(entity, dbTable, dbField, pField.DbFieldName, castColumnName))
            {
                AddLog("10.0.145", Log.Error, entity.Name + ":" + castColumnName);
                return;
            }
        }
        else
            DeleteDbField(entity, dbTable, dbField);

        if (!RenameDbField(entity, dbTable, dbField, tempColumnName, pField.DbFieldName))
            AddLog("10.0.147", Log.Error, ddl);
        if (pField.Required)
            SetDbFieldNotNull(entity, dbTable, dbField, pField, collation);
    }

    private static bool IsIdentity(Entity entity, EntityField pField)
    {
        var isIdentity = false;
        var auto = entity.AutoCalcs?.Calculations.FirstOrDefault(ac => ac.FieldId == pField.Id);
        if (auto != null && !auto.RecalcOnAnyChange && auto.GenerationType == AutoCalc.eGenerationType.DBInsert)
            isIdentity = true;
        return isIdentity;
    }

    private static bool CheckColumnDeleted(string fieldName)
    {
        return fieldName.StartsWith("del_");
    }

    private void RemoveAllFieldDependency(DbTable dbTable, Entity entity, DbField dbField)
    {
        if (dbField.Deleted)
        {
            var pkIndex = dbTable.PkIndex;
            if (pkIndex != null && pkIndex.Fields.Any(f => f.Name == dbField.Name))
                DropConstraint(entity, dbTable, pkIndex.Name);
        }

        if (dbField.Default != null)
            DropConstraint(entity, dbTable, dbField.Default.Name);
        if (dbTable.InForeignKeys != null)
        {
            foreach (var fk in dbTable.InForeignKeys.Values)
            {
                if (_pleaseStop) return;
                var drop = false;
                if (fk.BaceTable.Name != dbTable.Name)
                    drop = true;
                else
                {
                    if (fk.Columns.Any(col => col.BaseKeyColumn == dbField.Name))
                    {
                        drop = true;
                    }
                }

                if (drop)
                    DropConstraint(entity, fk.ForeignTable, fk.Name);
            }
        }

        if (dbTable.ForeignKeys != null)
        {
            foreach (var fk in dbTable.ForeignKeys.Values)
            {
                if (_pleaseStop) return;
                var drop = false;
                if (fk.ForeignTable.Name != dbTable.Name)
                    drop = true;
                else
                {
                    if (fk.Columns.Any(col => col.ForeignKeyColumn == dbField.Name))
                    {
                        drop = true;
                    }
                }

                if (drop)
                    DropConstraint(entity, dbTable, fk.Name);
            }
        }

        if (dbTable.Indexes != null)
        {
            var dropIndexes = dbTable.Indexes.Values.Where(idx => idx.Fields.Any(ff => ff.Name == dbField.Name))
                .ToDictionary(idx => idx.Name);
            foreach (var dbIndex in dropIndexes.Values)
            {
                if (_pleaseStop) return;
                DeleteDbIndex(entity, dbTable, dbIndex);
            }
        }

        if (dbField.NotNull && !dbField.IsIdentity)
            SetColumnNullable(entity, dbTable, dbField, GetSqlFullFieldType(dbField), false);
    }

    private void RemoveAllTableDependency(DbTable dbTable, Entity entity)
    {
        if (dbTable.InForeignKeys == null) return;
        foreach (var fk in dbTable.InForeignKeys.Values)
        {
            if (_pleaseStop) return;
            DropConstraint(entity, fk.ForeignTable, fk.Name);
        }
    }
}
