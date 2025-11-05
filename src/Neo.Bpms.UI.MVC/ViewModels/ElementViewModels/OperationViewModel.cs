using Neo.Bpms.Domain.Models.Bpmn.Core.Services;

namespace Neo.Bpms.UI.MVC.ViewModels.ElementViewModels;

public class OperationViewModel
{
    public string interfaceId { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string implementationRef { get; set; }
    public string inMessageRef { get; set; }
    public string outMessageRef { get; set; }
    public List<ErrorViewModel> errorRef { get; set; }

    public OperationViewModel()
    {
    }

    public OperationViewModel(Operation o)
    {
        interfaceId = o.Interface.Id;
        id = o.Id;
        name = o.Name;
        inMessageRef = o.inMessageRef?.Id;
        outMessageRef = o.outMessageRef?.Id;
        implementationRef = o.implementationRef;
        errorRef = o.errorRef?.Select(e => new ErrorViewModel(e)).ToList();
    }

    public Operation ToOperation(Interface @interface)
    {
        foreach (ErrorViewModel error in errorRef ?? Enumerable.Empty<ErrorViewModel>())
        {
            if (string.IsNullOrEmpty(error.id))
                error.id = "Error." + error.errorCode + "." + error.name + Guid.NewGuid().ToString("N");
            ProjectDefinition.Project.AddError(error.ToError(), false);
        }
        Operation operation = ProjectDefinition.Project.GetOperation(id);
        if (operation == null)
        {
            operation = new Operation(@interface, id, name,
            ProjectDefinition.Project.GetMessage(inMessageRef),
            ProjectDefinition.Project.GetMessage(outMessageRef),
                implementationRef)
            {
                errorRef = errorRef?.Select(error =>
                ProjectDefinition.Project.GetError(error.id)).ToList()
            };
        }
        else
        {
            operation.inMessageRef = ProjectDefinition.Project.GetMessage(inMessageRef);
            operation.outMessageRef = ProjectDefinition.Project.GetMessage(outMessageRef);
            operation.implementationRef = implementationRef;
            operation.errorRef = errorRef?.Select(error =>
            ProjectDefinition.Project.GetError(error.id)).ToList();
        }
        return operation;
    }
}