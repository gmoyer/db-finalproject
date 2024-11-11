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

            var user = _context.Users.Find(payment.UserId);

            if (user == null)
            {
                return Task.FromException(new InvalidOperationException("User not found for the payment."));
            }


            switch (context.ChangeType)
            {
                case ChangeType.Added:
                    if (payment.Valid)
                    {
                        user.Balance += payment.Amount;
                    }
                    break;
                case ChangeType.Modified:
                    var oldPayment = context.UnmodifiedEntity;
                    if (oldPayment == null)
                    {
                        return Task.FromException(new InvalidOperationException("Old payment not found for the payment."));
                    }
                    // Payment was invalid and now is valid
                    // Need to add the payment amount to the user balance
                    if (!oldPayment.Valid && payment.Valid)
                    {
                        user.Balance += payment.Amount;
                    }

                    // Payment was valid and now is invalid
                    // Need to remove what the payment was before becoming invalid
                    else if (oldPayment.Valid && !payment.Valid)
                    {
                        user.Balance -= oldPayment.Amount;
                    }

                    // Payment was valid and now is valid
                    // Need to update the user balance with the difference
                    else if (oldPayment.Valid && payment.Valid)
                    {
                        user.Balance += payment.Amount - oldPayment.Amount;
                    }
                    break;
                case ChangeType.Deleted:
                    if (payment.Valid)
                    {
                        user.Balance -= payment.Amount;
                    }
                    break;
            }

            _context.Entry(user).State = EntityState.Modified;

            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
