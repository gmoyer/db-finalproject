using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;
using System.Security.Claims;

namespace ServerSubscriptionManager.Services
{
    public class UserService(SubscriptionContext context)
    {
        private readonly SubscriptionContext _context = context;

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null;
            }
            if (user.Password != password)
            {
                return null;
            }
            return user;
        }

        public async Task<User?> GetRequestingUser(ClaimsPrincipal user)
        {
            string? userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
            {
                return null;
            }
            var userId = long.Parse(userIdString);
            return await _context.Users.FindAsync(userId);
        }

        public bool Authorized(ClaimsPrincipal user, long userId)
        {
            string? userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
            {
                return false;
            }
            var id = long.Parse(userIdString);
            return userId == id || user.IsInRole("Admin");
        }
    }
}
