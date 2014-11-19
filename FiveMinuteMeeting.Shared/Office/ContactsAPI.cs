using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Office365.OutlookServices;
using System.Threading.Tasks;

namespace FiveMinuteMeeting.Shared
{
  public static class ContactsAPI
  {
    
    public static async Task<IEnumerable<IContact>> GetContacts()
    {

      var client = await Client.GetContactsClient();
      // Obtain first page of contacts
      var contactsResults = await (from i in client.Me.Contacts
                                   orderby i.Surname
                                   select i).Take(100).ExecuteAsync();

      return contactsResults.CurrentPage;
    }

    public static async Task UpdateContact(IContact contact)
    {
      await contact.UpdateAsync();
    }

    public static async Task DeleteContact(IContact contact)
    {
      await contact.DeleteAsync();
    }

    public static async Task<bool> AddContact(IContact contact)
    {
      try
      {
        contact.FileAs = contact.Surname + ", " + contact.GivenName;
        var client = await Client.GetContactsClient();
        await client.Me.Contacts.AddContactAsync(contact);
      }
      catch(Exception ex)
      {
        return false;
      }
      return true;
    }

    public static async Task SendEmail(string email, string name, string subject, string body)
    {
      var message = new Message
      {
        Body = new ItemBody { Content = body, ContentType = BodyType.Text },
        Importance = Importance.High,
        Subject = subject,
        ToRecipients = new List<Recipient>
        {
          new Recipient
          {
            EmailAddress = new EmailAddress{Address = email, Name = name }
          }
        }
      };
      var client = await Client.GetContactsClient();
     
      await client.Me.SendMailAsync(message, true);
    }
  }
}