namespace ServerSubscriptionManager.Models
{
    public class Payment
    {
        public long Id { get; set; }
        public required decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public required long PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public bool Valid { get; set; } = false;
        public required long UserId { get; set; }
        public User? User { get; set; }
    }
}
