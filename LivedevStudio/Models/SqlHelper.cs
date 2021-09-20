using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LivedevStudio.Models
{
    public class SqlHelper
    {
        SqlConnection cn;
        public SqlHelper(string connectionString)
        {
            cn = new SqlConnection(connectionString);
        }
        public bool IsConnection
        {
            get
            {
                bool conStatus = false;
                if (cn.State == System.Data.ConnectionState.Closed)
                {
                    cn.Open();
                    return conStatus = true;
                }
                return conStatus;
            }
        }
    }
}