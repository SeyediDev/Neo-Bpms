using System.Data.Common;
using System.Data.OracleClient;
using Neo.Bpms.Domain.Entities.Cmmn.Data.Provider;
using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Oracle;

#pragma warning disable 618
public class OracleDatabaseConnection(IDataProvider provider, OracleDatabaseConnectionDefinition definition) 
    : AdoDotNetDatabaseConnection(provider, definition)
{
    //		private OracleConnection con;
    //		private OracleCommand cmd;
    //		private OracleDataAdapter da;
    //		public string MessageInfo = "";

    //void con_InfoMessage(object sender, OracleInfoMessageEventArgs e)
    //{
    //	MessageInfo += e.Message;
    //}
    public new OracleDatabaseConnectionDefinition Definition => base.Definition as OracleDatabaseConnectionDefinition;

    public override DbConnection GetNewConnection()
    {
        return new OracleConnection(Definition.ConnectionString);
    }

    //public override bool Open()
    //{
    //	try
    //	{
    //		con.ConnectionString = Definition.connectionString;
    //		con.Open();
    //		return true;
    //	}
    //	catch
    //	{
    //		return false;
    //	}
    //}
    //public override bool Close()
    //{
    //	try
    //	{
    //		con.Close();
    //		return true;
    //	}
    //	catch
    //	{
    //		return false;
    //	}
    //}
    //public override DataTable Select(string sql)
    //{
    //	cmd.CommandText = sql;
    //	cmd.ExecuteNonQuery();
    //	DataTable dt = new DataTable();
    //	da.Fill(dt);
    //	return dt;
    //}
    //public override bool doCommand(string sql)
    //{
    //	cmd.CommandText = sql;
    //	MessageInfo = "";
    //	try
    //	{
    //		cmd.ExecuteNonQuery();
    //		if (MessageInfo == "")
    //			return true;
    //		else
    //			return false;
    //	}
    //	catch (Exception ex)
    //	{
    //		MessageInfo = ex.Message;
    //		return false;
    //	}
    //}
    //public override string doCommandReturnResult(string sql)
    //{
    //	//cmd.CommandType = CommandType.Text;
    //	cmd.CommandText = sql;
    //	try
    //	{
    //		int res = cmd.ExecuteNonQuery();
    //		return "(" + res.ToString() + ") Rows Affected!";
    //	}
    //	catch (Exception ex)
    //	{
    //		return ex.Message;
    //	}
    //}
}
#pragma warning restore 618

