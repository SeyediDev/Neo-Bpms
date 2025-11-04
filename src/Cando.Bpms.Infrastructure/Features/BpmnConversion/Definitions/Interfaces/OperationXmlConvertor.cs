using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Services;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class OperationXmlConvertor
{
    internal static void Export(dynamic node, Operation operation)
    {
        var element = node.operation();
        BaseElementXmlConvertor.Export(element, operation);
        element.name = operation.Name;
        element.implementationRef = operation.implementationRef?.ToString();
        var inMessageRef = element.inMessageRef();
        BaseElementXmlConvertor.ExportRef(inMessageRef, operation.inMessageRef);
        if (operation.outMessageRef != null)
        {
            var outMessageRef = element.outMessageRef();
            BaseElementXmlConvertor.ExportRef(outMessageRef, operation.outMessageRef);
        }
        foreach (var error in operation.errorRef ?? Enumerable.Empty<Error>())
        {
            var errorRef = element.errorRef();
            BaseElementXmlConvertor.ExportRef(errorRef, error);
        }
    }

    internal static void Import(ElasticObject interfaceElement, Interface @interface)
    {
        foreach (var element in interfaceElement.GetElements("operation") ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id");
            var opname = element.GetString("name");
            var operation = @interface.operations.Values?.FirstOrDefault(o => o.Id == id);
            if (operation == null)
            {
                if (!@interface.operations.TryGetValue(opname, out operation))
                {
                    operation = new Operation(@interface, id, opname, null, null, null);
                    @interface.operations ??= [];
                    @interface.operations.Add(opname, operation);
                }
            }
            BaseElementXmlConvertor.Import(element, operation);
            operation.Name = opname;
            //TODO implementationRef
            operation.inMessageRef = ProjectDefinition.Project.GetMessage(element.GetElementRef("inMessageRef"));
            operation.outMessageRef = ProjectDefinition.Project.GetMessage(element.GetElementRef("outMessageRef"));
            operation.errorRef = null;
            foreach (var errorRefElement in element.GetElements("errorRef") ?? Enumerable.Empty<ElasticObject>())
            {
                operation.errorRef ??= [];
                var errorId = errorRefElement?.InternalValue?.ToString();
                var error = ProjectDefinition.Project.GetError(errorId);
                if (error == null)
                {
                    error = new Error(null, errorId, errorId, errorId, null);
                    ProjectDefinition.Project.AddError(error);
                }
                operation.errorRef.Add(error);
            }
        }
    }
}
