using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLibrary;

public class SqlDataAccess
{
    // a method to read data from database
    /**
    - Returns List of type T where T is a data model 
    - Receives a sql statement of type string
    - Receives a parameters of type U where U is a generic type
      these paramters might be the id to filter the rows or even nothing
    - Receives the connection string to the data soruce we will deal with
     */
    public List<T> LoadData<T, U>(string SqlStatement, U Params, string ConnectionString)
    {
        // to close the connection safely
        using(IDbConnection connection = new SqlConnection(ConnectionString))
        {
            // connection.Query<T> returns collection of T type
            // Query<>() is used for reading
            var data = connection.Query<T>(SqlStatement, Params).ToList();
            return data;
        }
    }


    // a method to save data to the database
    public void SaveData<T>(string SqlStatement, T Params, string ConnectionString)
    {
        using (IDbConnection connection = new SqlConnection(ConnectionString))
        {
            // .Execute() is used for writing
            connection.Execute(SqlStatement, Params);
        }
    }
}
