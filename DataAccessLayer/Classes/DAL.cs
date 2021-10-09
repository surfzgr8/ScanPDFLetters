using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer.Classes
{
    public static class DAL
    {
        public static void ExecReturnNoData(string query, string connectionstring, bool isProcedure = false)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, connection))
                {
                    sqlCommand.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static DataSet ExecReturnDataSet(
          string query,
          string connectionstring,
          bool isProcedure = false)
        {
            DataSet dataSet = new DataSet();
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand selectCommand = new SqlCommand(query, connection))
                {
                    selectCommand.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
                    {
                        connection.Open();
                        sqlDataAdapter.Fill(dataSet);
                        connection.Close();
                    }
                }
            }
            return dataSet;
        }

        public static int ExecReturnInt(string query, string connectionstring, bool isProcedure = false)
        {
            int num;
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, connection))
                {
                    sqlCommand.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    connection.Open();
                    num = int.Parse(sqlCommand.ExecuteScalar().ToString());
                    connection.Close();
                }
            }
            return num;
        }

        public static DataTable ExecReturnDataTable(
          string query,
          string connectionstring,
          bool isProcedure = false)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                using (SqlCommand selectCommand = new SqlCommand(query, connection))
                {
                    selectCommand.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
                    {
                        connection.Open();
                        sqlDataAdapter.Fill(dataTable);
                        connection.Close();
                    }
                }
            }
            return dataTable;
        }

        public static int ExecProcedureReturn(string query, string connectionString)
        {
            SqlParameter sqlParameter = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlParameter = sqlCommand.Parameters.Add("ReturnVal", SqlDbType.Int);
                    sqlParameter.Direction = ParameterDirection.ReturnValue;
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return (int)sqlParameter.Value;
        }

        public static int ExecProcedureReturn2(
          string query,
          SqlParameter[] param,
          string connectionString)
        {
            SqlParameter sqlParameter = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(param);
                    sqlParameter = sqlCommand.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    sqlParameter.Direction = ParameterDirection.ReturnValue;
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return (int)sqlParameter.Value;
        }

    }
}
