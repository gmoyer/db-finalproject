namespace ServerSubscriptionManager.Models
{
    public class User
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public decimal Balance { get; set; } = 0;
        public string Role { get; set; } = "User";
        public bool AutoInvoice { get; set; } = false;
        public ICollection<Payment> Payments { get; set; } = [];
        public ICollection<Invoice> Invoices { get; set; } = [];
    }
}
