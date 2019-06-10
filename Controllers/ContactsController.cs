using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Contacts.Models;

namespace Contacts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactsContext _context;

        public ContactsController(ContactsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            return await _context.Contacts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            var group = await _context.Groups.FindAsync(contact.GroupId);
            if (group == null)
            {
                return NotFound();
            }
            else
            {
                contact.Group = group;
            }

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContact), new { id = contact.ContactId }, contact);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Contact>> PutContact(int id, Contact contact)
        {
            if (id != contact.ContactId)
            {
                return NotFound(new { Error = $"Contact with id {id} not found" });
            }

            try
            {
                _context.Entry(contact).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (System.Exception e)
            {
                return BadRequest(new { Error = e.InnerException.Message });
            }

            return contact;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound(new { Error = $"Contact with id {id} not found" });
            }

            try
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (System.Exception e)
            {
                return BadRequest(new { Error = e.InnerException.Message });
            }
        }
    }
}
