using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;
using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.VuerdModels;

namespace Neo.Bpms.UI.MVC.Controllers.MetaDesign;

public class ErdController : BpmsController
{
    private const long GridColumnWidth = 500;

    // GET
    public IActionResult Index()
    {
        IdentityUser user = GetUser();
        if (!user.IsAdmin) throw new UnauthorizedAccessException();
        ViewBag.ContainerClass = "container-fluid";
        return View();
    }

    public JsonResult All(string namespaceId)
    {
        IdentityUser user = GetUser();
        if (!user.IsAdmin) throw new UnauthorizedAccessException();

        VuerdModel model = new() { canvas = { databaseName = ProjectDefinition.Project.Name } };
        HashSet<string> seenEntities = [];
        GridColumn[] grid = new GridColumn[] { new(), new(), new(), new() };
        long l = 0;
        long zIndex = 1;
        List<Entity> sortedDictionary = ProjectDefinition.Project.Namespaces[namespaceId].GetEntities()
            .Select(e => e.Value).ToList();
        foreach (Entity entity in sortedDictionary)
        {
            FillErdModel(model, entity, -1, ref l, ref zIndex, seenEntities, grid);
        }

        return Json(model);
    }

    public JsonResult Entity(string namespaceId, string entityId, int depth = 2)
    {
        IdentityUser user = GetUser();
        if (!user.IsAdmin) throw new UnauthorizedAccessException();

        VuerdModel model = new() { canvas = { databaseName = ProjectDefinition.Project.Name } };
        long l = 0;
        long zIndex = 1;
        FillErdModel(model, ProjectDefinition.Project.GetEntity(namespaceId, entityId), depth, ref l,
            ref zIndex, null, null);
        return Json(model);
    }

    private void FillErdModel(VuerdModel model, Entity rootEntity, int depth, ref long tableLeft, ref long zIndex,
        ISet<string> seenEntities = null, GridColumn[] grid = null)
    {
        grid ??= new GridColumn[] { new(), new(), new(), new() };
        seenEntities ??= new HashSet<string>();
        if (seenEntities.Contains(rootEntity.Id) || depth == 0)
            return;
        seenEntities.Add(rootEntity.Id);

        model.table.tables.Add(ToTable(rootEntity, ref tableLeft, grid,
            Math.Pow(model.canvas.zoomLevel, -1), zIndex));

        foreach (Association association in rootEntity.Associations)
        {
            if (seenEntities.Contains(association.DestEntity.Id) || depth != 1)
            {
                model.relationship.relationships.Add(new RelationshipItem()
                {
                    id = association.Id,
                    start = new RelationshipPoint
                    {
                        columnIds = [.. association.Maps.Select(m => m.SourceField)],
                        direction = Direction.right,
                        tableId = association.SourceEntity.Id,
                        // x = tableLeft,
                        // y = tableTop
                    },
                    end = new RelationshipPoint
                    {
                        columnIds = [.. association.Maps.Select(m => m.DestField)],
                        direction = Direction.left,
                        tableId = association.DestEntity.Id,
                        // x = tableLeft,
                        // y = tableTop
                    },
                    relationshipType = RelationshipType.One
                });
            }

            zIndex++;
            FillErdModel(model, association.DestEntity, depth - 1, ref tableLeft, ref zIndex, seenEntities, grid);
        }
    }

    private static TableItem ToTable(Entity entity, ref long tableLeft, GridColumn[] grid, double coefficient,
        long zIndex)
    {
        long gridIndex = tableLeft / GridColumnWidth;
        TableItem table = new()
        {
            id = entity.Id,
            name = entity.EnName,
            comment = entity.Name,
            ui = new TableUi
            {
                left = tableLeft,
                top = grid[gridIndex].Top,
                zIndex = zIndex
            }
        };
        foreach (EntityField field in entity.entityFields.Values.OrderBy(f => f.FieldType))
        {
            if (field.FieldType == TVariableTypes.Association)
                continue;
            table.columns.Add(new Column
            {
                name = field.Id,
                comment = field.Name,
                id = field.Id,
                dataType = field.GetTypeName(),
                option = new Option()
                {
                    autoIncrement = field.IsAutoIncrement(),
                    notNull = field.Required,
                    // primaryKey = field.
                },
                ui = new ColumnUi()
                {
                    fk = field.IsForeignParam()
                }
            });
        }

        double tableHeight = table.columns.Count * 25 * coefficient + 200;
        if (tableHeight > grid[gridIndex].LevelHeight)
            grid[gridIndex].LevelHeight = tableHeight;
        tableLeft += GridColumnWidth;
        if (tableLeft > 1500)
        {
            tableLeft = 0;
            foreach (GridColumn gridColumn in grid)
            {
                if (gridColumn.Top < 3500)
                {
                    gridColumn.Top += gridColumn.LevelHeight;
                    gridColumn.LevelHeight = 400;
                }
            }
        }

        return table;
    }

    class GridColumn
    {
        public double Top { get; set; }
        public double LevelHeight { get; set; }
    }
}