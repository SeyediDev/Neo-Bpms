namespace Neo.Bpms.Domain.Models.Cmmn.Entities;

public class StateDictionary
{
    public StateDictionary()
    {

    }

    public StateDictionary(string enumId)
    {
        EnumId = enumId;
    }

    public StateDictionary(IEnumerable<EntityState> states)
    {
        if (states == null)
        {
            return;
        }

        AddStates(states);
    }

    public string EnumId { get; set; }

    public Dictionary<string, EntityState> States;

    public EntityState GetState(string id)
    {
        return States.TryGetValue(id, out EntityState value) ? value : null;
    }

    public void AddStates(IEnumerable<EntityState> states)
    {
        Dictionary<string, EntityState> dic = states.ToDictionary(s => s.Id);
        States = [];
        foreach (KeyValuePair<string, EntityState> entityState in dic)
        {
            _ = States.TryAdd(entityState.Key, entityState.Value);
        }
    }

    public EntityState AddState(EntityState state)
    {
        States ??= [];
        if (States.TryGetValue(state.Id, out EntityState value))
        {
            return value;
        }

        _ = States.TryAdd(state.Id, state);
        return state;
    }
}
