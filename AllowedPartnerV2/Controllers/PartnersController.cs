using AllowedPartnerV2;
using AllowedPartnerV2.Dto;
using AllowedPartnerV2.Model;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace AllowedPartnerV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private readonly Context _context;

        public PartnersController(Context context)
        {
            _context = context;
        }

        // GET: api/Partners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartnerDto>>> GetPartners()
        {
            return await _context.Partners.Include(c => c.Items)
                .Select(c => new PartnerDto
                {
                    partnerkey = c.partnerkey,
                    partnerrefno = c.partnerrefno,
                    partnerpassword = c.partnerpassword,
                    totalamount = c.totalamount,
                    items = c.Items.Select(p => new ItemDto
                    {
                        partneritemref = p.partneritemref,
                        name = p.name,
                        qty = p.qty,
                        unitprice = p.unitprice
                    }).ToList(),
                    timestamp = c.timestamp,
                    sig = c.sig
                })
                .ToListAsync();
        }

        // GET: api/Partners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PartnerDto>> GetPartner(string id)
        {
            var partner = await _context.Partners
                .Include(c => c.Items)
                .Where(a => a.partnerkey == id)
                .Select(c => new PartnerDto
                {
                    partnerkey = c.partnerkey,
                    partnerrefno = c.partnerrefno,
                    partnerpassword = c.partnerpassword,
                    totalamount = c.totalamount,
                    items = c.Items.Select(p => new ItemDto
                    {
                        partneritemref = p.partneritemref,
                        name = p.name,
                        qty = p.qty,
                        unitprice = p.unitprice
                    }).ToList(),
                    timestamp = c.timestamp,
                    sig = c.sig
                })
                .SingleOrDefaultAsync();

            if (partner == null)
            {
                return NotFound();
            }

            return Ok(partner);
        }

        // PUT: api/Partners/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPartner(string id, Partner partner)
        {
            if (id != partner.partnerkey)
            {
                return BadRequest();
            }

            var ptnr = await _context.Partners
                            .Include(p => p.Items)
                            .FirstOrDefaultAsync(p => p.partnerkey == partner.partnerkey);

            if (ptnr == null)
                return NotFound();
            var sig = GetTimestampString(partner.timestamp) + partner.partnerrefno + partner.totalamount.ToString() + GetEncodedPassword(partner.partnerpassword);
            

            ptnr.partnerrefno = partner.partnerrefno;
            ptnr.totalamount = partner.totalamount;
            ptnr.partnerpassword = GetEncodedPassword(partner.partnerpassword);
            ptnr.sig = GetSignature(sig);
            ptnr.timestamp = DateTime.Now.ToString();

    
            var incomingItemIds = partner.Items.Select(i => i.partneritemref).ToList();


            var itemsToRemove = ptnr.Items
                .Where(existing => !incomingItemIds.Contains(existing.partneritemref))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                _context.Items.Remove(item);
            }


            foreach (var itemDto in partner.Items)
            {
                var existingItem = ptnr.Items.FirstOrDefault(i => i.partneritemref == itemDto.partneritemref);

                if (existingItem != null)
                {

                    existingItem.partneritemref = itemDto.partneritemref;
                    existingItem.unitprice = itemDto.unitprice;
                    existingItem.name = itemDto.name;
                    existingItem.qty = itemDto.qty;
                  
               
                }
                else
                {
            
                    ptnr.Items.Add(new Items
                    {
                        partneritemref = itemDto.partneritemref,
                        unitprice = itemDto.unitprice,
                        name = itemDto.name,
                        qty = itemDto.qty

                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Partners
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Partner>> PostPartner(Partner partner)
        {
            partner.timestamp = DateTime.Now.ToString();
            var sig = GetTimestampString(partner.timestamp) + partner.partnerrefno + partner.totalamount.ToString() + GetEncodedPassword(partner.partnerpassword);

            partner.sig = GetSignature(sig);

            _context.Partners.Add(partner);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PartnerExists(partner.partnerkey))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPartner", new { id = partner.partnerkey }, partner);
        }

        // DELETE: api/Partners/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartner(string id)
        {
            var partner = await _context.Partners
                                .Include(p => p.Items)
                                .FirstOrDefaultAsync(p => p.partnerkey == id);

            if (partner == null)
            {
                return NotFound();
            }

            _context.Items.RemoveRange(partner.Items);
            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PartnerExists(string id)
        {
            return _context.Partners.Any(e => e.partnerkey == id);
        }

        private string GetSignature(string input)
        {
            string sig = String.Empty;
            

            using (SHA256 hash = SHA256.Create())
            {

                byte[] byt = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
               
                sig = Convert.ToBase64String(byt);

            }

            return sig;
        }

        private string GetEncodedPassword(string password)
        {
            using (SHA256 s = SHA256.Create())
            {
                byte[] h = s.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(h); // e.g., store or compare
            }
        }

        private string GetTimestampString(string timestamp)
        {
            DateTime dt = DateTime.Parse(timestamp);

            string tsstr = dt.Year.ToString() + dt.Month.ToString("D2") + dt.Day.ToString("D2") + dt.Hour.ToString("D2") + dt.Minute.ToString("D2") + dt.Second.ToString("D2");

            return tsstr;
        }
    }
}
