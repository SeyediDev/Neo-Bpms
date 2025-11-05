using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Domain.Models.Cmmn.Partitions;
using PartitionScheme = Neo.Bpms.Domain.Models.Cmmn.Partitions.PartitionScheme;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadPartitionSchemes()
    {
        var sql = DDLGenerator.GetPartitionSchemes();
        var dt = Select(sql, "10.1.1.5");
        if (dt == null) return;
        foreach (var item in dt)
        {
            var dbPartitionFunction = _model.PartitionFunctions.Values.FirstOrDefault(f => f.Id == item.GetLong("function_id"));
            var id = item.GetLong("data_space_id");
            var name = item.GetString("name");
            var fileGroup = _model.FileGroups.Values.FirstOrDefault(ds => ds.Id == id) ??
                            (_model.FileGroups.TryGetValue(name, out DbFileGroup value) ? value : null);
            if (fileGroup != null)
                fileGroup.PartitionFunction = dbPartitionFunction;
            else
            {
                fileGroup = new DbFileGroup
                {
                    Id = id,
                    Name = name,
                    Type = item.GetString("type"),
                    PartitionFunction = dbPartitionFunction
                };
                _model.FileGroups.Add(fileGroup.Name, fileGroup);
            }
            _model.PartitionSchemes.Add(name, new DbPartitionScheme
            {
                Name = name,
                PartitionFunctionId = dbPartitionFunction.Name,
            });
        }
    }

    private void SyncPartitionSchemeFileGroups(PartitionScheme partitionScheme)
    {
        switch (partitionScheme.FileGroupSelectionType)
        {
            case FileGroupSelectionType.Monthly:
            case FileGroupSelectionType.Daily:
                var start = Convert.ToDateTime(partitionScheme.PartitionFunction.StartOfRange);
                var end = Convert.ToDateTime(partitionScheme.PartitionFunction.EndOfRange);
                do
                {
                    var fileGroup = partitionScheme.FileGroupPrefix + start.Year + "_" + Get2Digit(start.Month);
                    if (partitionScheme.FileGroupSelectionType == FileGroupSelectionType.Daily)
                        fileGroup += "_" + Get2Digit(start.Day);
                    if (!partitionScheme.FileGroups.Contains(fileGroup))
                    {
                        SyncFileGroup(fileGroup);
                    }
                    switch (partitionScheme.PartitionFunction.FunctionType)
                    {
                        case PartitionFunctionType.Monthly:
                            start = start.AddMonths(1);
                            break;
                        case PartitionFunctionType.Daily:
                            start = start.AddDays(1);
                            break;
                    }
                } while (start <= end);

                break;
            case FileGroupSelectionType.FromList:
                break;
        }
    }

    private void SyncPartitionSchemes(ModelNamespace model)
    {
        foreach (var partitionScheme in model.PartitionSchemes?.Values ?? Enumerable.Empty<PartitionScheme>())
            CheckPartitionScheme(model, partitionScheme);
    }

    private void CheckPartitionScheme(ModelNamespace model, PartitionScheme partitionScheme)
    {
        SyncPartitionSchemeFileGroups(partitionScheme);
        if (!_model.PartitionSchemes.TryGetValue(partitionScheme.Id, out var dbPartitionScheme)|| dbPartitionScheme==null)
        {
            CreatePartitionScheme(model, partitionScheme);
        }
        else if(dbPartitionScheme.PartitionFunctionId!= partitionScheme.PartitionFunction.Id)
        {
            //todo change PartitionFunctionId
        }
    }

    private void CreatePartitionScheme(ModelNamespace model, PartitionScheme partitionScheme)
    {
        AddMessage(model, null, "Create Partition Scheme " + partitionScheme.Id);
        var ddl =
            $"CREATE PARTITION SCHEME[{partitionScheme.Id}] " +
            $"AS PARTITION[{partitionScheme.PartitionFunction.Id}] " +
            $"TO({string.Join(",", partitionScheme.FileGroups.Select(fg => $"[{fg}]"))},[{partitionScheme.FileGroups.LastOrDefault()}])";
        AddToCommandList(null, ddl, false);
        DoSqlCommand(null, ddl, "", "44.0");
        _model.PartitionSchemes.Add(partitionScheme.Id, 
            new DbPartitionScheme 
            { 
                Name = partitionScheme.Id, 
                PartitionFunctionId = partitionScheme.PartitionFunction.Id 
            });
    }
}
