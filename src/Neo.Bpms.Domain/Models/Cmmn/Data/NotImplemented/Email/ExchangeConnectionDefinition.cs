/*using Neo.Bpms.Domain.Model.Data.ExternalDatabase;


namespace Neo.Bpms.Domain.Model.Data.Email
{
	public class ExchangeConnectionDefinition : OledbConnectionDefinition
	{
//		string address;
		public ExchangeConnectionDefinition(string Address)//, bool IntegratedSecurity, string UserId, string Password
			: base("ExOLEDB.DataSource", null, Address, null, null, false, null, null)//IntegratedSecurity, UserId, Password
		{
//			this.address = Address;
		}
		//public override bool Open()
		//{
		//	dbConnection = getNewConnection() as OleDbConnection;
		//	if (dbConnection != null)
		//		dbConnection.Open(address);
		//	return dbConnection != null;
		//}
	}
}
*/