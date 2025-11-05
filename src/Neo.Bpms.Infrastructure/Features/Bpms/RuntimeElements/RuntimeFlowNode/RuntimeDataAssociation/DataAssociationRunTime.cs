using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataAssociation;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;

namespace Neo.Bpms.Infrastructure.Features.Bpms;
public abstract partial class FlowNodeRunTime
{
    internal void RunDataAssociation(DataAssociation dataAssociation,
        FlowNodeInstance ai, LocalParameters inputParams, LocalParameters outputParams,
        ProcessInstance pi = null)
    {
        if (dataAssociation?.targetRef == null) return;
        var targetName = dataAssociation.targetRef.Name;
        if ((pi ??= ai?.pi) == null) return;
        if (dataAssociation.transformation != null)
        {
            RunTransformation(dataAssociation, pi, ai, inputParams, outputParams);
        }
        else if (dataAssociation.assignment != null && dataAssociation.assignment.Count > 0)
        {
            AuditTrace($"Run data association {targetName ?? dataAssociation.Id}", pi, ai);
            var src = dataAssociation.sourceRef?.FirstOrDefault();
            var srcName = src?.Name;
            var srcValue = src != null ? GetValue(inputParams, pi, ai, srcName, null) : null;
            switch (srcValue)
            {
                case ICollection<ElasticObject> elasticObjects:
                    {
                        var outList = new List<ElasticObject>();
                        foreach (var elasticObject in elasticObjects)
                        {
                            var outData = new ElasticObject(targetName);
                            var inputParam = new LocalParameters();
                            outList.Add(outData);
                            inputParam.Set(inputParams);
                            inputParam.AddOrUpdate(elasticObject.Attributes);
                            AuditTrace($"Run data association record {targetName ?? dataAssociation.Id} => {outList.Count}",
                                pi, ai);
                            foreach (var assignment in dataAssociation.assignment)
                                RunAssignment(assignment, inputParams, ai, pi, outData);
                        }

                        SetValue(dataAssociation, targetName, pi, ai, outList, null);
                        outputParams?.AddOrUpdate(targetName, outList);
                        if (dataAssociation is DataOutputAssociation)
                        {
                            foreach (var outData in outList)
                            {
                                SaveDataStoreRecord(dataAssociation, outData, ai);
                            }
                        }
                    }
                    break;
                case ICollection<object> objects:
                    {
                        var outList = new List<ElasticObject>();
                        foreach (var Object in objects)
                        {
                            if (Object is not Dictionary<string, object> item) continue;
                            var outData = new ElasticObject(targetName);
                            var inputParam = new LocalParameters();
                            outList.Add(outData);
                            inputParam.Set(inputParams);
                            inputParam.Set(item);
                            AuditTrace($"Run data association record {targetName ?? dataAssociation.Id} => {outList.Count}",
                                pi, ai);
                            foreach (var assignment in dataAssociation.assignment)
                                RunAssignment(assignment, inputParam, ai, pi, outData);
                        }

                        SetValue(dataAssociation, targetName, pi, ai, outList, null);
                        outputParams?.AddOrUpdate(targetName, outList);
                        if (dataAssociation is DataOutputAssociation)
                        {
                            foreach (var outData in outList)
                            {
                                SaveDataStoreRecord(dataAssociation, outData, ai);
                            }
                        }
                    }
                    break;
                case Dictionary<string, object> dicObject:
                    {
                        var outData = new ElasticObject(targetName);
                        var inputParam = new LocalParameters();
                        inputParam.Set(inputParams);
                        inputParam.Set(dicObject);
                        AuditTrace($"Run data association record {targetName ?? dataAssociation.Id}", pi, ai);
                        foreach (var assignment in dataAssociation.assignment)
                            RunAssignment(assignment, inputParam, ai, pi, outData);
                        SetValue(dataAssociation, targetName, pi, ai, outData, null);
                        outputParams?.AddOrUpdate(targetName, outData);
                        if (dataAssociation is DataOutputAssociation)
                            SaveDataStoreRecord(dataAssociation, outData, ai);
                    }
                    break;
                default:
                    {
                        if (srcValue is not ElasticObject elasticObject)
                        {
                            if (srcValue?.GetType().IsClass ?? false)
                                elasticObject = new ElasticObject(targetName, srcValue);
                            else
                            {
                                if (dataAssociation is DataInputAssociation &&
                                    pi.GetData(targetName, out var elasticObj,
                                        [.. dataAssociation.assignment.Select(FromAssignmentToString)])
                                    && elasticObj is ElasticObject eObject)
                                    elasticObject = eObject;
                                else
                                    elasticObject = new ElasticObject();
                            }
                        }

                        var outData = new ElasticObject(targetName);
                        var inputParam = new LocalParameters();
                        inputParam.Set(inputParams);
                        inputParam.AddOrUpdate(elasticObject.Attributes);
                        AuditTrace($"Run data association record {targetName ?? dataAssociation.Id}", pi, ai);
                        foreach (var assignment in dataAssociation.assignment)
                            RunAssignment(assignment, inputParam, ai, pi, outData);
                        SetValue(dataAssociation, targetName, pi, ai, outData, null);
                        outputParams?.AddOrUpdate(targetName, outData);
                        if (dataAssociation is DataOutputAssociation)
                            SaveDataStoreRecord(dataAssociation, outData, ai);
                    }
                    break;
            }
        }
        else
        {
            AuditTrace($"Run data association {targetName ?? dataAssociation.Id}", pi, ai);
            TransferAssociation(dataAssociation, pi, ai, inputParams, outputParams);
            if (dataAssociation is DataOutputAssociation)
            {
                if (GetValue(inputParams, pi, ai, targetName, null) is ElasticObject dsData)
                    SaveDataStoreRecord(dataAssociation, dsData, ai);
            }
        }

        inputParams.Set(outputParams);
    }

    private void SaveDataStoreRecord(DataAssociation da, ElasticObject outVal, FlowNodeInstance ai)
    {
        if (da?.targetRef == null) return;
        var dataStoreReference = da.targetRef as DataStoreReference;
        var lc = new LocalParameters(ai.AuditTrail.User);
        if (!DataStorage.UpsertDataStore(dataStoreReference, outVal, ai, lc))
            LogDebug($"can not save data store {da.TargetRef?.Id ?? da.Id} record {outVal}", ai.pi, ai);
    }

    private void SetValue(DataAssociation dataAssociation, string targetName,
        ProcessInstance pi, FlowNodeInstance ai, object val, LocalParameters localParams)
    {
        switch (dataAssociation.TargetDataFlow?.type)
        {
            case DataFlowElement.eDataFlowElementTypes.DataObject:
            case DataFlowElement.eDataFlowElementTypes.DataObjectRef:
            case DataFlowElement.eDataFlowElementTypes.DataStoreRef:
                pi.SetData(targetName, val);
                break;
            default:
                switch (dataAssociation.TargetDataElement?.type)
                {
                    case DataElement.eDataElementTypes.DataOutput:
                        pi.SetData(targetName, val);
                        break;
                    case DataElement.eDataElementTypes.Property:
                        if (ProcessVersion.definition.properties?.Any(p => p.Name == targetName) ?? false)
                            pi.SetData(targetName, val);
                        else
                            ai.SetData(targetName, val);
                        break;
                    //case DataElement.eDataElementTypes.DataInput:
                    default:
                        if (ai != null)
                            ai.SetData(targetName, val);
                        else
                            pi.SetData(targetName, val);
                        localParams?.AddOrUpdate(targetName, val);
                        break;
                }

                break;
        }
    }

    private static string FromAssignmentToString(Assignment a)
    {
        if (a.from is FormalExpression expression)
            return expression.Expression?.ExpressionString;
        return a.from?.ToString();
    }

    private void TransferAssociation(DataAssociation dataAssociation,
        ProcessInstance pi, FlowNodeInstance ai, LocalParameters inputParams, LocalParameters outputParams)
    {
        if (dataAssociation.targetRef == null)
        {
            Logger.LogCritical("null targetRef {0}", dataAssociation.Id);
            return;
        }

        var targetName = dataAssociation.targetRef.Name;
        if (dataAssociation.sourceRef.Count > 1)
        {
            var obj = new ElasticObject();
            foreach (var source in dataAssociation.sourceRef)
            {
                var name = source.Name;
                obj.SetField(name, GetValue(null, pi, ai, name, null));
            }

            SetValue(dataAssociation, targetName, pi, ai, obj, inputParams);
            outputParams?.AddOrUpdate(targetName, obj);
        }
        else
        {
            var source = dataAssociation.sourceRef.FirstOrDefault();
            if (source == null)
            {
                Logger.LogCritical("no source {0}", dataAssociation.Id);
                return;
            }

            var name = source.Name;
            var val = GetValue(null, pi, ai, name, null);
            SetValue(dataAssociation, targetName, pi, ai, val, inputParams);
            outputParams?.AddOrUpdate(targetName, val);
        }
    }

    private void RunAssignment(Assignment assignment, LocalParameters inputParams,
        FlowNodeInstance ai, ProcessInstance pi, ElasticObject dataObject)
    {
        if (assignment.from is not FormalExpression from || assignment.to is not FormalExpression to) return;
        var expressionInInstance = new ExpressionInInstance(pi, ai, inputParams, from.Expression);
        var value = expressionInInstance.EvalExpression($"Run assignment {to.Expression?.ExpressionString} =");
        if (to.Expression?.Root is VariableNameExpressionNode vtoExp)
        {
            dataObject.SetField(vtoExp.Name, value);
        }
        else
            to.Expression?.SetVal(pi.Data,
                value); //path and index and other formula variable setting is supported only for process level data-aware elements(not activity instance properties)
    }

    private void RunTransformation(DataAssociation dataAssociation,
        ProcessInstance pi, FlowNodeInstance ai,
        LocalParameters inputParams, LocalParameters outputParams)
    {
        if (dataAssociation.targetRef == null)
        {
            Logger.LogCritical("null targetRef {0}", dataAssociation.Id);
            return;
        }

        if (!FetchTransformationValue(dataAssociation, pi, ai, inputParams, out var value))
            return;
        SetValue(dataAssociation, dataAssociation.targetRef.Name, pi, ai, value, inputParams);
        outputParams?.AddOrUpdate(dataAssociation.targetRef.Name, value);
    }

    internal static bool FetchTransformationValue(DataAssociation dataAssociation, ProcessInstance pi, FlowNodeInstance ai,
        LocalParameters inputParams, out object value)
    {
        var exp = dataAssociation.transformation;
        if (exp != null)
        {
            var expressionInInstance = new ExpressionInInstance(pi, ai, inputParams, exp.Expression);
            value = expressionInInstance.EvalExpression($"Run transformation {dataAssociation.targetRef.Name}");
        }
        else value = null;
        return true;
    }

    internal bool IsValid(InputSet inputSet, LocalParameters inputData)
    {
        //object value;
        if (inputSet.dataInputRefs == null || inputSet.dataInputRefs.Count == 0) return true;
        if (inputData == null) return false;
        //if (inputSet.dataInputRefs.Count == 1)
        //	return inputData != null;
        //var el = inputData as ElasticObject;
        //if (el == null) return false;
        foreach (var input in inputSet.dataInputRefs)
        {
            if (input.isOptional) continue;
            if (!inputData.ContainsKey(input.dataInput.Name))
                //				if (!el.GetField(input.dataInput.Name, out value))
                return false;
        }

        return true;
    }

    private static object GetValue(LocalParameters data, ProcessInstance pi, FlowNodeInstance ai, string name,
        IList<string> fields)
    {
        if (data != null && data.TryGetValue(name, out var value))
            return value;
        if (ai != null && ai.GetData(name, out value, fields))
            return value;
        pi.GetData(name, out value, fields);
        return value;
    }
}
