using Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Domain.Entities.Cmmn.Partitions;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadPartitionFunctionsItems()
    {
        //if(database is not sql server) return;
        //create agent to sync partitions continuously
        ReadPartitionFunctions();
        ReadPartitionFunctionsParameters();
        ReadPartitionFunctionsRanges();
    }

    private void ReadPartitionFunctions()
    {
        _model.PartitionFunctions = [];
        var sql = DDLGenerator.GetPartitionFunctions();
        var dt = Select(sql, "10.1.1.2");
        if (dt == null) return;
        foreach (var item in dt)
        {
            var partitionFunction = new DbPartitionFunction
            {
                Id = item.GetLong("function_id"),
                Name = item.GetString("name"),
                Type = item.GetString("type"),
                BoundaryValueOnRight = item.GetString("boundary_value_on_right")
            };
            _model.PartitionFunctions.Add(partitionFunction.Name, partitionFunction);
        }
    }

    private void ReadPartitionFunctionsParameters()
    {
        var sql = DDLGenerator.GetPartitionFunctionsParameters();
        var dt = Select(sql, "10.1.1.3");
        if (dt == null) return;
        foreach (var item in dt)
        {
            var functionId = item.GetLong("function_id");
            var partitionFunction = _model.PartitionFunctions.Values.FirstOrDefault(ds => ds.Id == functionId);
            if (partitionFunction != null)
            {
                var parameter = new DbPartitionFunction.Parameter
                {
                    Id = item.GetLong("parameter_id"),
                    Type = item.GetString("system_type_id")
                };
                partitionFunction.Parameters.Add(parameter.Id, parameter);
            }
        }
    }

    private void ReadPartitionFunctionsRanges()
    {
        var sql = DDLGenerator.GetPartitionFunctionsRanges();
        var dt = Select(sql, "10.1.1.4");
        if (dt == null) return;
        foreach (var item in dt)
        {
            var functionId = item.GetLong("function_id");
            var partitionFunction = _model.PartitionFunctions.Values.FirstOrDefault(ds => ds.Id == functionId);
            if (partitionFunction != null)
            {
                var rangeValue = new DbPartitionFunction.RangeValue
                {
                    BoundaryId = item.GetString("boundary_id"),
                    Value = item.GetString("value")
                };
                var parameterId = item.GetLong("parameter_id");
                partitionFunction.Parameters.TryGetValue(parameterId, out rangeValue.Parameter);
                partitionFunction.RangeValues.Add(rangeValue);
            }
        }
    }

    
    private void SyncPartitionFunctions(ModelNamespace model)
    {
        foreach (var partitionFunction in model.PartitionFunctions?.Values ?? Enumerable.Empty<PartitionFunction>())
            CheckPartitionFunction(model, partitionFunction);
    }

    private void CheckPartitionFunction(ModelNamespace model, PartitionFunction partitionFunction)
    {
        if (partitionFunction.Values.Count == 0)
            GetPartitionFunctionValues(partitionFunction);
        if (!_model.PartitionFunctions.ContainsKey(partitionFunction.Id))
            CreatePartitionFunction(model, partitionFunction);
        else
        {
            //todo check parameters is valid
        }
    }

    private void CreatePartitionFunction(ModelNamespace model, PartitionFunction partitionFunction)
    {
        AddMessage(model, null, "Create Partition Function " + partitionFunction.Id);
        var ddl =
            $"CREATE PARTITION FUNCTION [{partitionFunction.Id}]({GetSqlFieldType(partitionFunction.ValueType)}) AS RANGE {partitionFunction.BoundaryType.ToString().ToUpper()} FOR VALUES ({string.Join(",", partitionFunction.Values.Select(fg => $"N'{fg}'"))})";
        AddToCommandList(null, ddl, false);
        DoSqlCommand(null, ddl, "", "44.1");
        _model.PartitionFunctions.Add(partitionFunction.Id, new DbPartitionFunction { Name = partitionFunction.Id });
    }


    private static void GetPartitionFunctionValues(PartitionFunction partitionFunction)
    {
        if (partitionFunction.FunctionType == PartitionFunctionType.Monthly ||
            partitionFunction.FunctionType == PartitionFunctionType.Daily)
        {
            var start = Convert.ToDateTime(partitionFunction.StartOfRange);
            var end = Convert.ToDateTime(partitionFunction.EndOfRange);
            do
            {
                var value = start.Year + "-" + Get2Digit(start.Month) + "-" + Get2Digit(start.Day) + "T00:00:00.000";
                partitionFunction.Values.Add(value);
                start = partitionFunction.FunctionType switch
                {
                    PartitionFunctionType.Monthly => start.AddMonths(1),
                    PartitionFunctionType.Daily => start.AddDays(1),
                    PartitionFunctionType.FixRange => end.AddDays(1),
                    _ => end.AddDays(1),
                };
            } while (start <= end);
        }
    }

    private static string GetSqlFieldType(Type valueType)
    {
        if (valueType == typeof(DateTime))
        {
            return "datetime";
        }
        if (valueType == typeof(bool))
        {
            return "bit";
        }
        if (valueType == typeof(long))
        {
            return "bigint";
        }
        if (valueType == typeof(int))
        {
            return "int";
        }

        return valueType.ToString().ToLower(); //todo sql server only
    }

    private static string Get2Digit(int digit)
    {
        var s = digit.ToString();
        return (s.Length == 1 ? "0" : "") + s;
    }
}
