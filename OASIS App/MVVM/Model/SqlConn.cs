using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace OASIS_App.MVVM.Model
{
    public static class SqlConn
    {
        public static SqlConnection Connection => GetSqlConnection();

        public static SqlConnection GetSqlConnection()
        {
            string dbPath = Path.GetFullPath("..\\..\\DataBase\\Tours.mdf");
            
            return new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True");
        }
    }
}
