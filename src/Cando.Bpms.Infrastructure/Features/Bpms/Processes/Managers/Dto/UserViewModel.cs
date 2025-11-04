namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

public class UserViewModel : IEquatable<UserViewModel>
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Family { get; set; }
    public string AvatarId { get; set; }

    public bool Equals(UserViewModel other)
    {
        if (ReferenceEquals(null, other)) return false;
        return ReferenceEquals(this, other) ? true : string.Equals(Id, other.Id);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() != GetType() ? false : Equals((UserViewModel)obj);
    }

    public override int GetHashCode()
    {
        return Id != null ? Id.GetHashCode() : 0;
    }
}
