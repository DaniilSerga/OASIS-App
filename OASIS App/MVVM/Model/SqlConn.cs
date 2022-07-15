using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace OASIS_App.MVVM.Model
{
    public static class SqlConn
    {
        //public static SqlConnection Connection => new SqlConnection(ConfigurationManager.ConnectionStrings["Tours"].ConnectionString);
        public static SqlConnection Connection => GetSqlConnection();

        public static SqlConnection GetSqlConnection()
        {
            // Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="C:\Users\User\Source\Repos\DaniilSerga\OASIS-App\OASIS App\DataBase\Tours.mdf";Integrated Security=True
            //                                                     "C:\\Users\\User\\Source\\Repos\\DaniilSerga\\OASIS-App\\OASIS App\\DataBase\\Tours.mdf"
            // Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="C:\Users\Supre\source\repos\OASIS\OASIS App\DataBase\Tours.mdf";Integrated Security=True
            string dbPath = Path.GetFullPath("..\\..\\DataBase\\Tours.mdf");
            
            return new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True");
        }
    }
}
