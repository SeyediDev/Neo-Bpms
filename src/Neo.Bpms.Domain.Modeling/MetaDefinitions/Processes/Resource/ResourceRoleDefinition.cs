using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Resources;
using Neo.Bpms.Domain.Entities.Bpmn.Extensions.ResourceRoles;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    private ResourceRole _currentResourceRole;

    /// <summary>
    /// Adds the process manager. for detail descriptions please refer to the model.
    /// </summary>
    /// <param name="resourceId">The resource identifier.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    protected bool AddProcessManager(string resourceId, string name = null)
    {
        if (process == null) return false;
        _currentResource = ProjectDefinition.Project.GetResource(resourceId);
        if (_currentResource == null)
            throw new Exception($"can not find resource {resourceId} for process {process.Id}");
        var id = $"Process.{process.Id}.Manager.{resourceId}";
        _currentResourceRole = new ProcessManagerResource(process, id, name, _currentResource, null);
        process.AddResourceRole(_currentResourceRole);
        return true;
    }

    /// <summary>
    /// Adds Resource Parameter Binding
    /// </summary>
    /// <param name="type">The resource type</param>
    /// <param name="name">The name</param>
    /// <param name="expression">The expression</param>
    protected void AddResourceParameterBinding(ResourceParameter.eType type, string name, string expression)
    {
        if (_currentResourceRole == null || _currentResource == null)
            throw new Exception("can not used this function in current place.");
        var resourceParameter = _currentResource.GetParameter(type, name) ?? throw new Exception($"can not find resource parameter {type}.{name}");
        _currentResourceRole.resourceParameterBindings ??= [];
        var id = $"{_currentResourceRole?.Id}.{_currentResourceRole.Id}.{type}.{name}";
        _currentResourceRole.resourceParameterBindings.Add(new ResourceParameterBinding(
            _currentResourceRole, id, resourceParameter,
            new FormalExpression($"{id}.Expression", Parser.ParseTree(expression))
        ));
    }

    /// <summary>
    /// Sets Resource Assignment Expression
    /// </summary>
    /// <param name="resourceAssignmentExpression">The resource assignment expression</param>
    protected void SetResourceAssignmentExpression(string resourceAssignmentExpression)
    {
        if (_currentResourceRole == null) return;
        var aId = $"{_currentResourceRole.Id}.Assignment";
        _currentResourceRole.resourceAssignmentExpression =
            new ResourceAssignmentExpression(_currentResourceRole, aId,
                new FormalExpression($"{aId}.Expression",
                    Parser.ParseTree(resourceAssignmentExpression)
                ));
    }
}
