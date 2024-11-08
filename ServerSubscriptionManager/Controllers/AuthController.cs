using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;
using ServerSubscriptionManager.Services;
using SQLitePCL;
using System.Security.Claims;

namespace ServerSubscriptionManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SubscriptionContext _context;
        private readonly UserService _userService;

        public AuthController(SubscriptionContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userService.ValidateUserAsync(loginRequest.Username, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Keeps the session across browser restarts if true
            };

            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

            return user;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return Ok("Logged out");
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok(new { Authenticated = true });
            }
            else
            {
                return Ok(new { Authenticated = false });
            }
        }
    }
}
