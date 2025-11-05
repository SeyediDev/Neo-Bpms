using System.Data;
using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;

namespace Neo.Bpms.Engine.Data.ADODotNet;

public abstract partial class AdoDotNetDatabaseDataSource
{
    public override Entity extractEntityMetaData(ILogger logger)
    {
        if (DataReader == null) return null;
        if (DataSrcDefinition.Entity == null)
            ExtractEntityFromSchema();
        _fieldInfos = [];
        for (var i = 0; i < DataReader.FieldCount; i++)
        {
            var dbFieldName = DataReader.GetName(i);
            var type = DataReader.GetFieldType(i);
            var entityField = DataSrcDefinition.Entity?.GetFieldByDbName(dbFieldName)
                              ?? new EntityField(DataSrcDefinition.Entity, dbFieldName, dbFieldName, dbFieldName, type,
                                  EntityField.GetFieldType(type, logger));
            _fieldInfos.TryAdd(dbFieldName, entityField);
            if (!Outfields.ContainsKey(dbFieldName))
            {
                var columnDefinition = new FieldDefinition(entityField)
                {
                    fieldName = dbFieldName,
                    overFieldName = dbFieldName
                };
                AddOutField(columnDefinition);
            }
        }

        return DataSrcDefinition.Entity;
    }

    private void ExtractEntityFromSchema()
    {
        DataSrcDefinition.Entity = new Entity(null, null, DataSrcDefinition.name, DataSrcDefinition.name, DataSrcDefinition.name);
        var schema = DataReader.GetSchemaTable();
        if (schema != null)
        {
            foreach (DataColumn column in schema.Columns)
            {
                var entityField = new EntityField(DataSrcDefinition.Entity,
                    column.ColumnName, column.ColumnName, column.ColumnName,
                    column.DataType, EntityField.GetFieldType(column.DataType, logger));
                if (!column.AllowDBNull)
                    entityField.Required = true;
                if (column.AutoIncrement)
                {
                    if (DataSrcDefinition.Entity.AutoCalcs == null)
                        DataSrcDefinition.Entity.InitAutoCalcs();
                    var formulaEx = Parser.ParseTree("AutoIncrement()");
                    DataSrcDefinition.Entity.AutoCalcs.AddAutoCalc(new AutoCalc
                    {
                        Formula = formulaEx,
                        Loaction = AutoCalc.AutoCalcLocation.None,
                        FieldId = column.ColumnName,
                        GenerationType = AutoCalc.eGenerationType.DBInsert
                    });
                }
            }

            foreach (var pk in schema.PrimaryKey)
            {
                var f = DataSrcDefinition.Entity.GetField(pk.ColumnName);
                if (f != null)
                    f.Flags |= EntityFieldFlags.IncludeInPKV;
            }
        }
    }
}
