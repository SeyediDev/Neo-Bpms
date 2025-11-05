namespace Neo.Bpms.Domain.Features.MetaDefinitions.Projects;

/// <summary>
/// Base class to define project and its details.
/// There must be a single project definition to contain all business definitions
/// for the project and it must be sub class of this object.
/// A Project defines the entities, processes for the project and also the main menu
/// definition of the project.
/// </summary>
public abstract class ProjectMetaDefinition : BaseModelingDefinition
{
    public Dictionary<string, ModelDefinition> Namespaces { get; set; } = [];

    /// <summary>
    /// Identifies this instance.
    /// </summary>
    /// <returns></returns>
    public abstract ProjectContext Identify();

    public abstract void DefineNamespaceNames();

    public void LoadBPMNDefinitions()
    {
        DefineBPMNDefinitions();
        DefineProcesses();
    }

    protected virtual void DefineProcesses()
    {
    }

    public virtual void DefineBPMNDefinitions()
    {
    }

    /// <summary>
    /// Defines the project.
    /// </summary>
    /// <param name="customerName">Name of the customer.</param>
    /// <param name="projectName">Name of the project.</param>
    /// <param name="projectCode">The project code.</param>
    /// <param name="startDate">The start date.</param>
    /// <param name="supportStartDate">The support start date.</param>
    /// <param name="fileMethod"></param>
    /// <param name="defaultCalendar">shamsi/miladi</param>
    /// <param name="defaultCollation">SQL Server Collation</param>
    /// <param name="defaultCulture">e.g. 'fa', 'en', 'en-US'</param>
    /// <param name="captchaInLoginEnabled">to validate captcha on login form</param>
    /// <param name="hasDesignFeatures"></param>
    /// <param name="notifyInLogin"></param>
    /// <returns></returns>
    protected ProjectContext DefineProject(string customerName, string projectName,
        string projectCode, string startDate, string supportStartDate, string fileMethod,
        string defaultCalendar = "shamsi", string defaultCollation = "Persian_100_CI_AI",
        string defaultCulture = "fa", bool captchaInLoginEnabled = true, bool hasDesignFeatures = false,
        bool notifyInLogin = false)
    {
        ProjectDefinition.Project = new ProjectContext(projectName)
        {
            CustomerName = customerName,
            //ProjectName = projectName,
            ProjectCode = projectCode,
            StartDate = startDate,
            SupportStartDate = supportStartDate,
            FileMethod = fileMethod,
            DefaultCalendar = defaultCalendar,
            DefaultCollation = defaultCollation,
            DefaultCulture = defaultCulture,
            CaptchaInLoginEnabled = captchaInLoginEnabled,
            HasDesignFeatures = hasDesignFeatures,
            NotifyInLogin = notifyInLogin
        };
        currentBaseElement = ProjectDefinition.Project;
        return ProjectDefinition.Project;
    }
    /// <summary>
    /// Adds the namespace.
    /// </summary>
    /// <returns></returns>
    protected bool AddNamespace<TModelDefinition>()
        where TModelDefinition : ModelDefinition
    {
        return AddNamespace(typeof(TModelDefinition));
    }
    /// <summary>
    /// Adds the namespace.
    /// </summary>
    /// <param name="namespaceModelDefinitionType">Type of the namespace model definition.</param>
    /// <returns></returns>
    protected bool AddNamespace(Type namespaceModelDefinitionType)
    {
        if (!namespaceModelDefinitionType.IsSubclassOf(typeof(ModelDefinition))) return false;
        Type[] types = [];
        var cons = namespaceModelDefinitionType.GetConstructor(types);
        object[] parameters = [];
        if (cons?.Invoke(parameters) is not ModelDefinition nsDefinition) return false;
        Namespaces.Add(namespaceModelDefinitionType.ToString(), nsDefinition);
        return true;
    }

    /// <summary>
    /// Defines Process
    /// </summary>
    /// <typeparam name="T">Type of the bpmn root element definition.</typeparam>
    /// <returns></returns>
    protected void DefineBpmnDefinitions<T>() where T : BpmnDefinitionsDefinition, new()
    {
        var t = new T();
        var rootElementDefinitionType = t.GetType();
        if (!rootElementDefinitionType.IsSubclassOf(typeof(BpmnDefinitionsDefinition))) return;
        Type[] types = [];
        var cons = rootElementDefinitionType.GetConstructor(types);
        object[] parameters = [];
        if (cons?.Invoke(parameters) is not BpmnDefinitionsDefinition rootElementDefinition) return;
        rootElementDefinition.DefineAll();
    }

    /// <summary>
    /// Defines Process
    /// </summary>
    /// <typeparam name="T">Process</typeparam>
    /// <param name="isActiveLog">if set to <c>true</c> activate log for process</param>
    /// <returns></returns>
    protected bool DefineProcess<T>(bool isActiveLog = false) where T : ProcessDefinition, new()
    {
        var process = BpmnDefinitionsDefinition.DefineOneProcess<T>(isActiveLog);
        return process != null;
    }
}
