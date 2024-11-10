namespace ServerSubscriptionManager.Models
{
    public class SubscriptionPeriod
    {
        public long Id { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public required decimal ServerCost { get; set; }
        public int UserCount { get; set; } = 0;
        public decimal UserCost => UserCount > 0 ? ServerCost / UserCount : ServerCost;
        public decimal NextUserCost => (UserCount + 1) > 0 ? ServerCost / (UserCount + 1) : ServerCost;
        public ICollection<Invoice> Invoices { get; set; } = [];
    }
}
