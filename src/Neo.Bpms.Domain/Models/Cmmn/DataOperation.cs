namespace Neo.Bpms.Domain.Models.Cmmn;

public class DataOperation : BaseModelClass
{
    public enum eFlags
    {
        None = 0,
        DontSaveActionLog = 0x1000,
        DontCheckTransaction = 0x2000,
        DontReadInputDoc = 0x4000,
    }
    public enum eOperationType
    {
        Update = 1,
        Destructor = 2,
        Constructor = 3,
        //Read = 21,
        UpdateAll = 22,
        //Method = 23,
        //StaticMethod = 24,
    }
    public DataOperation(Entity entity, string Id, string name, eOperationType eOperationType) :
    base(entity, Id, name)
    {
        type = eOperationType;
    }
    public DataOperation()
    {
    }

    private eFlags flags;
    public eOperationType type;
    public void SetFlag(eFlags flag)
    {
        flags = (eFlags)((int)flags | (int)flag);
    }
    public int outputStateId;
    /*
    static ExceptionInformation defaultOperationFailedException = getDefaultOperationFailedException();
		static ExceptionInformation getDefaultOperationFailedException()
		{
			var de = new ExceptionInformation(eDataEngineException.DefaultOperationFailed) { ErrorText = "خطا در انجام عملیات.", enErrorText="Error in operation." };
			return de;
		}*/
    #region auto calcs
    public AutoCalcList AutoCalcs { get; set; }
    public void InitAutoCalcs()
    {
        AutoCalcs ??= new AutoCalcList();
    }
    #endregion auto calcs
    #region validations
    public List<Validation> Validations { get; set; }
    public void AddValidation(Validation validation)
    {
        Validations ??= [];

        Validations.Add(validation);
    }
    #endregion validations
    #region triggers
    public List<Trigger> triggers;
    public void addTrigger(Trigger trigger)
    {
        triggers ??= [];

        triggers.Add(trigger);
    }
    #endregion triggers
    #region data events
    //public List<DataEvent> dataEvents = null;
    //public void addDataEvent(DataEvent dataEvent)
    //{
    //	if (dataEvents == null)
    //		dataEvents = new List<DataEvent>();
    //	dataEvents.Add(dataEvent);
    //}
    #endregion data events

    public void Clear()
    {
        outputStateId = 0;
        AutoCalcs = null;
        Validations = null;
    }
}
