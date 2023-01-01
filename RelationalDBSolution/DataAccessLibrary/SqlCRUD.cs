using DataAccessLibrary.Models;

namespace DataAccessLibrary;
// this class talk to the SqlDataAccess class which talk dierctly to Database in order to perform the crud operations
public class SqlCRUD
{
    private readonly SqlDataAccess _dataAccess = new SqlDataAccess();
    private readonly string _connString;

    public SqlCRUD(string connString)
    {
        _connString = connString;
    }

    // the service which returns all the contacts [basic version]
    public List<BasicContactInfo> GetAllContacts()
    {
        // the sql query to be performed
        var sqlQuery = "SELECT Id, FirstName, LastName FROM dbo.Contacts";

        // utilize the data access layer [our custom ORM]
        // give it dynamic so we can pass an anonymous object because we don't have params here in this query
        return _dataAccess.LoadData<BasicContactInfo, dynamic>(sqlQuery, new { }, _connString);
    }

    // this service recieves an id of contact and returns complete details of this contact
    public ContactInfoDetails GetContactDetailsByContactId(int id)
    {
        // write the query
        var sqlQuery = @"SELECT Id, FirstName, LastName FROM dbo.Contacts WHERE Id = @ContactID";

        // utlize the data access layer [our custom orm]
        // give it the id as param

        var detailedContact = new ContactInfoDetails();

        // 1)=> first we need to retreive the contact 
        detailedContact.basicContactInfo = _dataAccess.LoadData<BasicContactInfo, dynamic>(
            sqlQuery, 
            new { ContactID = id }, 
            _connString).FirstOrDefault();

        // update the query
        sqlQuery = @"SELECT E.EmailAddress 
                            FROM dbo.EmailAddresses AS E 
                            INNER JOIN dbo.ContactsEmails AS CE 
                            ON CE.EmailAddressId = E.Id 
                            WHERE CE.ContactId = @ContactID";

        // 2)=> then we need to retreive all the emails of this contact
        detailedContact.Emails = _dataAccess.LoadData<EmailAddress, dynamic>(
            sqlQuery, 
            new { ContactID = id }, 
            _connString);

        // update the query
        sqlQuery = @"SELECT P.Phone
                            FROM dbo.PhoneNumbers AS P 
                            INNER JOIN dbo.ContactsPhones AS CP
                            ON CP.PhoneNumberId = P.Id 
                            WHERE CP.ContactId = @ContactID";

        // 3)=> finally we need to retreive all the phone numbers of this contact
        detailedContact.PhoneNumbers = _dataAccess.LoadData<PhoneNumber, dynamic>(
            sqlQuery, 
            new { ContactID = id },
            _connString);

        return detailedContact;
    }

    public void CreateNewContact(ContactInfoDetails contact)
    {
        // save the basic contact from the given full contact details object
        string query = "INSERT INTO dbo.Contacts (FirstName, LastName) VALUES (@FirstName, @LastName);";
        _dataAccess.SaveData<dynamic>(
            query,
            new
            {
                contact.basicContactInfo.FirstName,
                contact.basicContactInfo.LastName
            },
            _connString);

        // now read this created Contact entitiy again 
        query = "SELECT Id FROM dbo.Contacts WHERE FirstName=@FirstName AND LastName=@LastName;";
        int createdContactId = _dataAccess.LoadData<IdLookUP, dynamic>(query, new { contact.basicContactInfo.FirstName, contact.basicContactInfo.LastName }, _connString).FirstOrDefault().Id;

        // loop through the phone numbers
        foreach (var phone in contact.PhoneNumbers)
        {
            // the defualt of int is 0, so if its not sent from the front end to our model, it will be zero by defualt which means its a new entitiy entered by the user not an old entitiy selected from some drop down menu or something by the user
            if (phone.Id == 0)
            {
                // if its a new one, so insert it into the PhoneNumbers table
                query = "INSERT INTO dbo.PhoneNumbers (Phone) VALUES (@phoneNumber);";
                _dataAccess.SaveData<dynamic>(
                    query,
                    new { phoneNumber = phone.Phone },
                    _connString);

                // read the id of the created phoneNumber entry by the database and set it into the Id property of the PhoneNumber model 
                query = "SELECT Id FROM dbo.PhoneNumbers WHERE Phone=@phoneNUmber;";
                phone.Id = _dataAccess.LoadData<IdLookUP, dynamic>(
                    query,
                    new { phoneNUmber = phone.Phone },
                    _connString).FirstOrDefault().Id;
            }
            // and now insert into the M-N table 
            query = "INSERT INTO dbo.ContactsPhones (ContactId, PhoneNumberId) VALUES (@ContactID, @PhoneID);";
            _dataAccess.SaveData<dynamic>(
                query,
                new
                {
                    ContactID = createdContactId,
                    PhoneID = phone.Id
                },
                _connString);

            // loop through the emails
            foreach (var email in contact.Emails)
            {
                // the defualt of int is 0, so if its not sent from the front end to our model, it will be zero by defualt which means its a new entitiy entered by the user not an old entitiy selected from some drop down menu or something by the user
                if (email.Id == 0)
                {
                    // if its a new one, so insert it into the EmailAddress table
                    query = "INSERT INTO dbo.EmailAddresses (EmailAddress) VALUES (@Email);";
                    _dataAccess.SaveData<dynamic>(
                        query,
                        new { email.Email },
                        _connString);

                    // read the id of the created emailAddress entry by the database and set it into the Id property of the email Address model 
                    query = "SELECT Id FROM dbo.EmailAddresses WHERE EmailAddress=@Email;";
                    email.Id = _dataAccess.LoadData<IdLookUP, dynamic>(
                        query,
                        new { email.Email },
                        _connString).FirstOrDefault().Id;
                }
                // and now insert into the M-N table 
                query = "INSERT INTO dbo.ContactsEmails (ContactId, EmailAddressId) VALUES (@ContactID, @EmailID);";
                _dataAccess.SaveData<dynamic>(
                    query,
                    new
                    {
                        ContactID = createdContactId,
                        EmailID = email.Id
                    },
                    _connString);

            }
        }
    }

    // service method that update the contact basic info
    public void UpdateContactBasicInfo(BasicContactInfo UpdatedContactInfo)
    {
        // define the sql query
        var query = @"UPDATE dbo.Contacts SET FirstName=@FName, LastName=@LName WHERE Id=@Id;";

        // execute it against the table
        _dataAccess.SaveData<dynamic>(query, 
            new {
                FName = UpdatedContactInfo.FirstName, 
                LName=UpdatedContactInfo.LastName, 
                Id=UpdatedContactInfo.Id }, 
            _connString);
    }

    // service method that takes contact id and the phone number id and remove the phone number from this contact
    // => if this phone number is only assigned to this specific contact, so we will mark it as archieved in PhoneNumbers table because its not used by other contacts
    // => if its linked iwth other contacts, we just remove the linked-entitiy in the link-table
    public void DeleteContactPhoneNumber(int ContactId, int PhoneId)
    {
        // we first need to grap all the link-entities that this phone number is part of 
        var query = @"SELECT Id, ContactId, PhoneNumberId FROM dbo.ContactsPhones WHERE PhoneNumberId=@PhoneId;";

        // execute the query and return the result into the ContactPhone model
        var queryResult = _dataAccess.LoadData<ContactPhone, dynamic>(query, new { PhoneId }, _connString);

        // and if or if not we should delete the linked-entitiy between this phone number and this specific contact
        query = @"DELETE FROM dbo.ContactsPhones WHERE ContactId=@ContactId AND PhoneNumberId=@PhoneId ;";
        _dataAccess.SaveData<dynamic>(query, new {ContactId, PhoneId}, _connString);
        
        // if this queryResult list has more than one entry, that means that this phone number is assigned to more than one user, so we can't mark it as soft deleted
        // => do nothing

        // if not, so mark it as soft deleted
        if(queryResult.Count == 1)
        {
            query = @"DELETE FROM dbo.PhoneNumbers WHERE Id=@PhoneId;";
            _dataAccess.SaveData<dynamic>(query, new { PhoneId}, _connString);
        }
    }
}
