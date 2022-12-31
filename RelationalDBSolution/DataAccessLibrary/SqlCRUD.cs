using DataAccessLibrary.Models;

namespace DataAccessLibrary;
// this class talk to the data access layer application to perform the crud operations
public class SqlCRUD
{
    private readonly SqlDataAccess _dataAccess = new SqlDataAccess();
    private readonly string _connString;

    public SqlCRUD(string connString)
    {
        _connString = connString;
    }

    // the service which returns all the contacts [basic version]
    public List<BasicContactInfo> GetAll()
    {
        // the sql query to be performed
        var sqlQuery = "SELECT Id, FirstName, LastName FROM dbo.Contacts";

        // utilize the data access layer [our custom ORM]
        // give it dynamic so we can pass an anonymous object because we don't have params here in this query
        return _dataAccess.LoadData<BasicContactInfo, dynamic>(sqlQuery, new { }, _connString);
    }
}
