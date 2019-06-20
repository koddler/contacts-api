using Newtonsoft.Json;
using CsvHelper.Configuration;

namespace Contacts.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }

        public int GroupId { get; set; }
        [JsonIgnore]
        public Group Group { get; set; }
    }

    public sealed class ContactMap : ClassMap<Contact>
    {
        public ContactMap()
        {
            AutoMap();
            Map(m => m.ContactId).Ignore();
        }
    }
}
