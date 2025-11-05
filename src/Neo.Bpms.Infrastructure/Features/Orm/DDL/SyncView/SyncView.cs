using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Domain.Models.Cmmn.Entities;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadViews()
    {
        _model.Views.Set([]);
        var sql = DDLGenerator.GetViews();
        var dt = Select(sql, "10.1.1.7");
        if (dt == null) return;
        foreach (var item in dt)
        {
            var dbView = new DbView(item.GetString("SchemaName"), item.GetString("Name"))
            {
                ViewDefinition = item.GetString("VIEW_DEFINITION")
            };
            _model.Views.Add(dbView.FullName, dbView);
        }
    }

    private void SyncView(Entity entity)
    {
        if (!entity.IsView) return;
        var viewName = EntityDbNameManager.GetTableDbFullName(entity);
        if (!_model.Views.TryGetValue(viewName, out var dbView) || dbView == null)
        {
            if (!_model.Views.TryGetValue(EntityDbNameManager.GetTableDbDeletedFullName(entity), out dbView) || dbView == null)
            {
                if (!string.IsNullOrEmpty(entity.OldDbTableNameMap))
                    _model.Views.TryGetValue(EntityDbNameManager.GetTableOldDbFullName(entity), out dbView);
            }

            if (dbView != null)
                RenameDbView(entity, dbView, viewName);
            else
            {
                CreateDbView(entity);
                return;
            }
        }

        SyncDbView(entity, dbView, viewName);
    }

    private void RenameOrDropExtraViews(IList<ModelNamespace> models)
    {
        if (!Options.RenameUndefinedViews) return;
        AddMessage(null, null, "حذف و یا تغییر نام ویو های اضافی پایگاه داده");
        var dropViews = new Dictionary<string, DbView>();
        foreach (var dbView in _model.Views.Enumerable())
        {
            if (_pleaseStop) return;
            if (!CheckNeededToSync(dbView.Schema, dbView.Name))
                continue;
            var entity = GetEntityWithDbName(models, dbView.Name);
            if (entity == null || !entity.IsView)
                dropViews.Add(dbView.FullName, dbView);
            if (Options.SpecificEntity != null)
                break;
        }

        foreach (var dbView in dropViews.Values)
        {
            if (_pleaseStop) return;
            if (Options.DropUndefinedViews)
                DropSqlView(null, dbView);
            else if (!dbView.Name.StartsWith("del_"))
                RenameDbView(null, dbView, $"del_{dbView.Name}");
        }
    }

    private void SyncDbView(Entity entity, DbView dbView, string viewName)
    {
        var viewDefinition = Trim(GetViewDefinition(entity));
        dbView.ViewDefinition = Trim(dbView.ViewDefinition);
        if (viewDefinition.Equals(dbView.ViewDefinition, StringComparison.OrdinalIgnoreCase))
        {
            if (Options.RefreshView)
                RefreshView(entity, viewName);
            return;
        }

        DropSqlView(entity, dbView);
        CreateDbView(entity);
    }

    private void CreateDbView(Entity entity)
    {
        var viewName = EntityDbNameManager.GetDbTableName(entity);
        var viewDefinition = GetViewDefinition(entity);
        AddToCommandList(entity, viewDefinition, false);
        DoSqlCommand(entity, viewDefinition, "", "10.2.1");
        var dbView = new DbView(DbNameManager.GetDbSchemaName(entity), viewName)
        {
            ViewDefinition = viewDefinition
        };
        _model.Views.Add(dbView.FullName, dbView);
    }

    private void RefreshView(Entity entity, string viewName)
    {
        var ddl = $"EXECUTE sp_refreshview '{viewName}'";
        DoSqlCommand(entity, ddl, "", "10.2.4");
    }

    private static string GetViewDefinition(Entity entity)
    {
        var viewName = EntityDbNameManager.GetTableDbFullName(entity);
        var viewSetting = entity.ViewSetting;
        var query = viewSetting.Query;
        /*todo
        if (!viewSetting.IsDbQuery)
        {
            var baseEntity = ProjectDefinition.Project.GetEntity(entity.model.Id, viewSetting.entityName);
            if (baseEntity == null) return null;
            var q = new QueryUtility(baseEntity)
            {
                bDistinct = viewSetting.bDistinct,
                fields = viewSetting.fields,
                filters = viewSetting.filters,
                havingFilters = viewSetting.havingFilters,
                orderbys = viewSetting.orderbys,
                groupbys = viewSetting.groupbys,
                formulaInfos = viewSetting.formulaInfos
            };
            //viewSetting.filters;
            //viewSetting.havingFilters;
            if (viewSetting.subs != null)
            {
                foreach (var sub in viewSetting.subs.Values)
                {
                    if (q.subQuerys == null) q.subQuerys = new Dictionary<string, QueryUtility.SubQueryDefinition>();
                    var s = new QueryUtility.SubQueryDefinition
                    {
                        joinType = sub.joinType,
                        fieldMappings = sub.fieldMappings,
                        Query = CreateQueryFromSetting(entity, sub.viewSetting)
                    };
                    if( s.Query!=null )
                    {
                        var key = QueryUtility.GetSubTableKey(s.joinType, s.Query.Entity.model.Id, s.Query.Entity.Id);
                        if( !q.subQuerys.ContainsKey(key) )
                            q.subQuerys.Add(key, s);
                    }
                }
            }
        }*/
        return $"CREATE VIEW {viewName} AS {query}";
    }
    private static string GetFullViewName(Entity entity, string viewName)
        => $"[{DbNameManager.GetDbSchemaName(entity)}].[{viewName}]";
    private void RenameDbView(Entity entity, DbView dbView, string newViewName)
    {
        var ddl = DDLGenerator.RenameView(dbView.FullName, GetFullViewName(entity, newViewName));
        AddToCommandList(entity, ddl, true);
        if (!DoSqlCommand(entity, ddl, "", "10.2.2"))
            return;
        _model.Tables.Remove(dbView.FullName);
        dbView.Name = newViewName;
        _model.Tables.Add(dbView.FullName, dbView);
    }

    private void DropSqlView(Entity entity, DbView dbView)
    {
        var ddl = $"DROP VIEW {dbView.FullName}";
        AddToCommandList(entity, ddl, false);
        DoSqlCommand(entity, ddl, "", "10.2.3");
        _model.Views.Remove(dbView.FullName);
    }

    private static string Trim(string str)
    {
        var trim = "";
        var oldIdWhiteSpace = false;
        foreach (var c in str)
        {
            if (char.IsWhiteSpace(c) || c == '\r' || c == '\n' || c == '\t')
            {
                if (!oldIdWhiteSpace)
                    trim += ' ';
                oldIdWhiteSpace = true;
            }
            else
            {
                trim += c;
                oldIdWhiteSpace = false;
            }
        }

        return trim.Trim();
    }
}
