namespace ServerSubscriptionManager.Models
{
    public class PaymentMethod
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Payment> Payments { get; set; } = [];
    }
}
