namespace Neo.Bpms.Domain;
/// <summary>
/// for old injection
/// </summary>
public class DependencyInjectionHolder
{
    private static readonly Lazy<DependencyInjectionHolder> Lazy = new(() => new DependencyInjectionHolder());

    public static DependencyInjectionHolder Instance => Lazy.Value;
    public ILogger Logger { get; set; }
    public IBuiltInFunctionFinder BuiltInFunctionFinder { get; set; }

    public IAccessServices AccessServices { get; set; }

    /// <summary>
    /// for sso. if inject this then active sso
    /// </summary>
    public ISsoIntegrator SsoIntegrator { get; set; }
    public IBpmsEngine BpmsEngine { get; set; }
    
    private IConfiguration _configuration;
    public IConfiguration Configuration
    {
        get => _configuration;
        set
        {
            if (_configuration != null)
            {
                throw new Exception("AppSettings is already initialized.");
            }

            _configuration = value;
        }
    }
}
