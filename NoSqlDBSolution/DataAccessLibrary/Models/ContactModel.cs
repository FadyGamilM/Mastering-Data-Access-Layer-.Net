using MongoDB.Bson.Serialization.Attributes;

namespace DataAccessLibrary.Models;

public class ContactModel
{
   [BsonId]
   public Guid Id {get; set;} = Guid.NewGuid();

   public string FirstName {get; set;} = string.Empty;
   public string LastName {get; set;} = string.Empty;

   public List<EmailAddressModel> Emails {get; set;} = new List<EmailAddressModel>();
   public List<PhoneNumberModel> Phones {get; set;} = new List<PhoneNumberModel>();
}