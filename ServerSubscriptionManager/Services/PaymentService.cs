using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Models;

namespace ServerSubscriptionManager.Services
{
    public class PaymentService(SubscriptionContext context) : IEntityService<Payment>
    {
        private readonly SubscriptionContext _context = context;

        public async Task<bool> AddAsync(Payment payment)
        {
            var user = await _context.Users.FindAsync(payment.UserId);

            if (user == null)
            {
                return false;
            }

            if (payment.Valid)
            {
                user.Balance += payment.Amount;
            }

            _context.Payments.Add(payment);
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAsync(long id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
            {
                return false;
            }

            var user = await _context.Users.FindAsync(payment.UserId);

            if (user == null)
            {
                return false;
            }

            if (payment.Valid)
            {
                user.Balance -= payment.Amount;
            }

            _context.Payments.Remove(payment);
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Payment payment)
        {
            var oldPayment = await _context.Payments.FindAsync(payment.Id);

            if (oldPayment == null)
            {
                return false;
            }

            var user = await _context.Users.FindAsync(payment.UserId);

            if (user == null)
            {
                return false;
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

            oldPayment.Amount = payment.Amount;
            oldPayment.Valid = payment.Valid;

            _context.Entry(user).State = EntityState.Modified;
            _context.Entry(oldPayment).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
