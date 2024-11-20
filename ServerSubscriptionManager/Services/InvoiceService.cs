using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;

namespace ServerSubscriptionManager.Services
{
    public class InvoiceService(SubscriptionContext context, IServiceProvider services) : IEntityService<Invoice>
    {

        private readonly SubscriptionContext _context = context;
        private readonly IServiceProvider _services = services;
        public async Task<bool> AddAsync(Invoice invoice)
        {
            var periodService = (IEntityService<SubscriptionPeriod>?)_services.GetService(typeof(IEntityService<SubscriptionPeriod>));
            if (periodService == null)
            {
                return false;
            }

            var user = await _context.Users.FindAsync(invoice.UserId);

            if (user == null)
            {
                return false;
            }

            var subscriptionPeriod = await _context.SubscriptionPeriods.FindAsync(invoice.SubscriptionPeriodId);

            if (subscriptionPeriod == null) {
                return false;
            }

            if (subscriptionPeriod.NextUserCost > user.Balance)
            {
                return false;
            }

            subscriptionPeriod.UserCount++;

            _context.Invoices.Add(invoice);
            _context.Entry(subscriptionPeriod).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            await periodService.UpdateAsync(subscriptionPeriod);

            return true;
        }

        public async Task<bool> RemoveAsync(long id)
        {
            var periodService = (IEntityService<SubscriptionPeriod>?)_services.GetService(typeof(IEntityService<SubscriptionPeriod>));
            if (periodService == null)
            {
                return false;
            }

            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return false;
            }

            var user = await _context.Users.FindAsync(invoice.UserId);

            if (user == null)
            {
                return false;
            }

            var subscriptionPeriod = await _context.SubscriptionPeriods.FindAsync(invoice.SubscriptionPeriodId);

            if (subscriptionPeriod == null)
            {
                return false;
            }

            subscriptionPeriod.UserCount--;

            _context.Invoices.Remove(invoice);
            _context.Entry(subscriptionPeriod).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            await periodService.UpdateAsync(subscriptionPeriod);

            return true;
        }

        public async Task<bool> UpdateAsync(Invoice invoice)
        {
            var oldInvoice = await _context.Invoices.FindAsync(invoice.Id);

            if (oldInvoice == null)
            {
                return false;
            }

            var user = await _context.Users.FindAsync(invoice.UserId);

            if (user == null)
            {
                return false;
            }

            user.Balance += oldInvoice.Amount - invoice.Amount;

            oldInvoice.Amount = invoice.Amount;

            _context.Entry(user).State = EntityState.Modified;
            _context.Entry(oldInvoice).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
