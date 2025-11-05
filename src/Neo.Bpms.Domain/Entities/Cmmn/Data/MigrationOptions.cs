using Neo.Bpms.Domain.Entities.Cmmn.Data.Resources;
using Display = System.ComponentModel.DataAnnotations.DisplayAttribute;
namespace Neo.Bpms.Domain.Entities.Cmmn.Data;

public class MigrationOptions
{
    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(DoRenameUndefinedTables))]
    public bool DoRenameUndefinedTables { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(DoDropUndefinedTables))]
    public bool DoDropUndefinedTables { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(SetEnumerationItems))]
    public bool SetEnumerationItems { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(RenameUndefinedField))]
    public bool RenameUndefinedField { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(DropUndefinedField))]
    public bool DropUndefinedField { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(CheckIndexes))]
    public bool CheckIndexes { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(CreateForeignKeyIndex))]
    public bool CreateForeignKeyIndex { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(DropForeignKeyIndex))]
    public bool DropForeignKeyIndex { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(RebuildIndex))]
    public bool RebuildIndex { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(DropExtraIndex))]
    public bool DropExtraIndex { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(CheckForeignKey))]
    public bool CheckForeignKey { get; set; }


    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(Clean))]
    public bool Clean { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(CheckCollation))]
    public bool CheckCollation { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(RenameUndefinedViews))]
    public bool RenameUndefinedViews { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(DropUndefinedViews))]
    public bool DropUndefinedViews { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(SyncMetaData))]
    public bool SyncMetaData { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(SpecificNamespace))]
    public string SpecificNamespace { get; set; }

    [Display(ResourceType = typeof(DataTexts),
        Name = nameof(SpecificEntity))]
    public string SpecificEntity { get; set; }

    [Display(ResourceType = typeof(DataTexts),
    Name = nameof(SyncFileGroups))]
    public bool SyncFileGroups { get; set; }
    
    [Display(ResourceType = typeof(DataTexts),
    Name = nameof(RefreshView))]
    public bool RefreshView { get; set; }

    public MigrationOptions()
    {
        SetDefaults();
    }

    private void SetDefaults()
    {
        Clean = false;
        RenameUndefinedViews = false;
        DropUndefinedViews = false;

        DoRenameUndefinedTables = false;
        DoDropUndefinedTables = false;
        SetEnumerationItems = true;
        RenameUndefinedField = false;
        DropUndefinedField = false;
        DropExtraIndex = false;
        CheckIndexes = true;
        CreateForeignKeyIndex = false;
        DropForeignKeyIndex = false;
        RebuildIndex = false;
        CheckForeignKey = true;
        CheckCollation = true;
        SyncMetaData = false;
        SyncFileGroups = false;
        RefreshView = false;
        SpecificNamespace = null;
        SpecificEntity = null;
    }
}
