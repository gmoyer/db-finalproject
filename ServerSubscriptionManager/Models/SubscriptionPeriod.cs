namespace ServerSubscriptionManager.Models
{
    public class SubscriptionPeriod
    {
        public long Id { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public required decimal ServerCost { get; set; }
        public ICollection<User> Users { get; set; } = [];
    }
}
