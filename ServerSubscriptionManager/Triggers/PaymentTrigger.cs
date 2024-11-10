using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;

namespace ServerSubscriptionManager.Triggers
{
    public class PaymentTrigger(SubscriptionContext context) : IAfterSaveTrigger<Payment>
    {
        private readonly SubscriptionContext _context = context;

        public Task AfterSave(ITriggerContext<Payment> context, CancellationToken cancellationToken)
        {
            var payment = context.Entity;


            if (payment.User == null)
            {
                return Task.FromException(new InvalidOperationException("User not found for the payment."));
            }

            
            switch (context.ChangeType)
            {
                case ChangeType.Added:
                    payment.User.Balance += payment.Amount;
                    break;
                case ChangeType.Modified:
                    var oldPayment = context.UnmodifiedEntity;
                    if (oldPayment == null)
                    {
                        return Task.FromException(new InvalidOperationException("Old payment not found for the payment."));
                    }
                    payment.User.Balance += payment.Amount - oldPayment.Amount;
                    break;
                case ChangeType.Deleted:
                    payment.User.Balance -= payment.Amount;
                    break;
            }

            _context.Entry(payment.User).State = EntityState.Modified;

            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
