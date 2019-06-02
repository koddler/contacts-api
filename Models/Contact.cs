
namespace Contacts.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
