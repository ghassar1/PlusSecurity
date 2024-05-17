using System;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace AcSys.Core.Data.MsSqlServer
{
    public class SqlHelper
    {
        public static int RunScript(string connectionString, string script)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                Server server = new Server(new ServerConnection(connection));
                int result = server.ConnectionContext.ExecuteNonQuery(script);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool TableExists(string conStr, string tableName)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TABLE_NAME", con);
            cmd.Parameters.Add(new SqlParameter("TABLE_NAME", tableName));

            try
            {
                con.Open();
                string result = cmd.ExecuteScalar().ToString();
                return result != "0";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        public static bool TableDoesNotExist(string connectionString, string tableName)
        {
            return !SqlHelper.TableExists(connectionString, tableName);
        }
    }
}
