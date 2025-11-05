using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadDatabaseFiles()
    {
        var sql = DDLGenerator.DatabaseFiles();
        var dt = Select(sql, "10.1.1.1");
        if (dt == null) return;
        foreach (var item in dt)
        {
            var dataSpaceId = item.GetLong("data_space_id");
            var dataSpace = _model.FileGroups.Values.FirstOrDefault(ds => ds.Id == dataSpaceId);
            if (dataSpace == null) continue;
            var dataFile = new DbFileGroup.DataFile
            {
                Name = item.GetString("name"),
                PhysicalName = item.GetString("physical_name")
            };
            dataSpace.Files.Add(dataFile.Name, dataFile);
        }
    }

    private void SyncDbFile(string fileGroup)
    {
        var dataSpace = _model.FileGroups[fileGroup];
        var databasePath = Path.GetDirectoryName(_model.FileGroups.Values.FirstOrDefault(ds => ds.Files.Count > 0)
            ?.Files?.Values.FirstOrDefault()
            ?.PhysicalName);
        var fileName = fileGroup;
        var found = FoundDbFile(dataSpace, fileName);
        if (!found)
        {
            if (_model.FileGroups.Values.Any(ds => ds.Files.ContainsKey(fileName)))
                fileName += "Model";
            found = FoundDbFile(dataSpace, fileName);
        }

        if (!found)
            CreateDbFile(fileGroup, fileName, databasePath, dataSpace);
    }

    private static bool FoundDbFile(DbFileGroup dataSpace, string fileName)
    {
        var found = dataSpace.Files.ContainsKey(fileName);
        if (!found)
            found = dataSpace.Files.Values.Any(d =>
                string.Equals(d.Name, fileName, StringComparison.CurrentCultureIgnoreCase));
        return found;
    }

    private void CreateDbFile(string fileGroup, string fileName, string databasePath, DbFileGroup dataSpace)
    {
        var fileData = new DbFileGroup.DataFile
        { Name = fileName, PhysicalName = databasePath + "\\" + fileName + ".ndf" };
        AddMessage(null, null, "Create File " + fileData.Name);
        dataSpace.Files.Add(fileData.Name, fileData);
        var ddl = DDLGenerator.AddFile(fileData.Name, fileData.PhysicalName, fileGroup);

        AddToCommandList(null, ddl, false);
        DoSqlCommand(null, ddl, "", "44.1");
    }
}
