namespace Neo.Bpms.Infrastructure.Features.Orm.SqlServerJobUtils.Partition;

internal class PartitionScriptGenerator
{
    private readonly string _databaseName;
    private readonly string _partitionFunction;
    private readonly string _partitionSchema;
    private readonly string[] _tables;

    internal PartitionScriptGenerator(string databaseName,
        string partitionFunction, string partitionSchema, params string[] tables)
    {
        if (databaseName.Contains("'"))
            throw new Exception($"Dangerous databaseName: {databaseName}");
        if (partitionFunction.Contains("'"))
            throw new Exception($"Dangerous partitionFunction: {partitionFunction}");
        if (partitionSchema.Contains("'"))
            throw new Exception($"Dangerous partitionSchema: {partitionSchema}");
        foreach (var table in tables)
        {
            if (table.Contains("'"))
                throw new Exception($"Dangerous table: {table}");
        }
        _databaseName = databaseName;
        _partitionFunction = partitionFunction;
        _partitionSchema = partitionSchema;
        _tables = tables;
    }

    internal string GetTSqlCommand(int expireDays, int expandDays)
    {
        var sql = $@"
USE [{_databaseName}]
GO

declare @exprireDays int = {expireDays};
declare @expandDays int = {expandDays};

declare @physical_name nvarchar(1024)
select top 1 @physical_name=physical_name from sys.database_files
select @physical_name = LEFT(@physical_name, Len(@physical_name) - Charindex('\', Reverse(@physical_name)))

declare @current_data datetime = getdate();
declare @exprire_data datetime = DATEADD(day, -@exprireDays, @current_data);

select prv.boundary_id id, CAST(prv.value as datetime) value
into #prv
from sys.partition_range_values prv
join sys.partition_functions f on f.function_id=prv.function_id
where f.name='{_partitionFunction}'
and CAST(prv.value as datetime)<@exprire_data

WHILE (exists(select * from #prv))
BEGIN
	declare @id bigint;
	declare @value datetime;

	select top 1 @id=id, @value=value from #prv
	
	print @value
	ALTER PARTITION FUNCTION {_partitionFunction}()  
	MERGE RANGE(@value);

";
        foreach (var table in _tables)
        {
            sql += $@"
	TRUNCATE TABLE {table} WITH (PARTITIONS(1))";
        }
        sql += $@"
	delete #prv where id=@id;
END
drop table #prv;

declare @expand_data datetime;
declare @nextdays bigint;
select @nextdays=count(*), @expand_data=max(CAST(prv.value as datetime))
from sys.partition_range_values prv
join sys.partition_functions f on f.function_id=prv.function_id
where f.name='{_partitionFunction}'
and CAST(prv.value as datetime)>@current_data

declare @i_expand bigint = @expandDays-@nextdays;
while @i_expand>0
begin
	set @expand_data = dateadd(day, 1, @expand_data);
	declare @fg varchar(256);
	select @fg = concat('_',datepart(year,@expand_data),'_')
	if( datepart(month,@expand_data)<10 )
		select @fg = concat(@fg, '0')
	select @fg = concat(@fg, datepart(month,@expand_data))

	if( not exists(SELECT * FROM sys.data_spaces where name=@fg))
	begin
		exec('ALTER DATABASE [{_databaseName}] ADD FILEGROUP ['+@fg+']');
		exec('ALTER DATABASE [{_databaseName}] ADD FILE ( NAME = N''' + @fg +
						 ''', FILENAME = N''' + @physical_name + '\\' + @fg + '.ndf' +
						 ''' , SIZE = 512KB , FILEGROWTH = 1024KB ) TO FILEGROUP [' + @fg + ']')
	end
	
	exec('ALTER PARTITION SCHEME {_partitionSchema} NEXT USED '+@fg)
	
	ALTER PARTITION FUNCTION {_partitionFunction}()  
	SPLIT RANGE(@expand_data);
	
	set @i_expand = @i_expand-1;
end
";
        return sql;
    }
}
