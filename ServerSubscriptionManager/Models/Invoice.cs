namespace ServerSubscriptionManager.Models
{
    public class Invoice
    {
        public long Id { get; set; }
        public required long UserId { get; set; }
        public required long SubscriptionPeriodId { get; set; }
        public decimal Amount { get; set; } = 0;
        public User? User { get; set; }
        public SubscriptionPeriod? SubscriptionPeriod { get; set; }
    }
}
