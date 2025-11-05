using Neo.Bpms.Domain.Models.Bpmn.Core.Services;

namespace Neo.Bpms.UI.MVC.ViewModels.ElementViewModels;

public class InterfaceViewModel
{
    public InterfaceViewModel()
    {

    }

    public InterfaceViewModel(Interface i)
    {
        id = i.Id;
        name = i.Name;
        implementationRef = i.implementationRef;
        operations = i.operations?.Values.Select(o => new OperationViewModel(o)).ToList();
    }

    public string id { get; set; }
    public string name { get; set; }
    public string implementationRef { get; set; }
    public List<OperationViewModel> operations { get; set; }

    public Interface ToInterface()
    {
        Interface @interface = new(null, id, name, null)
        {
            implementationRef = implementationRef,
        };
        @interface.operations = operations?.Select(o => o.ToOperation(@interface)).ToDictionary(o => o.Name, o => o);
        return @interface;
    }
}