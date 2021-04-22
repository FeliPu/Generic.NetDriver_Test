using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace GenericNetDb_Driver10
{
  public class DbManager
  {
    private SqlConnection sqlConnection;
    private const string tableName = "AllSimulationData"; // target table in our DB
    private const string dataColumnName = "Value"; // target Column in our DB
    private const string connectionString = @"Data Source=ATSZG-WKS026\ZENON_2017;Initial Catalog=MerckpolTest;Integrated Security=true;";
    //var connectionString = @"Data Source=DEMUC-LPT051\ZENON_2017;Initial Catalog=MerckpolTest;Integrated Security=true;";


    /// <summary>
    /// Get a specific data from the table filtering by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetValue(int id)
    {
      string result = "";
      try
      {
        OpenSqlConnexion();
        var strQuery = $"SELECT {dataColumnName} FROM {tableName} WHERE id = '{id}'";
        using (var sqlCommand = new SqlCommand(strQuery, sqlConnection))
        {
          SqlDataReader sdr = sqlCommand.ExecuteReader();

          while (sdr.Read())
          {
            result = (string)sdr[dataColumnName];
          }

          return result;
        }
      }
      catch (Exception ex)
      {
        return "Error";
      }
      finally
      {
        CloseSqlConnexion();
      }
    }

    private void OpenSqlConnexion()
    {
      if (sqlConnection == null)
      {
        sqlConnection = new SqlConnection(connectionString);
      }
      if (sqlConnection.State != ConnectionState.Open)
      {
        sqlConnection.Close(); // first close to avoid connectionStates like (broken, connecting, exectuing or fetching)
        sqlConnection.Open();
      }
    }
    private void CloseSqlConnexion()
    {
      if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
      {
        sqlConnection.Close();
      }
    }
  }
}
