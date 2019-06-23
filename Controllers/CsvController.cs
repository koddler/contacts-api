using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
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
        private const string CACHE_KEY_COUNT = "CACHE_KEY_COUNT";
        private const string CACHE_KEY_TOTAL = "CACHE_KEY_TOTAL";
        private readonly ContactsContext _context;
        private IMemoryCache _cache;

        public CsvController(ContactsContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            int count, total;
            double progress = 0.0;
            if (!_cache.TryGetValue(CACHE_KEY_COUNT, out count))
            {
                count = 0;
            }
            if (!_cache.TryGetValue(CACHE_KEY_TOTAL, out total))
            {
                total = 0;
            }

            await Task.Delay(0);

            progress = Math.Truncate((double)count / total * 100);
            return Ok(new { count, total, progress });
        }

        [HttpPost]
        public async Task<IActionResult> PostCSV([FromForm] IFormFile file)
        {
            try
            {
                var stream = file.OpenReadStream();
                var name = file.FileName;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(3));

                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.RegisterClassMap<ContactMap>();
                    var contacts = csv.GetRecords<Contact>();
                    var contactsList = contacts.ToList();

                    int count = 0;
                    _cache.Set(CACHE_KEY_TOTAL, contactsList.Count);

                    foreach (var contact in contactsList)
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

                        count++;
                        _cache.Set(CACHE_KEY_COUNT, count, cacheEntryOptions);

                        await Task.Delay(1000);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"__LOG__ error occurred");
                System.Console.WriteLine(e);
                return BadRequest(new { Error = e });
            }

            return Ok();
        }


    }
}
