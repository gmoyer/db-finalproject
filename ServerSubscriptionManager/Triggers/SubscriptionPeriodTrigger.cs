using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;

namespace ServerSubscriptionManager.Triggers
{
    public class SubscriptionPeriodTrigger(SubscriptionContext context) : IAfterSaveTrigger<SubscriptionPeriod>
    {
        private readonly SubscriptionContext _context = context;

        public Task AfterSave(ITriggerContext<SubscriptionPeriod> context, CancellationToken cancellationToken)
        {
            var subscriptionPeriod = context.Entity;

            if (subscriptionPeriod.Invoices == null)
            {
                return Task.FromException(new InvalidOperationException("Invoices not found for the subscription period."));
            }

            var users = _context.Users.ToList();
            var invoices = _context.Invoices.Where(i => i.SubscriptionPeriodId == subscriptionPeriod.Id).ToList();

            switch (context.ChangeType)
            {
                case ChangeType.Added:
                    // Automatically invoice users who have auto-invoice enabled and have a balance
                    foreach (var user in users)
                    {
                        if (user.AutoInvoice && user.Balance >= subscriptionPeriod.NextUserCost)
                        {
                            var invoice = new Invoice
                            {
                                UserId = user.Id,
                                SubscriptionPeriodId = subscriptionPeriod.Id
                            };
                            _context.Invoices.Add(invoice);
                        }
                    }
                    break;
                case ChangeType.Modified:
                    // Need to update all the invoices with the new cost
                    foreach (var invoice in invoices)
                    {
                        invoice.Amount = subscriptionPeriod.UserCost;
                        _context.Entry(invoice).State = EntityState.Modified;
                    }
                    break;
                case ChangeType.Deleted:
                    // This likely won't happen, but if it does, we need to remove the invoices
                    // associated with the subscription period
                    // This is a cascading delete, so we don't need to do anything here
                    break;
            }

            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
