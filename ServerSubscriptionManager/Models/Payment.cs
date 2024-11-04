namespace ServerSubscriptionManager.Models
{
    public class Payment
    {
        public long Id { get; set; }
        public required decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public required PaymentMethod PaymentMethod { get; set; }
        public bool Valid { get; set; } = false;
        public long UserId { get; set; }
        public User? User { get; set; }
    }
}
