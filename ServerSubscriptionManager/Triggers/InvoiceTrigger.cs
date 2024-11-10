using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;

namespace ServerSubscriptionManager.Triggers
{
    public class InvoiceTrigger(SubscriptionContext context) : IAfterSaveTrigger<Invoice>
    {
        private readonly SubscriptionContext _context = context;

        public Task AfterSave(ITriggerContext<Invoice> context, CancellationToken cancellationToken)
        {
            var invoice = context.Entity;

            var user = _context.Users.Find(invoice.UserId);
            var subscriptionPeriod = _context.SubscriptionPeriods.Find(invoice.SubscriptionPeriodId);

            if (user == null)
            {
                return Task.FromException(new InvalidOperationException("User not found for the invoice."));
            }

            if (subscriptionPeriod == null)
            {
                return Task.FromException(new InvalidOperationException("Subscription period not found for the invoice."));
            }

            switch (context.ChangeType)
            {
                case ChangeType.Added:
                    user.Balance -= invoice.Amount;
                    subscriptionPeriod.UserCount++;
                    _context.Entry(subscriptionPeriod).State = EntityState.Modified;
                    break;
                case ChangeType.Modified:
                    var oldInvoice = context.UnmodifiedEntity;
                    if (oldInvoice == null)
                    {
                        return Task.FromException(new InvalidOperationException("Old invoice not found for the invoice."));
                    }
                    user.Balance -= invoice.Amount - oldInvoice.Amount;
                    break;
                case ChangeType.Deleted:
                    user.Balance += invoice.Amount;
                    subscriptionPeriod.UserCount--;
                    _context.Entry(subscriptionPeriod).State = EntityState.Modified;
                    break;
            }

            _context.Entry(user).State = EntityState.Modified;

            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
