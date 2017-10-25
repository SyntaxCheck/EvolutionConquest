using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class ConnectionManager
{
    public ConnectionManager()
    {
    }

    public SqlConnection GetSqlConnection(GameSettings settings)
    {
        SqlConnection connection = new SqlConnection(
            "user id=" + settings.UserName + 
            ";password=" + settings.Password + 
            ";database=" + settings.DatabaseName + 
            ";Server=" + settings.ServerName
        );

        connection.Open();

        return connection;
    }
    public SqlCommand GetSqlCommand(SqlConnection connection, string sql)
    {
        //****Not production code, SQL Injection risk****
        //For simplicity sake I am not using parametized SQL since I have no user input that can translate into a SQL Injection attack. Once I get user input I need to modify this
        SqlCommand sqlCommand = new SqlCommand(sql, connection);

        return sqlCommand;
    }
}