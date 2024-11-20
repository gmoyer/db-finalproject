using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;

namespace ServerSubscriptionManager.Services
{
    public class SubscriptionPeriodService(SubscriptionContext context, IEntityService<Invoice> invoiceService) : IEntityService<SubscriptionPeriod>
    {
        private readonly SubscriptionContext _context = context;
        private readonly IEntityService<Invoice> _invoiceService = invoiceService;
        public async Task<bool> AddAsync(SubscriptionPeriod period)
        {
            // Automatically invoice users who have auto-invoice enabled and have a balance
            var users = await _context.Users.Where(u => u.AutoInvoice).ToListAsync();

            // Filter out users who don't have enough balance to pay for the server cost
            // Dynamically recalculate the server cost for each user filtered out
            while (users.Count != 0)
            {
                var pendingCost = period.ServerCost / users.Count;
                var userRemoved = false;
                for (int i = 0; i < users.Count; i++)
                {
                    User user = users.ElementAt(i);
                    if (user.Balance < pendingCost)
                    {
                        users.RemoveAt(i);
                        userRemoved = true;
                        i--;
                    }
                }
                if (!userRemoved)
                {
                    break;
                }
            }

            _context.SubscriptionPeriods.Add(period);
            await _context.SaveChangesAsync();

            // Create invoices for each user (if any remain)
            foreach (var user in users)
            {
                var invoice = new Invoice
                {
                    UserId = user.Id,
                    SubscriptionPeriodId = period.Id
                };
                var success = await _invoiceService.AddAsync(invoice);
                if (!success)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> RemoveAsync(long id)
        {
            var period = await _context.SubscriptionPeriods.FindAsync(id);

            if (period == null)
            {
                return false;
            }

            _context.SubscriptionPeriods.Remove(period);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(SubscriptionPeriod period)
        {
            var oldPeriod = await _context.SubscriptionPeriods.FindAsync(period.Id);
            if (oldPeriod == null)
            {
                return false;
            }

            var invoices = await _context.Invoices.Where(i => i.SubscriptionPeriodId == period.Id).AsNoTracking().ToListAsync();
            // Need to update all the invoices with the new cost
            for (int i = 0; i < invoices.Count; i++)
            {
                var invoice = invoices.ElementAt(i);
                invoice.Amount = period.UserCost;
                var success = await _invoiceService.UpdateAsync(invoice);
                if (!success)
                {
                    return false;
                }
            }

            oldPeriod.StartDate = period.StartDate;
            oldPeriod.EndDate = period.EndDate;
            oldPeriod.ServerCost = period.ServerCost;
            oldPeriod.UserCount = period.UserCount;

            _context.Entry(oldPeriod).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
