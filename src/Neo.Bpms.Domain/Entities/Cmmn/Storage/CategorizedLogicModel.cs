namespace Neo.Bpms.Domain.Entities.Cmmn.Storage;

public interface ICategorizedLogicModel
{
    string CategorizedKey(string name);
}

public abstract class CategorizedLogicModel : LogicModel, ICategorizedLogicModel
{
    public abstract string CategorizedKey(string name);
}

public class CategorizedKeyException(string message, string paramName) : ArgumentException(message, paramName)
{
}