using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingApp.Data
{
    public static class DbHelper
    {
        private const string ConnectionString = "Server=127.0.0.1;Port=3306;Database=client_schedule;Uid=sqlUser;Pwd=Passw0rd!";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }

}
