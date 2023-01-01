using DataAccessLibrary;
using Microsoft.Extensions.Configuration;



// a method that takes a connection string name as a paramter 
// and fetch the connection string value from json file 
// so later we can just replace the connectionStringName paramter 
// to use another data source rather than the local Sql Server DB
string GetConnString (string connStringKey = "DefaultConnection")
{
    // define the builder 
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");

    // define the config variable
    var config = builder.Build();

    // get the connection string from the json file and return the result
    var connStringValue = config.GetConnectionString(connStringKey);
    return connStringValue;
}

// utilize the service layer into the application layer
// to read the data we need an instance of the service layer [which is the SqlCRUD class]
void ReadContactsInfo (SqlCRUD CrudService) // application layer method
{
    var rows = CrudService.GetAllContacts();
    foreach(var row in rows)
    {
        Console.WriteLine($"{row.Id}, Name is {row.FirstName} {row.LastName}");
    }
}

// given contact id, find all details of this contact
void ReadContactDetails (SqlCRUD CrudService, int ContactId)
{
    var contactDetails = CrudService.GetContactDetailsByContactId(ContactId);
    Console.WriteLine($"{contactDetails.basicContactInfo.Id}, Name is {contactDetails.basicContactInfo.FirstName} {contactDetails.basicContactInfo.LastName}");
    foreach(var email in contactDetails.Emails)
    {
        Console.WriteLine($"{email.Email}");
    }
    foreach (var phoneNumber in contactDetails.PhoneNumbers)
    {
        Console.WriteLine($"{phoneNumber.Phone}");
    }
}

// now use the application layer
var sqlCrudService = new SqlCRUD(GetConnString());
ReadContactsInfo(sqlCrudService);
ReadContactDetails(sqlCrudService, 1);