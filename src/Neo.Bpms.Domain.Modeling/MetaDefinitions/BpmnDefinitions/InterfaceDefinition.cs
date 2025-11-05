using Neo.Bpms.Domain.Entities.Bpmn.Core.Services;
using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class BpmnDefinitionsDefinition
{
    private Interface _currentInterface;

    protected Interface SelectInterface<T>() where T : new()
    {
        var t = new T();
        var tType = t.GetType();
        return SelectInterface(tType.Name);
    }

    /// <summary>
    /// Selects Interface
    /// </summary>
    /// <param name="id">The interface identifier</param>
    /// <returns></returns>
    private Interface SelectInterface(string id)
    {
        var ifc = definitions.GetRootElement(id) as Interface
                    ?? ProjectDefinition.Project.GetInterface(id);
        if (ifc != null)
            _currentInterface = ifc;
        else
            throw new Exception("Can not select interface " + id + " in process " + definitions.Id);
        return ifc;
    }

    /// <summary>
    /// Add an operation to bpmn interface operations
    /// </summary>
    /// <param name="ifc"></param>
    /// <param name="operationDefinition"></param>
    public static void AddOperation(Interface ifc, ServiceOperationDefinition operationDefinition)
    {
        var inputStructure = operationDefinition.InParamsEntity;
        var outputStructure = operationDefinition.OutParamsEntity;
        var id = Operation.GenerateId(ifc.Name, operationDefinition.Name);
        var inMessageRef = AddMessage2RootElements(inputStructure, $"{id}.InMsg", $"{operationDefinition.Name} input");
        var outMessageRef = AddMessage2RootElements(outputStructure, $"{id}.OutMsg", $"{operationDefinition.Name} output");
        var op = new Operation(ifc, id, operationDefinition.Name, inMessageRef, outMessageRef, operationDefinition.Name);
        ifc.operations.Add(operationDefinition.Name, op);
    }

    /// <summary>
    /// انتخاب کردن عملیات
    /// </summary>
    /// <param name="name">نام</param>
    /// 
    /// <returns></returns>
    protected Operation SelectOperation(string name)
    {
        _currentInterface ??= ProjectDefinition.Project.Interfaces.FirstOrDefault();
        if (_currentInterface?.operations.ContainsKey(name) != true)
            throw new Exception($"operation {name} not defined in interface {_currentInterface?.Name}");
        return _currentInterface?.operations[name];
    }
}
