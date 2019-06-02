using System.Collections.Generic;

namespace Contacts.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }

        public List<Contact> Contacts { get; set; }
    }
}
