using Microsoft.EntityFrameworkCore;

namespace Contacts.Models
{
    public class ContactsContext : DbContext
    {
        public ContactsContext(DbContextOptions<ContactsContext> options) : base(options)
        {

        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
