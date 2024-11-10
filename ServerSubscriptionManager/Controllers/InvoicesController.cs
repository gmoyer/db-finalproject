using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;
using ServerSubscriptionManager.Services;

namespace ServerSubscriptionManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly SubscriptionContext _context;
        private readonly UserService _userService;

        public InvoicesController(SubscriptionContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }

        // GET: api/Invoices
        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            var user = await _userService.GetRequestingUser(User);

            if (user == null)
            {
                return Unauthorized();
            }

            if (user.Role == "Admin")
            {
                return await _context.Invoices
                    .Include(i => i.User)
                    .Include(i => i.SubscriptionPeriod)
                    .ToListAsync();
            }
            else
            {
                return await _context.Invoices
                    .Where(i => i.UserId == user.Id)
                    .Include(i => i.SubscriptionPeriod)
                    .ToListAsync();
            }
        }

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Invoice>> GetInvoice(long id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            if (!_userService.Authorized(User, invoice.UserId))
            {
                return Unauthorized();
            }

            return invoice;
        }

        // PUT: api/Invoices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> PutInvoice(long id, Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return BadRequest();
            }

            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Invoices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoiceDto)
        {
            var user = await _userService.GetRequestingUser(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var userId = user.Id;

            if (user.Role == "Admin")
            {
                userId = invoiceDto.UserId;
            }

            var invoice = new Invoice
            {
                UserId = userId,
                SubscriptionPeriodId = invoiceDto.SubscriptionPeriodId
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvoice", new { id = invoice.Id }, invoice);
        }

        // DELETE: api/Invoices/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteInvoice(long id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvoiceExists(long id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }
    }
}
