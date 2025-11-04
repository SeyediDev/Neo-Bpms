namespace Neo.Bpms.Engine.DDL;

abstract partial class DDLManager
{
    private void ReadDatabaseDefinitions()
    {
        ReadDataSpaces();
        if (_pleaseStop) return;
        ReadDatabaseFiles();
        if (_pleaseStop) return;
        ReadPartitionFunctionsItems();
        ReadPartitionSchemes();
        if (_pleaseStop) return;
        ReadShemas();
        if (_pleaseStop) return;
        ReadViews();
        if (_pleaseStop) return;
        ReadTables();
        if (_pleaseStop) return;
        ReadTablesDataSpaces();
        if (_pleaseStop) return;
        ReadTablesFields();
        if (_pleaseStop) return;
        ReadTablesFieldsDefaultConstraint();
        if (_pleaseStop) return;
        if (Options.Clean)
            CleanDatabaseTablesAndViews();
        if (_pleaseStop) return;
        ReadTablesTriggers();
        if (_pleaseStop) return;
        ReadIndexes();
        if (_pleaseStop) return;
        ReadForeignKeys();
    }
}
