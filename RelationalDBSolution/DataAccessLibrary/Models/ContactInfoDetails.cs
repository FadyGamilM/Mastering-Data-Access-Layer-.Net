namespace DataAccessLibrary.Models;

public class ContactInfoDetails
{ 
    public BasicContactInfo basicContactInfo { get; set; }
    public List<EmailAddress> Emails { get; set; } = new List<EmailAddress>();
    public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
}

