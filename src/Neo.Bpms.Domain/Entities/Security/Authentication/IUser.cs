namespace Neo.Bpms.Domain.Entities.Security.Authentication;

/// <summary>
///     Minimal interface for a user with id and username
/// </summary>
public interface IUser : IUser<string>
{
}

/// <summary>
///     Minimal interface for a user with id and username
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IUser<out TKey>
{
    /// <summary>
    ///     Unique key for the user
    /// </summary>
    TKey Id { get; }

    /// <summary>
    ///     Unique username
    /// </summary>
    string UserName { get; set; }
}

//public class User : IUser<string>
//{
//	public long Id { get; set; }
//	public string UserName { get; set; }
//	//		public virtual ICollection<UserRole> Roles { get; }


//	public virtual ICollection<UserClaim> Claims { get; }
//	public virtual ICollection<UserLogin> Logins { get; }
//	public virtual string PasswordHash { get; set; }
//	public virtual string SecurityStamp { get; set; }
//	public string lang { get; set; }
//	//public UCertClass ucert{ get; set; }
//	//public DateTime StartDate;
//	//public DateTime EndDate;

//	//public bool IsDevelopment
//	//{
//	//	get
//	//	{
//	//		return id == 2;
//	//	}
//	//}
//}
