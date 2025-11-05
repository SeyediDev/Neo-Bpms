using Newtonsoft.Json.Converters;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.VuerdModels;

public class VuerdModel
{
    public Canvas canvas { get; set; } = new();
    public Table table { get; set; } = new();
    public Memo memo { get; set; } = new();
    public Relationship relationship { get; set; } = new();
}

public class Canvas
{
    public string version { get; set; } = "2.2.7";
    public double width { get; set; } = 3000;
    public double height { get; set; } = 7000;
    public double scrollTop { get; set; } = 0;
    public double scrollLeft { get; set; } = 0;
    public double zoomLevel { get; set; } = 1;
    public Show show { get; set; } = new();
    public string database { get; set; } = "MSSQL";
    public string databaseName { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public CanvasType canvasType { get; set; } = CanvasType.ERD;

    public string language { get; set; } = "C#";
    public string tableCase { get; set; } = "pascalCase";
    public string columnCase { get; set; } = "pascalCase";
    public Setting setting { get; set; } = new();
}

public class Show
{
    public bool tableComment { get; set; } = true;
    public bool columnComment { get; set; } = true;
    public bool columnDataType { get; set; } = true;
    public bool columnDefault { get; set; } = false;
    public bool columnAutoIncrement { get; set; } = false;
    public bool columnPrimaryKey { get; set; } = true;
    public bool columnUnique { get; set; } = false;
    public bool columnNotNull { get; set; } = false;
    public bool relationship { get; set; } = true;
}

public class Setting
{
    public bool relationshipDataTypeSync { get; set; } = true;

    public string[] columnOrder { get; set; } =
    {
        "columnName",
        "columnComment",
        "columnDataType",
        // "columnNotNull",
        // "columnUnique",
        // "columnAutoIncrement",
        // "columnDefault",
    };
}

public class Table
{
    public List<TableItem> tables { get; set; } = [];
    public object[] indexes { get; set; } = { };
}

public class TableItem
{
    public string name { get; set; }
    public string comment { get; set; } = "";
    public List<Column> columns { get; set; } = [];
    public TableUi ui { get; set; }
    public string id { get; set; }
}

public class TableUi
{
    public bool active { get; set; } = false;
    public double left { get; set; }
    public double top { get; set; }
    public long zIndex { get; set; } = 1;
    public int widthName { get; set; } = 60;
    public int widthComment { get; set; } = 60;
}

public class Column
{
    public string id { get; set; }
    public string name { get; set; }
    public string comment { get; set; } = "";
    public string dataType { get; set; } = "";
    public string @default { get; set; } = "";
    public Option option { get; set; }
    public ColumnUi ui { get; set; }
}

public class Option
{
    public bool autoIncrement { get; set; }
    public bool primaryKey { get; set; }
    public bool unique { get; set; }
    public bool notNull { get; set; }
}

public class ColumnUi
{
    public bool active { get; set; } = false;
    public bool pk { get; set; }
    public bool fk { get; set; }
    public bool pfk { get; set; }
    public int widthName { get; set; } = 60;
    public int widthComment { get; set; } = 60;
    public int widthDataType { get; set; } = 60;
    public int widthDefault { get; set; } = 60;
}

public class Memo
{
    public object[] memos { get; set; } = { };
}

public class Relationship
{
    public List<RelationshipItem> relationships { get; set; } = [];
}

public class RelationshipItem
{
    public bool identification { get; set; }
    public RelationshipPoint start { get; set; }
    public RelationshipPoint end { get; set; }
    public string id { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public RelationshipType relationshipType { get; set; }
}

public class RelationshipPoint
{
    public string tableId { get; set; }
    public List<string> columnIds { get; set; } = [];
    public float x { get; set; }
    public float y { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public Direction direction { get; set; }
}

public enum RelationshipType
{
    ZeroOneN,
    ZeroOne,
    ZeroN,
    OneOnly,
    OneN,
    One,
    N
}

public enum Direction
{
    left,
    right,
    top,
    bottom
}

public enum CanvasType
{
    ERD,
    SQL,
    Grid,
    GeneratorCode,
    Visualization
}
