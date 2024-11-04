namespace ServerSubscriptionManager.Models
{
    public class User
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string Role { get; set; } = "User";
        public ICollection<Payment> Payments { get; set; } = [];
        public ICollection<SubscriptionPeriod> Subscriptions { get; set; } = [];
    }
}
