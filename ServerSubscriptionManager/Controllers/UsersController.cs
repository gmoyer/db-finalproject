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
    public class UsersController(UserService userService, SubscriptionContext context) : ControllerBase
    {
        private readonly SubscriptionContext _context = context;
        private readonly UserService _userService = userService;


        // GET: api/Users
        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<User>> GetMe()
        {
            var user = await _userService.GetRequestingUser(User);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (!_userService.Authorized(User, id))
            {
                return Unauthorized();
            }

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(long id, User userDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (!_userService.Authorized(User, id))
            {
                return Unauthorized();
            }

            var users = await _context.Users.ToListAsync();
            if (users.Any(u => u.Email == userDto.Email && u != user))
            {
                return BadRequest("Email already exists");
            }

            if (userDto.Name != "")
            {
                user.Name = userDto.Name;
            }
            if (userDto.Email != "")
            {
                user.Email = userDto.Email;
            }
            if (userDto.Playertag != "")
            {
                user.Playertag = userDto.Playertag;
            }
            if (userDto.Password != "")
            {
                user.Password = userDto.Password;
            }
            if (userDto.AutoInvoice != user.AutoInvoice)
            {
                user.AutoInvoice = userDto.AutoInvoice;
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User userDto)
        {
            var users = await _context.Users.ToListAsync();
            if (users.Any(u => u.Email == userDto.Email))
            {
                return BadRequest("Email already exists");
            }

            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Playertag = userDto.Playertag,
                Password = userDto.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (!_userService.Authorized(User, id))
            {
                return Unauthorized();
            }

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted");
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
