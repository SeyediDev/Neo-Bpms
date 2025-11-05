using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class BpmnDefinitionsDefinition : BaseModelingDefinition
{
    public virtual BpmnDefinitions DefineAll()
    {
        if (!Identify())
        {
            return null;
        }

        DefineCollaboration();

        DefineEscalation();
        DefineMessage();
        DefineSignal();
        DefineError();

        DefineInterface();
        DefineResource();
        DefineDataStore();
        DefinePartnerEntity();
        DefinePartnerRole();
        DefineCategory();
        DefineItemDefinition();
        DefineEndPoint();
        DefineChoreography();

        DefineProcess();
        return definitions;
    }

    protected BpmnDefinitions definitions;
    protected new BaseElement currentBaseElement;

    /// <summary>
    /// Define 
    /// </summary>
    /// <returns></returns>
    protected abstract bool Identify();
    protected bool Identify(string name)
    {
        definitions = CreateBpmnDefinitions(GetType().Name, name);
        currentBaseElement = definitions;
        return true;
    }

    public static string ExporterVersion = "1.0";

    protected static BpmnDefinitions CreateBpmnDefinitions(string id, string name)
    {
        BpmnDefinitions definitions = new(id, name,
            "www.bmi.ir/xmlns/e-bpmn2.xsd", "NeoBpms", ExporterVersion);
        return definitions;
    }

    protected virtual void DefineCollaboration()
    {
    }

    protected virtual void DefineProcess()
    {
    }

    protected virtual void DefineInterface()
    {
    }

    protected virtual void DefineEscalation()
    {
    }

    protected virtual void DefineMessage()
    {
    }

    protected virtual void DefineSignal()
    {
    }

    protected virtual void DefineError()
    {
    }

    protected virtual void DefineResource()
    {
    }
    protected virtual void DefineDataStore()
    {
    }

    protected virtual void DefinePartnerEntity()
    {
    }

    protected virtual void DefinePartnerRole()
    {
    }

    protected virtual void DefineCategory()
    {
    }

    protected virtual void DefineItemDefinition()
    {
    }

    protected virtual void DefineEndPoint()
    {
    }

    protected virtual void DefineChoreography()
    {
    }
}
