using Microsoft.Extensions.Configuration;
using DataAccessLibrary;
using DataAccessLibrary.Models;

public class Program
{
   // the collection name we will deal with
   private static readonly string collectionName = "Contacts";
   private static MongoDataAccess? _serviceLayer; 
   public static void Main()
   {
      // instantiate a new instance from the _ServiceLayer class
      _serviceLayer = new MongoDataAccess("ContactsDB", GetConnectionString());

      //! => create new contact
      // CreateNewContact(
      //    ContactModelFactory(
      //       "magy", 
      //       "magdy", 
      //       new List<EmailAddressModel> { new EmailAddressModel{Email="magy@gmail.com"}, new EmailAddressModel{Email="BackendTeam@gmail.com"}}, 
      //       new List<PhoneNumberModel>{new PhoneNumberModel{Phone="01283233951"}}
      //    )
      // );

      //! => get all contacts
      GetAllContacts();

      //! => get contact by its id
      // GetContactById("c6697762-60fa-4989-b768-0996f525c01d");

      //! => update contact first name
      // UpdateContactFirstName("foda", "14b2e882-1ed6-4100-ae83-903531f62844");
      // GetContactById("14b2e882-1ed6-4100-ae83-903531f62844");

      //! => delete an email from a contact
      RemoveContactEmail("c6697762-60fa-4989-b768-0996f525c01d", new EmailAddressModel{Email = "magy@gmail.com"});
      GetAllContacts();
      
      System.Console.WriteLine("Saving to DB is Done");
   }

   public static string GetConnectionString(string connString = "Default")
   {
      var output = "";

      var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");
      
      var config = builder.Build();

      output = config.GetConnectionString(connString);

      return output;
   }

   public static ContactModel ContactModelFactory(string firstName, string lastName, List<EmailAddressModel> emails, List<PhoneNumberModel> phones)
   {
      var contact = new ContactModel{
         FirstName = firstName,
         LastName = lastName
      };
      emails.ForEach(email => contact.Emails.Add(email));
      phones.ForEach(phone => contact.Phones.Add(phone));
      return contact;
   }


   // application layer method to create a new record in the Contacts collection
   public static void CreateNewContact(ContactModel contact)
   {
      _serviceLayer?.UpsertRecord<ContactModel>(collectionName, contact.Id, contact);
   }

   // application layer method to get all contacts 
   public static void GetAllContacts()
   {
      var contacts = _serviceLayer.LoadRecords<ContactModel>(collectionName);
      contacts.ForEach(
         contact => 
         {
            System.Console.WriteLine($"{contact.Id} => {contact.FirstName} {contact.LastName}");
            contact.Emails.ForEach(email => System.Console.WriteLine($"\t \t {email.Email}"));
            contact.Phones.ForEach(phone => System.Console.WriteLine($"\t \t {phone.Phone}"));
         }
      );
   }

   // application layer method to get contact by his/her id
   public static void GetContactById(string contactId)
   {
      var contact = _serviceLayer?.LoadRecordById<ContactModel>(collectionName,new Guid (contactId)) ?? null;
      if(contact == null){
         System.Console.WriteLine("There is no contact with this id");
         return;
      }  
      System.Console.WriteLine($"{contact.Id} => {contact.FirstName} {contact.LastName}");
      contact.Emails.ForEach(email => System.Console.WriteLine($"\t \t {email.Email}"));
      contact.Phones.ForEach(phone => System.Console.WriteLine($"\t \t {phone.Phone}"));
   }

   // application layer method to update specific field in an existing contact
   public static void UpdateContactFirstName(string firstName, string contactId)
   {
      var contact = _serviceLayer?.LoadRecordById<ContactModel>(collectionName, new Guid(contactId)) ?? null;
      if(contact == null){
         System.Console.WriteLine("There is no contact with this id");
         return;
      }  
      
      contact.FirstName=firstName;
      
      _serviceLayer?.UpsertRecord<ContactModel>(collectionName, new Guid(contactId), contact);
   }

   // application layer method to remove an email from the emails attached to specific contact given the email and contact id
   public static void RemoveContactEmail(string contactId, EmailAddressModel email)
   {
      var contact = _serviceLayer?.LoadRecordById<ContactModel>(collectionName, new Guid(contactId));
      if(contact == null){
         System.Console.WriteLine("There is no contact with this id");
         return;
      }
      // return all the emails by comparing each email and if itsn't the same email we need to remove .. return this one 
      contact.Emails = contact.Emails.Where(contactEmail => contactEmail.Email != email.Email).ToList();
      _serviceLayer?.UpsertRecord<ContactModel>(collectionName, contact.Id, contact);
   }


   // application layer method to remove a phone number from the phone numbers attached to specific contact given the phone number and contact id
}