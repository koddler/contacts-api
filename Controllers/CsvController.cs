using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Contacts.Models;
using CsvHelper;

namespace Contacts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsvController : ControllerBase
    {
        private readonly ContactsContext _context;

        public CsvController(ContactsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostCSV([FromForm] IFormFile file)
        {
            try
            {
                var stream = file.OpenReadStream();
                var name = file.FileName;

                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.RegisterClassMap<ContactMap>();
                    var contacts = csv.GetRecords<Contact>();
                    foreach (var contact in contacts)
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
                    }
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"__LOG__ error occurred");
                System.Console.WriteLine(e);
                return BadRequest(new { Error = e.InnerException.Message });
            }

            return Ok();
        }


    }
}
