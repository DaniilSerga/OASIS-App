using System.Configuration;
using System.Data.SqlClient;

namespace OASIS_App.MVVM.Model
{
    public static class SqlConn
    {
        public static SqlConnection Connection => new SqlConnection(ConfigurationManager.ConnectionStrings["Tours"].ConnectionString);
    }
}
