using Neo.Bpms.Domain.Models.Cmmn.Data.Base;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Entities;

public static class EntityConversions
{
    public static ExpressionNode ConvertNames(ExpressionNode input, object param)
    {
        if (input == null) return null;
        var entityOrmUtility = param as EntityConnection;
        var queryDef = entityOrmUtility as QueryUtility;
        var entity = entityOrmUtility?.Entity ?? param as Entity;
        switch (input.NodeType)
        {
            case ExpressionNode.eNodeType.Variable:
                {
                    var variableName = input as VariableNameExpressionNode;
                    if (variableName?.Reference != null) break;
                    var entity1 = entity;
                    if (variableName != null)
                    {
                        if (entity1 != null)
                        {
                            var entityField = entity1.GetField(variableName.Name);
                            var parent = entityField?.ReferenceFields?.FirstOrDefault(r => r.Relationship is ParentEntity);
                            if (parent != null)
                            {
                                var jq = queryDef?.Join(parent.ParentAssociationField.ParentEntity.DestEntity);
                                if (jq != null && parent.ParentAssociationField.ParentEntity.Maps != null)
                                {
                                    if (jq.fieldMappings.Count == 0)
                                        jq.SetMapping(parent.ParentAssociationField);
                                }
                            }
                            if (entityField == null && queryDef?.subQuerys != null)
                            {
                                foreach (var sub in queryDef.subQuerys.Values)
                                {
                                    entityField = sub.Query.Entity.GetField(variableName.Name);
                                    if (entityField != null)
                                    {
                                        entity1 = sub.Query.Entity;
                                        break;
                                    }
                                }
                            }
                            if (entityField != null)
                            {
                                if (entityField.AssociationEntity?.Maps != null && !entityField.NotMapped)
                                {
                                    ExpressionNode exp = null;
                                    foreach (var map in entityField.AssociationEntity.Maps)
                                    {
                                        var field = entity1.GetField(map.SourceField);
                                        if (field == null) continue;
                                        var exp1 = GetDbFieldName(entity1, field, queryDef?.DatabaseName);
                                        if (exp == null)
                                            exp = exp1;
                                        else
                                            exp = new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Addition, exp,
                                                new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Addition,
                                                    new ConstantExpressionNode("_"), exp1, 0), 0);
                                    }
                                    return exp;
                                }
                                if (entityField.Formula == null) //Model.Entities.TVariableTypes.varFormula
                                {
                                    return GetDbFieldName(entity1, entityField, queryDef?.DatabaseName);
                                }
                            }
                        }
                    }
                }
                break;
            case ExpressionNode.eNodeType.PathExpression:
                {
                    var pathExp = input as PathExpressionNode;
                    if (pathExp == null || pathExp.Reference != null) break;
                    if (pathExp.Expression is PathExpressionNode)
                    {
                        pathExp.Expression = ConvertNames(pathExp.Expression, param);
                        var exp = pathExp.Expression as PathExpressionNode;
                        if (exp?.Reference != null)
                            return pathExp.RightExpression.processAndReplace(ConvertNames, exp.Reference);
                    }
                    if (pathExp.Expression is VariableNameExpressionNode &&
                         (pathExp.RightExpression is VariableNameExpressionNode ||
                          pathExp.RightExpression is PathExpressionNode))
                    {
                        var leftExp = pathExp.Expression as VariableNameExpressionNode;
                        var entity1 = entity;
                        var leftEntityField = entity1?.GetField(leftExp?.Name);
                        if (leftExp?.Name == entityOrmUtility?.Parent?.Entity?.Id)
                        {
                            return pathExp.RightExpression.processAndReplace(ConvertNames, entityOrmUtility?.Parent?.Entity);
                        }
                        if ((leftEntityField == null || leftEntityField.NotMapped) && queryDef?.subQuerys != null)
                        {
                            foreach (var sub in queryDef.subQuerys.Values)
                            {
                                leftEntityField = sub.Query.Entity.GetField(leftExp?.Name);
                                if (leftEntityField != null && !leftEntityField.NotMapped)
                                {
                                    //entity1 = sub.Query.entity;
                                    break;
                                }
                            }
                        }
                        if (leftEntityField != null && !leftEntityField.NotMapped)
                        {
                            if (leftEntityField.AssociationEntity?.Maps != null)
                            {
                                QueryUtility.SubQueryDefinition subQueryJoin;
                                if (IsConvertableToLeftOuterJoin(queryDef, leftEntityField, out subQueryJoin) || subQueryJoin != null)
                                {
                                    var jq = subQueryJoin ??
                                                queryDef?.LeftOuterJoin(leftEntityField.AssociationEntity.DestEntity);
                                    if (jq != null && leftEntityField.AssociationEntity.Maps != null)
                                    {
                                        if (jq.fieldMappings.Count == 0)
                                            jq.SetMapping(leftEntityField);
                                        return pathExp.RightExpression.processAndReplace(ConvertNames, jq.Query);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var rightExp = pathExp.RightExpression;
                            Entity entity2 = null;
                            if (entity != null && entity.Id == leftExp.Name)
                                entity2 = entity;
                            else if (leftExp.Reference != null)
                                entity2 = leftExp.Reference as Entity;
                            else if (queryDef?.subQuerys != null)
                            {
                                foreach (var sub in queryDef.subQuerys.Values)
                                {
                                    if (sub.Query.Entity.Id == leftExp.Name)
                                    {
                                        entity2 = sub.Query.Entity;
                                        break;
                                    }
                                }
                            }
                            if (entity2 == null)
                            {
                                var model = ProjectDefinition.Project.GetModel(leftExp.Name);
                                if (model != null)
                                {
                                    if (rightExp is PathExpressionNode rightpathexp)
                                    {
                                        if (rightpathexp.Expression is VariableNameExpressionNode lmEntity)
                                        {
                                            entity2 = ProjectDefinition.Project.GetEntity(model.Id, lmEntity.Name);
                                            rightExp = rightpathexp.RightExpression;
                                        }
                                    }
                                    else
                                    {
                                        entity2 = ProjectDefinition.Project.GetEntity(model.Id, rightExp.toText());
                                        if (entity2 != null)
                                        {
                                            var exp2 = new VariableNameExpressionNode(GetTableDbFullName(entity2, queryDef?.DatabaseName), entity2);
                                            return exp2;
                                        }
                                    }
                                }
                                if (entity2 == null && entity != null)
                                {
                                    entity2 = ProjectDefinition.Project.GetEntity(entity.model.Id, leftExp.Name) ?? ProjectDefinition.Project.GetEntityByDbNameOrKey(leftExp.Name);
                                }
                            }
                            if (entity2 != null)
                                return rightExp.processAndReplace(ConvertNames, entity2);
                        }
                    }
                }
                break;
            case ExpressionNode.eNodeType.FunctionInvocation:
                return ConvertNamesOfFunctions(input as FunctionInvocationExpressionNode, param);
        }
        return input;
    }
    public static ExpressionNode ConvertFormulaFields(ExpressionNode input, object param)
    {
        var entityOrmUtility = param as EntityConnection;
        var entity = entityOrmUtility?.Entity ?? param as Entity;
        switch (input.NodeType)
        {
            case ExpressionNode.eNodeType.PathExpression:
                if (input is PathExpressionNode pathExp)
                {
                    pathExp.Expression = ConvertFormulaFields(pathExp.Expression, param);
                    var left = pathExp.Expression as VariableNameExpressionNode;
                    var query = param as QueryUtility;
                    QueryUtility subQuery = null;
                    var subEntity = entity;
                    if (left != null)
                    {
                        var entityField = entity?.GetField(left.Name);
                        if (entityField?.AssociationEntity?.Maps != null && entityField.AssociationEntity.Maps.All(f => !(entity.GetField(f.SourceField)?.DonSync ?? true)))
                            subQuery = query?.LeftOuterJoin(entityField)?.Query;
                        else
                            subEntity = ProjectDefinition.Project.GetEntityByDbNameOrKey(left.Name) ?? entity;
                    }
                    pathExp.RightExpression = ConvertFormulaFields(pathExp.RightExpression, (object)subQuery ?? subEntity);
                }
                break;
            case ExpressionNode.eNodeType.Variable:
                var variableexp = input as VariableNameExpressionNode;
                var efld = entity?.GetField(variableexp?.Name ?? "");
                if (efld?.AssociationEntity?.Maps?.FirstOrDefault() != null)
                {
                    efld = entity.GetField(efld.AssociationEntity.Maps.FirstOrDefault()?.SourceField);
                }
                if (efld?.Formula != null)
                {
                    if (efld.Formula.FormulaBody != null)
                        return ConvertFormulaFields(efld.Formula.FormulaBody.clone(), param);
                }
                break;
        }
        return input;
    }

    private static bool IsConvertableToLeftOuterJoin(QueryUtility queryDef, EntityField leftefld, out QueryUtility.SubQueryDefinition subQueryJoin)
    {
        var relEn = leftefld.AssociationEntity.DestEntity;
        subQueryJoin = null!;
        while (true)
        {
            if (queryDef == null)
                return false;
            subQueryJoin = queryDef.GetSubQueryJoin(relEn.NamespaceId, relEn.Id, eJoinType.LeftOuterJoin);
            if (subQueryJoin != null)
                return true;
            if (queryDef.Parent == null)
                return true;
            if (queryDef.Parent.Entity.NamespaceId == relEn.NamespaceId && queryDef.Parent.Entity.Id == relEn.Id)
                return false;
            queryDef = queryDef.ParentQuery;
        }
    }

    private static ExpressionNode GetDbFieldName(Entity entity, EntityField field, string databaseName)
    {
        var fieldEntity = entity;
        if (field.IsForParent)
            fieldEntity = field.ReferenceFields
                ?.FirstOrDefault(r => r.Relationship is ParentEntity)?.Relationship.DestEntity;
        var expLeft = new VariableNameExpressionNode(GetTableDbFullName(fieldEntity, databaseName), fieldEntity);
        var expRight = new VariableNameExpressionNode("[" + field.DbFieldName + "]", field);
        var exp = new PathExpressionNode(expLeft, expRight) { Reference = entity };
        return exp;
    }

    public static ExpressionNode GetFunctionArgument(int argIndex, FunctionInvocationExpressionNode funcInvocExp)
    {
        if (funcInvocExp.PositionalParameters == null) return null;
        if (argIndex < 0 || argIndex >= funcInvocExp.PositionalParameters.Count) return null;
        return funcInvocExp.PositionalParameters[argIndex];
    }

    private static ExpressionNode ConvertNamesOfFunctions(FunctionInvocationExpressionNode funcInvocExp, object param)
    {
        var entityOrmUtility = param as EntityConnection;
        var queryDef = entityOrmUtility as QueryUtility;
        var entity = entityOrmUtility?.Entity ?? param as Entity;
        switch (funcInvocExp.FunctionName.ToLower())
        {
            #region Select

            case "groupby": //GroupBy(entities;requestFromula;[where];[group bys];[having];[order bys];[DISTINCT|[ALL]];[TOP])
            case "notexists": //NotExists(entities;[where];[group bys];[having];[order bys];[DISTINCT|[ALL]];[TOP])
            case "exists": //NotExists(entities;[where];[group bys];[having];[order bys];[DISTINCT|[ALL]];[TOP])
            case "select": //Select(entities;requestFromula;[where];[order by];[DISTINCT|[ALL];TOP])
            case "concatrows": //ConcatRows(entities;requestFromula;seperator;[where];[order by];[DISTINCT|[ALL];TOP])
                ConvertSelectFunction(funcInvocExp, param, queryDef?.DatabaseName);
                break;

            #endregion Select

            #region Insert

            case "insert": //Insert(entities;destColumns;values)
                {
                    funcInvocExp.FunctionName = "dbinsert";
                    //var argCount = funcInvocExp.positionalParameters.Count;
                    var entities = new List<Entity>();
                    var positionalParameters = new List<ExpressionNode>();
                    GetEntitiesOfSelect(funcInvocExp, entities, 0);
                    if (entities.Count == 0) return new ConstantExpressionNode(null);
                    var destQueryDef = new QueryUtility(entities[0], "EntityConversion.1");
                    var queryDefIsNew = false;
                    if (queryDef == null)
                    {
                        queryDef = new QueryUtility(entity, "EntityConversion.2");
                        queryDefIsNew = true;
                    }
                    var entitiesItems = new List<ExpressionNode>();
                    var tables = new Dictionary<string, string>();
                    foreach (var e in entities)
                    {
                        var tableName = EntityDbNameManager.GetDbTableName(e);
                        if (!tables.ContainsKey(tableName))
                        {
                            var texp = new VariableNameExpressionNode(tableName, e);
                            entitiesItems.Add(texp);
                            tables.Add(tableName, tableName);
                        }
                    }
                    positionalParameters.Add(new ListExpressionNode(entitiesItems));
                    var destColumns = GetSelectArgument(funcInvocExp, param, entities, destQueryDef, 1);
                    var values = GetSelectArgument(funcInvocExp, param, entities, queryDef, 2);
                    /*
					 * اضافه کردن مقدار دهی خودکار موجودیت فیلدهای 
					خیلی سخت بود
					if (destQueryDef.entity.AutoCalcs!=null && destQueryDef.entity.AutoCalcs.calculations!=null)
					{
						foreach (var ac in destQueryDef.entity.AutoCalcs.calculations)
						{
							if (ac.condition != null) continue;//todo
							if (ac.generationType == AutoCalc.eGenerationType.IfNull )
							{
								//check value is null change it
							}
							var field = destQueryDef.entity.GetField(ac.fieldId);
							if (field == null) continue;
							var vfn = new VariableNameExpressionNode("["+field.DBFieldName+"]");
							if (values is ListExpressionNode)
							{
								var lvalue = values as ListExpressionNode;
								lvalue.items.Add(vfn);
							}
							else if (values is VariableNameExpressionNode)
							{
								var vvalue = values as VariableNameExpressionNode;
								if (vvalue.name != vfn.name)
									values = new ListExpressionNode(values, vfn);
							}
							else if (values is FunctionInvocationExpressionNode)
							{
								var fvalue = values as FunctionInvocationExpressionNode;
								if (fvalue.functionName.ToLower() == "dbselect")
								{
									var arg = fvalue.positionalParameters[2];
								}
							}
						}
					}*/
                    positionalParameters.Add(destColumns);
                    //positionalParameters.Add(new VariableNameExpressionNode(entities.FirstOrDefault()?.KeyFields?.FirstOrDefault()?.DbFieldName ?? "Id"));
                    positionalParameters.Add(values);
                    funcInvocExp.PositionalParameters = positionalParameters;
                    destQueryDef.Release();
                    if (queryDefIsNew)
                        queryDef.Release();
                }
                break;

            #endregion Insert

            #region FirstTI

            case "firstti": //FirstTI(NameSpaceId;EntityId;requestFormula;filter)
                {
                    funcInvocExp.FunctionName = "dbselect";
                    var positionalParameters =
                        new List<ExpressionNode> { new ConstantExpressionNode(null), new ConstantExpressionNode("1") };
                    //DISTINCT, ALL
                    //TOP 1
                    var argCount = funcInvocExp.PositionalParameters.Count;
                    var namespaceId = "";
                    Entity referEntity = null;
                    var dbTableName = "";
                    ExpressionNode filterExp = null;
                    for (var argIndex = 0; argIndex < argCount; argIndex++)
                    {
                        switch (argIndex + (argCount == 4 ? 0 : 1))
                        {
                            case 0: //namespaceId
                                {
                                    funcInvocExp.PositionalParameters[argIndex] =
                                        funcInvocExp.PositionalParameters[argIndex].processAndReplace(ConvertNames, param);
                                    var vExp = funcInvocExp.PositionalParameters[argIndex] as VariableNameExpressionNode;
                                    namespaceId = vExp?.Name;
                                }
                                break;
                            case 1: //entityId
                                {
                                    funcInvocExp.PositionalParameters[argIndex] =
                                        funcInvocExp.PositionalParameters[argIndex].processAndReplace(ConvertNames, param);
                                    var vExp = funcInvocExp.PositionalParameters[argIndex] as VariableNameExpressionNode;
                                    var entityId = vExp?.Name;
                                    referEntity = ProjectDefinition.Project.GetEntity(namespaceId, entityId);
                                    if (referEntity != null)
                                        dbTableName = GetTableDbFullName(referEntity, queryDef?.DatabaseName);
                                }
                                break;
                            case 2: //requestFormula
                                {
                                    if (referEntity != null)
                                        funcInvocExp.PositionalParameters[argIndex] = funcInvocExp.PositionalParameters[argIndex]
                                            .processAndReplace(ConvertNames, referEntity);
                                    funcInvocExp.PositionalParameters[argIndex] =
                                        funcInvocExp.PositionalParameters[argIndex].processAndReplace(ConvertNames, param);
                                    positionalParameters.Add(funcInvocExp.PositionalParameters[argIndex]);
                                }
                                break;
                            case 3: //filter
                                {
                                    if (referEntity != null)
                                        funcInvocExp.PositionalParameters[argIndex] = funcInvocExp.PositionalParameters[argIndex]
                                            .processAndReplace(ConvertNames, referEntity);
                                    funcInvocExp.PositionalParameters[argIndex] =
                                        funcInvocExp.PositionalParameters[argIndex].processAndReplace(ConvertNames, param);
                                    filterExp = funcInvocExp.PositionalParameters[argIndex];
                                }
                                break;
                        }
                    }
                    if (referEntity != null)
                    {
                        positionalParameters.Add(new ConstantExpressionNode(dbTableName));
                        positionalParameters.Add(new ConstantExpressionNode(null));
                        var rel = referEntity.Associations.FirstOrDefault(re => re.DestEntity.NamespaceId == entity?.NamespaceId && re.DestEntity.Id == entity?.Id);
                        if (rel?.Maps != null)
                        {
                            var T = GetTableDbFullName(entity, queryDef?.DatabaseName);
                            //var fromList = new ListExpressionNode(new ConstantExpressionNode(T), new ConstantExpressionNode(DBTableName));
                            foreach (var map in rel.Maps)
                            {
                                var d = entity?.GetField(map.DestField);
                                var s = referEntity.GetField(map.SourceField);
                                var fExp = new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.Equal,
                                    new ConstantExpressionNode(T + ".[" + d?.DbFieldName + "]"),
                                    new ConstantExpressionNode(dbTableName + ".[" + s.DbFieldName + "]"), 0);
                                filterExp = filterExp == null
                                    ? fExp
                                    : new LogicalExpressionNode(LogicalExpressionNode.eLogicalExpressionType.And,
                                        fExp, filterExp, 0);
                            }
                        }
                    }
                    positionalParameters.Add(filterExp ?? new ConstantExpressionNode(""));
                    funcInvocExp.PositionalParameters = positionalParameters;
                }
                break;

            #endregion FirstTI

            case "avg":
            case "checksum_agg":
            case "count":
            case "count_big":
            case "grouping":
            case "max":
            case "min":
            case "sum":
            case "stdev":
            case "stdevp":
            case "var":
            case "varp":
            case "strany": //strany
            case "autoincrement":
            case "newguid":
            case "suser_sname":
            case "internalunion":
            case "isnull":
            case "converttolong":
            case "converttodouble":
            case "persiandate":
            case "persiandatetime":
            case "persianyear":
            case "persianmonth":
            case "persianday":
            case "persianyearmonth":
            case "getdate":
                break;
            default:
                return EvalIfConstant(funcInvocExp, param);
        }
        return funcInvocExp;
    }

    private static void ConvertSelectFunction(FunctionInvocationExpressionNode funcInvocExp, object param, string databaseName)
    {
        var i_arg_requestFormula = 1;
        var i_arg_where = 2;
        var i_arg_groupby = 3;
        var i_arg_having = 4;
        var i_arg_orderby = 5;
        var i_arg_DISTINCT = 6;
        var i_arg_TOP = 7;
        var i_arg_seperator = -1;

        switch (funcInvocExp.FunctionName.ToLower())
        {
            case "groupby": //GroupBy(entities;requestFromula;[where];[group bys];[having];[order bys];[DISTINCT|[ALL]];[TOP])
                i_arg_requestFormula = 1;
                i_arg_where = 2;
                i_arg_groupby = 3;
                i_arg_having = 4;
                i_arg_orderby = 5;
                i_arg_DISTINCT = 6;
                i_arg_TOP = 7;
                funcInvocExp.FunctionName = "dbselect";
                break;
            case "select": //Select(entities;requestFromula;[where];[order by];[DISTINCT|[ALL];TOP])
                i_arg_requestFormula = 1;
                i_arg_where = 2;
                i_arg_groupby = -1;
                i_arg_having = -1;
                i_arg_orderby = 3;
                i_arg_DISTINCT = 4;
                i_arg_TOP = 5;
                funcInvocExp.FunctionName = "dbselect";
                break;
            case "concatrows": //Select(entities;requestFromula;[where];[order by];[DISTINCT|[ALL];TOP])
                i_arg_requestFormula = 1;
                i_arg_seperator = 2;
                i_arg_where = 3;
                i_arg_groupby = -1;
                i_arg_having = -1;
                i_arg_orderby = 4;
                i_arg_DISTINCT = 5;
                i_arg_TOP = 6;
                funcInvocExp.FunctionName = "dbconcatrows";
                break;
            case "notexists":
            case "exists":
                i_arg_requestFormula = -1;
                i_arg_where = 1;
                i_arg_groupby = 2;
                i_arg_having = 3;
                i_arg_orderby = 4;
                i_arg_DISTINCT = 5;
                i_arg_TOP = 6;
                funcInvocExp.FunctionName = "db" + funcInvocExp.FunctionName;
                break;
        }
        var argCount = funcInvocExp.PositionalParameters.Count;
        var entities = new List<Entity>();
        QueryUtility selectQueryDef = null;
        ExpressionNode reqestExp = null,
            whereExp = null,
            groupbyExp = null,
            havingExp = null,
            orderbyExp = null,
            distinctExp = null,
            topExp = null,
            seperatorExp = null;
        for (var argIndex = 0; argIndex < argCount; argIndex++)
        {
            if (argIndex == 0)
            {
                //entities
                GetEntitiesOfSelect(funcInvocExp, entities, argIndex);
                if (entities.Count > 0)
                {
                    selectQueryDef = new QueryUtility(entities[0], "EntityConversion.3") { Parent = param as QueryUtility };
                }
            }
            else if (argIndex == i_arg_requestFormula)
                reqestExp = GetSelectArgument(funcInvocExp, param, entities, selectQueryDef, argIndex);
            else if (argIndex == i_arg_seperator)
                seperatorExp = GetSelectArgument(funcInvocExp, param, entities, selectQueryDef, argIndex);
            else if (argIndex == i_arg_where)
                whereExp = GetSelectArgument(funcInvocExp, param, entities, selectQueryDef, argIndex);
            else if (argIndex == i_arg_groupby)
                groupbyExp = GetSelectArgument(funcInvocExp, param, entities, selectQueryDef, argIndex);
            else if (argIndex == i_arg_having)
                havingExp = GetSelectArgument(funcInvocExp, param, entities, selectQueryDef, argIndex);
            else if (argIndex == i_arg_orderby)
                orderbyExp = GetSelectArgument(funcInvocExp, param, entities, selectQueryDef, argIndex);
            else if (argIndex == i_arg_DISTINCT)
                distinctExp = GetSelectArgument(funcInvocExp, param, entities, selectQueryDef, argIndex);
            else if (argIndex == i_arg_TOP)
                topExp = GetSelectArgument(funcInvocExp, param, entities, selectQueryDef, argIndex);
        }
        var positionalParameters = new List<ExpressionNode>
        {
            distinctExp ?? new ConstantExpressionNode(null),
            topExp ?? new ConstantExpressionNode(null),
            reqestExp ?? new VariableNameExpressionNode("*")
        };
        if (i_arg_seperator > 0)
            positionalParameters.Add(seperatorExp ?? new ConstantExpressionNode(null));
        var entitiesItems = new List<ExpressionNode>();
        var tables = new Dictionary<string, string>();
        foreach (var e in entities)
        {
            var tableName = GetTableDbFullName(e, databaseName);
            if (tables.ContainsKey(tableName)) continue;
            entitiesItems.Add(new VariableNameExpressionNode(tableName, e));
            tables.Add(tableName, tableName);
        }
        positionalParameters.Add(new ListExpressionNode(entitiesItems));
        var joins = new NeoStringBuilder();
        if (selectQueryDef?.subQuerys != null)
        {
            selectQueryDef.PreProcessQuery(null);
            selectQueryDef.GenerateQuery_WriteJoins(ref joins, ref tables);
        }
        positionalParameters.Add(new VariableNameExpressionNode(joins.ToString()));
        positionalParameters.Add(whereExp ?? new ConstantExpressionNode(null));
        positionalParameters.Add(groupbyExp ?? new ConstantExpressionNode(null));
        positionalParameters.Add(havingExp ?? new ConstantExpressionNode(null));
        positionalParameters.Add(orderbyExp ?? new ConstantExpressionNode(null));
        funcInvocExp.PositionalParameters = positionalParameters;
        selectQueryDef.Release();
    }

    private static ExpressionNode EvalIfConstant(FunctionInvocationExpressionNode funcInvocExp, object param)
    {
        var localParameters = (param as ApplyUtility)?.LocalParameters
                                     ?? (param as QueryUtility)?.LocalParameters
                                     ?? [];

        if (funcInvocExp.PositionalParameters != null)
        {
            for (var i = 0; i < funcInvocExp.PositionalParameters.Count; i++)
            {
                var fparam = funcInvocExp.PositionalParameters[i];
                if (fparam is FunctionInvocationExpressionNode)
                {
                    funcInvocExp.PositionalParameters[i] =
                        ConvertNamesOfFunctions(fparam as FunctionInvocationExpressionNode, param);
                }
            }
            foreach (var fparam in funcInvocExp.PositionalParameters)
            {
                if (fparam is not ConstantExpressionNode)
                {
                    if (fparam is VariableNameExpressionNode)
                    {
                        if ((fparam as VariableNameExpressionNode).Name == "user")
                            continue;
                    }
                    return funcInvocExp;
                }
            }
        }
        else
        {
            var obj = funcInvocExp.Eval(null, localParameters);
            if (obj != null)
                return new ConstantExpressionNode(obj);
            return funcInvocExp;
        }
        return new ConstantExpressionNode(funcInvocExp.Eval(null, localParameters));
    }

    private static ExpressionNode GetSelectArgument(FunctionInvocationExpressionNode funcInvocExp, object param,
        List<Entity> entities, QueryUtility selectQueryDef, int argIndex)
    {
        //if (selectQueryDef.Parent != null)
        //	funcInvocExp.positionalParameters[argIndex] = funcInvocExp.positionalParameters[argIndex]
        //		.processAndReplace(convertNames, selectQueryDef.Parent);
        foreach (var e in entities)
        {
            if (e.NamespaceId + e.Id == selectQueryDef.Entity.NamespaceId + selectQueryDef.Entity.Id)
                funcInvocExp.PositionalParameters[argIndex] = funcInvocExp.PositionalParameters[argIndex]
                    .processAndReplace(ConvertNames, selectQueryDef);
            else
                funcInvocExp.PositionalParameters[argIndex] =
                    funcInvocExp.PositionalParameters[argIndex].processAndReplace(ConvertNames, e);
        }
        funcInvocExp.PositionalParameters[argIndex] =
            funcInvocExp.PositionalParameters[argIndex].processAndReplace(ConvertNames, param);
        return funcInvocExp.PositionalParameters[argIndex];
    }

    private static void GetEntitiesOfSelect(FunctionInvocationExpressionNode funcInvocExp,
        List<Entity> entities, int argIndex)
    {
        //funcInvocExp.positionalParameters[argIndex] = funcInvocExp.positionalParameters[argIndex].processAndReplace(convertNames, param);
        var arg = funcInvocExp.PositionalParameters[argIndex];
        Entity sentity;
        if (arg is ListExpressionNode)
        {
            var lExp = arg as ListExpressionNode;
            if (lExp.Items == null) return;
            foreach (var item in lExp.Items)
            {
                sentity = ProjectDefinition.Project.GetEntityByDbNameOrKey(item.toText());
                if (sentity != null) entities.Add(sentity);
            }
        }
        else if (arg is FunctionInvocationExpressionNode)
        {
            var lExp = arg as FunctionInvocationExpressionNode;
            if (lExp.FunctionName.ToLower() == "list" && lExp.PositionalParameters != null)
            {
                foreach (var item in lExp.PositionalParameters)
                {
                    sentity = ProjectDefinition.Project.GetEntityByDbNameOrKey(item.toText());
                    if (sentity != null) entities.Add(sentity);
                }
            }
        }
        else
        {
            sentity = ProjectDefinition.Project.GetEntityByDbNameOrKey(arg.toText());
            if (sentity != null) entities.Add(sentity);
        }
    }
    public static string GetTableDbFullName(Entity entity, string databaseName)
    {
        if (!string.IsNullOrEmpty(databaseName))
            return $"[{databaseName}].{EntityDbNameManager.GetTableDbFullName(entity)}";
        return EntityDbNameManager.GetTableDbFullName(entity);
    }
}
