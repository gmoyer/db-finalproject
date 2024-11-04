using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;
using System.Security.Claims;

namespace ServerSubscriptionManager.Services
{
    public class UserService
    {
        private readonly SubscriptionContext _context;

        public UserService(SubscriptionContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

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

        public bool Authorized(ClaimsPrincipal user, long userId)
        {
            return user.HasClaim("NameIdentifier", userId.ToString()) || user.IsInRole("Admin");
        }
    }
}
