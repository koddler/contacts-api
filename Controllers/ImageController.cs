using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Contacts.Models;
using CsvHelper;

namespace Contacts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ContactsContext _context;

        public ImageController(ContactsContext context)
        {
            _context = context;
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] Contact contact)
        {
            try
            {
                var folderName = Path.Combine("Resources", "Images");
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(savePath, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var baseUrl = String.Format("{0}://{1}", Request.Scheme, Request.Host);
                    var relativePath = dbPath.Replace("\\", "/");
                    var url = baseUrl + "/" + relativePath;
                    System.Console.WriteLine(url);

                    var group = await _context.Groups.FindAsync(contact.GroupId);
                    if (group == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        contact.Group = group;
                    }
                    contact.Avatar = relativePath;

                    _context.Contacts.Add(contact);
                    await _context.SaveChangesAsync();
                    contact.Avatar = url;

                    return Ok(new { contact });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("BEGIN__LOG__ERROR @ {0}", DateTime.Now);
                Console.WriteLine(new { Error = e });
                Console.WriteLine("END__LOG__ERROR");
                return StatusCode(500, new { Error = "Internal Server Error" });
            }
        }

        private void PatchContact(Contact _contact, Contact contact)
        {
            if (contact.Name != null)
            {
                _contact.Name = contact.Name;
            }
            if (contact.Phone != null)
            {
                _contact.Phone = contact.Phone;
            }
            if (contact.Email != null)
            {
                _contact.Email = contact.Email;
            }
            if (contact.Avatar != null)
            {
                _contact.Avatar = contact.Avatar;
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Contact>> PatchContact(int id, [FromForm] IFormFile file, [FromForm] Contact contact)
        {
            var _contact = await _context.Contacts.FindAsync(id);
            if (_contact == null)
            {
                return NotFound(new { Error = $"Contact with id {id} not found" });
            }

            try
            {
                PatchContact(_contact, contact);
                _context.Entry(_contact).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (System.Exception e)
            {
                return BadRequest(new { Error = e.Message });
            }

            return Ok(new { _contact });
        }
    }
}
