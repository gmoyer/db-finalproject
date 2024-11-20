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
    public class SubscriptionPeriodsController(UserService userService, SubscriptionContext context, IEntityService<SubscriptionPeriod> periodService) : ControllerBase
    {
        private readonly SubscriptionContext _context = context;
        private readonly UserService _userService = userService;
        private readonly IEntityService<SubscriptionPeriod> _periodService = periodService;


        // GET: api/SubscriptionPeriods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionPeriod>>> GetSubscriptionPeriods()
        {
            var user = await _userService.GetRequestingUser(User);

            if (user == null)
            {
                return Unauthorized();
            }
            return await _context.SubscriptionPeriods
                .Include(s => s.Invoices.Where(i => i.UserId == user.Id))
                .OrderBy(s => s.StartDate)
                .ToListAsync();
        }

        // GET: api/SubscriptionPeriods/all
        [HttpGet("all")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<IEnumerable<SubscriptionPeriod>>> GetAllSubscriptionPeriods()
        {
            var user = await _userService.GetRequestingUser(User);

            if (user == null)
            {
                return Unauthorized();
            }

            return await _context.SubscriptionPeriods
                .Include(s => s.Invoices)
                .ThenInclude(i => i.User)
                .OrderBy(s => s.StartDate)
                .ToListAsync();
        }

        // GET: api/SubscriptionPeriods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPeriod>> GetSubscriptionPeriod(long id)
        {
            var subscriptionPeriod = await _context.SubscriptionPeriods.FindAsync(id);

            if (subscriptionPeriod == null)
            {
                return NotFound();
            }

            return subscriptionPeriod;
        }

        // PUT: api/SubscriptionPeriods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> PutSubscriptionPeriod(long id, SubscriptionPeriod subscriptionPeriod)
        {
            if (id != subscriptionPeriod.Id)
            {
                return BadRequest();
            }

            var success = await _periodService.UpdateAsync(subscriptionPeriod);
            if (!success) {
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/SubscriptionPeriods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<SubscriptionPeriod>> PostSubscriptionPeriod(SubscriptionPeriod subscriptionPeriodDto)
        {

            var subscriptionPeriod = new SubscriptionPeriod
            {
                StartDate = subscriptionPeriodDto.StartDate,
                EndDate = subscriptionPeriodDto.EndDate,
                ServerCost = subscriptionPeriodDto.ServerCost
            };

            var success = await _periodService.AddAsync(subscriptionPeriod);
            if (!success)
            {
                return BadRequest();
            }

            return CreatedAtAction("GetSubscriptionPeriod", new { id = subscriptionPeriod.Id }, subscriptionPeriod);
        }

        // DELETE: api/SubscriptionPeriods/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteSubscriptionPeriod(long id)
        {
            var subscriptionPeriod = await _context.SubscriptionPeriods.FindAsync(id);
            if (subscriptionPeriod == null)
            {
                return NotFound();
            }

            var success = await _periodService.RemoveAsync(id);
            if (!success)
            {
                return BadRequest();
            }

            return NoContent();
        }

        private bool SubscriptionPeriodExists(long id)
        {
            return _context.SubscriptionPeriods.Any(e => e.Id == id);
        }
    }
}
