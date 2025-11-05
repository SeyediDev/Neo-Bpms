using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataAssociation;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds Data Output Association
    /// </summary>
    /// <typeparam name="TDataStoreEntity">DataStoreEntity</typeparam>
    /// <param name="names">The names.</param>
    /// <returns></returns>
    protected DataAssociation AddDataOutputAssociation<TDataStoreEntity>(params string[] names)
    {
        var targetRefName = typeof(TDataStoreEntity).Name + "DataStore";
        return AddDataOutputAssociation(targetRefName, null, names);
    }

    /// <summary>
    /// Adds Data Output Association
    /// </summary>
    /// <param name="targetName">The target name</param>
    /// <param name="formula">The formula</param>
    /// <param name="names">The names</param>
    /// <returns></returns>
    protected DataAssociation AddDataOutputAssociation(string targetName, string formula, params string[] names)
    {
        if (_currentFlowNode is not IDataOutputAssociationContainer container)
            throw new Exception($"element {_currentFlowNode.Name} can not have DataOutputAssociation");
        container.dataOutputAssociations ??= [];

        var targetRef = container.GetItemAwareElement("", targetName, false)
                            ?? ProjectDefinition.Project.GetDataStoreByName(targetName);
        var id = _currentFlowNode.Id + ".DOA." + (container.dataOutputAssociations.Count + 1) + "." + targetName;
        if (targetRef == null)
        {
            throw new Exception("Can not find target reference " + targetName + " in process " + definitions.Id);
        }

        var dataOutputAssociation = new DataOutputAssociation(container, id, targetRef)
        {
            sourceRef = [.. names.Select(name =>
            {
                var dataOutputContainer = container as IDataOutputContainer;
                if (dataOutputContainer == null)
                {
                    if (container is IIoSpecificationContainer ioSpecificationContainer)
                    {
                        ioSpecificationContainer.ioSpecification ??=
                                new InputOutputSpecification(ioSpecificationContainer, _currentFlowNode?.Id + ".ioSpec");
                        dataOutputContainer = ioSpecificationContainer.ioSpecification;
                    }
                }

                if (dataOutputContainer == null)
                    throw new Exception(
                        $"element Type {_currentFlowNode.flowElementType} not dataOutputs. element {_currentFlowNode.Name}");
                var dataOutput =
                    new DataOutput(dataOutputContainer, (dataOutputContainer as BaseElement)?.Id + ".DataOutput." + name,
                        name, null, false);
                dataOutputContainer.dataOutputs ??= [];
                dataOutputContainer.dataOutputs.Add(dataOutput);
                return (IItemAwareElement) dataOutput;
            })]
        };
        if (formula != null)
        {
            dataOutputAssociation.transformation =
                new FormalExpression(dataOutputAssociation.Id + "_transform", Parser.ParseTree(formula));
        }

        currentDataAssociation = dataOutputAssociation;
        container.dataOutputAssociations.Add(dataOutputAssociation);
        return dataOutputAssociation;
    }

    private DataAssociation currentDataAssociation;

    /// <summary>
    /// Adds Data Input Association
    /// </summary>
    /// <param name="targetName">The target name</param>
    /// <param name="transformationFormula">The transformation Formula</param>
    /// <param name="sourceNames">The source names</param>
    /// <returns></returns>
    protected DataAssociation AddDataInputAssociation(string targetName,
        string transformationFormula, params string[] sourceNames)
    {
        if (_currentFlowNode is not IDataInputAssociationContainer container)
            throw new Exception($"element {_currentFlowNode.Name} can not have DataInputAssociation");
        container.dataInputAssociations ??= [];
        var id = _currentFlowNode.Id + ".DIA." + (container.dataInputAssociations.Count + 1) + "." + targetName;
        var dataInputAssociation = new DataInputAssociation(container, id, null, null, null);
        currentDataAssociation = dataInputAssociation;
        container.dataInputAssociations.Add(dataInputAssociation);

        dataInputAssociation.targetRef = container.GetItemAwareElement("", targetName, false)
                                                    ?? ProjectDefinition.Project.GetDataStoreByName(targetName);
        if (dataInputAssociation.targetRef == null)
        {
            var dataInputContainer = container as IDataInputContainer;
            if (dataInputContainer == null)
            {
                if (container is IIoSpecificationContainer ioSpecificationContainer)
                {
                    ioSpecificationContainer.ioSpecification ??=
                            new InputOutputSpecification(ioSpecificationContainer, _currentFlowNode?.Id + ".ioSpec");
                    dataInputContainer = ioSpecificationContainer.ioSpecification;
                }
            }

            if (dataInputContainer == null)
                throw new Exception(
                    $"element Type {_currentFlowNode.flowElementType} not dataInputs. element {_currentFlowNode.Name}");
            var dataInput = new DataInput(dataInputContainer,
                (dataInputContainer as BaseElement)?.Id + ".DataInput." + targetName, targetName, null, false);
            dataInputContainer.dataInputs ??= [];
            dataInputContainer.dataInputs.Add(dataInput);
            dataInputAssociation.targetRef = dataInput;
        }

        dataInputAssociation.sourceRef =
        [
            .. sourceNames.Select(name => container.GetItemAwareElement("", name, true) ??
            ProjectDefinition.Project.GetDataStoreByName(name)),
        ];

        if (!string.IsNullOrEmpty(transformationFormula))
            dataInputAssociation.transformation =
                new FormalExpression(id + "_transform", Parser.ParseTree(transformationFormula));
        return dataInputAssociation;
    }

    /// <summary>
    /// Adds Assignment
    /// </summary>
    /// <param name="toFormula">to formula</param>
    /// <param name="fromFormula">from formula</param>
    /// <returns></returns>
    protected bool AddAssignment(string toFormula, string fromFormula)
    {
        if (currentDataAssociation == null) return false;
        currentDataAssociation.assignment ??= [];
        var id = $"{currentDataAssociation.Id}.assignment.{currentDataAssociation.assignment.Count + 1}.{toFormula}";
        var fromExp = new FormalExpression(id + ".from", Parser.ParseTree(fromFormula));
        if (fromExp.Expression == null && !string.IsNullOrEmpty(fromFormula))
            throw new Exception($"Can not parse {fromFormula} in AddAssignment Element {_currentFlowNode.Id}");
        var toExp = new FormalExpression(id + ".to", Parser.ParseTree(toFormula));
        //todo itemAwareElement
        currentDataAssociation.assignment.Add(new Assignment(currentDataAssociation, id, fromExp, toExp));
        return true;
    }
}
