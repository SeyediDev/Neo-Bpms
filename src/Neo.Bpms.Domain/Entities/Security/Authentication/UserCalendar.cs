using Neo.Bpms.Domain.Entities.Cmmn.Storage;

namespace Neo.Bpms.Domain.Entities.Security.Authentication;

/// <summary>
///     EntityType that represents one specific user claim
/// </summary>
public class UserCalendarItem : UserCalendarItem<string>
{
}

/// <summary>
///     EntityType that represents one specific user claim
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class UserCalendarItem<TKey> : CategorizedLogicModel
{
    /// <summary>
    ///     User Id for the user who owns this login
    /// </summary>
    public virtual TKey UserId { get; set; }

    public virtual DateTime Date { get; set; }
    public virtual long RepeatPeriod { get; set; }
    public virtual long StartTime { get; set; }
    public virtual long EndTime { get; set; }

    public override string CategorizedKey(string name)
    {
        return name switch
        {
            nameof(UserId) => UserId.ToString(),
            _ => throw new CategorizedKeyException(name, name),
        };
    }
}