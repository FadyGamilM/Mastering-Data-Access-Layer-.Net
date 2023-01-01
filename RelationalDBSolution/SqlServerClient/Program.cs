using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;



// a method that takes a connection string name as a paramter 
// and fetch the connection string value from json file 
// so later we can just replace the connectionStringName paramter 
// to use another data source rather than the local Sql Server DB
string GetConnString(string connStringKey = "DefaultConnection")
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
void ReadContactsInfo(SqlCRUD CrudService) // application layer method
{
    var rows = CrudService.GetAllContacts();
    foreach (var row in rows)
    {
        Console.WriteLine($"{row.Id}, Name is {row.FirstName} {row.LastName}");
    }
}



// given contact id, find all details of this contact
void ReadContactDetails(SqlCRUD CrudService, int ContactId)
{
    var contactDetails = CrudService.GetContactDetailsByContactId(ContactId);
    Console.WriteLine($"{contactDetails?.basicContactInfo?.Id ?? -1}, Name is {contactDetails?.basicContactInfo?.FirstName} {contactDetails?.basicContactInfo?.LastName}");
    foreach (var email in contactDetails?.Emails)
    {
        Console.WriteLine(email.Email);
    }
    foreach (var phoneNumber in contactDetails.PhoneNumbers)
    {
        Console.WriteLine($"{phoneNumber.Phone}");
    }
}


// Method that create a complete contact info details object
ContactInfoDetails ContactInfoFactory()
{
    var contactDetails = new ContactInfoDetails();
    contactDetails.basicContactInfo = new BasicContactInfo
    {
        FirstName = "peter",
        LastName = "gamil"
    };

    // newly created email [because it doesn't has an id]
    contactDetails.Emails.Add(new EmailAddress { Email="peter@mail.com"});

    // old selected email from the frontend [because it sent with its id]
    contactDetails.Emails.Add(new EmailAddress { Id = 4, Email = "frontendTeam@mail.com" });

    // newly created phone number by the user [no id]
    contactDetails.PhoneNumbers.Add(new PhoneNumber { Phone = "01201095076" });
    
    // selected phone number by the user [with id from frontend]
    contactDetails.PhoneNumbers.Add(new PhoneNumber { Id=1, Phone = "01283233951" });

    return contactDetails;
}

BasicContactInfo BasicContactInfoFactory()
{
    var contactInfo = new BasicContactInfo
    {
        Id = 1,
        FirstName = "Fady",
        LastName = "Gamil"
    };
    return contactInfo;
}

// method to create a new entitiy
void CreateContactInfoDetails(SqlCRUD sqlCrud)
{
    var contactDetails = ContactInfoFactory();

    try
    {
        sqlCrud.CreateNewContact(contactDetails);

        Console.WriteLine("Created Successfully");
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}


// now use the application layer
var sqlCrudService = new SqlCRUD(GetConnString());
Console.WriteLine("################# Read All Contacts Basic Info #################");
ReadContactsInfo(sqlCrudService);

Console.WriteLine("\n \n");

Console.WriteLine("################# Read Contact Details given the contact id #################");
ReadContactDetails(sqlCrudService, 1);

Console.WriteLine("\n \n");

Console.WriteLine("################# Create Contact Details entitiy #################");
CreateContactInfoDetails(sqlCrudService);