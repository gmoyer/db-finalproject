using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;
using ServerSubscriptionManager.Services;

namespace ServerSubscriptionManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly SubscriptionContext _context;
        private readonly UserService _userService;

        public PaymentsController(SubscriptionContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }

        // GET: api/Payments
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            var user = await _userService.GetRequestingUser(User);

            if (user == null)
            {
                return Unauthorized();
            }

            if (user.Role == "Admin")
            {
                return await _context.Payments
                    .Include(p => p.PaymentMethod)
                    .Include(p => p.User)
                    .ToListAsync();
            } else
            {
                return await _context.Payments
                    .Where(p => p.UserId == user.Id)
                    .Include(p => p.PaymentMethod)
                    .ToListAsync();
            }
        }

        // GET: api/Payments/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Payment>> GetPayment(long id)
        {
            var payment = await _context.Payments
                .Include(p => p.PaymentMethod)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return NotFound();
            }

            if (!_userService.Authorized(User, payment.UserId))
            {
                return Unauthorized();
            }

            return payment;
        }

        // PUT: api/Payments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> PutPayment(long id, Payment payment)
        {
            if (id != payment.Id)
            {
                return BadRequest();
            }

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
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

        // POST: api/Payments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Payment>> PostPayment(Payment paymentDto)
        {

            var user = await _userService.GetRequestingUser(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var payment = new Payment
            {
                Amount = paymentDto.Amount,
                PaymentMethodId = paymentDto.PaymentMethodId,
                UserId = user.Id
            };

            if (user.Role == "Admin")
            {
                payment.UserId = paymentDto.UserId;
                payment.Valid = true;
            }

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayment", new { id = payment.Id }, payment);
        }

        // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePayment(long id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            if (!_userService.Authorized(User, payment.UserId))
            {
                return Unauthorized();
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentExists(long id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
