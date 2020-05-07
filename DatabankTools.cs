using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EscapeFromTheWoods
{
    class DatabankTools
    {
        public DatabankTools()
        { }
        private readonly string ConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        public void ClearDatabase()
        {
            SqlConnection conn = new SqlConnection(ConnString);
            //Console.WriteLine("Deleting all data in the database..");
            string sqlTruncLogs = "DELETE FROM Logs;";
            SqlCommand cmd0 = new SqlCommand(sqlTruncLogs, conn);
            try
            {
                conn.Open();
                cmd0.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            string sqlTruncMonkey = "DELETE FROM MonkeyRecords;";
            SqlCommand cmd1 = new SqlCommand(sqlTruncMonkey, conn);
            try
            {
                conn.Open();
                cmd1.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            string sqlTruncWood = "DELETE FROM WoodRecords;";
            SqlCommand cmd2 = new SqlCommand(sqlTruncWood, conn);
            try
            {
                conn.Open();
                cmd2.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
